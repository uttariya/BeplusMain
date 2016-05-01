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

namespace WebApplication1
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var g = Request.QueryString["userid"];
            var x = Request.QueryString["type"];
            string pass = "MTSpEOSVNXNxGyoVoeZMqnnIIGBqmk93";
            string responsetxt;
            if ((x.ToString()).Equals("1"))
            {
                WebRequest request = WebRequest.Create(new Uri("http://beplus.azure-mobile.net/api/activateDonor?Id=" + g.ToString()));
                String encoded = System.Convert.ToBase64String(Encoding.ASCII.GetBytes("" + ":" + pass));
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Method = "GET";
                request.Headers.Add("Authorization", "Basic " + encoded);
                request.PreAuthenticate = true;
                try {
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
                            responsetxt =reader.ReadToEnd();
                            
                        }
                    }
                    var results = JsonConvert.DeserializeObject<dynamic>(responsetxt);
                    Label1.Text = results.message;
                }

                

            }
            else if ((x.ToString()).Equals("2"))
            {
                //Bug fix for "Error: 500 No Resource Found" during Organization/Community Registration
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
                            responsetxt =reader.ReadToEnd();
                            
                        }
                    }
                    var results = JsonConvert.DeserializeObject<dynamic>(responsetxt);
                    Label1.Text = results.message;
                }

                
            }
            else
                Label1.Text ="malformed URL";
        }
    }
}