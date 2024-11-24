namespace MessagingWebApp
{
    public interface IChatClient
    {
        Task ReceiveMessage(string message);
    }
}
