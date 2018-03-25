using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Legacy.Services.Models._ViewModels.Inventory
{
    public class InventorySearchViewModel : _SearchViewModel
    {
        public int UnitId { get; set; }
        public DateTime? CheckInStart { get; set; }
        public DateTime? CheckInEnd { get; set; } 
        public BedroomSize? BedroomSize { get; set; }
        public InventoryType? InventoryType { get; set; }
        public decimal? MaximumNetRate { get; set; }
    }
}
