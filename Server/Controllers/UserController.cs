using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private DatabaseContext _db;
    private List<User> Users { get; set; }

    public UserController(DatabaseContext db, ILogger<UserController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // Authenticate the credentials received from the POST request and return the User object including all ads to the requester.
    [Route("get")]
    [HttpPost]
    public IActionResult GetUser([FromBody] UserCredsRequest request)
    {
        _logger.LogInformation("Attempting to log in");
        
        User? user = _db.Users.Include(u => u.BuyAds).Include(u => u.SellAds).FirstOrDefault(u => u.Username == request.Username.Trim());
        if (user == null)
        {
            _logger.LogInformation("A user with username {Username} does not exist", request.Username);
            return BadRequest(new { message = "A user with this username does not exist" });
        }
        if (user.Password != request.Password.Trim())
        {
            _logger.LogInformation("The password you have entered, {Password}, does not match the password of user {Username}", request.Password, request.Username);
            return BadRequest(new { message = "Incorrect password" });
        }

        // If the user's LoggedIn state is not true, then log in the user. But don't save it in the database; just pass the status to the client.
        if (!user.LoggedIn)
        {
            user.LoggedIn = true;
        }
        
        _logger.LogInformation("{User} successfully authenticated", request.Username);
        return Ok(JsonSerializer.Serialize(user));
    }

    [Route("new")]
    [HttpPost]
    public IActionResult AddUser([FromBody] UserCredsRequest request)
    {
        User? user = new User(username: request.Username, password: request.Password);
        if (user == null || string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
        {
            _logger.LogInformation("Error. User creds missing. User = {User}, Username = {Username}, Password = {Password}", user, request.Username, request.Password);
            return BadRequest(new { message = "Incomplete User Credentials" });
        }
        _db.Users.Add(user);
        _db.SaveChanges();
        _logger.LogInformation("User {User} was created successfully", request.Username);
        return Ok();
    }
}