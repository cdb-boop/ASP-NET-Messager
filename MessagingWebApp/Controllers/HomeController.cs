using System.Diagnostics;
using MessagingWebApp.Models;
using MessagingWebApp.Models.Domain;
using MessagingWebApp.Models.ViewModels;
using MessagingWebApp.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MessagingWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IChatGroupRepository _chatGroupRepository;

        public HomeController(ILogger<HomeController> logger, IChatGroupRepository chatGroupRepository)
        {
            _logger = logger;
            _chatGroupRepository = chatGroupRepository;
        }

        public async Task<IActionResult> Index()
        {
            var chatGroups = await _chatGroupRepository.GetAllAsync();
            chatGroups = chatGroups.OrderBy(x => x.Messages.OrderByDescending(x => x.SentDate).First().SentDate);

            var chatGroupPreviews = new List<ChatGroupPreview>();
            foreach (var chatGroup in chatGroups.Reverse())
            {
                var lastMessage = chatGroup.Messages.OrderByDescending(x => x.SentDate).First();
                chatGroupPreviews.Add(
                    new ChatGroupPreview
                    {
                        Name = chatGroup.Name,
                        LastMessage = lastMessage,
                        ChatGroupId = chatGroup.Id.ToString(),
                        UnreadMessages = chatGroup.UnreadMessages,
                    }
                );
            };
            return View(chatGroupPreviews);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
