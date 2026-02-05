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

namespace Pets.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController(
        IChatService chatService,
        IUserProfileService userProfileService,
        UserManager<ApplicationUser> userManager)
        : ControllerBase
    {
        [Authorize]
        [HttpGet("messages/{userIdChat:Guid}")]
        public async Task<ActionResult<List<ChatMessageDto>>> GetMessages(Guid userIdChat, [FromQuery] ChatMessageQueryParams chatMessageQueryParams)
        {
            var userId = Guid.Parse(userManager.GetUserId(HttpContext.User));

            var profile = await userProfileService.GetUserProfile(userIdChat.ToString());
            if(profile == null)
            {
                return BadRequest("User does not exist");
            }

            var messages = await chatService.GetMessages(userId, userIdChat, chatMessageQueryParams);

            return Ok(messages);
        }       

        [Authorize]
        [HttpPost("messages/{userIdChat:Guid}")]
        public async Task<ActionResult<string>> Post(Guid userIdChat, ChatMessageRequest request)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            var profile = await userProfileService.GetUserProfile(userIdChat.ToString());
            if (profile == null)
            {
                return BadRequest("User does not exist");
            }

            var timestampMessage = await chatService.SendMessage(user.Id, user.UserName, userIdChat, profile.UserName, request);

            return Ok(timestampMessage);
        }

        [Authorize]
        [HttpGet("infos")]
        public async Task<ActionResult<List<ChatMessageDto>>> GetChats()
        {
            // TODO: Firebase not configured - returning empty list for now
            // var userId = Guid.Parse(userManager.GetUserId(HttpContext.User));
            // var chats = await chatService.GetChats(userId);
            // return Ok(chats);
            
            return Ok(new List<ChatMessageDto>());
        }
    }
}
