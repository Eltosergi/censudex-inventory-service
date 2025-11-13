using censudex_inventory_service.src.Interface;
using MassTransit;


namespace censudex_inventory_service.src.Service
{
    public class RabbitMqService:IRabbitMqService
    {
    private readonly IBus _bus;

        public RabbitMqService(IBus bus)
        {
            _bus = bus;
        }


        public async Task PublishAsync<T>(T message) where T : class
        {
            try
            {
                await _bus.Publish(message);
                Console.WriteLine($"✅ Mensaje publicado: {typeof(T).Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al publicar mensaje: {ex.Message}");
                throw;
            }
        }
    }
}