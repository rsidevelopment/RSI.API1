using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legacy.Services.Models._ViewModels.Inventory
{
    public class InventoryItemViewModel : _ItemViewModel
    {
        [JsonProperty(PropertyName = "unit_id")]
        public int? UnitId { get; set; }
        [JsonProperty(PropertyName = "owner_id")]
        public int OwnerId { get; set; } = 0;
        [JsonProperty(PropertyName = "inventory_id")]
        public int InventoryId { get; set; }
        [JsonProperty(PropertyName = "provider_inventory_id")]
        public string ProviderInventoryId { get; set; }
        [JsonProperty(PropertyName = "net_rate")]
        public decimal? NetRate { get; set; }
        [JsonProperty(PropertyName = "check_in_date")]
        public DateTime? CheckInDate { get; set; }
        [JsonProperty(PropertyName = "check_out_date")]
        public DateTime? CheckOutDate { get; set; }
        [JsonProperty(PropertyName = "quantity")]
        public int? Quantity { get; set; }
        [JsonProperty(PropertyName = "bedroom_size")]
        public string BedroomSize { get; set; }
        [JsonProperty(PropertyName = "max_guests")]
        public int? MaxGuests { get; set; }
        [JsonProperty(PropertyName = "kitchen_type")]
        public string KitchenType { get; set; }
        [JsonProperty(PropertyName = "privacy")]
        public int? Privacy { get; set; }
        [JsonProperty(PropertyName = "inventory_type")]
        public string InventoryType { get; set; }
        [JsonIgnore]
        public int MaxRows { get; set; }
    }
}
