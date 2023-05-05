using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Models;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdController : ControllerBase
{
    private DatabaseContext _db;

    public AdController(DatabaseContext db)
    {
        _db = db;
    }

    [Route("get")]
    [HttpGet]
    public ActionResult<Ad> GetUser(int id)
    {
        User user = _db.Users.Find(id);
        if (user == null)
        {
            return BadRequest();
        }
        return Ok(user);
    }
}