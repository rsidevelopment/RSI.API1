using Legacy.Services.Data;
using Legacy.Services.Interfaces;
using Legacy.Services.Models._ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legacy.Services
{
    

    public class CountriesStatesService : ICountriesStatesService
    {
        private readonly LegacyDbContext _context;
        public CountriesStatesService(LegacyDbContext context)
        {
            _context = context;
        }

        public async Task<_ListViewModel<_KeyValuePairViewModel>> GetCountriesAsync(string filter = null)
        {
            _ListViewModel<_KeyValuePairViewModel> model = new _ListViewModel<_KeyValuePairViewModel>();

            try
            {
                var tmp = from t in _context.Translators
                          where t.Type == "COUNTRY" && t.Language == "EN"
                          select new { t.Reference, t.Value };

                if (!String.IsNullOrEmpty(filter))
                {
                    tmp = tmp.Where(x => x.Reference.Contains(filter) || x.Value.Contains(filter));
                }

                model.TotalCount = tmp.Count();
                model.Rows = await (from t in tmp
                                    orderby t.Value.ToString()
                                    select new _KeyValuePairViewModel
                                    {
                                        Key = t.Reference,
                                        Value = t.Value
                                    }).ToListAsync();

                model.Message = "Success";
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new _ListViewModel<_KeyValuePairViewModel>();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }

        public async Task<_ListViewModel<_KeyValuePairViewModel>> GetStatesAsync(string countryCode = null, string filter = null)
        {
            _ListViewModel<_KeyValuePairViewModel> model = new _ListViewModel<_KeyValuePairViewModel>();

            try
            {
                if (!String.IsNullOrEmpty(countryCode))
                {
                    var tmp = from t in _context.Translators
                              where t.Language == "EN" && t.Type == "_S_" + countryCode
                              select new { t.Reference, t.Value };

                    if (!String.IsNullOrEmpty(filter))
                    {
                        tmp = tmp.Where(x => x.Reference.Contains(filter) || x.Value.Contains(filter));
                    }

                    model.TotalCount = tmp.Count();
                    model.Rows = await (from t in tmp
                                        orderby t.Value.ToString()
                                        select new _KeyValuePairViewModel
                                        {
                                            Key = t.Reference,
                                            Value = t.Value
                                        }).ToListAsync();

                    model.Message = "Success";

                    model.Message = "Success";
            }

                model.Message = "Success";
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new _ListViewModel<_KeyValuePairViewModel>();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }
    }
}
