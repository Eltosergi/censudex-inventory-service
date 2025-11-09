using Microsoft.AspNetCore.Mvc;
using Supabase;
using System.Threading.Tasks;
using censudex_inventory_service.src.Service;
using censudex_inventory_service.src.Models;
using censudex_inventory_service.src.DTOs;

namespace censudex_inventory_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupabaseController : ControllerBase
    {
        private readonly Client _supabase;

        public SupabaseController(Client supabase)
        {
            _supabase = supabase;
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

    }
}
