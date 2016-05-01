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
        //Boolean donfound = false;
        protected override void Initialize(HttpControllerContext controllerContext)
        {   //initializing the controller context which connects to the database
            base.Initialize(controllerContext);
            beplusContext context = new beplusContext();
            DomainManager = new EntityDomainManager<BepBloodRequest>(context, Request, Services);
        }

        // GET tables/BepBloodRequest
        public IQueryable<BepBloodRequest> GetAllBepBloodRequest()
        {   //returns all the bloodRequests information as a json file
            return Query();
        }

        // GET tables/BepBloodRequest/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<BepBloodRequest> GetBepBloodRequest(string id)
        {   //returns the information of the bloodrequest identified by id
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
            var count = context.BepBloodRequests.Where(x => x.RecipientPhone == item.RecipientPhone && x.Honored == false).Count();
            if (count > 0)
            {
                return BadRequest("Phone number already registered a request,please wait!");
            }
            count = context.BepBloodRequests.Where(x => x.RecipientEmail == item.RecipientEmail && x.Honored==false).Count();
            if (count > 0)
            {
                return BadRequest("Email already registered a request,please wait!");
            }
            BepBloodRequest current = await InsertAsync(item);
            int time;
            //TODO Check if emergency or non emergency, write code to increase the radius accordingly as per given time using jobs/threads
            if (item.Emergency == true)
                time = 360000;
            else
                time = 7200000;
            //creating a new thread for sending out the requests
            Thread th = new Thread(() => funk(time, current));
            th.Start();
            //creating the new bloodrequest object and returning updated info
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }
        [Route("api/honorBloodRequest", Name = "HonorBloodRequest")]
        [HttpGet]
        public async Task<IHttpActionResult> HonorBloodRequest(string Id, string donorId)
        { //API call to accept a bloodrequest
            var count = context.BepBloodRequests.Where(x => (x.Id == Id && x.Honored == true)).Count();
            //checking if honored or not by checking if x.id matches and x.honored is true.if count is one ->honored
            if (count == 1)
            {
                //Display message that another donor has already accepted the blood request
                return BadRequest("Sorry request honored.");
            }
            else
            {
                using (var db = new beplusContext())
                {//storing the info of the registered donor
                    //donfound = true;
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
                    string msg = "<!DOCTYPE html><html><head></head><body><p>dear  " + bloodRequest.RecipientName + " ,</br>your request has been accepted.the donor name is " + donor.Name + ".please click <a href=\"http://maps.google.com/maps?q="+donor.LocationLat+","+donor.LocationLong+"\">here</a> to view donors current location</br>Thank you.</p></body></html>";
                    Sender.SendMail(bloodRequest.RecipientEmail, "Donor found!", msg);
                }
                //confirming the donor that his request has been accepted
                return Ok("Thank you for honoring blood request.");
            }
        }

        // DELETE tables/BepBloodRequest/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteBepBloodRequest(string id)
        {//delete requests,use only for debugging
            return DeleteAsync(id);
        }
        private void funk(int time, BepBloodRequest item)
        {
            int i = 4;
            double kms = 10;
            List<BepDonor> sentlist=new List<BepDonor> { };
            beplusContext db;
            while (i > 0)
            {
                db = new beplusContext();
                //var count = db.BepBloodRequests.Where(x => x.Id== item.Id && x.Honored==true).Count();
                item = db.BepBloodRequests.Single(x => x.Id == item.Id);
                //BepBloodRequest bloodRequest = Lookup(item.Id).Queryable.Single();
                //checking if blood request has been honored then it will return
                //Sender.SendMail("uttariya@outlook.com", "honored", "<!DOCTYPE html><html><head></head><body><p> id----->"+item.Id.ToString()+ "</p><p> name----->" + item.RecipientName.ToString() + "</p><p> honored----->" + item.Honored.ToString() + "</p><p> count----->" + count.ToString() + "</p></body></html>");

                if (item.Honored)
                {
                    return;
                }
                //checking based on distance given in variable kms.kms is increased by 10 every iteration
                double ulat = item.LocationLat + (kms / 110.574), llat = item.LocationLat - (kms / 110.574);
                double longdist = Math.Cos((Math.PI * item.LocationLat) / 180) * 111.320;
                double ulng = item.LocationLong + (kms / longdist), llng = item.LocationLong - (kms / longdist);
                List<BepDonor> onlineDonorList = (context.BepDonors.Where(x => ((
                (x.LocationLat < ulat && x.LocationLat > llat && x.LocationLong < ulng && x.LocationLong > llng)) && x.OnlineStatus == true && x.ReceiverGroups.Contains(","+item.BloodType))).ToList());
                var result = (onlineDonorList.Except(sentlist)).ToList();
                foreach (BepDonor donor in result)
                {
                     sentlist.Add(donor);
                    //Send mail with get query of the event id that will display the event details on a webapp based on the response
                    string mail = "<!DOCTYPE html><html><head><style>table, th, td {border:1px solid black;border-collapse:collapse;}th, td {padding:5px;}</style></head><body><div style=\"border:5px solid #800000; padding:10px\"><div style=\"background-color:#800000;padding:20px\"><h1 style=\"color:white \">Welcome!</h1></div><p> dear" + " " + donor.Name + ",</p><p> a person needs blood please help him in this time of need. The details are as given below.</br>Thank you.</p><table style=\"width:100%\"><tbody><tr><td>name</td><td>" +
                        item.RecipientName + "</td></tr><tr><td>amount</td><td>" + item.BloodUnits + "</td></tr><tr><td>type</td><td>" + item.BloodType + "</td></tr><tr><td>hospital name</td><td>" + item.HospitalName + "</td></tr><tr><td>hospital address</td><td>" + item.HospitalAddress + "</td></tr></tbody></table></div><div>To accept this request please click <a href=\"http://bplusemailverify.azurewebsites.net/Webform2.aspx?Id=" + item.Id + "&donorId=" + donor.Id + "\">here.</a></br>please click <a href=\"http://maps.google.com/maps?q=" + item.LocationLat + "," + item.LocationLong + "\">here</a> to view recipients current location</div></body></html>";
                    if(donor.Email!=item.RecipientEmail || donor.Phone!=item.RecipientPhone)
                    Sender.SendMail(donor.Email, "Donor details", mail);
                }
                //sleep based on time got from sender
                kms = kms + 10;
                i--;
                Thread.Sleep(time);
                //in each loop find only the online and activated donors within the radius with location of the inserted bloodrequest object (current)
                //send the mail to donors by checking their RecieverGroups attributes by checking if the recipients
                //blood type matches any of donor's reciever groups
                //the email should contain a get request link with
                //bloodrequest object id (current.Id) and donorID that will call a the get API HonorBloodRequest (the function below this)
                //with the bloodrequestId and donorID. In the email pls mention details like blood request location, recipient name, timing duration,etc
                
            }
            item.Honored = true;
            string end_mail = "<!DOCTYPE html><html><head><head><body><h1>HaHaHa!!! die bit**!!</h1></body></html>";
            Sender.SendMail(item.RecipientEmail, "sorry donor not found", end_mail);
        }
    }
    }

