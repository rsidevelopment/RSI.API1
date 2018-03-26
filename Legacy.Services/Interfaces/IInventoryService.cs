using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Inventory;
using System.Threading.Tasks;

namespace Legacy.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<_ListViewModel<InventoryListViewModel>> GetInventory(InventorySearchViewModel inventorySearchViewModel);
    }
}
