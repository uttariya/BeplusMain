using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using SendGrid;

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
                myMessage.From = new MailAddress("notifications@beplus.azure-mobile.net", "bePlus team");
                myMessage.Subject = subject;
                myMessage.Text = body;
                //var apikey = "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";
                var transportWeb = new Web(new NetworkCredential("azure_516c1b4f721446b88e4658edda90e4e8@azure.com", "Nagin420"));
                transportWeb.Deliver(myMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
