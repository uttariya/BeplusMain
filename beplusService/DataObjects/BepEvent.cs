using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.ComponentModel.DataAnnotations.Schema;
//Tirthanu Ghosh
namespace beplusService.DataObjects
{
    public class BepEvent:EntityData
    {
        public string About { get; set; }
        public string Venue { get; set; }
        public DateTime Date { get; set; }
        public string OrgId { get; set; }
        [ForeignKey("OrgId")]
        public virtual BepOrganization BepOrganization { get; set; }
    }
}