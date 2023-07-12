using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Configuration;
using System.Net;

namespace HandShakeService
{
    public class EmailService
    {
        public void SendGMail(string To_Address, string Subject, string EmailBody, string Cc_Address = default(string), string Bcc_Address = default(string))
        {
            try
            {
                var DisplayName = "";
                var From_Address = "";
                var EmailPassword = "";
                From_Address = ConfigurationManager.AppSettings["ServiceNotificationAddress"];
                EmailPassword = ConfigurationManager.AppSettings["ServiceNotificationPassword"];
                DisplayName = "Captorra Service Notifications";

                var Password = "";
                byte[] data = System.Convert.FromBase64String(EmailPassword);
                Password = System.Text.ASCIIEncoding.ASCII.GetString(data);

                MailAddress From = new MailAddress(From_Address, DisplayName);
                MailAddress To = new MailAddress(To_Address);
                MailMessage objMailMessage = new MailMessage(From, To);
                objMailMessage.Subject = Subject;
                objMailMessage.Body = EmailBody;
                //objMailMessage.IsBodyHtml = isHTML;

                //Multiple CC address seperated by ;
                if (!string.IsNullOrEmpty(Cc_Address))
                {
                    foreach (var address in Cc_Address.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        MailAddress cc = new MailAddress(address);
                        objMailMessage.CC.Add(cc);
                    }
                }

                if (!string.IsNullOrEmpty(Bcc_Address))
                {
                    MailAddress bcc = new MailAddress(Bcc_Address);
                    objMailMessage.Bcc.Add(bcc);
                }
                SmtpClient objSmtpClient = new SmtpClient();

                var cred = new NetworkCredential(From_Address, Password);
                objSmtpClient.Host = "HOSTNAME";
                objSmtpClient.Port = 0;
                objSmtpClient.EnableSsl = true;
                objSmtpClient.Credentials = cred;

                try
                {
                    objSmtpClient.Send(objMailMessage);
                    objMailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
                }
                catch (SmtpException ex)
                {
                    throw new System.Net.Mail.SmtpException(ex.Message);
                }
                finally
                {
                    objSmtpClient.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SendGMail(string EmailBody)
        {
            try
            {
                var DisplayName = "";
                var From_Address = "";
                var EmailPassword = "";
                var To_Address = "";
                var Cc_Address = "";
                var Bcc_Address = "";
                var Subject = "";

                From_Address = ConfigurationManager.AppSettings["ServiceNotificationAddress"];
                To_Address = ConfigurationManager.AppSettings["To_Address"];
                Cc_Address = ConfigurationManager.AppSettings["Cc_Address"];
                Bcc_Address = ConfigurationManager.AppSettings["Bcc_Address"];

                if(EmailBody == "")
                {
                    EmailBody = ConfigurationManager.AppSettings["EmailBody"];
                }

                Subject = ConfigurationManager.AppSettings["Subject"];
                EmailPassword = ConfigurationManager.AppSettings["ServiceNotificationPassword"];
                DisplayName = "Captorra Service Notifications";

                var Password = "";
                byte[] data = System.Convert.FromBase64String(EmailPassword);
                Password = System.Text.ASCIIEncoding.ASCII.GetString(data);

                MailAddress From = new MailAddress(From_Address, DisplayName);
                MailAddress To = new MailAddress(To_Address);
                MailMessage objMailMessage = new MailMessage(From, To);
                objMailMessage.Subject = Subject;
                objMailMessage.Body = EmailBody;
                //objMailMessage.IsBodyHtml = isHTML;

                //Multiple CC address seperated by ;
                if (!string.IsNullOrEmpty(Cc_Address))
                {
                    foreach (var address in Cc_Address.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        MailAddress cc = new MailAddress(address);
                        objMailMessage.CC.Add(cc);
                    }
                }

                if (!string.IsNullOrEmpty(Bcc_Address))
                {
                    MailAddress bcc = new MailAddress(Bcc_Address);
                    objMailMessage.Bcc.Add(bcc);
                }
                SmtpClient objSmtpClient = new SmtpClient();

                var cred = new NetworkCredential(From_Address, Password);
                objSmtpClient.Host = "HOSTNAME";
                objSmtpClient.Port = 0;
                objSmtpClient.EnableSsl = true;
                objSmtpClient.Credentials = cred;

                try
                {
                    objSmtpClient.Send(objMailMessage);
                    objMailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
                }
                catch (SmtpException ex)
                {
                    throw new System.Net.Mail.SmtpException(ex.Message);
                }
                finally
                {
                    objSmtpClient.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
