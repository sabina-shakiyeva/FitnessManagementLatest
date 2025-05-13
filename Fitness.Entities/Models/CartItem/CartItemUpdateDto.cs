using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models.CartItem
{
    public class CartItemUpdateDto
    {
        public int ProductId { get; set; }
        public int NewQuantity { get; set; }
    }
}
