using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdController : ControllerBase
{
    private DatabaseContext _db;
    private List<User> Users { get; set; }

    public AdController(DatabaseContext db)
    {
        _db = db;
    }
}