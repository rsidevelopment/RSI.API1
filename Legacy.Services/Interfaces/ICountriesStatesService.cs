using Legacy.Services.Models._ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Legacy.Services.Interfaces
{
    public interface ICountriesStatesService
    {
        Task<_ListViewModel<_KeyValuePairViewModel>> GetCountriesAsync(string filter);
        Task<_ListViewModel<_KeyValuePairViewModel>> GetStatesAsync(string countryCode, string filter);

    }
}
