using MessagingWebApp.Models.Domain;

namespace MessagingWebApp.Models.ViewModels
{
    public class ChatGroupPreview
    {
        public string Name { get; set; }
        public string ChatGroupId { get; set; }
        public Message LastMessage { get; set; }
        public uint UnreadMessages { get; set; }
    }
}
