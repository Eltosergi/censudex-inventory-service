using Supabase;
using System;
using System.Threading.Tasks;

namespace censudex_inventory_service.src.Service
{
    public class SupabaseHelper
    {
        public static async Task<bool> IsConnectedAsync(Client client)
        {
            try
            {
                await client.InitializeAsync(); // 🔹 Intenta inicializar conexión
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al conectar a Supabase: {ex.Message}");
                return false;
            }
        }
    }
}
