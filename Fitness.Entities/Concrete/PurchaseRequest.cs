using Fitness.Core.Abstraction;
using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Concrete
{
	public class PurchaseRequest:IEntity
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public bool IsApproved { get; set; }
		public DateTime RequestedAt { get; set; }
		public DateTime? ApprovedAt { get; set; }
        public User User { get; set; }
        public Product Product { get; set; }
    }
}
