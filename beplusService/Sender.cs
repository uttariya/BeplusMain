using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using SendGrid;
//uttariya bandhu
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
                myMessage.Html = body;
                //var apikey = "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";
                var transportWeb = new Web(new NetworkCredential("azure_a9b6040e6f10e841c60a8e2b7af87e82@azure.com", "Nagin420"));
                transportWeb.Deliver(myMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
