using System.ComponentModel.DataAnnotations;

namespace Tasker_Opdracht_MVC.Data.Entities
{
	public class Email
	{
		[Key]
		public int Id { get; set; }


		[Required(ErrorMessage = "Vul een onderwerp in")]
		[MaxLength(200, ErrorMessage = "Het onderwerp mag maximaal 200 karakters bevatten")]
		public string Subject { get; set; }

		[Required(ErrorMessage = "Vul een emailadres in")]
		[MaxLength(200, ErrorMessage = "Het emailadres mag maximaal 200 karakters bevatten")]
		[EmailAddress(ErrorMessage = "Vul geldig een emailadres in")]
		public string fromEmail { get; set; }


		[Required(ErrorMessage = "Vul een bericht in")]
		[MaxLength(600, ErrorMessage = "Het bericht mag maximaal 600 karakters bevatten")]
		public string Message { get; set; }
		public DateTime TimeSend { get; set; }


	}
}
