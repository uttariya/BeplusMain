using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using beplusService.DataObjects;
using beplusService.Models;

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
            //TODO Check if emergency or non emergency, write code to increase the radius accordingly as per given time using jobs/threads
            //in each loop find only the online and activated donors within the radius with location of the inserted bloodrequest object (current)
            //send the mail to donors by checking their RecieverGroups attributes by checking if the recipients
            //blood type matches any of donor's reciever groups
            //the email should contain a get request link with
            //bloodrequest object id (current.Id) and donorID that will call a the get API HonorBloodRequest (the function below this)
            //with the bloodrequestId and donorID. In the email pls mention details like blood request location, recipient name, timing duration,etc

            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }
        [Route("api/activateOrganization", Name = "ActivateOrganization")]
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
        
    }
}