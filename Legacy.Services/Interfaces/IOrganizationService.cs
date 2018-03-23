using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Member;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Legacy.Services.Interfaces
{
    public interface IOrganizationService
    {
        Task<_ListViewModel<_KeyValuePairViewModel>> GetOrganizationsAsync();
    }
}
