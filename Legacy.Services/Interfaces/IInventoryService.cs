using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Inventory;
using System;
using System.Threading.Tasks;

namespace Legacy.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<_ListViewModel<InventoryListViewModel>> GetInventory(InventorySearchViewModel inventorySearchViewModel);
        Task<BookingResponseViewModel> BookInventory(BookingRequestViewModel bookingRequestViewModel);
        Task<InventoryItemViewModel> GetInventoryById(int inventoryId);
        Task<InventoryItemViewModel> GetInventoryByProviderInventoryId(int providerId, string inventoryId);
        Task<(bool isSuccess, string message)> ResortSearch(string origionalResortId, DateTime startDate, DateTime endDate);
        Task<HoldResponseViewModel> HoldUnitAsync(int inventoryId);
    }
}
