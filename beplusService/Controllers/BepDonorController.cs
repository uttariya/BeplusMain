﻿using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using beplusService.DataObjects;
using beplusService.Models;
using System.Collections.Generic;

namespace beplusService.Controllers
{
    public class BepDonorController : TableController<BepDonor>
    {
        beplusContext context = new beplusContext();
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DomainManager = new EntityDomainManager<BepDonor>(context, Request, Services);
        }

        // GET tables/BepDonor
        public IQueryable<BepDonorDTO> GetAllBepDonor()
        {
            return Query().Select(x => new BepDonorDTO()
            {
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                Email = x.Email,
                BloodGroup = x.BloodGroup,
                ReceiverGroups = x.ReceiverGroups,
                Allergies = x.Allergies,
                Diseases = x.Diseases,
                LocationLat = x.LocationLat,
                LocationLong = x.LocationLong,
                EmergencyAvailability = x.EmergencyAvailability,
                Subscribed = x.Subscribed,
                OnlineStatus = x.OnlineStatus,
                Activated = x.Activated,
                Imgurl = x.Imgurl,
                OrgId = x.BepOrganization.Id,
                OrgName = x.BepOrganization.Name,
                OrgAbout = x.BepOrganization.About,
                OrgImgurl = x.BepOrganization.Imgurl,
                OrgEmail = x.BepOrganization.Email,
                OrgPhone = x.BepOrganization.Phone,
                OrgLocality = x.BepOrganization.Locality
            });
        }

        // GET tables/BepDonor/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<BepDonorDTO> GetBepDonor(string id)
        {
            var result = Lookup(id).Queryable.Select(x => new BepDonorDTO()
            {
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                Email = x.Email,
                BloodGroup = x.BloodGroup,
                ReceiverGroups = x.ReceiverGroups,
                Allergies = x.Allergies,
                Diseases = x.Diseases,
                LocationLat = x.LocationLat,
                LocationLong = x.LocationLong,
                EmergencyAvailability = x.EmergencyAvailability,
                Subscribed = x.Subscribed,
                OnlineStatus = x.OnlineStatus,
                Activated = x.Activated,
                Imgurl = x.Imgurl,
                OrgId = x.BepOrganization.Id,
                OrgName = x.BepOrganization.Name,
                OrgAbout = x.BepOrganization.About,
                OrgImgurl = x.BepOrganization.Imgurl,
                OrgEmail = x.BepOrganization.Email,
                OrgPhone = x.BepOrganization.Phone,
                OrgLocality = x.BepOrganization.Locality
            });
            return SingleResult<BepDonorDTO>.Create(result); 
        }

        // PATCH tables/BepDonor/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<BepDonor> PatchBepDonor(string id, Delta<BepDonor> patch)
        {
             return UpdateAsync(id, patch);
        }
        
        // POST tables/BepDonor
        public async Task<IHttpActionResult> PostBepDonor(BepDonor item)
        {

            BepDonor current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }
        
        // DELETE tables/BepDonor/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteBepDonor(string id)
        {
             return DeleteAsync(id);
        }
        [Route("api/registerDonor", Name = "RegisterNewDonor")]
        public async Task<IHttpActionResult> RegisterNewDonor(BepDonor donor)
        {
            // Does the donor data exist?
            var count = context.BepDonors.Where(x => (x.Phone == donor.Phone && x.OnlineStatus == true)).Count();
            if (count >0)
            {
                return BadRequest("Phone number already registered!");
            }
            count = context.BepDonors.Where(x => (x.Email == donor.Email && x.OnlineStatus == true)).Count();
            if (count > 0)
            {
                return BadRequest("Email number already registered!");
            }
            if(string.IsNullOrEmpty(donor.ReceiverGroups))
                switch(donor.BloodGroup)
                {
                    case "A+": donor.ReceiverGroups = "A+,AB+"; break;
                    case "O+": donor.ReceiverGroups = "A+,AB+,O+,B+"; break;
                    case "B+": donor.ReceiverGroups = "B+,AB+"; break;
                    case "AB+": donor.ReceiverGroups = "AB+"; break;
                    case "A-": donor.ReceiverGroups = "A+,AB+,A-,AB-"; break;
                    case "O-": donor.ReceiverGroups = "A+,AB+,O+,B+,A-,AB-,O-,B-"; break;
                    case "B-": donor.ReceiverGroups = "B+,AB+,B-,AB-"; break;
                    case "AB-": donor.ReceiverGroups = "AB+,AB-"; break;
                }
            donor.Subscribed = true;
            donor.OnlineStatus = true;
            //Provision to send out activation email. Until implemented, the activation status will be true for all registering parties
            donor.Activated = true;

            BepDonor current = await InsertAsync(donor);
            return Ok("Donor registered successfully!");
        }
        [Route("api/registerOfflineDonor", Name = "RegisterOfflineDonor")]
        public async Task<IHttpActionResult> RegisterOfflineDonor(BepDonor input)
        {
            // Does the donor data exist?
            var count = context.BepDonors.Where(x => (x.Phone == input.Phone && x.OnlineStatus == true)).Count();
            if (count > 0)
            {
                return BadRequest("Phone number already registered!");
            }
            count = context.BepDonors.Where(x => (x.Email == input.Email && x.OnlineStatus == true)).Count();
            if (count > 0)
            {
                return BadRequest("Email number already registered!");
            }

            BepDonor donor = context.BepDonors.Where(x => (x.Email == input.Email || x.Phone == input.Phone)).First();
            donor.Subscribed = true;
            donor.OnlineStatus = true;
            donor.Activated = true;
            donor.Password = input.Password;
            donor.LocationLat = input.LocationLat;
            donor.LocationLong = input.LocationLong;

            using (var dbCtx = new beplusContext())
            {
                //3. Mark entity as modified
                dbCtx.Entry(donor).State = System.Data.Entity.EntityState.Modified;

                //4. call SaveChanges
                dbCtx.SaveChanges();
            }
            return Ok("Donor registered successfully!");
        }
        [Route("api/importDonorData", Name = "ImportDonorData")]
        public async Task<IHttpActionResult> ImportDonorData(BepDonor donor)
        {
            // Does the donor data exist?
            var count = context.BepDonors.Where(x => x.Phone == donor.Phone).Count();
            if (count > 0)
            {
                return BadRequest("Phone number already registered!");
            }
            count = context.BepDonors.Where(x => x.Email == donor.Email).Count();
            if (count > 0)
            {
                return BadRequest("Email number already registered!");
            }
            if(!string.IsNullOrEmpty(donor.BloodGroup))
                switch (donor.BloodGroup)
                {
                    case "A+": donor.ReceiverGroups = "A+,AB+"; break;
                    case "O+": donor.ReceiverGroups = "A+,AB+,O+,B+"; break;
                    case "B+": donor.ReceiverGroups = "B+,AB+"; break;
                    case "AB+": donor.ReceiverGroups = "AB+"; break;
                    case "A-": donor.ReceiverGroups = "A+,AB+,A-,AB-"; break;
                    case "O-": donor.ReceiverGroups = "A+,AB+,O+,B+,A-,AB-,O-,B-"; break;
                    case "B-": donor.ReceiverGroups = "B+,AB+,B-,AB-"; break;
                    case "AB-": donor.ReceiverGroups = "AB+,AB-"; break;
                }
            donor.Subscribed = true;
            donor.EmergencyAvailability = false;
            donor.OnlineStatus = false;
            donor.Activated = false;

            BepDonor current = await InsertAsync(donor);

            //Send Mail to offline donor with link to download the application


            //End send mail code
            return Ok("Offline Donor registered successfully!");
        }
        [Route("api/loginDonor", Name = "LoginDonor")]
        public IHttpActionResult LoginDonor(LoginData logindata)
        {
            // Does the donor data exist?
            List<BepDonor> donorlist;
            if(logindata.Phone!=null)
                donorlist = context.BepDonors.Where(x => (x.Phone == logindata.Phone && x.Password == logindata.Password)).ToList();
            else donorlist = context.BepDonors.Where(x => (x.Email == logindata.Email && x.Password == logindata.Password)).ToList();
            int count = donorlist.Count;
            if (count==1)
            {
                var current = donorlist[0];
                var result = Lookup(current.Id).Queryable.Select(x => new BepDonorDTO()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Phone = x.Phone,
                    Email = x.Email,
                    BloodGroup = x.BloodGroup,
                    ReceiverGroups = x.ReceiverGroups,
                    Allergies = x.Allergies,
                    Diseases = x.Diseases,
                    LocationLat = x.LocationLat,
                    LocationLong = x.LocationLong,
                    EmergencyAvailability = x.EmergencyAvailability,
                    Subscribed = x.Subscribed,
                    OnlineStatus = x.OnlineStatus,
                    Activated = x.Activated,
                    Imgurl = x.Imgurl,
                    OrgId = x.BepOrganization.Id,
                    OrgName = x.BepOrganization.Name,
                    OrgAbout = x.BepOrganization.About,
                    OrgImgurl = x.BepOrganization.Imgurl,
                    OrgEmail = x.BepOrganization.Email,
                    OrgPhone = x.BepOrganization.Phone,
                    OrgLocality = x.BepOrganization.Locality
                });
                return Ok(SingleResult<BepDonorDTO>.Create(result));
            }
            else return BadRequest("Invalid login credentials!");
        }

    }
}