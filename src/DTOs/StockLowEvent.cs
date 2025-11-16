using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;

namespace censudex_inventory_service.src.DTOs
{
    [EntityName("stock.low")]
    public class StockLowEvent

    {
        public Guid ProductId { get; set; }
        public long CurrentStock { get; set; }
        public long Threshold { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Message { get; set; } = "Alerta de stock bajo generado.";
    }
}