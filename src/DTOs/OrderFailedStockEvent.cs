using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;

namespace censudex_inventory_service.src.DTOs
{
    [EntityName("order.failed.stock")]
    public class OrderFailedStockEvent
    {
        public Guid ProductId { get; set; }
        public long CurrentStock { get; set; }
        public long AttemptedChange { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}