using LightGallery.Models;
using Microsoft.AspNetCore.Identity;

namespace LightGallery.Service;

public class SeedService
{
    private readonly IConfiguration _config;
    private readonly DefaultDatabaseContext _context;
    private readonly UserManager<User> _userManager;

    public SeedService(IConfiguration config, UserManager<User> userManager, DefaultDatabaseContext context)
    {
        _config = config;
        _userManager = userManager;
        _context = context;
    }

    public async Task Seed()
    {
        await SeedUsers();
    }

    public async Task SeedUsers()
    {
        Console.WriteLine("=== Seeding root user start ===");
        var username = _config["DefaultRootUser:Username"];
        var password = _config["DefaultRootUser:Password"];

        var user = await _userManager.FindByNameAsync(username);
        if (user != null)
        {
            Console.WriteLine("Root user exists. Quitting seeding");
            return;
        }

        // TODO: Replace with some service
        var result = await _userManager.CreateAsync(new User()
        {
            UserName = username
        }, password);

        if (!result.Succeeded)
        {
            Console.WriteLine("Creating user was not successful due to these errors: ");
            Console.WriteLine(string.Join(",",result.Errors.Select(x => x.Description)));
            Console.WriteLine("Ending seeding");
        }
        
        Console.WriteLine("Root user created");
    }
}