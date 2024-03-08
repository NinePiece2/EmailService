using EmailService.Models;
using System.Net.Mail;
using System.Configuration;

namespace EmailService
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                using (EmailServiceEntities entity = new EmailServiceEntities())
                {
                    while (true)
                    {
                        if (entity.VwMessageQueues.Count() > 0)
                        {
                            await EmailRun();
                        }
                        await Task.Delay(5000);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await SendAlertEmail(e);
            }
        }

        static async Task EmailRun()
        {
            try
            {
                using (EmailServiceEntities entity = new EmailServiceEntities())
                {
                    var messageQueue = entity.VwMessageQueues.ToList();

                    var emailsDictionary = messageQueue.GroupBy(e => e.IncomingMessageId).ToDictionary(e => e.Key, e => e.ToList());

                    foreach (var dictItem in emailsDictionary)
                    {
                        var incomingMessageId = dictItem.Key;
                        var messages = dictItem.Value;
                        var sampleMessage = messages[0];
                        bool IsSecure = false;
                        bool IsImportantTag = false;
                        if (sampleMessage.IsSecure == true)
                        {
                            IsSecure = true;
                        }
                        if (sampleMessage.IsImportantTag == true)
                        {
                            IsImportantTag = true;
                        }
                        var toRecepients = String.Join(",", messages.Where(m => (m.IsBcc == false || m.IsBcc is null) && (m.IsCc == false || m.IsCc is null)).Select(m => m.RecepientEmail));
                        var ccRecepients = String.Join(",", messages.Where(m => m.IsCc == true).Select(m => m.RecepientEmail));
                        var bccRecepients = String.Join(",", messages.Where(m => m.IsBcc == true).Select(m => m.RecepientEmail));

                        var fromname = entity.IncomingMessageTexts.Where(x => x.Uid == incomingMessageId).Select(x => x.CreatedName).FirstOrDefault();
                        fromname = string.IsNullOrEmpty(fromname) ? null : fromname;

                        var fromemail = entity.IncomingMessageTexts.Where(x => x.Uid == incomingMessageId).Select(x => x.CreatedEmail).FirstOrDefault();

                        await sendMessagesInEmails(toRecepients, ccRecepients, bccRecepients, sampleMessage.Title ?? "", sampleMessage.BodyHtml ?? "", fromname, fromemail, IsSecure, IsImportantTag);

                        var messageRecepient1 = entity.IncomingMessageRecepients.Where(e => e.IncomingMessageId == sampleMessage.IncomingMessageId && e.IsProcessed != true).ToList();
                       
                        foreach (var messageRecepient in messageRecepient1)
                        {
                            messageRecepient.IsEmailSent = true;
                            messageRecepient.EmailSentDateTime = DateTime.Now;
                            messageRecepient.IsProcessed = true;
                        }
                       
                        entity.SaveChanges();
                    }

                }
            } catch (Exception e)
            {
                await SendAlertEmail(e);
            }
        }

        private static async Task sendMessagesInEmails(string to, string cc, string bcc, string Title, string BodyHtml, string fromname, string fromemail, bool IsSecure = false, bool IsImporantTag = false)
        {
            try
            {
                MailMessage mail = new MailMessage();
                
                mail.Body = BodyHtml;
                if (!String.IsNullOrEmpty(to))
                    mail.To.Add(to);
                if (!String.IsNullOrEmpty(cc))
                    mail.CC.Add(cc);
                if (!String.IsNullOrEmpty(bcc))
                    mail.Bcc.Add(bcc);

                mail.Subject = string.IsNullOrWhiteSpace(Title) ? "Test" : Title;
                mail.IsBodyHtml = true;

                if (IsImporantTag)
                {
                    mail.Headers.Add("Importance", "High");
                    mail.Headers.Add("X-Priority", "1");
                }

                if (fromname != null)
                {
                    SendEmailSmtpClient(mail, fromname);

                }
                else
                {
                    SendEmailSmtpClient(mail);
                }

            }
            catch (Exception e)
            {
                 await SendAlertEmail(e);
            }
        }

        private static void SendEmailSmtpClient(MailMessage mailMessage, string fromname = "Email Service")
        {
            try
            {
                mailMessage.From = new MailAddress("server@romitsagu.com", fromname);
                mailMessage.Sender = new MailAddress("server@romitsagu.com", fromname);
                SmtpClient client = new SmtpClient();
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("server@romitsagu.com", ConfigurationManager.AppSettings["Password"]);
                client.Port = 587;
                client.Host = "smtp.office365.com";
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.Send(mailMessage);
            }
            catch (Exception e)
            {
                SendAlertEmail(e);
            }
        }
        private async static Task SendAlertEmail(Exception ex, string application = "Email Service - Errors")
        {
            var message = new MailMessage();
            message.To.Add("romit.sagu@hotmail.com");
            message.Headers.Add("Importance", "High");
            message.Headers.Add("X-Priority", "1");
            message.Subject = "Exception in " + application;
            message.Body = GetBody(ex, application);
            message.IsBodyHtml = true;
            SendEmailSmtpClient(message, application);

        }

        private static string GetBody(Exception ex, string application = "Email Service")
        {
            try
            {
                string body;
                body = "<b>Application: </b>" + application + " <br><br>";
                body += "<b>Exception:</b> " + ex.Message + " <br><br>";
                body += "<br><br>";
                body += "<b>Stack Trace:</b><br>" + ex.StackTrace.Replace(Environment.NewLine, "<br>") + "<br><br>";
                body += "<br><br>";

                if (ex.InnerException != null)
                    body += "<b>Inner Exception:</b> " + ex.InnerException.Message + " ";
                return body;
            }
            catch (Exception e)
            {
                return "Getting Messages out of Exception Property.";
            }
        }
    }

}
