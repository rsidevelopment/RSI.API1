using Legacy.Services.Interfaces;
using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Unit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RSI.API.Controllers
{
    [Route("api/[controller]")]//, Authorize]
    public class UnitController : Controller
    {
        private readonly IUnitService _context;
        public UnitController(IUnitService context)
        {
            _context = context;
        }

        // GET: api/unit/5 
        [HttpGet("{unitId}")]
        public async Task<UnitDetailsModel> Get(int unitId)
        {
            var model = new UnitDetailsModel();

            try
            {
                model = await _context.GetUnit(unitId);

            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new UnitDetailsModel();

                if (model.Message.Length > 0)
                    model.Message += " | ";
                else
                    model.Message = "Error: ";

                model.Message += ex.Message;
            }

            return model;
        }

        // GET: api/unit    
        [HttpGet]
        public async Task<_ListViewModel<UnitListViewModel>> Get(OwnerType? ownerType, DateTime? checkInStart, DateTime? checkInEnd, 
            string regionCode, string countryCode, string stateCode, string city, BedroomSize? bedroomSize, InventoryType? inventoryType, decimal? maximumNetRate, 
            int? startRowIndex = 1, int? numberOfRows = 10, string orderBy = "price", string orderDirection = "asc")
        {
            var model = new _ListViewModel<UnitListViewModel>();

            try
            {
                var search = new UnitSearchViewModel()
                {
                    OwnerType = ownerType,
                    CheckInStart = checkInStart,
                    CheckInEnd = checkInEnd,
                    RegionCode = regionCode,
                    CountryCode = countryCode,
                    StateCode = stateCode,
                    City = city,
                    BedroomSize = bedroomSize,
                    InventoryType = inventoryType,
                    MaximumNetRate = maximumNetRate,
                    StartRowIndex = startRowIndex,
                    NumberOfRows = numberOfRows,
                    SortColumn = orderBy,
                    SortDirection = orderDirection,
                    ExactMatch = true,
                };

                model = await _context.GetUnits(search);
                
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new _ListViewModel<UnitListViewModel>();

                if (model.Message.Length > 0)
                    model.Message += " | ";
                else
                    model.Message = "Error: ";

                model.Message += ex.Message;
            }

            return model;
        }

        
    }
}
