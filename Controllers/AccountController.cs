using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VybeCheck.Models;
using VybeCheck.Services;
using VybeCheck.ViewModels;
using System.Threading.Tasks;

[Route("account")]
public class AccountController : Controller
{
    private readonly ApplicationContext _context;
    private readonly IPasswordService _passwords;

    public AccountController(ApplicationContext context, IPasswordService passwords)
    {
        _context = context;
        _passwords = passwords;
    }

    [HttpGet("register")]
    public IActionResult GetRegistrationForm()
    {
        return View(new RegisterFormViewModel());
    }

    [HttpPost("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessRegistration(RegisterFormViewModel vm)
    {
        vm.Username = (vm.Username ?? "").Trim();
        vm.Email = (vm.Email ?? "").Trim().ToLowerInvariant();
        vm.Password = (vm.Password ?? "").Trim();
        vm.ConfirmPassword = (vm.ConfirmPassword ?? "").Trim();

        if (!ModelState.IsValid) return View("GetRegistrationForm", vm);

        bool emailExists = _context.Users.Any(user => user.Email == vm.Email);
        if (emailExists)
        {
            ModelState.AddModelError("Email", "That email is already in use.");
            return View("GetRegistrationForm", vm);
        }

        bool usernameExists = _context.Users.Any(user => user.Username == vm.Username);
        if (usernameExists)
        {
            ModelState.AddModelError("Username", "That username is already in use.");
            return View("GetRegistrationForm", vm);
        }

        var hashed = _passwords.Hash(vm.Password);
        var newUser = new User { Username = vm.Username, Email = vm.Email, PasswordHash = hashed };
        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();

        HttpContext.Session.SetInt32("userId", newUser.Id);
        return RedirectToAction("VybeIndex", "Vybe");
    }

    [HttpGet("login")]
    public IActionResult GetLoginForm(string? error)
    {
        return View(new LoginFormViewModel { Error = error });
    }

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public IActionResult ProcessLogin(LoginFormViewModel vm)
    {
        vm.Email = (vm.Email ?? "").Trim().ToLowerInvariant();
        vm.Password = (vm.Password ?? "").Trim();

        if (!ModelState.IsValid) return View("GetLoginForm", vm);

        var user = _context.Users.SingleOrDefault(user => user.Email == vm.Email);

        if (user is null || !_passwords.Verify(vm.Password, user.PasswordHash))
        {
            vm.Error = "invalid-credentials";
            return View("GetLoginForm", vm);
        }

        HttpContext.Session.SetInt32("userId", user.Id);
        return RedirectToAction("VybeIndex", "Vybe");
    }

    [HttpGet("logout")]
    public IActionResult LogoutConfirm()
    {
        return View();
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home", new { message = "logout-successful" });
    }

    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        var userId = HttpContext.Session.GetInt32("userId");
        if (userId is null) return Unauthorized();

        var user = await _context.Users.AsNoTracking().Include(u => u.Albums).Include(u => u.Comments).SingleOrDefaultAsync(u => u.Id == userId);

        if (user is null) return NotFound();

        var vm = new ProfileViewModel
        {
            Username = user.Username,
            UserId = user.Id,
            Email = user.Email,
            AlbumsPostedCount = user.Albums.Count,
            CommentsPostedCount = user.Comments.Count,
        };
        return View(vm);
    }
}