using Microsoft.AspNetCore.Mvc;
using Supabase;
using System.Threading.Tasks;
using censudex_inventory_service.src.Service;
using censudex_inventory_service.src.Models;

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

        [HttpGet("check-connection")]
        public async Task<IActionResult> CheckConnection()
        {
            bool conectado = await SupabaseHelper.IsConnectedAsync(_supabase);

            return Ok(new
            {
                status = conectado ? "✅ Conectado a Supabase" : "❌ No se pudo conectar a Supabase",
                conectado
            });
        }

        [HttpPost("add-inventory")]
        public async Task<IActionResult> AddInventory([FromBody] Inventory inventory)
        {
            try
            {
                // Aseguramos que venga un GUID válido (si no, se genera uno)
                if (inventory.productid == Guid.Empty)
                    inventory.productid = Guid.NewGuid();

                var result = await SupabaseHelper.AddInventoryAsync(_supabase, inventory);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
