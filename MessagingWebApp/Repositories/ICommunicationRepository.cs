using MessagingWebApp.Models.Domain;
using Twilio.Rest.Api.V2010.Account; // TODO: decouple

namespace MessagingWebApp.Repositories
{
    public interface ICommunicationRepository
    {
        Task<MessageResource?> SendMessageAsync(string fromPhoneNumber, string toPhoneNumber, string content);
    }
}
