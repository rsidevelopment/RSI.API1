using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Package;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Legacy.Services.Interfaces
{
    public interface IPackageService
    {
        Task<PackageViewModel> GetPackageInfoByRSIIdAsync(int rsiId, bool grabBenefits = true);
        Task<_ListViewModel<PackageListViewModel>> GetPackagesByRSIOrgIdAsync(int rsiOrgId);
    }
}
