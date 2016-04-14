using System;

namespace beplusService.DataObjects
{
    public class BepEventDTO
    {
        public string About { get; set; }
        public string Venue { get; set; }
        public DateTime Date { get; set; }
        public string OrgId { get; set; }
        public string OrgName { get; set; }
        public string OrgAbout { get; set; }
        public string OrgImgurl { get; set; }
        public string OrgLocality { get; set; }
        public string OrgEmail { get; set; }
        public string OrgPhone { get; set; }
    }
}
