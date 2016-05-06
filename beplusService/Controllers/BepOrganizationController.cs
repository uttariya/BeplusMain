using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using beplusService.DataObjects;
using beplusService.Models;
using System.Collections.Generic;
using AutoMapper;

namespace beplusService.Controllers
{
    public class BepOrganizationController : TableController<BepOrganization>
    {

        beplusContext context = new beplusContext();
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DomainManager = new EntityDomainManager<BepOrganization>(context, Request, Services);
        }

        // GET tables/BepOrganization
        public ICollection<BepOrganizationDTO> GetAllBepOrganization()
        {
            var orglist = Query().ToList();
            List<BepOrganizationDTO> dtolist = new List<BepOrganizationDTO>();
            foreach (BepOrganization org in orglist)
                dtolist.Add(Mapper.Map<BepOrganizationDTO>(org));
            return dtolist;

        }

        // GET tables/BepOrganization/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public BepOrganizationDTO GetBepOrganization(string id)
        {
            var org = Lookup(id).Queryable.SingleOrDefault();
            BepOrganizationDTO orgdto = Mapper.Map<BepOrganizationDTO>(org);
            return orgdto;
        }

        // PATCH tables/BepOrganization/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<BepOrganization> PatchBepOrganization(string id, Delta<BepOrganization> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/BepOrganization
        public async Task<IHttpActionResult> PostBepOrganization(BepOrganization item)
        {
            BepOrganization current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/BepOrganization/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteBepOrganization(string id)
        {
            return DeleteAsync(id);
        }
        [Route("api/registerOrganization", Name = "RegisterNewOrganization")]
        public async Task<IHttpActionResult> RegisterNewOrganization(BepOrganization organization)
        {
            // Does the Organization data exist?
            var count = context.BepOrganizations.Where(x => x.Phone == organization.Phone).Count();
            if (count > 0)
            {
                return BadRequest("Phone number already registered!");
            }
            count = context.BepOrganizations.Where(x => x.Email == organization.Email).Count();
            if (count > 0)
            {
                return BadRequest("Email Id already registered!");
            }
            //Provision to send out activation email. Until implemented, the activation status will be true for all registering parties
            

            BepOrganization current = await InsertAsync(organization);
            string body = "<!DOCTYPE html><html><head></head><body><div style=\"background-color:#800000;padding:20px\"><h1 style=\"color:white \">Welcome!</h1></div><p>please click <a title=\"here\" href=\"http://bplusemailverify.azurewebsites.net/Webform1.aspx?type=2&userid=" + current.Id + "\" target=\"_blank\">here</a>to register yourself successfully.</p></body></html>";
            Sender.SendMail(current.Email, "Please Activate Your Account", body);
            return Ok("Organization registered successfully!");
        }
        [Route("api/activateOrganization", Name = "ActivateOrganization")]
        [HttpGet]
        public async Task<IHttpActionResult> ActivateOrganization(string Id)
        {
            var count = context.BepOrganizations.Where(x => (x.Id == Id && x.Activated == true)).Count();
            if (count == 1)
            {
                return BadRequest("You are already registered. Please login using our application.");
            }
            count = context.BepOrganizations.Where(x => (x.Id == Id && x.Activated == false)).Count();
            if (count == 0)
            {
                return BadRequest("An error has occured. Please try registering again.");
            }
            else
            {
                using (var db = new beplusContext())
                {
                    BepOrganization org = db.BepOrganizations.SingleOrDefault(x => x.Id == Id);
                    org.Activated = true;
                    db.Entry(org).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return Ok("Your account has been activated! Please login using our app.");
            }
        }
        [Route("api/loginOrganization", Name = "LoginOrganization")]
        public IHttpActionResult LoginOrganization(LoginData logindata)
        {
            // Does the Organization data exist?
            List<BepOrganization> orglist;
            if (logindata.Phone != null)
                orglist = context.BepOrganizations.Where(x => (x.Phone == logindata.Phone && x.Password == logindata.Password)).ToList();
            else orglist = context.BepOrganizations.Where(x => (x.Email == logindata.Email && x.Password == logindata.Password)).ToList();
            int count = orglist.Count;
            if (count == 1)
            {
                return Ok(Mapper.Map<BepOrganizationDTO>(orglist[0]));
            }
            else return BadRequest("Invalid login credentials!");
        }
    }
}
