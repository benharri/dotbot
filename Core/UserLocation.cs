namespace dotbot.Core
{
    public class UserLocation
    {
        public ulong Id { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string City { get; set; }
        public int CityId { get; set; }
        public string TimeZone { get; set; }
    }
}
