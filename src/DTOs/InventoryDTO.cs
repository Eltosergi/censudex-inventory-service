using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace censudex_inventory_service.src.DTOs
{
    public class InventoryDTO
    {
        [Required]
        public required Guid productid { get; set; }
        [Required]
        public required long stock { get; set; }
    }
}