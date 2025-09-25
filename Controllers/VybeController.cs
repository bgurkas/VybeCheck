using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VybeCheck.Models;
using VybeCheck.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using System.Text.Json;

[Route("albums")]
public class VybeController : Controller
{
    private readonly ApplicationContext _context;
    private readonly IHttpClientFactory _clientFactory;

    public VybeController(ApplicationContext context, IHttpClientFactory clientFactory)
    {
        _context = context;
        _clientFactory = clientFactory;
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
            LastUpdated = album.UpdatedAt,
            ArtworkUrl60 = album.ArtworkUrl60
        }).ToList();

        return View(new VybeIndexViewModel { Items = indexItems });
    }

    [HttpGet("user/{id}")]
    public async Task<IActionResult> FilterCollection(int id, VybeIndexViewModel vm)
    {
        if (!AuthCheck()) return Unauthorized();

        var user = await _context.Users.FindAsync(id);
        if (user is null) return Unauthorized();

        var albums = await _context.Albums.AsNoTracking().Include(album => album.User).Where(a => a.UserId == id).ToListAsync();

        var indexItems = albums.Select(album => new IndexItemViewModel
        {
            AlbumId = album.Id,
            Title = album.Title,
            Artist = album.Artist,
            Username = album.User!.Username,
            LastUpdated = album.UpdatedAt,
            ArtworkUrl60 = album.ArtworkUrl60
        }).ToList();

        vm.Items = indexItems;

        return View(vm);
    }

    [HttpPost("{uid}/comment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PostComment(int uid, AlbumDetailsViewModel vm)
    {
        if (!AuthCheck()) return Unauthorized();

        // If user ID doesn't exist in the database
        var user = await _context.Users.FindAsync(uid);
        if (user is null) return Unauthorized();

        if (!ModelState.IsValid) return View("AlbumDetails", vm);

        var newComment = new Comment
        {
            UserId = user.Id,
            AlbumId = vm.CVM!.AlbumId,
            Content = vm.CVM!.CommentContent
        };

        await _context.Comments.AddAsync(newComment);
        await _context.SaveChangesAsync();

        return RedirectToAction("AlbumDetails", new { id = newComment.AlbumId });
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

        // Populate some album fields with data from API
        var client = _clientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        var searchStr = $"{vm.Artist} {vm.Title}";
        var response = await client.GetAsync($"https://itunes.apple.com/search?term={searchStr}&entity=album&limit=1");

        if (!response.IsSuccessStatusCode) return BadRequest();

        var jString = await response.Content.ReadAsStringAsync();
        var albumResponse = JsonSerializer.Deserialize<ResponseWrapper>(jString);
        var albumData = albumResponse!.Results[0];

        var newAlbum = new Album
        {
            Title = vm.Title,
            Artist = vm.Artist,
            Description = vm.Description,
            Genre = albumData.Genre,
            ReleaseDate = albumData.ReleaseDate,
            CollectionId = albumData.CollectionId,
            ArtworkUrl60 = albumData.ArtworkUrl60,
            ArtworkUrl100 = albumData.ArtworkUrl100,
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
            Genre = album.Genre,
            ReleaseDate = DateOnly.FromDateTime(album.ReleaseDate),
            CollectionId = album.CollectionId,
            ArtworkUrl100 = album.ArtworkUrl100,
            SnippetUrl = album.SnippetUrl,
            SnippetName = album.SnippetName,
            UploadedBy = album.User!.Username,
            UploaderId = album.UserId,
            LastUpdated = album.UpdatedAt,
            Comments = album.Comments.OrderByDescending(c => c.CreatedAt).ToList(),
            CVM = new CommentViewModel { AlbumId = album.Id}
        };
        return View(vm);
    }

    [HttpGet("{id}/snippet")]
    public async Task<IActionResult> GetSnippetForm(int id)
    {
        if (!AuthCheck()) return Unauthorized();
        var uid = GetSessionId();
        var album = await _context.Albums.AsNoTracking().SingleOrDefaultAsync(a => a.Id == id);

        if (album is null) return NotFound();

        if (album.UserId != (int)uid!) return StatusCode(403);

        //Query the API with this album's ID
        var client = _clientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        var response = await client.GetAsync($"https://itunes.apple.com/lookup?id={album.CollectionId}&entity=song");

        if (!response.IsSuccessStatusCode) return BadRequest();

        var jString = await response.Content.ReadAsStringAsync();
        var trackResponse = JsonSerializer.Deserialize<TrackWrapper>(jString);
        var tracklist = new List<TrackData>();
        foreach (TrackData item in trackResponse!.Results)
        {
            if (item.WrapperType != "collection")
            {
                tracklist.Add(item);
            }
        }

        // Populate VM and pass it to View
        var vm = new SnippetViewModel
        {
            Tracklist = tracklist,
            AlbumId = album.Id
        };
        return View(vm);
    }

    [HttpPost("snippet")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSnippet(SnippetViewModel vm)
    {
        if (!AuthCheck()) return Unauthorized();
        //if (id != vm.AlbumId) return BadRequest();

        if (ModelState.IsValid)
        {
            var album = await _context.Albums.FindAsync(vm.AlbumId);
            if (album is null) return NotFound();
            if (album.UserId != (int)GetSessionId()!) return StatusCode(403); // If the request isn't made by the album's author

            //Query the API with the Track ID that was selected
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = await client.GetAsync($"https://itunes.apple.com/lookup?id={vm.TrackId}");

            if (!response.IsSuccessStatusCode) return BadRequest();

            var jString = await response.Content.ReadAsStringAsync();
            var trackResponse = JsonSerializer.Deserialize<TrackWrapper>(jString);
            var trackData = trackResponse!.Results[0];

            album.SnippetUrl = trackData.SnippetUrl;
            album.SnippetName = trackData.TrackName;

            await _context.SaveChangesAsync();
            return RedirectToAction("AlbumDetails", new { id = album.Id });
        }
        return View("GetSnippetForm", vm);
    }

    [HttpGet("{id}/edit")]
    public async Task<IActionResult> EditAlbumForm(int id)
    {
        if (!AuthCheck()) return Unauthorized();
        var uid = GetSessionId();

        var album = await _context.Albums.AsNoTracking().SingleOrDefaultAsync(a => a.Id == id);

        if (album is null) return NotFound();

        if (album.UserId != (int)uid!) return StatusCode(403);

        return View(new AlbumViewModel { Id = album.Id, Title = album.Title, Artist = album.Artist, Description = album.Description });
    }

    [HttpPost("update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAlbum(int id, AlbumViewModel vm)
    {
        if (!AuthCheck()) return Unauthorized();
        if (id != vm.Id) return BadRequest();

        if (ModelState.IsValid)
        {
            var album = await _context.Albums.FindAsync(id);
            if (album is null) return NotFound();
            if (album.UserId != (int)GetSessionId()!) return StatusCode(403); // If the request isn't made by the album's author

            // Update genre and release date in case title or artist changed.s
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var searchStr = $"{vm.Artist} {vm.Title}";
            var response = await client.GetAsync($"https://itunes.apple.com/search?term={searchStr}&entity=album&limit=1");

            if (!response.IsSuccessStatusCode) return BadRequest();

            var jString = await response.Content.ReadAsStringAsync();
            var albumResponse = JsonSerializer.Deserialize<ResponseWrapper>(jString);
            var albumData = albumResponse!.Results[0];

            album.Title = vm.Title;
            album.Artist = vm.Artist;
            album.Description = vm.Description;
            album.Genre = albumData.Genre;
            album.ReleaseDate = albumData.ReleaseDate;
            album.CollectionId = albumData.CollectionId;
            album.ArtworkUrl60 = albumData.ArtworkUrl60;
            album.ArtworkUrl100 = albumData.ArtworkUrl100;
            album.SnippetUrl = "";
            album.SnippetName = "";
            album.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToAction("AlbumDetails", new { id = album.Id });
        }
        return View("EditAlbumForm", vm);
    }

    [HttpGet("{id}/delete")]
    public async Task<IActionResult> ConfirmDelete(int id)
    {
        if (!AuthCheck()) return Unauthorized();

        var album = await _context.Albums.FindAsync(id);
        if (album is null) return NotFound();
        if (album.UserId != (int)GetSessionId()!) return StatusCode(403); // If the request isn't made by the album's author

        var vm = new AlbumViewModel
        {
            Id = album.Id,
            Title = album.Title,
            Artist = album.Artist,
            Description = album.Description
        };
        return View(vm);
    }

    [HttpPost("{id}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAlbum(AlbumViewModel vm)
    {
        if (!AuthCheck()) return Unauthorized();
        if (vm.Id is null) return BadRequest();

        var album = await _context.Albums.FindAsync(vm.Id);
        if (album is null) return NotFound();
        if (album.UserId != (int)GetSessionId()!) return StatusCode(403); // If the request isn't made by the album's author

        _context.Albums.Remove(album);
        await _context.SaveChangesAsync();

        return RedirectToAction("VybeIndex");
    }
}