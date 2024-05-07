using EmailService.Models;
using System.Net.Mail;
using System.Configuration;
using sibAPI = sib_api_v3_sdk.Api;
using sibClient = sib_api_v3_sdk.Client;
using sibModel = sib_api_v3_sdk.Model;
using Newtonsoft.Json.Linq;

namespace EmailService
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                using (EmailServiceContext entity = new EmailServiceContext())
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
                Console.WriteLine(e);
                await SendAlertEmail(e);
            }
        }

        static async Task EmailRun()
        {
            try
            {
                using (EmailServiceContext entity = new EmailServiceContext())
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
                if (!IsSecure)
                {
                    sibModel.SendSmtpEmail mail = new sibModel.SendSmtpEmail();
                    mail.Subject = Title;
                    mail.HtmlContent = BodyHtml;

                    if (!String.IsNullOrEmpty(to)) {
                        List<sibModel.SendSmtpEmailTo> recipients = new List<sibModel.SendSmtpEmailTo>();

                        string[] emails = to.Split(',');

                        foreach(var email in emails)
                        {
                            recipients.Add(new sibModel.SendSmtpEmailTo(email));
                        }
                        mail.To = recipients;
                    }

                    if (!String.IsNullOrEmpty(cc))
                    {
                        List<sibModel.SendSmtpEmailCc> recipients = new List<sibModel.SendSmtpEmailCc>();

                        string[] emails = cc.Split(',');

                        foreach (var email in emails)
                        {
                            recipients.Add(new sibModel.SendSmtpEmailCc(email));
                        }
                        mail.Cc = recipients;
                    }

                    if (!String.IsNullOrEmpty(bcc))
                    {
                        List<sibModel.SendSmtpEmailBcc> recipients = new List<sibModel.SendSmtpEmailBcc>();

                        string[] emails = bcc.Split(',');

                        foreach (var email in emails)
                        {
                            recipients.Add(new sibModel.SendSmtpEmailBcc(email));
                        }
                        mail.Bcc = recipients;
                    }

                    if (IsImporantTag)
                    {
                        JObject Headers = new JObject();
                        Headers.Add("Importance", "High");
                        Headers.Add("X-Priority", "1");
                        mail.Headers = Headers;
                    }


                    if (fromname != null && fromemail != null)
                    {

                        SendEmailBrevo(mail, fromname, fromemail);
                    }
                    else
                    {
                        SendEmailBrevo(mail);
                    }
                }
                else
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

                    if (fromname != null && fromemail != null)
                    {
                        SendEmailSmtpClient(mail, fromname, fromemail);

                    }
                    else
                    {
                        SendEmailSmtpClient(mail);
                    }

                }
            }
            catch (Exception e)
            {
                await SendAlertEmail(e);
            }
        }

        private static void SendEmailSmtpClient(MailMessage mailMessage, string fromname = "Email Service", string fromeemail = "server@romitsagu.com")
        {
            try
            {
                EmailServiceContext entity = new EmailServiceContext();

                mailMessage.From = new MailAddress(fromeemail, fromname);
                mailMessage.Sender = new MailAddress(fromeemail, fromname);

                SmtpClient client = new SmtpClient();
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(fromeemail, entity.Credentials.Where(c => c.UserName == fromeemail).Select(c => c.Password).FirstOrDefault());
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

        private static void SendEmailBrevo(sibModel.SendSmtpEmail mail, string fromname = "Email Service", string fromeemail = "server@romitsagu.com")
        {
            try
            {
                Console.Log(ConfigurationManager.AppSettings["BrevoAPIKey"]);
                sibClient.Configuration.Default.AddApiKey("api-key", ConfigurationManager.AppSettings["BrevoAPIKey"]);
                sibAPI.TransactionalEmailsApi apiInstance = new sibAPI.TransactionalEmailsApi();

                mail.Sender = new sibModel.SendSmtpEmailSender(fromname, fromeemail);
                mail.ReplyTo = new sibModel.SendSmtpEmailReplyTo("noreply@romitsagu.com", "No-Reply");

                sibModel.CreateSmtpEmail result = apiInstance.SendTransacEmail(mail);
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
