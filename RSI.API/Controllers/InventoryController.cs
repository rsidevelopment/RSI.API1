using Legacy.Services.Interfaces;
using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RSI.API.Controllers
{
    [Route("api/[controller]"), Authorize]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _context;
        public InventoryController(IInventoryService context)
        {
            _context = context;
        }

        // GET: api/inventory/5
        [HttpGet("{unitId}")]
        public async Task<_ListViewModel<InventoryListViewModel>> Get(int unitId, DateTime? checkInStart, DateTime? checkInEnd, 
            BedroomSize? bedroomSize, InventoryType? inventoryType, decimal? maximumNetRate, 
            int? startRowIndex = 1, int? numberOfRows = 10, string orderBy = "price", string orderDirection = "asc")
        {
            var model = new _ListViewModel<InventoryListViewModel>();

            try
            {
                var search = new InventorySearchViewModel()
                {
                    UnitId = unitId,
                    CheckInStart = checkInStart,
                    CheckInEnd = checkInEnd,
                    BedroomSize = bedroomSize,
                    InventoryType = inventoryType,
                    MaximumNetRate = maximumNetRate,
                    StartRowIndex = startRowIndex,
                    NumberOfRows = numberOfRows,
                    SortColumn = orderBy,
                    SortDirection = orderDirection,
                    ExactMatch = true,
                };

                model = await _context.GetInventory(search);
                
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new _ListViewModel<InventoryListViewModel>();

                if (model.Message.Length > 0)
                    model.Message += " | ";
                else
                    model.Message = "Error: ";

                model.Message += ex.Message;
            }

            return model;
        }

        // POST: api/inventory/123/book
        [HttpPost("{inventoryId}/book")]
        public async Task<BookingResponseViewModel> Get(int inventoryId)
        {
            var bookingInfo = new BookingRequestViewModel()
            {
                InventoryId = inventoryId
            };

            return await _context.BookInventory(bookingInfo);
        }

    }
}
