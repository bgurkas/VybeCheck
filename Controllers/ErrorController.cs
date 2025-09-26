using Microsoft.AspNetCore.Mvc;

namespace VybeCheck.Controllers;

public class ErrorController : Controller
{
    [HttpGet("error/{code}")]
    public IActionResult Handle(int code)
    {   
        if (code == 404)
        {
            // 404 not found
            return View("PageNotFound");
        }
        else if (code == 401)
        {
            // 401 unauthorized
            return View("Unauthorized");
        }
        else if (code == 403)
        {
            // 403 forbidden
            return View("Forbidden");
        }
        else if (code == 400)
        {
            return View("BadRequest");
        }
        else
        {
            // 500 errors
            return View("ServerError");

        }

    }
}