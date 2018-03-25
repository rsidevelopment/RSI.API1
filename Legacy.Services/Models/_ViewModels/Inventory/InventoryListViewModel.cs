using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legacy.Services.Models._ViewModels.Inventory
{
    public class InventoryListViewModel
    {
        public int UnitId { get; set; }
        public int InventoryId { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int? Quantity { get; set; }
        public string UnitSize { get; set; }
        public int? MaxGuests { get; set; }
        public string NetRate { get; set; }
        public double? Bedrooms { get; set; }
        public double? Bathrooms { get; set; }
        public string KitchenType { get; set; }
        public int? Adults { get; set; }
        public int? InventoryType { get; set; }
        [JsonIgnore]
        public int MaxRows { get; set; }
    }
}
