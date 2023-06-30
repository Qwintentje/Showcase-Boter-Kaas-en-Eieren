using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using Tasker_Opdracht_MVC.Data.Entities;

namespace Tasker_Opdracht_MVC.Controllers;

public class EmailController : Controller
{
    [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<ActionResult> Send(Email model)
    {
        try
        {
            string toEmail = "ohqwintentje@gmail.com";
            string hostEmail = "quintenvnimwegen@gmail.com";
            string? apiKey = "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";
            string? recaptchaResponse = Request.Form["g-recaptcha-response"];
            model.TimeSend = DateTime.Now;

            if (!ModelState.IsValid)
            {
                string? errorMessage = ModelState.Values
                    .Where(v => v.Errors.Any())
                    .Select(v => v.Errors[0].ErrorMessage)
                    .FirstOrDefault();

                TempData["ErrorMessage"] = errorMessage ?? "Er is een onbekende validatiefout opgetreden.";
                return View("~/Views/Home/Contact.cshtml", model);
            }

            if (!await Task.Run(() => ValidateRecaptcha(recaptchaResponse)))
            {
                TempData["ErrorMessage"] = "De captcha is niet juist ingevuld";
                return View("~/Views/Home/Contact.cshtml", model);
            }
            else
            {
                bool isSent = await SendEmail(apiKey, toEmail, hostEmail, model.fromEmail, model.Subject, model.Message, model.TimeSend);
                if (!isSent)
                {
                    TempData["ErrorMessage"] = "Er is iets fout gegaan, probeer het later opnieuw";
                    return View("~/Views/Home/Contact.cshtml", model);
                }
                else
                {
                    bool isStored = await StoreForm(model);
                    if (!isStored)
                    {
                        TempData["ErrorMessage"] = "Er is iets mis met de DataBase2";
                        return View("~/Views/Home/Contact.cshtml", model);
                    }
                    TempData["SuccessMessage"] = "De mail is succesvol verzonden";
                }
            }
            return View("~/Views/Home/Contact.cshtml");
        }
        catch (HttpRequestException ex)
        {
            TempData["ErrorMessage"] = ex;
            return View("~/Views/Home/Contact.cshtml", model);
        }
    }

    #region SendMail methods
    //Sendmail for form
    public async Task<bool> SendEmail(string? apiKey, string toEmail, string hostEmail, string fromEmail, string subject, string body, DateTime timeSend)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            var message = new HttpRequestMessage(HttpMethod.Post, "https://api.sendgrid.com/v3/mail/send");
            message.Content = new StringContent($@"{{""personalizations"":[{{""to"":[{{""email"":""{toEmail}""}}]}}],""from"":{{""email"":""{hostEmail}""}},""subject"":""{subject}"",""content"":[{{""type"":""text/plain"",""value"":""Bericht ontvangen van: {fromEmail} op {timeSend.ToString("dddd, dd MMMM yyyy")}\n\n{body}""}}]}}", Encoding.UTF8, "application/json");
            var response = await client.SendAsync(message);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
        }
        return true;
    }

    //Sendmail for forgot password
    public static async Task<bool> SendEmail(string toEmail, string emailMessage)
    {
        string hostEmail = "quintenvnimwegen@gmail.com";
        string? apiKey = /*Environment.GetEnvironmentVariable("SENDGRID_API_KEY");*/ "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";
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            var message = new HttpRequestMessage(HttpMethod.Post, "https://api.sendgrid.com/v3/mail/send");
            message.Content = new StringContent($@"{{""personalizations"":[{{""to"":[{{""email"":""{toEmail}""}}]}}],""from"":{{""email"":""{hostEmail}""}},""subject"":""Wachtwoord resetten"",""content"":[{{""type"":""text/plain"",""value"":""{emailMessage}\n\n""}}]}}", Encoding.UTF8, "application/json");
            var response = await client.SendAsync(message);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
        }
        return true;
    }

    #endregion

    //Validate Recaptcha
    private async Task<bool> ValidateRecaptcha(string recaptchaResponse)
    {
        var _recaptchaSecretKey = "6LclT4ckAAAAABZf8mivpyj85G9tU4F80RjaZK6t";

        using (var client = new HttpClient())
        {
            var result = client.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={_recaptchaSecretKey}&response={recaptchaResponse}").Result;
            if (result.IsSuccessStatusCode)
            {
                var content = result.Content.ReadAsStringAsync().Result;
                if (JObject.Parse(content) is JObject jsonObject)
                {
                    var successToken = jsonObject.GetValue("success");
                    return successToken != null ? successToken.ToObject<bool>() : false;
                }
            }
        }
        return false;
    }

    //Store form into database
    private async Task<bool> StoreForm(Email model)
    {
        try
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7009");

                var response = await client.PostAsJsonAsync("api/EmailApi", model);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Er is iets mis met de DataBase";
                    Console.WriteLine(content);
                    return false;
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }
}
