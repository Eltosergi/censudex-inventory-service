using Microsoft.AspNetCore.Mvc;
using Supabase;
using System.Threading.Tasks;
using censudex_inventory_service.src.Service;
using censudex_inventory_service.src.Models;
using censudex_inventory_service.src.DTOs;
using censudex_inventory_service.src.Interface;

namespace censudex_inventory_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupabaseController : ControllerBase
    {
        private readonly Client _supabase;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly int _threshold = 10;

        public SupabaseController(Client supabase, IRabbitMqService rabbitMqService)
        {
            _supabase = supabase;
            _rabbitMqService = rabbitMqService;
        }

        [HttpGet("check")]
        public async Task<IActionResult> CheckConnection()
        {
            bool conectado = await SupabaseHelper.IsConnectedAsync(_supabase);

            return Ok(new
            {
                status = conectado ? "✅ Conectado a Supabase" : "❌ No se pudo conectar a Supabase",
                conectado
            });
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddInventory([FromBody] InventoryDTO inventoryDTO)
        {
            try
            {
                var inventory = new Inventory
                {
                    productid = inventoryDTO.productid,
                    stock = inventoryDTO.stock
                };
                var result = await SupabaseHelper.AddInventoryAsync(_supabase, inventory);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("get/{productId}")]
        public async Task<IActionResult> GetInventory(Guid productId)
        {
            var inventory = await SupabaseHelper.GetInventoryAsync(_supabase, productId);
            return inventory != null ? Ok(inventory) : NotFound("Inventario no encontrado");
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllInventory()
        {
            var inventoryList = await SupabaseHelper.GetAllInventoryAsync(_supabase);

            if (inventoryList == null || inventoryList.Count == 0)
            {
                return NotFound("No se encontraron inventarios");
            }

            return Ok(inventoryList);
        }


        [HttpPatch("update/{productId}")]
        public async Task<IActionResult> UpdateDecInventory(Guid productId, [FromBody] long stock)
        {
            try
            {
                var existingInventory = await SupabaseHelper.GetInventoryAsync(_supabase, productId);
                if (existingInventory == null)
                {
                    return NotFound("Inventario no encontrado para actualizar");
                }

                if (existingInventory.stock + stock < 0)
                {
                    return BadRequest("No hay suficiente stock para decrementar");
                }

                var inventory = new Inventory
                {
                    productid = productId,
                    stock = existingInventory.stock + stock
                };

                if (inventory.stock < _threshold)
                {
                    var alert = new InventoryAlertDTO
                    {
                        ProductId = productId,
                        CurrentStock = inventory.stock,
                        Threshold = _threshold
                    };

                    await _rabbitMqService.PublishAsync(alert);

                }

                var result = await SupabaseHelper.UpdateInventoryAsync(_supabase, inventory);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        
        
        [HttpPatch("update/set/{productId}")]
        public async Task<IActionResult> UpdateSetInventory(Guid productId, [FromBody] long stock)
        {
            try
            {
                var existingInventory = await SupabaseHelper.GetInventoryAsync(_supabase, productId);
                if (existingInventory == null)
                {
                    return NotFound("Inventario no encontrado para actualizar");
                }

                var inventory = new Inventory
                {
                    productid = productId,
                    stock = stock
                };

                if (inventory.stock < _threshold)
                {
                    var alert = new InventoryAlertDTO
                    {
                        ProductId = productId,
                        CurrentStock = inventory.stock,
                        Threshold = _threshold
                    };

                    await _rabbitMqService.PublishAsync(alert);
                    
                }

                var result = await SupabaseHelper.UpdateInventoryAsync(_supabase, inventory);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



    }
}
