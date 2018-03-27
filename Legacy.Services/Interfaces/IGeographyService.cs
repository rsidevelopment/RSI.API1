using Legacy.Services.Models._ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Legacy.Services.Interfaces
{
    public interface IGeographyService
    {
        Task<_ListViewModel<_KeyValuePairViewModel>> GetRegionsAsync(string filter);
        Task<_ListViewModel<_KeyValuePairViewModel>> GetCountriesAsync(string filter);
        Task<_ListViewModel<_KeyValuePairViewModel>> GetStatesAsync(string countryCode, string filter);
        Task<_ListViewModel<string>> GetCitiesAsync(string countryCode, string stateCode, string filter);
    }
}
