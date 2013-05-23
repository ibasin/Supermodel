namespace Supermodel.DDD.Models.CommonValueTypes
{
    public class Geolocation
    {
        public Geolocation() { }
        public Geolocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    //public class GeolocationTypeMapping : ComplexTypeConfiguration<Geolocation>
    //{
    //    public GeolocationTypeMapping()
    //    {
    //        Property(o => o.Latitude);
    //        Property(o => o.Longitude);
    //    }
    //}
}
