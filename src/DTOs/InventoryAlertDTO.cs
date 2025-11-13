using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace censudex_inventory_service.src.DTOs
{
    public class InventoryAlertDTO

    {
        public Guid ProductId { get; set; }
        public long CurrentStock { get; set; }
        public long Threshold { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}