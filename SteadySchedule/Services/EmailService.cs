using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var host = _config["Email:Host"];
        var port = int.Parse(_config["Email:Port"] ?? "587");
        var username = _config["Email:Username"];
        var password = _config["Email:Password"];
        var fromEmail = _config["Email:FromEmail"];

        var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true,
            UseDefaultCredentials = false
        };

        var mail = new MailMessage();
        mail.From = new MailAddress(fromEmail, "SteadySchedule");
        mail.To.Add(toEmail);
        mail.Subject = subject;
        mail.Body = body;

        await client.SendMailAsync(mail);
    }

    public async Task SendInviteEmail(string toEmail, string code)
    {
        var baseUrl = _config["App:BaseUrl"];
        var joinLink = $"{baseUrl}/Account/Join?code={code}";

        var subject = "You're invited to SteadySchedule";

        var body = $@"
            Welcome to SteadySchedule!

            You’ve been invited to join your team.

            Use this invite code:

            CODE: {code}

            Or click this link:
            {joinLink}

            (This code expires in 24 hours)
            ";

        await SendEmailAsync(toEmail, subject, body);
    }
}