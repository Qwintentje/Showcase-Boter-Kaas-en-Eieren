using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Tasker_Opdracht_MVC.Areas.Identity.Data;
using Tasker_Opdracht_MVC.Data;
using Tasker_Opdracht_MVC.Models;
using IdentityRole = Microsoft.AspNetCore.Identity.IdentityRole;

namespace Tasker_Opdracht_MVC.Controllers;

public class HomeController : Controller
{
    private readonly GameDBContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext applicationDb;
    private readonly RoleManager<IdentityRole> roleManager;

    public HomeController(GameDBContext context, UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDb, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        this.applicationDb = applicationDb;
        this.roleManager = roleManager;
    }
    public IActionResult AboutMe()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }
    public async Task<IActionResult> Rooms()
    {
        try
        {
            LeaderboardModel Leaderboard = await GetLeaderboardAsync();
            return View(Leaderboard);
        }
        catch
        {
            return View();
        }
    }

    public IActionResult Game(string Id)
    {
        if (Id == null)
        {
            return RedirectToAction("Rooms");
        }
        return View();
    }

    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> Administrator()
    {
        var games = await GetAllGames();
        return View(games);
    }


    [Authorize(Roles = "manager")]
    public async Task<IActionResult> Manager()
    {
        List<ApplicationUser> users = await GetUserAsync();
        List<IdentityRole> roles = await applicationDb.Roles.ToListAsync();
        ChangeRoleViewModel changeRoleViewModel = new ChangeRoleViewModel()
        {
            Roles = roles,
            Users = users,
        };
        return View(changeRoleViewModel);
    }

    [Authorize(Roles = "manager")]
    public async Task<IActionResult> UpdateRole(string userId, string roleId)
    {
        var user = applicationDb.Users.Find(userId);
        var newRole = applicationDb.Roles.Find(roleId);
        if (user == null || newRole == null) return RedirectToAction("manager");

        //Check if user has roles
        var currentRole = applicationDb.UserRoles.Where(e => e.UserId == userId).FirstOrDefault();
        if (currentRole != null)
        {
            //Check if already has the chosen role
            if (currentRole.RoleId != roleId)
            {
                //Remove current role
                var currentRoleName = applicationDb.Roles.Where(e => e.Id == currentRole.RoleId).FirstOrDefault()?.Name;
                await _userManager.RemoveFromRoleAsync(user, currentRoleName);
            }
            else
            {
                //Already has this role
                return RedirectToAction("manager");
            }
        }
        //Add chosen role
        await _userManager.AddToRoleAsync(user, newRole.Name);
        return RedirectToAction("Manager");
    }

    [Authorize(Roles = "manager")]
    public async Task<IActionResult> RemoveRole(string userId)
    {
        var user = applicationDb.Users.Find(userId);
        var currentRole = applicationDb.UserRoles.Where(e => e.UserId == userId).FirstOrDefault();

        if (currentRole != null)
        {
            var currentRoleName = applicationDb.Roles.Where(e => e.Id == currentRole.RoleId).FirstOrDefault()?.Name;
            await _userManager.RemoveFromRoleAsync(user, currentRoleName);
        }
        return RedirectToAction("manager");
    }



    [Authorize(Roles = "manager")]
    public async Task<List<ApplicationUser>> GetUserAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var users = await applicationDb.Users.Where(e => e.Id != currentUser.Id).ToListAsync();
        foreach (ApplicationUser user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            user.UserRoles = new List<IdentityRole>();
            foreach (var role in roles)
            {
                user.UserRoles.Add(await roleManager.FindByNameAsync(role));
            }
        }
        return users;
    }
    public async Task<LeaderboardModel> GetLeaderboardAsync()
    {
        LeaderboardModel Leaderboard = new LeaderboardModel
        {
            Leaderboard = new List<PlayerModel>()
        };
        var leaderboardAccounts = _context.Games
            .Where(g => !string.IsNullOrEmpty(g.PlayerWonEmail))
            .GroupBy(g => g.PlayerWonEmail)
            .Select(g => new { Email = g.Key, Wins = g.Count() })
            .OrderByDescending(x => x.Wins)
            .Take(10)
            .ToList();

        foreach (var account in leaderboardAccounts)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Email == account.Email);
            if (user != null)
            {
                Leaderboard.Leaderboard.Add(new PlayerModel
                {
                    Rank = Leaderboard.Leaderboard.Count + 1,
                    Name = $"{user.FirstName} {user.LastName}",
                    Wins = account.Wins
                });
            }
        }
        return Leaderboard;
    }

    [Authorize(Policy = "RequireAdmin")]
    public async Task<List<GamesHistoryModel>> GetAllGames()
    {
        var games = await _context.Games.ToListAsync();

        //games to a list
        List<GamesHistoryModel> gameHistoryModels = new List<GamesHistoryModel>();
        foreach (var game in games)
        {
            gameHistoryModels.Add(new GamesHistoryModel
            {
                User1 = game.User1,
                User2 = game.User2,
                Board = game.Board.Split(","),
                PlayerWon = game.PlayerWon,
                TimeFinished = game.TimeFinished,
                //BoardGrid will show the played game in a 3x3 grid
                BoardGrid = $"<table>{string.Join("", game.Board.Split(',').Select((val, i) => i % 3 == 0 ? $"<tr><td>{val}</td>" : i % 3 == 2 ? $"<td>{val}</td></tr>" : $"<td>{val}</td>"))}</table>"
            });
        }
        return gameHistoryModels;
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}