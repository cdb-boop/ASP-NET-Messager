using MessagingWebApp.Models.ViewModels;

namespace MessagingWebApp.Models.Domain
{
    public class ChatGroup
    {
        public Guid Id { get; set; }
        public uint UnreadMessages { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Message> Messages { get; set; }
        public string Name { get; set; }
        public string LastMessage { get; set; }
        public string LastAuthor { get; set; }
        public string Owner { get; set; }
    }
}
