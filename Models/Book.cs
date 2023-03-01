namespace Web_1670.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string NameBook { get; set; }
        public int QtyBook { get; set; }
        public double Price { get; set; }
        public DateTime CreateAt { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public virtual ICollection<Order>? Order { get; set; }
    }
}
