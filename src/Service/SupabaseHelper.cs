using censudex_inventory_service.src.Models;
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
                await client.InitializeAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al conectar a Supabase: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> AddInventoryAsync(Client client, Inventory inventory)
        {
            try
            {
                var response = await client.From<Inventory>().Insert(inventory);

                return response.Models != null && response.Models.Count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al insertar inventario: {ex.Message}");
                return false;
            }
        }
    }
}
