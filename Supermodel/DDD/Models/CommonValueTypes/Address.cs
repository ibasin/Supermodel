namespace Supermodel.DDD.Models.CommonValueTypes
{
    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }

        public override string ToString()
        {
            var address = Street;
            address += ", " + City;
            address += ", " + State;
            address += ", " + Zip;
            address += ", " + Country;

            return address;
        }
    }

    //public class AddressTypeMapping : ComplexTypeConfiguration<Address>
    //{
    //    public AddressTypeMapping()
    //    {
    //        Property(o => o.Street);
    //        Property(o => o.City);
    //        Property(o => o.State);
    //        Property(o => o.Zip);
    //        Property(o => o.Country);
    //    }
    //}
}
