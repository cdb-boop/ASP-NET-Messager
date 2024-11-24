namespace MessagingWebApp.Models.ViewModels
{
    public class CreateChatGroupRequest
    {
        public string Name { get; set; }
        public string Owner { get; set; }
        public string Members { get; set; }
        public string FirstMessage { get; set; }
    }
}
