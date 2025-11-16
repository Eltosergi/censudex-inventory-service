using censudex_inventory_service.src.DTOs;
using censudex_inventory_service.src.Models;
using MassTransit;
using Supabase;

namespace censudex_inventory_service.src.Service
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly Client _supabase;
        private readonly int _threshold = 10;

        public OrderCreatedConsumer(Client supabase)
        {
            _supabase = supabase;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            try
            {
                var validItems = new List<OrderItem>();

                // 1) VALIDAR TODOS LOS ITEMS PRIMERO
                foreach (var item in context.Message.Items)
                {
                    var existingInventory = await SupabaseHelper.GetInventoryAsync(_supabase, item.ProductId);

                    if (existingInventory == null)
                    {
                        await context.Publish(new OrderFailedStockEvent
                        {
                            ProductId = item.ProductId,
                            CurrentStock = 0,
                            AttemptedChange = item.Quantity,
                            Reason = "Producto no encontrado"
                        });
                        return;
                    }

                    if (existingInventory.stock < item.Quantity)
                    {
                        await context.Publish(new OrderFailedStockEvent
                        {
                            ProductId = item.ProductId,
                            CurrentStock = existingInventory.stock,
                            AttemptedChange = item.Quantity,
                            Reason = "Stock insuficiente"
                        });
                        return;
                    }

                    validItems.Add(item);
                }

                // 2) SI TODO ESTÁ OK → ACTUALIZAR STOCK
                foreach (var item in validItems)
                {
                    var existingInventory = await SupabaseHelper.GetInventoryAsync(_supabase, item.ProductId);

                    var updated = new Inventory
                    {
                        productid = item.ProductId,
                        stock = existingInventory.stock - item.Quantity
                    };

                    await SupabaseHelper.UpdateInventoryAsync(_supabase, updated);

                    // Publicar alerta si stock bajo
                    if (updated.stock < _threshold)
                    {
                        await context.Publish(new StockLowEvent
                        {
                            ProductId = item.ProductId,
                            CurrentStock = updated.stock,
                            Threshold = _threshold
                        });
                    }
                }

                // 3) PUBLICAR EVENTO DE ÉXITO
                await context.Publish(new OrderCreatedSuccessEvent
                {
                    Items = validItems,
                    Message = "Stock reservado exitosamente"
                });

            }
            catch (Exception ex)
            {
                await context.Publish(new OrderFailedStockEvent
                {
                    ProductId = Guid.Empty,
                    CurrentStock = 0,
                    AttemptedChange = 0,
                    Reason = $"Error interno: {ex.Message}"
                });
            }
        }
    }
}
