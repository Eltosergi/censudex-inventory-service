using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace censudex_inventory_service.src.Interface
{
    public interface IRabbitMqService
    {
        Task PublishAsync<T>(T message)where T : class;
    }
}