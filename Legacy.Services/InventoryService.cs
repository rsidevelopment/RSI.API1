using Legacy.Services.Data;
using Legacy.Services.Interfaces;
using Legacy.Services.Models;
using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Inventory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Legacy.Services
{
    public class InventoryService : IInventoryService
    {
        private const string _userName = "Dennis";
        private const string _password = "dll";
        private const string _url = "https://accessrsi.com/rsiwebservices/xinventoryws.asmx";

        private readonly LegacyDbContext _legacyContext;
        private readonly RSIDbContext _rsiContext;
        private readonly IUnitService _unitService;
        private readonly IConfiguration _configuration;

        public InventoryService(LegacyDbContext legacyContext, RSIDbContext rsiContext, IUnitService unitService, IConfiguration configuration)
        {
            _legacyContext = legacyContext;
            _rsiContext = rsiContext;
            _unitService = unitService;
            _configuration = configuration;
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
                var result = await _legacyContext.LoadStoredProc("dbo.apiInventorySearch")
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
                    MaxRows = i.maxrows//,
                    //OwnerId = i.owner
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
                var holdResponse = await HoldUnitAsync(bookingRequestViewModel.InventoryId);
                if (!holdResponse.IsSuccess)
                {
                    model.Message = holdResponse.Message;
                    return model;
                }

                var hldUpdate = _rsiContext.Holds.FirstOrDefault(x => x.keyid == holdResponse.HoldId);
                if (hldUpdate != null)
                {
                    hldUpdate.isBooking = true;
                    await _rsiContext.SaveChangesAsync();
                }

                string message = "";
                bool? status = null;
                var result = await _rsiContext.LoadStoredProc("dbo.vipConfirmAHold")
                    .WithSqlParam("message", message, ParameterDirection.InputOutput)
                    .WithSqlParam("status", status, ParameterDirection.InputOutput)
                    .WithSqlParam("holdID", holdResponse.HoldId)
                    .WithSqlParam("creatorID", holdResponse.HoldUser)
                    .WithSqlParam("travelerFirstName", bookingRequestViewModel.FirstName)
                    .WithSqlParam("travelerMiddleInitial", bookingRequestViewModel.MiddleName)
                    .WithSqlParam("travelerLastName", bookingRequestViewModel.LastName)
                    .WithSqlParam("travelerAddress", bookingRequestViewModel.Address)
                    .WithSqlParam("travelerCity", bookingRequestViewModel.City)
                    .WithSqlParam("travelerStateCode", bookingRequestViewModel.State)
                    .WithSqlParam("travelerPostalCode", bookingRequestViewModel.Zip)
                    .WithSqlParam("travelerCountryCode", bookingRequestViewModel.Country)
                    .WithSqlParam("travelerPhone1", bookingRequestViewModel.Phone1)
                    .WithSqlParam("travelerPhone2", bookingRequestViewModel.Phone2)
                    .WithSqlParam("travelerEmail", bookingRequestViewModel.Email)
                    .WithSqlParam("billingFirstName", bookingRequestViewModel.FirstName)
                    .WithSqlParam("billingMiddleInitial", bookingRequestViewModel.MiddleName)
                    .WithSqlParam("billingLastName", bookingRequestViewModel.LastName)
                    .WithSqlParam("billingAddress", bookingRequestViewModel.Address)
                    .WithSqlParam("billingCity", bookingRequestViewModel.City)
                    .WithSqlParam("billingStateCode", bookingRequestViewModel.State)
                    .WithSqlParam("billingPostalCode", bookingRequestViewModel.Zip)
                    .WithSqlParam("billingCountryCode", bookingRequestViewModel.Country)
                    .WithSqlParam("billingPhone1", bookingRequestViewModel.Phone1)
                    .WithSqlParam("billingPhone2", bookingRequestViewModel.Phone2)
                    .WithSqlParam("billingEmail", bookingRequestViewModel.Email)
                    .WithSqlParam("creditCardNumber", string.Empty)
                    .WithSqlParam("mM", string.Empty)
                    .WithSqlParam("yYYY", string.Empty)
                    .WithSqlParam("cVV", string.Empty)
                    .WithSqlParam("return_value", 0, ParameterDirection.ReturnValue)
                    .ExecuteStoredProcAsync<int>();

                //these fields don't appear to be used in the old logic
                //message = (string)result.DbParameters["message"].Value;
                //status = (bool?)result.DbParameters["status"].Value;

                if (holdResponse.OwnerId == 100)
                {
                    //TODO: RCI.HoldResponse hr = _rci.ConfirmHold(_userName, _password, hld.inventorytype, hld.refnum);
                }

                //TODO: ? reservationId = vipMoveHoldToResTableDoNotUpdatePoints(hld.keyid, authCode);
                model.BookingId = holdResponse.HoldId;
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new BookingResponseViewModel();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }

        public async Task<InventoryItemViewModel> GetInventoryById(int inventoryId)
        {
            InventoryItemViewModel model = new InventoryItemViewModel();

            try
            {
                model = await (from i in _legacyContext.Inventories
                               where i.keyid == inventoryId
                               select new InventoryItemViewModel
                               {
                                   BedroomSize = i.bedrooms.ToString(),
                                   CheckInDate = i.fdate,
                                   CheckOutDate = i.tdate,
                                   InventoryId = i.keyid,
                                   InventoryType = i.inventorytype.Trim(),
                                   KitchenType = i.kitchentype.Trim(),
                                   MaxGuests = i.maxguests,
                                   Privacy = i.adults,
                                   ProviderInventoryId = i.inventoryid.Trim(),
                                   Quantity = i.quantity - i.hold,
                                   UnitId = i.unitkeyid,
                                   NetRate = decimal.Parse(i.rsicost)
                               }).FirstOrDefaultAsync();
                if (model != null)
                {
                    model.Message = "Success";
                }
                else
                    model.Message = "Error: Inventory not found";
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new InventoryItemViewModel();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }

        public async Task<InventoryItemViewModel> GetInventoryByProviderInventoryId(int providerId, string inventoryId)
        {
            InventoryItemViewModel model = new InventoryItemViewModel();

            try
            {
                model = await (from i in _legacyContext.Inventories
                              where i.inventoryid == inventoryId && i.ownerid == providerId
                              select new InventoryItemViewModel
                              {
                                  BedroomSize = i.bedrooms.ToString(),
                                  CheckInDate = i.fdate,
                                  CheckOutDate = i.tdate,
                                  InventoryId = i.keyid,
                                  InventoryType = i.inventorytype.Trim(),
                                  KitchenType = i.kitchentype.Trim(),
                                  MaxGuests = i.maxguests,
                                  Privacy = i.adults,
                                  ProviderInventoryId = i.inventoryid,
                                  Quantity = i.quantity - i.hold,
                                  UnitId = i.unitkeyid,
                                  NetRate = decimal.Parse(i.rsicost)
                              }).FirstOrDefaultAsync();
                if (model != null)
                {
                    model.Message = "Success";
                }
                else
                    model.Message = "Error: Inventory not found";

                model.Message = "Success";
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new InventoryItemViewModel();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }

        public async Task<(bool isSuccess, string message)> ResortSearch(string origionalResortId, DateTime startDate, DateTime endDate)
        {
            (bool isSuccess, string message) model = (false, "");

            try
            {
                string startDateString = string.Format("{0:MM/dd/yyyy}", startDate);
                string endDateString = string.Format("{0:MM/dd/yyyy}", endDate);

                var soapClient = new RCI.XInventoryWSSoapClient(new RCI.XInventoryWSSoapClient.EndpointConfiguration(), "https://www.accessrsi.com/rsiwebservices/xinventoryws.asmx");

                using (new OperationContextScope(soapClient.InnerChannel))
                {
                    RCI.ResortSearchResponse result = await soapClient.ResortSearchAsync(_userName, _password, origionalResortId, startDateString, endDateString);
                    RCI.ResortResponse[] rows = result.Body.ResortSearchResult;
                }
            }
            catch (Exception ex)
            {
                model = (false, ex.Message);
            }

            return model;
        }

        public async Task<HoldResponseViewModel> HoldUnitAsync(int inventoryId)
        {
            var model = new HoldResponseViewModel()
            {
                HoldUser = Convert.ToInt32(_configuration.GetSection("Booking")["HoldUser"])
            };

            try
            {
                var inventory = await (from i in _legacyContext.Inventories
                                       where i.keyid == inventoryId
                                       select i).FirstOrDefaultAsync();

                if (inventory == null)
                {
                    model.Message = "Error: Inventory not found";
                    return model;
                }

                model.OwnerId = inventory.ownerid;
                if (inventory.quantity - inventory.hold <= 0 || inventory.finish > DateTime.Now)
                {
                    model.Message = "Error: Inventory not available";
                    return model;
                }

                if (inventory.ownerid == 100)
                {
                    if (!Convert.ToBoolean(_configuration.GetSection("Booking")["AllowRCI"]))
                    {
                        model.Message = "Error: RCI inventory not supported";
                        return model;
                    }

                    //TODO RCI Resort Search
                }

                decimal rsiCost = (string.IsNullOrEmpty(inventory.rsicost)) ? 0 : Decimal.Parse(inventory.rsicost);

                var result = await _rsiContext.LoadStoredProc("dbo.vipHoldACondoWithRetail")
                        .WithSqlParam("holdID", model.HoldId, ParameterDirection.InputOutput)
                        .WithSqlParam("message", model.Message, ParameterDirection.InputOutput)
                        .WithSqlParam("inventoryID", inventoryId)
                        .WithSqlParam("creatorID", model.HoldUser)
                        .WithSqlParam("customerPrice", rsiCost)
                        .WithSqlParam("retail", 0)
                        .WithSqlParam("overage", 0)
                        .WithSqlParam("discount", 0)
                        .WithSqlParam("reference", string.Empty)
                        .WithSqlParam("sendXML", string.Empty)
                        .WithSqlParam("receiveXML", string.Empty)
                        .WithSqlParam("return_value", 0, ParameterDirection.ReturnValue)
                        .ExecuteStoredProcAsync<int>();

                model.HoldId = ((int?)result.DbParameters["holdID"].Value).GetValueOrDefault(0);
                //model.Message = (string)result.DbParameters["message"].Value;

                if (model.HoldId == 0)
                {
                    model.Message = "Error: Unit no longer available.";
                    //TODO: ? ri.InternalMessage = "Unit successfully held with RCI, but error trying to write it to holds table.  User doesn't think it is held.";
                    return model;
                }
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new HoldResponseViewModel();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }
    }
}
