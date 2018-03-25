using Legacy.Services.Data;
using Legacy.Services.Interfaces;
using Legacy.Services.Models;
using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Unit;
using Legacy.Services.Models._ViewModels.Inventory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace Legacy.Services
{
    public class ReservationService : IReservationService
    {
        private readonly LegacyDbContext _context;

        public ReservationService(LegacyDbContext context)
        {
            _context = context;
        }
    }
}
