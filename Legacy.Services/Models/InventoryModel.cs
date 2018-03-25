using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Legacy.Services.Models
{
    [Table("inventory")]
    public class InventoryModel
    {
        [Key]
        public int keyid { get; set; }
        public int? unitkeyid { get; set; }
        public DateTime? fdate { get; set; }
        public DateTime? tdate { get; set; }
        public int? quantity { get; set; }
        public int? hold { get; set; }
        public string unitsize { get; set; }
        public int? creatorid { get; set; }
        public int? ownerid { get; set; }
        public DateTime? creationdate { get; set; }
        public DateTime? finish { get; set; }
        public int? maxguests { get; set; }
        public string rsicost { get; set; }
        public string cost { get; set; }
        public string price { get; set; }
        public string currency { get; set; }
        public int? los { get; set; }
        public int? discount { get; set; }
        public string origionalid { get; set; }
        public double? bedrooms { get; set; }
        public double? bathrooms { get; set; }
        public string kitchentype { get; set; }
        public int? adults { get; set; }
        public string inventorytype { get; set; }
        public string inventoryid { get; set; }
        public string regioncode { get; set; }
        public string sessionid { get; set; }
        public string updateid { get; set; }
        public int? timesupdated { get; set; }
        public bool? hw { get; set; }
        public int? specialinventory { get; set; }
        public int extendedhold { get; set; }
        public bool? fake { get; set; }
        public bool showExactPrice { get; set; }
        public decimal? basePrice { get; set; }
        public byte[] condoCategory { get; set; }
        public string category { get; set; }
    }
}
