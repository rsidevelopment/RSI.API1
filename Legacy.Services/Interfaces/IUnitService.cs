using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Unit;
using System.Threading.Tasks;

namespace Legacy.Services.Interfaces
{
    public interface IUnitService
    {
        Task<UnitDetailsModel> GetUnit(int unitId);
        Task<UnitDetailsModel> GetUnitByInventoryId(int inventoryId);
        Task<_ListViewModel<UnitListViewModel>> GetUnits(UnitSearchViewModel unitSearchViewModel);
    }
}
