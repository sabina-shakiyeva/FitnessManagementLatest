using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models.Puchase
{
    public class PurchaseProductDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public string UserName { get; set; }
        public int Price { get; set; }
        public DateTime RequestedAt { get; set; }
        public int UserPoint { get; set; }
        public string? userImageUrl { get; set; }
        public string? productImageUrl { get; set; }
    }
}
