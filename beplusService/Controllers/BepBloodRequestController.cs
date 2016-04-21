using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using beplusService.DataObjects;
using beplusService.Models;
using System;
using System.Threading;
using System.Collections.Generic;

namespace beplusService.Controllers
{
    public class BepBloodRequestController : TableController<BepBloodRequest>
    {
        beplusContext context = new beplusContext();
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            beplusContext context = new beplusContext();
            DomainManager = new EntityDomainManager<BepBloodRequest>(context, Request, Services);
        }

        // GET tables/BepBloodRequest
        public IQueryable<BepBloodRequest> GetAllBepBloodRequest()
        {
            return Query(); 
        }

        // GET tables/BepBloodRequest/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<BepBloodRequest> GetBepBloodRequest(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/BepBloodRequest/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<BepBloodRequest> PatchBepBloodRequest(string id, Delta<BepBloodRequest> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/BepBloodRequest
        public async Task<IHttpActionResult> PostBepBloodRequest(BepBloodRequest item)
        {
            BepBloodRequest current = await InsertAsync(item);
            int time;
            //TODO Check if emergency or non emergency, write code to increase the radius accordingly as per given time using jobs/threads
            if (item.Emergency == true)
                time = 360000;
            else
                time = 7200000;
            Thread th = new Thread(() => funk(time,item));
            th.Start();
            
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }
        [Route("api/honorBloodRequest", Name = "HonorBloodRequest")]
        [HttpGet]
        public async Task<IHttpActionResult> HonorBloodRequest(string Id, string donorId)
        {
            var count = context.BepBloodRequests.Where(x => (x.Id == Id && x.Honored == true)).Count();
            if (count == 1)
            {
                //Display message that another donor has already accepted the blood request
                return BadRequest("Sorry request honored.");
            }
            else
            {
                using (var db = new beplusContext())
                {
                    BepBloodRequest bloodRequest = db.BepBloodRequests.SingleOrDefault(x => x.Id == Id);
                    bloodRequest.Honored = true;
                    BepDonor donor = db.BepDonors.Single(x => x.Id == donorId);
                    bloodRequest.DonorId = donor.Id;
                    bloodRequest.DonorEmail = donor.Email;
                    bloodRequest.DonorImgurl = donor.Imgurl;
                    bloodRequest.DonorName = donor.Name;
                    bloodRequest.DonorId = donor.Id;
                    db.Entry(bloodRequest).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    //Send the mail to the bloodRequest.recipientEmail saying the request was honored with name and location of the donor
                    string msg= "<!DOCTYPE html><html><head></head><body><p>dear" + bloodRequest.RecipientName + " ,</br>your request has been accepted.the donor name is " + donor.Name + " and you can contact him at " + donor.Phone + ".</br>Thank you.</p></body></html>";
                    Sender.SendMail(bloodRequest.RecipientEmail, "Donor found!", msg);
                }
                //Send a confirmation mail to the donor with the blood request hospital location and details of the recipient as well as display the requisit message below
                return Ok("Thank you for honoring blood request. Details have been mailed to you.");
            }
        }

        // DELETE tables/BepBloodRequest/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteBepBloodRequest(string id)
        {
             return DeleteAsync(id);
        }
        private void funk(int time, BepBloodRequest item)
        {
            int i = 4;
            double kms = 10;
            while (i > 0 && item.Honored!=true)
            {
                
                double ulat = item.LocationLat + (kms / 110.574), llat = item.LocationLat - (kms / 110.574);
                double longdist = Math.Cos((Math.PI * item.LocationLat) / 180) * 111.320;
                double ulng = item.LocationLong + (kms / longdist), llng = item.LocationLong - (kms / longdist);
                List<BepDonor> onlineDonorList = context.BepDonors.Where(x => ((
                (x.LocationLat < ulat && x.LocationLat > llat && x.LocationLong < ulng && x.LocationLong > llng)) && x.OnlineStatus == true /*&& x.BloodGroup.Equals(item.BloodType)*/)).ToList();
                foreach (BepDonor donor in onlineDonorList)
                {
                    //Send mail with get query of the event id that will display the event details on a webapp based on the response
                    string mail = "<!DOCTYPE html><html><head><style>table, th, td {border:1px solid black;border-collapse:collapse;}th, td {padding:5px;}</style></head><body><div style=\"border:5px solid #800000; padding:10px\"><div style=\"background-color:#800000;padding:20px\"><h1 style=\"color:white \">Welcome!</h1></div><p> dear" + " " + donor.Name + ",</p><p> a person needs blood please help him in this time of need. The details are as given below.</br>Thank you.</p><table style=\"width:100%\"><tbody><tr><td>name</td><td>" +
                        item.RecipientName + "</td></tr><tr><td>amount</td><td>" + item.BloodUnits + "</td></tr><tr><td>type</td><td>" + item.BloodType + "</td></tr><tr><td>hospital name</td><td>" + item.HospitalName+ "</td></tr><tr><td>hospital address</td><td>" + item.HospitalAddress + "</td></tr></tbody></table></div><div>To accept this request please click <a href=\"#\">here</a></div></body></html>";

                    Sender.SendMail(donor.Email, "Donor details", mail);
                }

                Thread.Sleep(time);
                //in each loop find only the online and activated donors within the radius with location of the inserted bloodrequest object (current)
                //send the mail to donors by checking their RecieverGroups attributes by checking if the recipients
                //blood type matches any of donor's reciever groups
                //the email should contain a get request link with
                //bloodrequest object id (current.Id) and donorID that will call a the get API HonorBloodRequest (the function below this)
                //with the bloodrequestId and donorID. In the email pls mention details like blood request location, recipient name, timing duration,etc
                kms = kms + 10;
                i--;
            }
        }
    }
}