using NLog;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace BSN.Resa.Commons.General
{
    public interface EmailDispatcher
    {
        bool Dispatch(MailAddress sender, MailAddress[] receivers, string subject, string body, bool isBodyHtml, Attachment[] attachments);
    }

    public class SMTPEmailDispatcher : EmailDispatcher
    {
        private readonly Logger _logger;
        private readonly SMTPConfiguration _configuration;

        public SMTPEmailDispatcher(SMTPConfiguration configuration)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _configuration = configuration;
        }

        public bool Dispatch(MailAddress sender, MailAddress[] receivers, string subject, string body, bool isBodyHtml, Attachment[] attachments)
        {
            try
            {
                _logger.Info("SMTP Message");

                var client = new SmtpClient(_configuration.Host, _configuration.Port);
                client.Credentials = new NetworkCredential(_configuration.Username, _configuration.Password);
                client.EnableSsl = _configuration.SSLEnabled;
                
                var message = new MailMessage();
                
                message.From = sender;
                message.Sender = sender;
                
                foreach (MailAddress address in receivers)
                    message.To.Add(address);
                
                message.Subject = subject;
                
                message.Body = body;
                message.IsBodyHtml = isBodyHtml;
                
                foreach (Attachment attachment in attachments)
                    message.Attachments.Add(attachment);

                _logger.Info($"From: {sender.DisplayName} <{sender.Address}>");
                _logger.Info($"To: {0}", string.Join(", ", receivers.Select(x => $"{x.DisplayName} <{x.Address}>")));
                _logger.Info($"Subject: {subject}");
                _logger.Info($"Body: {subject}");
                _logger.Info($"Attachments: {0}", string.Join(", ", attachments.Select(x => $"{x.Name} ({x.ContentType})")));

                client.Send(message);

                _logger.Info("=> OK");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Info($"=> FAILURE ({ex.Message}).");
                return false;
            }
        }
    }

    public class SMTPConfiguration
    {
        internal string Host;

        internal int Port;

        internal string Username;

        internal string Password;

        internal bool SSLEnabled;

        public SMTPConfiguration(string host, int port, string username, string password, bool ssl = true)
        {
            if (host == string.Empty)
                throw new ArgumentException("Host can't be blank.");

            if (port < 0 || port > 65536)
                throw new ArgumentOutOfRangeException("Port should be between 0 and 65536.");

            if (username == string.Empty)
                throw new ArgumentException("Username can't be blank.");

            Host = host;
            Port = port;
            Username = username;
            Password = password;
            SSLEnabled = ssl;
        }
    }
}
