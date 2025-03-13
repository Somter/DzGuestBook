using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using WebApplication3.Repository;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Message> _messageRepository;
        private readonly IRepository<User> _userRepository;

        public HomeController(IRepository<Message> messageRepository, IRepository<User> userRepository)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index()
        {
            var messages = await _messageRepository.GetAllAsync();
            var sortedMessages = messages.OrderByDescending(m => m.MessageDate).ToList();

            var users = await _userRepository.GetAllAsync();

            var messagesWithUsers = sortedMessages.Select(m => new
            {
                msg = m,
                Name = users.FirstOrDefault(u => u.Id == m.Id_User)?.Name ?? "Аноним"
            }).ToList();

            ViewBag.Messages = messagesWithUsers;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMessage(string messageText)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index");

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var message = new Message
            {
                Id_User = userId,
                MessageText = messageText,
                MessageDate = DateTime.Now
            };

            await _messageRepository.AddAsync(message);
            await _messageRepository.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
