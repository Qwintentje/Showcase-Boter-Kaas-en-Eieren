using Microsoft.EntityFrameworkCore;
using Tasker_Opdracht_MVC.Data.Entities;

namespace Tasker_Opdracht_MVC.Data;

public static class GamesSeeder
{
    public static void SeedData(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>().HasData(
                        new Game
                        {
                            Id = 2,
                            GameId = "0540fe66-b10b-4d28-b7c9-3036120220db",
                            User1 = "user1",
                            User2 = "user2",
                            Board = "X,X,,O,O,O,,X,",
                            PlayerWon = "user1",
                            PlayerWonEmail = "user1@example.com",
                            TimeFinished = DateTime.UtcNow
                        },
                        new Game
                        {
                            Id = 1,
                            GameId = "95276d1e-5b26-439d-b0bc-cec99754781b",
                            User1 = "user3",
                            User2 = "user4",
                            Board = "X,X,,O,O,O,,X,",
                            PlayerWon = "user3",
                            PlayerWonEmail = "user4@example.com",
                            TimeFinished = DateTime.UtcNow
                        }
                    );
    }
}
