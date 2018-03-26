using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Booking;
using System.Threading.Tasks;

namespace Legacy.Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponseViewModel> BookInventory(BookingRequestViewModel bookingRequestViewModel);
    }
}
