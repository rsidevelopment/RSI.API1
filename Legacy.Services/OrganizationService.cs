using Legacy.Services.Data;
using Legacy.Services.Interfaces;
using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Member;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Legacy.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly LegacyDbContext _context;
        public OrganizationService(LegacyDbContext context)
        {
            _context = context;
        }
        public async Task<_ListViewModel<_KeyValuePairViewModel>> GetOrganizationsAsync()
        {
            _ListViewModel<_KeyValuePairViewModel> model = new _ListViewModel<_KeyValuePairViewModel>();

            try
            {
                List<_KeyValuePairViewModel> qry = await (from x in _context.Organizations
                                                          select new _KeyValuePairViewModel
                                                          {
                                                              Key = x.OrganizationId.ToString(),
                                                              Value = x.OrganizationName
                                                          }).OrderBy(o => o.Value).ToListAsync();
                model.Rows = qry;
                model.TotalCount = qry.Count();
                model.Message = "Success";


                //_context.Organizations.OrderBy(o => o.OrganizationName).ToListAsync();

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
