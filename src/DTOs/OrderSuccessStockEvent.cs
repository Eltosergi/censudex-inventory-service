using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace censudex_inventory_service.src.DTOs
{
    public class OrderSuccessStockEvent
    {
        public Guid ProductId { get; set; }
        public long CurrentStock { get; set; }
        public long AttemptedChange { get; set; }
        
    }
}