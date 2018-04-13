using Legacy.Services.Data;
using Legacy.Services.Interfaces;
using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Unit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public async Task<UnitDetailsModel> GetUnit(int unitId)
        {
            var model = new UnitDetailsModel();

            try
            {
                model = await GetUnitFromDB(unitId, null);
            }
            catch (Exception ex)
            {
                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }

        public async Task<UnitDetailsModel> GetUnitByInventoryId(int inventoryId)
        {
            UnitDetailsModel model = new UnitDetailsModel();

            try
            {
                model = await GetUnitFromDB(null, inventoryId);
            }
            catch (Exception ex)
            {
                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }

        class apiUnitSearchResult
        {
            public int unitID { get; set; }
            public string origionalID { get; set; }
            public int ownerID { get; set; }
            public int imageID { get; set; }
            public string unitName { get; set; }
            public string address { get; set; }
            public string city { get; set; }
            public string stateCode { get; set; }
            public string stateFullName { get; set; }
            public string postalCode { get; set; }
            public string regionCode { get; set; }
            public string regionFullName { get; set; }
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
                .WithSqlParam("regionCode", search.RegionCode)
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
                    OriginalUnitId = u.origionalID,
                    UnitId = u.unitID,
                    UnitName = u.unitName,
                    Address = new AddressViewModel()
                    {
                        City = u.city,
                        CountryCode = u.countryCode,
                        CountryFullName = u.countryFullName,
                        PostalCode = u.postalCode,
                        RegionCode = u.regionCode,
                        RegionFullName = u.regionFullName,
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

        public partial class vipUrgentInfoResult
        {
            public string urgentInfo { get; set; }
        }
        async Task<UnitDetailsModel> GetUnitFromDB(int? unitId, int? inventoryId, bool includeRCI = false)
        {
            var model = await
                   (from u in _context.Units
                    join c in _context.Translators on new { Reference = u.country, Type = "COUNTRY", Language = "EN" } equals new { c.Reference, c.Type, c.Language }
                    join s in _context.Translators on new { Reference = u.state, Type = "_S_" + u.country, Language = "EN" } equals new { s.Reference, s.Type, s.Language } into States
                    from St in States.DefaultIfEmpty()
                    join r in _context.Regions on u.regioncode equals r.regioncode into Regions
                    from Rg in Regions.DefaultIfEmpty()
                    join i in _context.Inventories on u.keyid equals i.unitkeyid into Inventories
                    from Ig in Inventories.DefaultIfEmpty()
                    where u.keyid == (unitId ?? u.keyid)
                    && Ig.keyid == (inventoryId ?? Ig.keyid)
                    select new UnitDetailsModel()
                    {
                        Description = u.info,
                        ImageURL = $"http://accessrsi.com/dannoJR/ProductImageHandler.ashx?imageid={u.thumbID}",
                        OwnerId = u.ownerid,
                        OriginalUnitId = u.origionalid,
                        UnitId = u.keyid,
                        UnitName = u.name,
                        Address = new AddressViewModel()
                        {
                            City = u.city,
                            CountryCode = u.country,
                            CountryFullName = c.Value,
                            PostalCode = u.zip,
                            RegionCode = u.regioncode,
                            RegionFullName = Rg.regiondescription,
                            StateCode = u.state,
                            StateFullName = St.Value,
                            StreetAddress = u.address.Trim()
                        }
                    }).FirstOrDefaultAsync<UnitDetailsModel>();

            if (model != null)
            {
                model.Amenities = await
                    (from a in _context.Amenities
                     where a.unitkeyid == unitId
                     orderby a.sorder
                     select new Amenity()
                     {
                         Location = a.location,
                         Name = a.ref1,
                         Distance = a.distance,
                         MK = a.mk
                     }).ToListAsync();

                var urgentInfo = await _context.LoadStoredProc("dbo.vipUrgentInfo")
                        .WithSqlParam("resortID", unitId)
                        .ExecuteStoredProcAsync<vipUrgentInfoResult>();

                model.UrgentInfo = urgentInfo.Select(u => u.urgentInfo).ToList();

                if (model.OwnerId == 100)
                {
                    model.AdditionalImageURLs = new List<string>()
                    { $"http://rci.accessrsi.com/api/rci/getImage?resortId={model.OriginalUnitId}" };
                }
                else
                {
                    model.AdditionalImageURLs = await
                        (from p in _context.Pics
                         where p.ptype == "U_PIC" && p.ref1 == model.UnitId.ToString()
                         select $"http://accessrsi.com/dannoJR/ProductImageHandler.ashx?imageid={p.keyid}").ToListAsync();
                }
            }

            if (model != null) model.Message = "Success";
            return model ?? new UnitDetailsModel() { Message = "Error: Unit Not Found" };
        }
    }
}
