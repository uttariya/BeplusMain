using Microsoft.WindowsAzure.Mobile.Service;
using System.ComponentModel.DataAnnotations.Schema;

namespace beplusService.DataObjects
{
    public class BepDonor:EntityData
    {
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
        public string Password { get; set; }
        public string OrgId { get; set; }
        [ForeignKey("OrgId")]
        public virtual BepOrganization BepOrganization { get; set; }

    }
}
