using Pets.API.Requests;
using Pets.API.Responses.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Google.Cloud.Firestore;
using Pets.API.Requests.Chat;
using Microsoft.Extensions.Options;
using Pets.API.Settings;
using System.Drawing.Printing;

namespace Pets.API.Clients
{
    public interface IFirebaseClient
    {
        Task<List<ChatMessageDto>> GetMessages(string chatId, ChatMessageQueryParams chatMessageQueryParams);

        Task SendMessage(string chatId, string userName, string timestamp, ChatMessageRequest chatMessageRequest);

        Task<ChatInfoDto> GetChatInfo(Guid userId, Guid userChatId);

        Task<bool> UpdateChatInfo(Guid userId, ChatInfoDto chatInfo);

        Task<bool> AddChatInfo(Guid userId, ChatInfoDto chatInfo);

        Task<List<ChatInfoDto>> GetChats(Guid userId);
    }

    public class FirebaseClient : IFirebaseClient
    {
        private readonly int PAGE_SIZE;

        private readonly FirestoreDb _firestoreDB;

        public FirebaseClient(IOptions<FirestoreDbSettings> firestoreDbSettings)
        {
            PAGE_SIZE = firestoreDbSettings.Value.PageSize;

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", firestoreDbSettings.Value.CredentialsPath);

            _firestoreDB = FirestoreDb.Create(firestoreDbSettings.Value.DbName);
        }

        public async Task SendMessage(string chatId, string userName, string timestamp, ChatMessageRequest chatMessageRequest)
        {
            var collection = _firestoreDB.Collection(GetChatMessagesCollectionReference(chatId));
            await collection.AddAsync(new
            {
                UserName = userName,
                Message = chatMessageRequest.Message,
                Timestamp = timestamp
            });
        }

        public async Task<List<ChatMessageDto>> GetMessages(string chatId, ChatMessageQueryParams chatMessageQueryParams)
        {
            var result = new List<ChatMessageDto>();

            var query = GetQuery(chatId, chatMessageQueryParams);

            var querySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot queryResult in querySnapshot.Documents)
            {
                string userName = queryResult.GetValue<string>("UserName");
                string message = queryResult.GetValue<string>("Message");
                string timestamp = queryResult.GetValue<string>("Timestamp");
                result.Add(new ChatMessageDto { UserName = userName, Message = message, Timestamp = timestamp });
            }

            return result;
        }

        private Query GetQuery(string chatId, ChatMessageQueryParams chatMessageQueryParams)
        {
            var collection = _firestoreDB.Collection(GetChatMessagesCollectionReference(chatId));

            if (chatMessageQueryParams == null
                || chatMessageQueryParams.Type == null
                || string.IsNullOrEmpty(chatMessageQueryParams.Timestamp))
            {
                return collection.LimitToLast(PAGE_SIZE).OrderBy("Timestamp");
            }

            if (chatMessageQueryParams.Type == ChatMessageQueryType.Latest)
            {
                return collection.OrderBy("Timestamp").WhereGreaterThan("Timestamp", chatMessageQueryParams.Timestamp);
            }

            return collection.LimitToLast(PAGE_SIZE).OrderBy("Timestamp").WhereLessThan("Timestamp", chatMessageQueryParams.Timestamp);
        }

        public async Task<ChatInfoDto> GetChatInfo(Guid userId, Guid userChatId)
        {
            var collection = _firestoreDB.Collection(GetChatInfosCollectionReference(userId));
            var query = collection.WhereEqualTo("UserId", userChatId.ToString());

            var querySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot queryResult in querySnapshot.Documents)
            {
                string chtainfo_userId = queryResult.GetValue<string>("UserId");
                string chtainfo_userName = queryResult.GetValue<string>("UserName");
                bool chtainfo_hasNewMessages = queryResult.GetValue<bool>("HasNewMessages");

                return new ChatInfoDto
                {
                    ReferenceId = queryResult.Id,
                    UserId = chtainfo_userId,
                    UserName = chtainfo_userName,
                    HasNewMessages = chtainfo_hasNewMessages,
                };
            }

            return null;
        }

        public async Task<bool> UpdateChatInfo(Guid userId, ChatInfoDto chatInfo)
        {
            var collection = _firestoreDB.Collection(GetChatInfosCollectionReference(userId));
            var docReference = collection.Document(chatInfo.ReferenceId);
            await docReference.UpdateAsync("HasNewMessages", chatInfo.HasNewMessages);
            return true;
        }

        public async Task<bool> AddChatInfo(Guid userId, ChatInfoDto chatInfo)
        {
            var collection = _firestoreDB.Collection(GetChatInfosCollectionReference(userId));
            await collection.AddAsync(new
            {
                UserId = chatInfo.UserId,
                UserName = chatInfo.UserName,
                HasNewMessages = chatInfo.HasNewMessages
            });
            return true;
        }

        public async Task<List<ChatInfoDto>> GetChats(Guid userId)
        {
            var collection = _firestoreDB.Collection(GetChatInfosCollectionReference(userId));

            var chats = new List<ChatInfoDto>();
            var querySnapshot = await collection.GetSnapshotAsync();
            foreach (DocumentSnapshot queryResult in querySnapshot.Documents)
            {
                string chtainfo_userId = queryResult.GetValue<string>("UserId");
                string chtainfo_userName = queryResult.GetValue<string>("UserName");
                bool chtainfo_hasNewMessages = queryResult.GetValue<bool>("HasNewMessages");

                chats.Add(new ChatInfoDto
                {
                    ReferenceId = queryResult.Id,
                    UserId = chtainfo_userId,
                    UserName = chtainfo_userName,
                    HasNewMessages = chtainfo_hasNewMessages,
                });
            }

            return chats;
        }

        private static string GetChatMessagesCollectionReference(string chatId)
        {
            return $@"messages\{chatId}";
        }

        private static string GetChatInfosCollectionReference(Guid userId)
        {
            return $@"chats\{userId}";
        }
    }
}
