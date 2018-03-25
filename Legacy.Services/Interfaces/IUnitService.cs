using Legacy.Services.Models;
using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Unit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Legacy.Services.Interfaces
{
    public interface IUnitService
    {
        Task<_ListViewModel<UnitListViewModel>> GetUnits(UnitSearchViewModel unitSearchViewModel);
    }
}
