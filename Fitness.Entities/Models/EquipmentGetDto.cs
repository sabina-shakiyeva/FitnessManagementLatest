using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models
{
    public class EquipmentGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsAvailable { get; set; }
        public decimal? Price { get; set; }
        public decimal? Unit { get; set; }
        public string?  ImageUrl { get; set; }
    }
}
