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
        
        // Actualizar inventario - tres variantes: set, inc, dec
        // Set: establece el stock a un valor específico
        // Inc: incrementa el stock en una cantidad específica
        // Dec: decrementa el stock en una cantidad específica

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

                var result = await SupabaseHelper.UpdateInventoryAsync(_supabase, inventory);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPatch("update/inc/{productId}")]
        public async Task<IActionResult> UpdateIncInventory(Guid productId, [FromBody] long stock)
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
                    stock = existingInventory.stock + stock
                };

                var result = await SupabaseHelper.UpdateInventoryAsync(_supabase, inventory);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPatch("update/dec/{productId}")]
        public async Task<IActionResult> UpdateDecInventory(Guid productId, [FromBody] long stock)
        {
            try
            {
                var existingInventory = await SupabaseHelper.GetInventoryAsync(_supabase, productId);
                if (existingInventory == null)
                {
                    return NotFound("Inventario no encontrado para actualizar");
                }

                if (existingInventory.stock < stock)
                {
                    return BadRequest("No hay suficiente stock para decrementar");
                }

                var inventory = new Inventory
                {
                    productid = productId,
                    stock = existingInventory.stock - stock
                };

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
