using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using WebApplication3.Models;
using WebApplication3.Repository;
using WebApplication3.ViewModels;
using Microsoft.Extensions.Logging;

public class HomeController : Controller
{
    private readonly IRepository<Message> _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IRepository<Message> messageRepo, IUserRepository userRepo, IPasswordHasher passwordHasher, ILogger<HomeController> logger)
    {
        _messageRepository = messageRepo;
        _userRepository = userRepo;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> GetMessages()
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении сообщений");
            return StatusCode(500, "Ошибка сервера");
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddMessage(string messageText)
    {
        if (!User.Identity.IsAuthenticated)
            return Json(new { success = false, error = "Пользователь не авторизован." });

        if (string.IsNullOrWhiteSpace(messageText) || messageText.Length > 500)
            return Json(new { success = false, error = "Сообщение слишком длинное или пустое." });

        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            return Json(new { success = false, error = "Ошибка авторизации." });

        var message = new Message
        {
            Id_User = userId,
            MessageText = messageText,
            MessageDate = DateTime.Now
        };

        try
        {
            await _messageRepository.AddAsync(message);
            await _messageRepository.SaveAsync();
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при добавлении сообщения");
            return StatusCode(500, "Ошибка сервера");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Login(string name, string pwd)
    {
        var user = await _userRepository.GetByNameAsync(name);
        if (user == null || _passwordHasher.HashPassword(pwd, user.Salt) != user.Pwd)  
        {
            return Json(new { success = false, message = "Неверное имя пользователя или пароль." });
        }

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Name)
    };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return Json(new { success = true });
    }


    [HttpPost]
    public async Task<IActionResult> Register(string name, string pwd, string confirmPwd)
    {
        if (pwd != confirmPwd)
        {
            return Json(new { success = false, message = "Пароли не совпадают." });
        }

        if (await _userRepository.GetByNameAsync(name) != null)
        {
            return Json(new { success = false, message = "Пользователь с таким именем уже существует." });
        }

        var salt = _passwordHasher.GenerateSalt();
        var hashedPassword = _passwordHasher.HashPassword(pwd, salt);

        var user = new User
        {
            Name = name,
            Pwd = hashedPassword,
            Salt = salt  
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveAsync();

        return Json(new { success = true });
    }


    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Json(new { success = true });
    }


    [HttpGet]
    public IActionResult CheckAuth()
    {
        return Json(new { isAuthenticated = User.Identity.IsAuthenticated });
    }

}
