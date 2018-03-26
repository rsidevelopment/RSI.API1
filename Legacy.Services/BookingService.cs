using Legacy.Services.Data;
using Legacy.Services.Interfaces;
using Legacy.Services.Models._ViewModels.Booking;
using System.Threading.Tasks;


namespace Legacy.Services
{
    public class BookingService : IBookingService
    {
        private readonly LegacyDbContext _context;

        public BookingService(LegacyDbContext context)
        {
            _context = context;
        }

        public async Task<BookingResponseViewModel> BookInventory(BookingRequestViewModel bookingRequestViewModel)
        {
            return new BookingResponseViewModel();
        }
    }
}
