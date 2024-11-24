using System;
using System.Collections.Generic;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using MessagingWebApp.Models.Domain;
using Microsoft.Identity.Client;
using Microsoft.Extensions.Configuration;
using Twilio.Exceptions;

namespace MessagingWebApp.Repositories
{
    public class TwilioCommunicationRepository : ICommunicationRepository
    {
        private readonly IConfiguration _configuration;

        public TwilioCommunicationRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<MessageResource?> SendMessageAsync(string fromPhoneNumber, string toPhoneNumber, string content)
        {
            try
            {
                TwilioClient.Init(
                    _configuration.GetSection("Twilio")["AccountSid"],
                    _configuration.GetSection("Twilio")["AuthToken"]
                );

                var messageOptions = new CreateMessageOptions(
                    new PhoneNumber(toPhoneNumber)
                );
                messageOptions.From = new PhoneNumber(fromPhoneNumber);
                messageOptions.Body = content;
                var message = await MessageResource.CreateAsync(messageOptions);

                MessageResource messageStatus = null;
                int delay = 1000;
                while ((messageStatus == null || messageStatus.Status != MessageResource.StatusEnum.Sent) && delay <= 16000)
                {
                    messageStatus = await MessageResource.FetchAsync(message.Sid);
                    await Task.Delay(delay);
                    delay *= 2;
                }

                if (messageStatus.Status != MessageResource.StatusEnum.Sent)
                {
                    Console.WriteLine("Error: " + messageStatus.ErrorMessage);
                    return null;
                }
                return message;
            }
            catch (TwilioException e)
            {
                Console.WriteLine("Error: " + e.Message);
                return null;
            }
        }
    }
}
