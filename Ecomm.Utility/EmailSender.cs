
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm.Utility
{
    public class EmailSender : IEmailSender
    {
        private EmailSettings _emailSetting { get; }
        public EmailSender(IOptions<EmailSettings> emailSetting)
        {
            _emailSetting = emailSetting.Value;
        }
        public async Task Execute(String email, string Subject, string message)
        {
            try
            {
                string toEmail = string.IsNullOrEmpty(email) ? _emailSetting.ToEmail : email;
                MailMessage mailMessage = new MailMessage()
                {
                    From = new MailAddress(_emailSetting.UsernameEmail, "My email Name"),
                };
                mailMessage.To.Add(toEmail);
                mailMessage.CC.Add(_emailSetting.CcEmail);
                mailMessage.Subject = "Shopping App:" + Subject;
                mailMessage.Body = message;
                mailMessage.IsBodyHtml = true;
                mailMessage.Priority = MailPriority.High;

                using (SmtpClient smtp = new SmtpClient(
                    _emailSetting.PrimaryDomain,
                    _emailSetting.PrimaryPort))
                {
                    smtp.Credentials = new NetworkCredential(
                        _emailSetting.UsernameEmail,
                        _emailSetting.UsernamePassword);

                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mailMessage);
                };
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Execute(email, subject, htmlMessage).Wait();
            return Task.FromResult(0);
        }
    }
}
