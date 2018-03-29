using Legacy.Services.Data;
using Legacy.Services.Interfaces;
using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Inventory;
using Legacy.Services.Models._ViewModels.Unit;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace Legacy.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly LegacyDbContext _context;
        private readonly IUnitService _unitService;

        public InventoryService(LegacyDbContext context, IUnitService unitService)
        {
            _context = context;
            _unitService = unitService;
        }

        class apiInventorySearchResult
        {
            public int? unitID { get; set; }
            public int inventoryID { get; set; }
            public string netRate { get; set; }
            public DateTime? checkInDate { get; set; }
            public DateTime? checkOutDate { get; set; }
            public int? quantity { get; set; }
            public string unitSize { get; set; }
            public int? maxGuests { get; set; }
            public string kitchenType { get; set; }
            public int? adults { get; set; }
            public string inventoryType { get; set; }
            public int maxrows { get; set; }
        }
        public async Task<_ListViewModel<InventoryListViewModel>> GetInventory(InventorySearchViewModel search)
        {
            var model = new _ListViewModel<InventoryListViewModel>();

            try
            {
                var result = await _context.LoadStoredProc("dbo.apiInventorySearch")
                .WithSqlParam("resortID", search.UnitId)
                .WithSqlParam("startDate", search.CheckInStart)
                .WithSqlParam("endDate", search.CheckInEnd)
                .WithSqlParam("bedroomSize", (search.BedroomSize.HasValue) ? (int?)search.BedroomSize.Value : null)
                .WithSqlParam("inventoryType", (search.InventoryType.HasValue) ? search.InventoryType.Value.ToString() : null)
                .WithSqlParam("maximumRSICost", search.MaximumNetRate)
                .WithSqlParam("startRowIndex", search.StartRowIndex)
                .WithSqlParam("numberOfRows", search.NumberOfRows)
                .WithSqlParam("orderBy", search.SortColumn)
                .WithSqlParam("orderDirection", search.SortDirection)
                .ExecuteStoredProcAsync<apiInventorySearchResult>();

                model.Rows = result.Select(i => new InventoryListViewModel()
                {
                    CheckInDate = i.checkInDate,
                    CheckOutDate = i.checkOutDate,
                    InventoryId = i.inventoryID,
                    InventoryType = i.inventoryType,
                    KitchenType = i.kitchenType,
                    MaxGuests = i.maxGuests,
                    NetRate = (string.IsNullOrEmpty(i.netRate)) ? 0 : Decimal.Parse(i.netRate),
                    Privacy= i.adults,
                    Quantity = i.quantity,
                    BedroomSize = ((BedroomSize)Enum.Parse(typeof(BedroomSize), i.unitSize)).ToString().SplitCamelCase(),
                    UnitId = i.unitID,
                    MaxRows = i.maxrows
                }).ToList();

                model.TotalCount = (model.RowCount > 0) ? model.Rows[0].MaxRows : 0;
                model.Message = "Success";
            }
            catch (Exception ex)
            {
                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }

        public async Task<BookingResponseViewModel> BookInventory(BookingRequestViewModel bookingRequestViewModel)
        {
            BookingResponseViewModel model = new BookingResponseViewModel();

            try
            {
                if (bookingRequestViewModel.InventoryId > 0)
                {
                    UnitDetailsModel unit = await _unitService.GetUnitByInventoryId(bookingRequestViewModel.InventoryId);
                    model.Message = unit.Message;
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
