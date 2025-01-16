namespace UploadFileToCloudinary.Entities
{
    public class Invoice
    {
        public Guid VendorId { get; set; }
        public Guid InvoiceNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public string Amount { get; set; }
        public Guid CustomerId { get; set; }
        public decimal Quantity { get; set; }
        //LineItems
        public string Description { get; set; }
        public double Price { get; set; }

    }
}
