
namespace beplusService.DataObjects
{
    public class BepOrganizationDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public string Locality { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public double LocationLat { get; set; }
        public double LocationLong { get; set; }
        public int Estd { get; set; }
        public string Chairperson { get; set; }
        public bool Activated { get; set; }
        public string Imgurl { get; set; }
    }
}
