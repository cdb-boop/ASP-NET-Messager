namespace MessagingWebApp.Models.Domain
{
    public class Message
    {
        public Guid Id { get; set; }
        public string Author { get; set; }
        public DateTime SentDate { get; set; }
        public string ContentType { get; set; }
        public string Content { get; set; }
        public bool MessageRead { get; set; } // TODO: remove this?
        public ICollection<MessageReaction> Reactions { get; set; }
    }
}
