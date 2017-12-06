namespace dotbot.Core
{
    public class UserLocation
    {
        public ulong Id { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
        public string City { get; set; }
        public int CityId { get; set; }
        public string TimeZone { get; set; }
    }
}
