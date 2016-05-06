using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
//author-uttariya bandhu
namespace WebApplication1
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var g = Request.QueryString["userid"];
            var x = Request.QueryString["type"];
            string pass = key.pass;
            string responsetxt;
            if ((x.ToString()).Equals("1"))
            {//request to activate donor
                WebRequest request = WebRequest.Create(new Uri("http://beplus.azure-mobile.net/api/activateDonor?Id=" + g.ToString()));
                String encoded = System.Convert.ToBase64String(Encoding.ASCII.GetBytes("" + ":" + pass));
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Method = "GET";
                request.Headers.Add("Authorization", "Basic " + encoded);
                //authenticating
                request.PreAuthenticate = true;
                try
                {//creating a reaponse object
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    //encoding the request 
                    var encoding = Encoding.GetEncoding(response.CharacterSet);
                    //requesting for response and reading the message
                    using (var responseStream = response.GetResponseStream())
                    using (var reader = new StreamReader(responseStream, encoding))
                        responsetxt = reader.ReadToEnd();
                    Label1.Text = responsetxt;
                }
                catch (WebException b)
                {//handling the exception if response is not OK 200
                    using (WebResponse response = b.Response)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;

                        using (Stream data = response.GetResponseStream())
                        using (var reader = new StreamReader(data))
                        {
                            responsetxt = reader.ReadToEnd();

                        }
                    }
                    var results = JsonConvert.DeserializeObject<dynamic>(responsetxt);
                    Label1.Text = results.message;
                }



            }
            else if ((x.ToString()).Equals("2"))
            {
                //Fixed "Error 500: No resource found" during organization registration.
                WebRequest request = WebRequest.Create(new Uri("http://beplus.azure-mobile.net/api/activateOrganization?Id=" + g.ToString()));
                String encoded = System.Convert.ToBase64String(Encoding.ASCII.GetBytes("" + ":" + pass));
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Method = "GET";
                request.Headers.Add("Authorization", "Basic " + encoded);
                request.PreAuthenticate = true;
                responsetxt = "";
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    var encoding = Encoding.GetEncoding(response.CharacterSet);
                    using (var responseStream = response.GetResponseStream())
                    using (var reader = new StreamReader(responseStream, encoding))
                        responsetxt = reader.ReadToEnd();
                    Label1.Text = responsetxt;
                }
                catch (WebException b)
                {
                    using (WebResponse response = b.Response)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;

                        using (Stream data = response.GetResponseStream())
                        using (var reader = new StreamReader(data))
                        {
                            responsetxt = reader.ReadToEnd();

                        }
                    }
                    var results = JsonConvert.DeserializeObject<dynamic>(responsetxt);
                    Label1.Text = results.message;
                }


            }
            else if ((x.ToString()).Equals("3"))
            {
                WebRequest request = WebRequest.Create(new Uri("http://beplus.azure-mobile.net/api/VerifyBloodRequest?Id=" + g.ToString()));
                String encoded = System.Convert.ToBase64String(Encoding.ASCII.GetBytes("" + ":" + pass));
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Method = "GET";
                request.Headers.Add("Authorization", "Basic " + encoded);
                request.PreAuthenticate = true;
                responsetxt = "";
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    var encoding = Encoding.GetEncoding(response.CharacterSet);
                    using (var responseStream = response.GetResponseStream())
                    using (var reader = new StreamReader(responseStream, encoding))
                        responsetxt = reader.ReadToEnd();
                    Label1.Text = responsetxt;
                }
                catch (WebException b)
                {
                    using (WebResponse response = b.Response)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;

                        using (Stream data = response.GetResponseStream())
                        using (var reader = new StreamReader(data))
                        {
                            responsetxt = reader.ReadToEnd();

                        }
                    }
                    var results = JsonConvert.DeserializeObject<dynamic>(responsetxt);
                    Label1.Text = results.message;
                }


            }
            else
                Label1.Text = "malformed URL";
        }
    }
}