using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Legacy.Services.Models._ViewModels.Unit
{
    public class UnitSearchViewModel : _SearchViewModel
    {
        public OwnerType? OwnerType { get; set; }
        public DateTime? CheckInStart { get; set; }
        public DateTime? CheckInEnd { get; set; }
        public string CountryCode { get; set; }
        public string StateCode { get; set; }
        public string City { get; set; }
        public BedroomSize? BedroomSize { get; set; }
        public InventoryType? InventoryType { get; set; }
        public decimal? MaximumNetRate { get; set; }
    }
}
