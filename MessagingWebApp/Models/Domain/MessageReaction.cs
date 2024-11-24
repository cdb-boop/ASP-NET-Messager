namespace MessagingWebApp.Models.Domain
{
    public class MessageReaction
    {
        public Guid Id { get; set; }
        public ICollection<uint> Users { get; set; }
    }
}
