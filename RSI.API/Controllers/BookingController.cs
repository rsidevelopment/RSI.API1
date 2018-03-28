using Legacy.Services.Interfaces;
using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Booking;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RSI.API.Controllers
{
    [Route("api/[controller]")]//, Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingService _context;
        
        public BookingController(IBookingService context, IInventoryService inventoryService)
        {
            _context = context;
            
        }

        // POST: api/unit    
        [HttpPost ("{inventoryId}")]
        public async Task<BookingResponseViewModel> Post(int inventoryId)
        {
            var model = new BookingResponseViewModel();

            try
            {
                var request = new BookingRequestViewModel()
                {
                    InventoryId = inventoryId
                };

                model = await _context.BookInventory(request);
                
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new BookingResponseViewModel();

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
