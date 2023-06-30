using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Tasker_Opdracht_MVC.Data.Entities
{
    public class Game
    {
        //Model for database
        [Key]
        public int Id { get; set; }

        [Required]
        public string GameId { get; set; }
        [Required, MaxLength(255)]
        public string User1 { get; set; }

        [Required, MaxLength(255)]
        public string User2 { get; set; }

        [Required]
        public string Board { get; set; }

        [Required, MaxLength(255)]
        public string PlayerWon { get; set; }
        public string PlayerWonEmail { get; set; }

        [Required]
        public DateTime TimeFinished { get; set; }
    }
}
