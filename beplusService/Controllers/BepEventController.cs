using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using beplusService.DataObjects;
using beplusService.Models;
using System.Collections.Generic;
using System.Device.Location;

namespace beplusService.Controllers
{
    public class BepEventController : TableController<BepEvent>
    {

        beplusContext context = new beplusContext();
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DomainManager = new EntityDomainManager<BepEvent>(context, Request, Services);
        }

        // GET tables/BepEvent
        public IQueryable<BepEvent> GetAllBepEvent()
        {
            return Query(); 
        }

        // GET tables/BepEvent/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<BepEvent> GetBepEvent(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/BepEvent/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<BepEvent> PatchBepEvent(string id, Delta<BepEvent> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/BepEvent
        public async Task<IHttpActionResult> PostBepEvent(BepEvent item)
        {
            BepEvent current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/BepEvent/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteBepEvent(string id)
        {
             return DeleteAsync(id);
        }
        
        [Route("api/createEvent", Name = "CreateEvent")]
        public async Task<IHttpActionResult> CreateEvent(BepEventDTO bepEventDTO)
        {
            BepOrganization organization = context.BepOrganizations.SingleOrDefault(x => x.Id == bepEventDTO.OrgId);
            List<BepDonor> offlineDonorList = context.BepDonors.Where(x => (x.OrgId == organization.Id 
                                                                        && x.OnlineStatus ==false)).ToList();
            var OrgLocation = new GeoCoordinate(organization.LocationLat, organization.LocationLong);
            List<BepDonor> onlineDonorList = context.BepDonors.Where(x => ((x.OrgId == organization.Id ||
            (new GeoCoordinate(x.LocationLat,x.LocationLong)).GetDistanceTo(OrgLocation) < 5000 )&& x.OnlineStatus == true)).ToList();

            List<BepDonor> donorList = offlineDonorList;
            donorList.AddRange(onlineDonorList);
            //COMPLETE THIS CODE
            //Send Mail to offline donor with event details
            foreach (BepDonor donor in donorList)
                Sender.SendMail(donor.Email, "Event details", "You have been invited to a blood drive");
            //End send mail code
            foreach (BepDonor donor in onlineDonorList)
                //Send push message
                ;
            BepEvent bepEvent = AutoMapper.Mapper.Map<BepEvent>(bepEventDTO);
            BepEvent current = await InsertAsync(bepEvent);
            return CreatedAtRoute("CreateEvent", new { id = current.Id }, current);
        }
        [Route("api/getAllOrgEvents", Name = "GetAllOrgEvents")]
        [HttpGet]
        public List<BepEventDTO> GetAllOrgEvents(string OrgId)
        {
            List<BepEvent> eventlist = context.BepEvents.Where(x => x.OrgId == OrgId).ToList();
            List<BepEventDTO> eventdtolist = new List<BepEventDTO>();
            foreach (BepEvent bepevent in eventlist)
                eventdtolist.Add(AutoMapper.Mapper.Map<BepEventDTO>(bepevent));

            return eventdtolist;
        }
    }
}