using Microsoft.AspNetCore.Identity;

namespace Web_1670.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
        public string? Phone { get; set; }
        public DateTime OrderTime { get; set; }
        public int BookId { get; set; }
        public virtual Book? Book { get; set; }
        public string? IdentityUserId { get; set; }
        public virtual IdentityUser? IdentityUser { get; set; }
        public int Count { get; set; }
    }
}
