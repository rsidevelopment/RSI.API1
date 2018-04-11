using System;
using Legacy.Services.Models._ViewModels;

namespace Legacy.Services.Models._ViewModels.Inventory
{
    public class HoldResponseViewModel : _ItemViewModel
    {
        public int HoldId { get; set; }
        public int HoldUser { get; set; }
        public int? OwnerId { get; set; }
    }
}
