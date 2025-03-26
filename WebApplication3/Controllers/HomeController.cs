using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using WebApplication3.Repository;
using WebApplication3.ViewModels;

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
            var users = await _userRepository.GetAllAsync();

            var messagesWithUsers = messages
                .OrderByDescending(m => m.MessageDate)
                .Select(m => new MessageViewModel
                {
                    Id = m.Id,
                    MessageText = m.MessageText,
                    MessageDate = m.MessageDate,
                    UserName = users.FirstOrDefault(u => u.Id == m.Id_User)?.Name ?? "Аноним"
                })
                .ToList();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") 
            {
                return Json(messagesWithUsers);
            }

            ViewBag.Messages = messagesWithUsers;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMessage(string messageText)
        {
            if (!User.Identity.IsAuthenticated)
                return Json(new { success = false, error = "Пользователь не авторизован." });

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var message = new Message
            {
                Id_User = userId,
                MessageText = messageText,
                MessageDate = DateTime.Now
            };

            await _messageRepository.AddAsync(message);
            await _messageRepository.SaveAsync();

            return Json(new { success = true, message = "Сообщение добавлено." });
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages()
        {
            var messages = await _messageRepository.GetAllAsync();
            var users = await _userRepository.GetAllAsync();

            var messagesWithUsers = messages
                .OrderByDescending(m => m.MessageDate)
                .Select(m => new MessageViewModel
                {
                    Id = m.Id,
                    MessageText = m.MessageText,
                    MessageDate = m.MessageDate,
                    UserName = users.FirstOrDefault(u => u.Id == m.Id_User)?.Name ?? "Аноним"
                })
                .ToList();

            return Json(messagesWithUsers);
        }



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
