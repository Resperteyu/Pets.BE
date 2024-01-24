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
using System.Drawing;
using System.Threading.Tasks;

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
        public async Task<ActionResult<List<ChatMessageDto>>> GetMessges(Guid userIdChat, [FromQuery] ChatMessageQueryParams chatMessageQueryParams)
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
        public async Task<ActionResult<string>> Post(Guid userIdChat, ChatMessageRequest request)
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
        [HttpGet("messages")]
        public async Task<ActionResult<List<ChatMessageDto>>> GetChats()
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));

            var chats = await _chatService.GetChats(userId);

            return Ok(chats);
        }
    }
}
