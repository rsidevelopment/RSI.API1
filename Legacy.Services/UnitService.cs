using Legacy.Services.Data;
using Legacy.Services.Interfaces;
using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Unit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Legacy.Services
{
    public class UnitService : IUnitService
    {
        private readonly LegacyDbContext _context;

        public UnitService(LegacyDbContext context)
        {
            _context = context;
        }

        class apiUnitSearchResult
        {
            public int unitID { get; set; }
            public int ownerID { get; set; }
            public int imageID { get; set; }
            public string unitName { get; set; }
            public string address { get; set; }
            public string city { get; set; }
            public string stateCode { get; set; }
            public string stateFullName { get; set; }
            public string postalCode { get; set; }
            public string countryCode { get; set; }
            public string countryFullName { get; set; }
            public string description { get; set; }
            public decimal lowest { get; set; }
            public int maxrows { get; set; }
        }
        public async Task<_ListViewModel<UnitListViewModel>> GetUnits(UnitSearchViewModel search)
        {
            var model = new _ListViewModel<UnitListViewModel>();

            try
            {
                var result = await _context.LoadStoredProc("dbo.apiUnitSearch")
                .WithSqlParam("ownerType", search.OwnerType)
                .WithSqlParam("inventoryID", null)
                .WithSqlParam("resortID", null)
                .WithSqlParam("startDate", search.CheckInStart)
                .WithSqlParam("endDate", search.CheckInEnd)
                .WithSqlParam("countryCode", search.CountryCode)
                .WithSqlParam("stateCode", search.StateCode)
                .WithSqlParam("city", search.City)
                .WithSqlParam("bedroomSize", (search.BedroomSize.HasValue) ? (int?)search.BedroomSize.Value : null)
                .WithSqlParam("inventoryType", (search.InventoryType.HasValue) ? search.InventoryType.Value.ToString() : null)
                .WithSqlParam("maximumRSICost", search.MaximumNetRate)
                .WithSqlParam("startRowIndex", search.StartRowIndex)
                .WithSqlParam("numberOfRows", search.NumberOfRows)
                .WithSqlParam("orderBy", search.SortColumn)
                .WithSqlParam("orderDirection", search.SortDirection)
                .ExecuteStoredProcAsync<apiUnitSearchResult>();

                model.Rows = result.Select(u => new UnitListViewModel()
                {
                    Description = u.description.Replace("\n", " "),
                    ImageURL = $"http://accessrsi.com/dannoJR/ProductImageHandler.ashx?imageid={u.imageID}",
                    LowestNetRate = u.lowest,
                    OwnerId = u.ownerID,
                    UnitId = u.unitID,
                    UnitName = u.unitName,
                    Address = new AddressViewModel()
                    {
                        City = u.city,
                        CountryCode = u.countryCode,
                        CountryFullName = u.countryFullName,
                        PostalCode = u.postalCode,
                        StateCode = u.stateCode,
                        StateFullName = u.stateFullName,
                        StreetAddress = u.address.Trim()
                    },
                    MaxRows = u.maxrows
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
    }
}
