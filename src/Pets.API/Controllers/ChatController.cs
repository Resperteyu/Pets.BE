using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pets.API.Requests;
using Pets.API.Requests.Chat;
using Pets.API.Responses.Dtos;
using Pets.API.Services;
using Pets.Db.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace Pets.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IUserProfileService _userProfileService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatController(IChatService chatService,
            IUserProfileService userProfileService,
            UserManager<ApplicationUser> userManager)
        {
            _chatService = chatService;
            _userProfileService = userProfileService;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet("messages/{userIdChat:Guid}")]
        [SwaggerOperation(Summary = "Get chat messages", Description = "Retrieves messages between the current user and a specified user.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ChatMessageDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ChatMessageDto>>> GetMessges(
            [SwaggerParameter(Description = "ID of the user to get chat messages with.", Required = true)] Guid userIdChat, 
            [FromQuery][SwaggerParameter(Description = "Query parameters for pagination.")] ChatMessageQueryParams chatMessageQueryParams)
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));

            var profile = await _userProfileService.GetUserProfile(userIdChat.ToString());
            if(profile == null)
            {
                return BadRequest("User does not exist");
            }

            var messages = await _chatService.GetMessages(userId, userIdChat, chatMessageQueryParams);

            return Ok(messages);
        }       

        [Authorize]
        [HttpPost("messages/{userIdChat:Guid}")]
        [SwaggerOperation(Summary = "Send chat message", Description = "Sends a message from the current user to a specified user.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<string>> Post(
            [SwaggerParameter(Description = "ID of the user to send the message to.", Required = true)] Guid userIdChat, 
            [FromBody][SwaggerParameter(Description = "Chat message content.", Required = true)] ChatMessageRequest request)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var profile = await _userProfileService.GetUserProfile(userIdChat.ToString());
            if (profile == null)
            {
                return BadRequest("User does not exist");
            }

            var timestampMessage = await _chatService.SendMessage(user.Id, user.UserName, userIdChat, profile.UserName, request);

            return Ok(timestampMessage);
        }

        [Authorize]
        [HttpGet("infos")]
        [SwaggerOperation(Summary = "Get user chats", Description = "Retrieves a list of chats for the current user.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ChatMessageDto>))] // Based on current code return type
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ChatMessageDto>>> GetChats()
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));

            var chats = await _chatService.GetChats(userId);

            return Ok(chats);
        }
    }
}
