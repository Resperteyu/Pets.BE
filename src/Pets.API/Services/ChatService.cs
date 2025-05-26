using Pets.API.Clients;
using Pets.API.Requests;
using Pets.API.Requests.Chat;
using Pets.API.Responses.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pets.API.Services
{
    public interface IChatService
    {
        Task<List<ChatMessageDto>> GetMessages(Guid userId, Guid userIdChat, ChatMessageQueryParams chatMessageQueryParams);
        Task<string> SendMessage(Guid userId, string userName, Guid userIdChat, string userChatAlias, ChatMessageRequest chatMessageRequest);
        Task<List<ChatInfoDto>> GetChats(Guid userId);
    }

    public class ChatService(IFirebaseClient firebaseClient) : IChatService
    {
        public async Task<List<ChatMessageDto>> GetMessages(Guid userId, Guid userIdChat, ChatMessageQueryParams chatMessageQueryParams)
        {
            //OPTIMISATION: IF WE ARE DOING GET LATEST MESSAGES BETTER TO CHECK IF WE HAVE NEW MESSAGES FIRST??
            var messages =  await firebaseClient.GetMessages(GetChatId(userId, userIdChat), chatMessageQueryParams);

            if(messages.Count > 0 && (chatMessageQueryParams == null
                || chatMessageQueryParams.Type == null
                || chatMessageQueryParams.Type == ChatMessageQueryType.Latest)) 
            {
                //we want to update our chatinfo
                var chatInfo = await firebaseClient.GetChatInfo(userId, userIdChat);
                if(chatInfo != null && chatInfo.HasNewMessages) 
                {
                    chatInfo.HasNewMessages = false;
                    await firebaseClient.UpdateChatInfo(userId, chatInfo);
                }
            }

            return messages;
        }

        public async Task<string> SendMessage(Guid userId, string userName, Guid userIdChat, string userChatAlias, ChatMessageRequest chatMessageRequest)
        {
            var timestampMessage = GetTimestamp();
            await firebaseClient.SendMessage(GetChatId(userId, userIdChat), userName, timestampMessage, chatMessageRequest);

            //we want to update recipient chatinfo
            var chatInfo = await firebaseClient.GetChatInfo(userIdChat, userId);
            if(chatInfo == null) 
            {
                chatInfo = new ChatInfoDto
                {
                    UserId = userId.ToString(),
                    UserName = userName,
                    HasNewMessages = true,
                };
                await firebaseClient.AddChatInfo(userIdChat, chatInfo);
            }
            else
            if(!chatInfo.HasNewMessages){
                //update chat info
                chatInfo.HasNewMessages = true;
                await firebaseClient.UpdateChatInfo(userIdChat, chatInfo);
            }

            //be sure is added in our chat list             
            chatInfo = await firebaseClient.GetChatInfo(userId, userIdChat);
            if (chatInfo == null)
            {
                chatInfo = new ChatInfoDto
                {
                    UserId = userIdChat.ToString(),
                    UserName = userChatAlias,
                    HasNewMessages = false,
                };
                await firebaseClient.AddChatInfo(userId, chatInfo);
            }
            
            return timestampMessage;
        }

        public async Task<List<ChatInfoDto>> GetChats(Guid userId)
        {
            return await firebaseClient.GetChats(userId);
        }

        private static string GetChatId(Guid UserA, Guid UserB)
        {
            if (UserA < UserB)
                return UserA + "_" + UserB;
            return UserB + "_" + UserA;
        }

        public static string GetTimestamp()
        {
            return DateTime.UtcNow.ToString("yyyyMMddHHmmssffff");
        }
    }
}
