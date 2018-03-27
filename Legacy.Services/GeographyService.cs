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
    

    public class GeographyService : IGeographyService
    {
        private readonly LegacyDbContext _context;
        public GeographyService(LegacyDbContext context)
        {
            _context = context;
        }

        public async Task<_ListViewModel<_KeyValuePairViewModel>> GetRegionsAsync(string filter)
        {
            var model = new _ListViewModel<_KeyValuePairViewModel>();

            try
            {
                var tmp = from r in _context.Regions
                          select new { r.regioncode, r.regiondescription };

                if (!String.IsNullOrEmpty(filter))
                {
                    tmp = tmp.Where(x => x.regioncode.Contains(filter) || x.regiondescription.Contains(filter));
                }

                tmp = tmp.Distinct();

                model.TotalCount = tmp.Count();
                model.Rows = await (from t in tmp
                                    orderby t.regioncode
                                    select new _KeyValuePairViewModel
                                    {
                                        Key = t.regioncode,
                                        Value = t.regiondescription.Trim()
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

        public async Task<_ListViewModel<_KeyValuePairViewModel>> GetCountriesAsync(string filter = null)
        {
            var model = new _ListViewModel<_KeyValuePairViewModel>();

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
            var model = new _ListViewModel<_KeyValuePairViewModel>();

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

        public async Task<_ListViewModel<string>> GetCitiesAsync(string countryCode, string stateCode, string filter)
        {
            var model = new _ListViewModel<string>();

            try
            {
                if (!String.IsNullOrEmpty(countryCode))
                {

                    var tmp = from i in _context.Inventories
                              join u in _context.Units on i.unitkeyid equals u.keyid
                              where 
                              (u.country == countryCode) &&
                              (u.state == (string.IsNullOrEmpty(stateCode) ? u.state : stateCode)) &&
                              (DateTime.Now < i.finish) &&
                              (i.quantity - i.hold > 0)
                              select new { u.city };

                    if (!String.IsNullOrEmpty(filter))
                    {
                        tmp = tmp.Where(x => x.city.Contains(filter));
                    }

                    tmp = tmp.Distinct();

                    //tmp.GroupBy(x=>x.city)
                    model.TotalCount = tmp.Count();
                    model.Rows = await (from t in tmp
                                        orderby t.city
                                        select t.city).ToListAsync();
                }

                model.Message = "Success";
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new _ListViewModel<string>();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }
    }
}
