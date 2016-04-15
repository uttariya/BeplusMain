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
using System;

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
            double kms = 5;
            double ulat = organization.LocationLat + (kms / 110.574), llat = organization.LocationLat - (kms / 110.574);
            double longdist = Math.Cos((Math.PI * organization.LocationLat) /180)* 111.320;
            double ulng = organization.LocationLong + (kms / longdist), llng = organization.LocationLong - (kms / longdist);
            
            List<BepDonor> onlineDonorList = context.BepDonors.Where(x => ((x.OrgId == organization.Id ||
            (x.LocationLat < ulat && x.LocationLat > llat && x.LocationLong < ulng && x.LocationLong > llng))&& x.OnlineStatus == true)).ToList();

            List<BepDonor> donorList = offlineDonorList;
            donorList.AddRange(onlineDonorList);
            //COMPLETE THIS CODE
            //Send Mail to offline donor with event details
            foreach (BepDonor donor in donorList)
            {
                //Send mail with get query of the event id that will display the event details on a webapp based on the response
                //Sender.SendMail(donor.Email, "Event details", "You have been invited to a blood drive");
            }
            //End send mail code
            foreach (BepDonor donor in onlineDonorList)
                //Send push message
                ;
            BepEvent bepEvent = AutoMapper.Mapper.Map<BepEvent>(bepEventDTO);
            BepEvent current = await InsertAsync(bepEvent);
            return CreatedAtRoute("CreateEvent", new { id = current.Id }, AutoMapper.Mapper.Map<BepEventDTO>(current));
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