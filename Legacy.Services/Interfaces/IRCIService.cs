using Legacy.Services.Models._ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Legacy.Services.Interfaces
{
    public interface IRCIService
    {
        Task<(bool isSuccess, string message)> ResortSearch(string origionalResortId, DateTime startDate, DateTime endDate);
        Task<_ItemViewModel> HoldUnitAsync(int inventoryId);
    }
}
