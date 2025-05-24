namespace Pets.API.Responses.Dtos
{
    public class ChatInfoDto
    {
        public string ReferenceId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool HasNewMessages { get; set; }
    }
}
