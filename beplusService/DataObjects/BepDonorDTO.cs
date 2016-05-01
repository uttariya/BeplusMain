using Microsoft.WindowsAzure.Mobile.Service;

namespace beplusService.DataObjects
{
    public class BepDonorDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string BloodGroup { get; set; }
        public string ReceiverGroups { get; set; }
        public string Allergies { get; set; }
        public string Diseases { get; set; }
        public double LocationLat { get; set; }
        public double LocationLong { get; set; }
        public bool EmergencyAvailability { get; set; }
        public bool Subscribed { get; set; }
        public bool OnlineStatus { get; set; }
        public bool Activated { get; set; }
        public string Imgurl { get; set; }
        public string OrgId { get; set; }
        public string OrgName { get; set; }
        public string OrgAbout { get; set; }
        public string OrgImgurl { get; set; }
        public string OrgLocality { get; set; }
        public string OrgEmail { get; set; }
        public string OrgPhone { get; set; }
    }
}