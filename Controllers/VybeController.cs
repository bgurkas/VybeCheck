using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VybeCheck.Models;
using VybeCheck.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

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
    public async Task<IActionResult> VybeIndex()
    {
        if (!AuthCheck()) return Unauthorized();

        var allAlbums = await _context.Albums.AsNoTracking().Include(album => album.User).Include(album => album.Comments).ToListAsync();

        var indexItems = allAlbums.Select(album => new IndexItemViewModel
        {
            AlbumId = album.Id,
            Title = album.Title,
            Artist = album.Artist,
            CommentsCount = album.Comments.Count,
            Username = album.User!.Username,
            UserId = album.UserId,
            LastUpdated = album.UpdatedAt
        }).ToList();

        return View(new VybeIndexViewModel { Items = indexItems });
    }

    [HttpGet("new")]
    public IActionResult GetAlbumForm()
    {
        if (!AuthCheck()) return Unauthorized();
        return View(new AlbumViewModel());
    }

    [HttpPost("new")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAlbum(AlbumViewModel vm)
    {
        if (!AuthCheck()) return Unauthorized();

        if (!ModelState.IsValid) return View("GetAlbumForm", vm);

        var uid = GetSessionId();
        var newAlbum = new Album
        {
            Title = vm.Title,
            Artist = vm.Artist,
            Description = vm.Description,
            UserId = (int)uid!
        };

        await _context.Albums.AddAsync(newAlbum);
        await _context.SaveChangesAsync();

        var album_id = _context.Albums.AsNoTracking().OrderBy(a => a.Id).Last().Id;
        return RedirectToAction("AlbumDetails", new { id = album_id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> AlbumDetails(int id)
    {
        if (!AuthCheck()) return Unauthorized();

        var album = await _context.Albums.AsNoTracking().Include(a => a.User).Include(a => a.Comments).ThenInclude(c => c.User).SingleOrDefaultAsync(a => a.Id == id);

        if (album is null) return NotFound();

        var uid = GetSessionId();
        var currentUser = _context.Users.AsNoTracking().SingleOrDefault(u => u.Id == uid)!.Username;

        var vm = new AlbumDetailsViewModel
        {
            AlbumId = album.Id,
            CurrentUser = currentUser,
            Title = album.Title,
            Artist = album.Artist,
            Description = album.Description,
            UploadedBy = album.User!.Username,
            LastUpdated = album.UpdatedAt,
            Comments = album.Comments.ToList()
        };
        return View(vm);
    }
}