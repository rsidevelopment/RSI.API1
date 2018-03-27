using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Legacy.Services.Interfaces;
using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Member;
using Legacy.Services.Models._ViewModels.Package;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RSI.API.Controllers
{
    [Route("api/[controller]"), Authorize]
    public class OrganizationController : Controller
    {
        private readonly IOrganizationService _context;
        private readonly IPackageService _packageService;
        public OrganizationController(IOrganizationService context, IPackageService packageService)
        {
            _context = context;
            _packageService = packageService;
        }

        // GET: api/<controller>
        [HttpGet]
        public async Task<_ListViewModel<_KeyValuePairViewModel>> Get()
        {
            var model = new _ListViewModel<_KeyValuePairViewModel>();

            try
            {
                model = await _context.GetOrganizationsAsync();
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new _ListViewModel<_KeyValuePairViewModel>();

                if (model.Message.Length > 0)
                    model.Message += " | ";
                else
                    model.Message = "Error: ";

                model.Message += ex.Message;
            }

            return model;
        }

        [HttpGet("{rsiOrgId}/package")]
        public async Task<_ListViewModel<PackageListViewModel>> GetPackages(int rsiOrgId)
        {
            var model = new _ListViewModel<PackageListViewModel>();

            try
            {
                model = await _packageService.GetPackagesByRSIOrgIdAsync(rsiOrgId);
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new _ListViewModel<PackageListViewModel>();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }
    }
}
