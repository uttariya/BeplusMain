using Microsoft.WindowsAzure.Mobile.Service;
using System;

namespace beplusService.DataObjects
{
    public class BepBloodRequest:EntityData
    {
        /*type, amount, locationlat, locationlong, 
        emeregrncy/non emergency, honored(booleam), 
        donor object, recipient name, recipient email and recipient */
        public DateTime RequestTime { get; set; }
        public int MinsDur { get; set; }
        public double LocationLat { get; set; }
        public double LocationLong { get; set; }
        public string HospitalName { get; set; }
        public string HospitalAddress { get; set; }
        public bool Emergency { get; set; }
        public bool Verified { get; set; }
        public bool Honored { get; set; }
        public string BloodType { get; set; }
        public string BloodUnits { get; set; }
        public String RecipientName { get; set; }
        public String RecipientPhone { get; set; }
        public String RecipientEmail { get; set; }
        public String RecipientImgurl { get; set; }
        public String DonorId { get; set; }
        public String DonorName { get; set; }
        public String DonorPhone { get; set; }
        public String DonorEmail { get; set; }
        public String DonorImgurl { get; set; }
    }
}