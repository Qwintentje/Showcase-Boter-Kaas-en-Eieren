namespace Tasker_Opdracht_MVC.Models
{
    public class GamesHistoryModel
    {
        public string User1 { get; set; }
        public string User2 { get; set; }
        public string[] Board { get; set; }
        public string PlayerWon { get; set; }
        public DateTime TimeFinished { get; set; }
        public string BoardGrid { get; set; }
    }
}
