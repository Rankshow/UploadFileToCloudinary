namespace UploadFileToCloudinary.Entities
{
    public class Apartment : Entity
    {
        public Guid Id { get; set; }
        public Guid VendorId { get; set; }
        public Guid ManagerId { get; set; }
        public Guid LocationId { get; set; }
        public Guid ApartmentTypeId { get; set; }
        public Guid StatusId { get; set; }
        public string Name { get; set; }
        public string Facilities { get; set; }
        public List<Fees> Fees { get; set; }

    }
}
