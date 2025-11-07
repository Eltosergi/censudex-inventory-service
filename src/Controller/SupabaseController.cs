using Microsoft.AspNetCore.Mvc;
using Supabase;
using System.Threading.Tasks;
using censudex_inventory_service.src.Service;

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

        /// <summary>
        /// Verifica la conexión con la base de datos de Supabase.
        /// </summary>
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
    }
}
