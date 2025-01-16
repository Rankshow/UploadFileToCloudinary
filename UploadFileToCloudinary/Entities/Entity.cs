namespace UploadFileToCloudinary.Entities
{
    public class Entity
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool isDeleted { get; set; }
    }
}
