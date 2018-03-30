using Legacy.Services.Data;
using Legacy.Services.Interfaces;
using Legacy.Services.Models._ViewModels.Booking;
using Legacy.Services.Models._ViewModels.Unit;
using System;
using System.Threading.Tasks;


namespace Legacy.Services
{
    public class BookingService : IBookingService
    {
        private readonly LegacyDbContext _context;
        private readonly IUnitService _unitService;

        public BookingService(LegacyDbContext context, IUnitService unitService)
        {
            _context = context;
            _unitService = unitService;
        }

        public async Task<BookingResponseViewModel> BookInventory(BookingRequestViewModel bookingRequestViewModel)
        {
            BookingResponseViewModel model = new BookingResponseViewModel();

            try
            {
                if (bookingRequestViewModel.InventoryId > 0)
                {
                    UnitDetailsModel unit = await _unitService.GetUnitByRequest(bookingRequestViewModel);

                    if (unit != null)
                    {

                        model.Message = "Success";
                    }
                    else
                        model.Message = "Error: Resort not found";
                }
                else
                    model.Message = "Error: Inventory not found";
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new BookingResponseViewModel();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }
    }
}
