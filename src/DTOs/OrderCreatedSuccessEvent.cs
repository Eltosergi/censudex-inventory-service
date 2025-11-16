using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;

namespace censudex_inventory_service.src.DTOs
{
    [EntityName("order.success")]
    public class OrderCreatedSuccessEvent
    {
        public List<OrderItem> Items { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}