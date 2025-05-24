﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pets.API.Requests;
using Pets.API.Requests.Search;
using Pets.API.Responses.Dtos;
using Pets.API.Services;
using Pets.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pets.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly IPetProfileService _petProfileService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImageStorageService _imageStorageService;
        private readonly ILogger<PetsController> _logger;

        public PetsController(IPetProfileService petProfileService,
            UserManager<ApplicationUser> userManager,
            IImageStorageService imageStorageService,
            ILogger<PetsController> logger)
        {
            _petProfileService = petProfileService;
            _userManager = userManager;
            _imageStorageService = imageStorageService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("{petId:Guid}")]
        public async Task<ActionResult<PetProfileDto>> GetByPetId(Guid petId)
        {
            var petProfile = await _petProfileService.GetByPetId(petId);

            if (petProfile == null)
                return NotFound("Pet not found");

            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            if (petProfile.Owner.Id != userId)
                return Unauthorized("You don't own this pet");

            return Ok(petProfile);
        }

        [Authorize]
        [HttpGet("{petId:Guid}/view")]
        public async Task<ActionResult<PetProfileDto>> GetByPetIdView(Guid petId)
        {
            var petProfile = await _petProfileService.GetByPetId(petId);

            if (petProfile == null)
                return NotFound("Pet not found");

            if (petProfile.Private)
                return Unauthorized("Pet is private");

            return Ok(petProfile);
        }

        [Authorize]
        [HttpGet("infos")]
        public async Task<ActionResult<List<PetProfileDto>>> GetPetInfos()
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            var petProfiles = await _petProfileService.GetByOwnerId(userId);
            return Ok(petProfiles);
        }

        [Authorize]
        [HttpGet("user/{userId:Guid}")]
        public async Task<ActionResult<List<PetProfileDto>>> GetPetsView(Guid userId)
        {
            var petProfiles = await _petProfileService.GetPetsView(userId);

            //what to return if owner does not exist??

            return Ok(petProfiles);
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<ActionResult<List<PetProfileDto>>> Search([FromQuery] SearchParams searchParams)
        {
            //TODO: Location perimeter search
            //TODO: return image url
            searchParams.UserId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            var petProfiles = await _petProfileService.Search(searchParams);

            return Ok(petProfiles);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Guid>> Post(CreatePetRequest request)
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User)); // TODO Insert in context?
            var petId = await _petProfileService.CreatePet(request, userId);
            return Ok(petId);
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult> Put(UpdatePetRequest request)
        {
            var petEntity = await _petProfileService.GetEntityByPetId(request.Id);
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));

            if (petEntity == null)
                return NotFound("Pet not found");

            if (petEntity.OwnerId != userId)
                return Unauthorized("You don't own this pet");

            await _petProfileService.UpdatePet(request, petEntity);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{petId:Guid}")]
        public async Task<ActionResult> Delete(Guid petId)
        {
            var petEntity = await _petProfileService.GetEntityByPetId(petId);
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));

            if (petEntity == null)
                return NotFound("Pet not found");

            if (petEntity.OwnerId != userId)
                return Unauthorized("You don't own this pet");

            await _petProfileService.DeletePet(petEntity);
            return Ok();
        }

        [Authorize]
        [HttpGet("{petId:Guid}/mates")]
        public async Task<ActionResult<List<PetProfileDto>>> GetMates(Guid petId)
        {
            var petEntity = await _petProfileService.GetEntityByPetId(petId);

            if (petEntity == null)
                return NotFound("Pet not found");

            if (!petEntity.AvailableForBreeding)
                return BadRequest("Pet is not available for breeding");

            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));

            var petProfiles = await _petProfileService.GetMates(petEntity, userId);

            return Ok(petProfiles);
        }

        /// <summary>
        /// Saves a pet image. If image header has IsProfileImage then it gets saved as profile image
        /// </summary>
        /// <param name="petId"></param>
        /// <param name="imageFile"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{petId:Guid}/image")]
        public async Task<IActionResult> SaveImage(Guid petId,
            [FromForm] IEnumerable<IFormFile> imageFile, CancellationToken cancellationToken)
        {
            try
            {
                var img = imageFile.FirstOrDefault();

                if (img == null || img.Length == 0)
                {
                    return BadRequest("No image file provided.");
                }

                if (img.ContentType is not ("image/jpeg" or "image/png"))
                {
                    return BadRequest("Invalid content type.");
                }

                var petEntity = await _petProfileService.GetEntityByPetId(petId);

                if (petEntity == null)
                    return NotFound("Pet not found");

                var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
                if (petEntity.OwnerId != userId)
                    return Unauthorized("You don't own this pet");

                bool isProfileImage = img.Headers.ContainsKey("IsProfileImage");
                await _imageStorageService.UploadPetImage(petId, isProfileImage, img, cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding image for {PetId}", petId);
            }
            return StatusCode(500);
        }

        [HttpGet("{petId:Guid}/image")]
        public Task GetPetProfileImage(Guid petId, CancellationToken cancellationToken)
        {
            try
            {
                HttpContext.Response.Clear();
                HttpContext.Response.Headers.Clear();
                return _imageStorageService.GetImage(petId, true, HttpContext, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting image for {PetId}", petId);
            }
            return Task.CompletedTask;
        }

        [HttpGet("{petId:Guid}/image/{imageId:Guid}")]
        public Task GetPetImage(Guid petId, Guid imageId, CancellationToken cancellationToken)
        {
            try
            {
                HttpContext.Response.Clear();
                HttpContext.Response.Headers.Clear();
                return _imageStorageService.GetImage(imageId, HttpContext, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting image {ImageId} for {PetId}", imageId, petId);
            }
            return Task.CompletedTask;
        }

        [Authorize]
        [HttpDelete("{petId:Guid}/image/{imageId:Guid}")]
        public async Task<IActionResult> DeletePetImage(Guid petId,
            Guid imageId, CancellationToken cancellationToken)
        {
            try
            {
                var petEntity = await _petProfileService.GetEntityByPetId(petId);

                if (petEntity == null)
                    return NotFound("Pet not found");

                var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
                if (petEntity.OwnerId != userId)
                    return Unauthorized("You don't own this pet");

                await _imageStorageService.DeleteImage(imageId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image {ImageId} for {PetId}", imageId, petId);
            }
            return StatusCode(500);
        }
    }
}
