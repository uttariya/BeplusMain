using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using SendGrid;
//author-uttariya bandhu
namespace beplusService
{
    class Sender
    {
        public static void SendMail(string Email, string subject, string body)
        {
            try
            {
                var myMessage = new SendGrid.SendGridMessage();
                myMessage.AddTo(Email);
                myMessage.From = new MailAddress("sender_address", "sender_name");
                myMessage.Subject = subject;
                myMessage.Html = body;
                //send mail;
                var transportWeb = new Web(new NetworkCredential("email", "pwd"));
                transportWeb.Deliver(myMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
