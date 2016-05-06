using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//uttariya bandhu
namespace WebApplication1
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var g = Request.QueryString["Id"];
            var x = Request.QueryString["donorId"];
            string pass = key.pass;
            string responsetxt;
            {
                WebRequest request = WebRequest.Create(new Uri("http://beplus.azure-mobile.net/api/honorBloodRequest?Id=" + g.ToString()+"&donorId="+x.ToString()));
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
                        responsetxt =reader.ReadToEnd();
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
                            responsetxt =reader.ReadToEnd();

                        }
                    }
                    var results = JsonConvert.DeserializeObject<dynamic>(responsetxt);
                    Label1.Text = results.message;
                }

                

            }
        }
    }
}