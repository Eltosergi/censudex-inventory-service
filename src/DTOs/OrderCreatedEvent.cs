using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace censudex_inventory_service.src.DTOs
{
    public class OrderCreatedEvent
    {
        public List<OrderItem> Items { get; set; } = new();
    }
}