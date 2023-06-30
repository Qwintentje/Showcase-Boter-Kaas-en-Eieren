using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tasker_Opdracht_MVC.Models
{
    public class GameModel
    {
        //Model for the game
        [Key]
        public int Id { get; set; }

        [NotMapped]
        public string GameId { get; set; }

        [NotMapped]
        public string User1Id { get; set; }

        [Required]
        public string User1 { get; set; }

        public string? User1Email { get; set; }

        [NotMapped]
        public string User2Id { get; set; }

        [Required]
        public string User2 { get; set; }
        public string? User2Email { get; set; }


        [NotMapped]
        public bool IsGameFull { get; set; }

        [NotMapped]
        public GameStatus Status { get; set; }

        [NotMapped]
        public string[] Board { get; set; }

        [NotMapped]
        public string Symbol { get; set; }

        [Required]
        public string DatabaseBoard { get; set; }

        [Required]
        public string PlayerWon { get; set; }

        [Required]
        public DateTime TimeFinished { get; set; }
    }


    public enum GameStatus
    {
        WaitingForPlayer,
        Player1Turn,
        Player2Turn,
        GameOver
    }
}
