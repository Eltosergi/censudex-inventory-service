
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace censudex_inventory_service.src.Models
{
    [Table("Inventory")]
    public class Inventory:BaseModel
    {
        [PrimaryKey("productID", false)]
        [Column("productID")]
        public Guid productid { get; set; }
       
       [Column("stock")]
       public long stock { get; set; }
    }
}