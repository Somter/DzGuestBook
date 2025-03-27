using WebApplication3.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication3.Repository;

namespace WebApplication3.Controllers
{
    //public class AuthController : Controller
    //{
    //    private readonly IUserRepository _userRepository;   
    //    private readonly IPasswordHasher _passwordHasher;

    //    public AuthController(IUserRepository userRepository, IPasswordHasher passwordHasher)
    //    {
    //        _userRepository = userRepository;
    //        _passwordHasher = passwordHasher;
    //    }

    //    public IActionResult Login() => View();

    //    [HttpPost]
    //    public async Task<IActionResult> Login(string name, string pwd)
    //    {
    //        var user = await _userRepository.GetByNameAsync(name);
    //        if (user == null || user.Pwd != _passwordHasher.HashPassword(pwd, user.Salt))
    //        {
    //            return Json(new { success = false, message = "Не верный логин или пароль" });
    //        }

    //        var claims = new List<Claim>
    //        {
    //            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    //            new Claim(ClaimTypes.Name, user.Name)
    //        };

    //        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    //        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

    //        return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
    //    }


    //    public IActionResult Register() => View();

    //    [HttpPost]
    //    public async Task<IActionResult> Register(string name, string pwd, string confirmPwd)
    //    {
    //        if (pwd != confirmPwd)
    //        {
    //            return Json(new { success = false, message = "Пароли должны совпадать" });
    //        }

    //        if (await _userRepository.GetByNameAsync(name) != null)
    //        {
    //            return Json(new { success = false, message = "Пользователь уже существует" });
    //        }

    //        var salt = _passwordHasher.GenerateSalt();
    //        var hashedPwd = _passwordHasher.HashPassword(pwd, salt);

    //        var user = new User { Name = name, Pwd = hashedPwd, Salt = salt };
    //        await _userRepository.AddAsync(user);
    //        await _userRepository.SaveAsync();

    //        return Json(new { success = true, redirectUrl = Url.Action("Login") });
    //    }
    //}
}
