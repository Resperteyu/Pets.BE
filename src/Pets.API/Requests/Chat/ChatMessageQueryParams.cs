namespace Pets.API.Requests.Chat
{
    public class ChatMessageQueryParams
    {
        public ChatMessageQueryType? Type { get; set; }
        public string Timestamp { get; set; }
    }
}
