using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VybeCheck.Models;
using VybeCheck.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

[Route("albums")]
public class VybeController : Controller
{
    private readonly ApplicationContext _context;

    public VybeController(ApplicationContext context)
    {
        _context = context;
    }

    // Helper function to check if userId exists in the session.
    private bool AuthCheck()
    {
        var userId = HttpContext.Session.GetInt32("userId");

        if (userId is null) return false;
        return true;
    }

    // Helper function to get userId quickly
    private int? GetSessionId()
    {
        return HttpContext.Session.GetInt32("userId");
    }

    [HttpGet]
    public IActionResult VybeIndex()
    {
        if (!AuthCheck()) return Unauthorized();
        return Content("hello");
    }
}