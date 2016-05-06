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
        {   //date check working
            DateTime date = (DateTime.Today.AddMinutes(330)).Date;
            var c = ((bepEventDTO.Date).Date).CompareTo(date);
            if (c == -1)
                return BadRequest("please give valid dates given date is earlier than todays date");
            //ends here
            //check singular event or not
            
                var count = context.BepEvents.Where(x => (x.OrgId == bepEventDTO.OrgId)) ;
                foreach(BepEvent a in count)
                {
                    if ((a.Date).Date.Equals((bepEventDTO.Date).Date))
                        return BadRequest(" An event posted by you is already scheduled on  the given date");
                }
            
            //
            BepOrganization organization = context.BepOrganizations.SingleOrDefault(x => x.Id == bepEventDTO.OrgId);
            List<BepDonor> offlineDonorList = context.BepDonors.Where(x => (x.OrgId == organization.Id
                                                                        && x.OnlineStatus == false)).ToList();
            double kms = 5;
            double ulat = organization.LocationLat + (kms / 110.574), llat = organization.LocationLat - (kms / 110.574);
            double longdist = Math.Cos((Math.PI * organization.LocationLat) / 180) * 111.320;
            double ulng = organization.LocationLong + (kms / longdist), llng = organization.LocationLong - (kms / longdist);

            List<BepDonor> onlineDonorList = context.BepDonors.Where(x => ((x.OrgId == organization.Id ||
            (x.LocationLat < ulat && x.LocationLat > llat && x.LocationLong < ulng && x.LocationLong > llng)) && x.OnlineStatus == true)).ToList();

            List<BepDonor> donorList = offlineDonorList;
            donorList.AddRange(onlineDonorList);
            
            //Send Mail to offline donor with event details
            foreach (BepDonor donor in donorList)
            {
                //Send mail with get query of the event id that will display the event details on a webapp based on the response
                string mail = "<!DOCTYPE html><html><head><style>table, th, td {border:1px solid black;border-collapse:collapse;}th, td {padding:5px;}</style></head><body><div style=\"border:5px solid #800000; padding:10px\"><div style=\"background-color:#800000;padding:20px\"><h1 style=\"color:white \">Welcome!</h1></div><p> dear" + " " + donor.Name + ",</p><p> you are cordially invited to this event.please grace us with the your presence. The details are as given below.</br>Thank you.</p><table style=\"width:100%\"><tbody><tr><td>organization</td><td>" +
                    bepEventDTO.OrgName + "</td></tr><tr><td>venue</td><td>" + bepEventDTO.Venue + "</td></tr><tr><td>about</td><td>" + bepEventDTO.About + "</td></tr><tr><td>date</td><td>" + (bepEventDTO.Date).ToString() + "</td></tr><tr><td>phone</td><td>" + bepEventDTO.OrgPhone + "</td></tr><tr><td>email</td><td>" + bepEventDTO.OrgEmail + "</td></tr></tbody></table></div></body></html>";

                Sender.SendMail(donor.Email, "Event details", mail);
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
