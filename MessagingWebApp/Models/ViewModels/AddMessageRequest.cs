namespace MessagingWebApp.Models.ViewModels
{
    public class AddMessageRequest
    {
        public string Content { get; set; }
        public string ContentType { get; set; }
        public string Author { get; set; }
        public string GroupChatId {  get; set; }
    }
}
