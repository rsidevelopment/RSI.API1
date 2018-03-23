﻿using Legacy.Services.Data;
using Legacy.Services.Interfaces;
using Legacy.Services.Models;
using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Member;
using Legacy.Services.Models._ViewModels.Package;
using LegacyData.Service.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legacy.Services
{
    public class MemberServices : IMemberService
    {
        private readonly LegacyDbContext _context;
        private readonly IPackageService _packageService;

        public MemberServices(LegacyDbContext context, IPackageService packageService)
        {
            _context = context;
            _packageService = packageService;
        }

        public async Task<(bool isSuccess, string message, List<FamilyMemberViewModel> family)> GetFamilyAsync(int rsiId)
        {
            (bool isSuccess, string message, List<FamilyMemberViewModel> family) returnModel = (false, "", new List<FamilyMemberViewModel>());

            try
            {
                DateTime now = DateTime.Today;
                //int age = now.Year - tmp.ActivationDate.GetValueOrDefault().Year;
                //if (tmp.ActivationDate.GetValueOrDefault() > now.AddYears(-age)) age--;

                MemberModel member = await _context.Users.FirstOrDefaultAsync(x => x.MemberId == rsiId);
                if (member != null && member.MemberId > 0)
                {
                    using (var conn = new SqlConnection(SqlHelper.GetConnectionString()))
                    {
                        var parameters = new[]
                        {
                            new SqlParameter("@RSIId", rsiId)
                        };

                        var rdr = await SqlHelper.ExecuteReaderAsync(
                               conn,
                               CommandType.StoredProcedure,
                               "[dbo].[GetFamilyMembersByRSIId]",
                               parameters);
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                string relationship = !rdr.IsDBNull(5) ? rdr.GetString(5) : "";

                                if (relationship == "Significant_Other" || relationship == "SPOUSE")
                                {
                                    PersonViewModel person = new PersonViewModel()
                                    {
                                        RSIId = unchecked((int)rdr.GetInt64(0)),
                                        FirstName = !rdr.IsDBNull(2) ? rdr.GetString(2) : "",
                                        LastName = !rdr.IsDBNull(3) ? rdr.GetString(3) : "",
                                        Address1 = !rdr.IsDBNull(11) ? rdr.GetString(11) : "",
                                        Address2 = "",
                                        City = !rdr.IsDBNull(12) ? rdr.GetString(12) : "",
                                        State = !rdr.IsDBNull(13) ? rdr.GetString(13) : "",
                                        PostalCode = !rdr.IsDBNull(14) ? rdr.GetString(14) : "",
                                        Country = !rdr.IsDBNull(15) ? rdr.GetString(15) : "",
                                        Email = !rdr.IsDBNull(6) ? rdr.GetString(6) : "",
                                        HomePhone = !rdr.IsDBNull(7) ? rdr.GetString(7) : "",
                                        MobilePhone = !rdr.IsDBNull(9) ? rdr.GetString(9) : ""

                                    };

                                    person.MiddleName = "";
                                    //model.SecondaryMember = person;
                                }
                                else
                                {

                                    FamilyMemberViewModel family = new FamilyMemberViewModel
                                    {
                                        MiddleName = "",

                                        FamilyMemberId = rdr.GetInt64(0),
                                        FistName = !rdr.IsDBNull(2) ? rdr.GetString(2) : "",
                                        LastName = !rdr.IsDBNull(3) ? rdr.GetString(3) : ""
                                    };

                                    if (!rdr.IsDBNull(4))
                                        family.DateOfBirth = rdr.GetDateTime(4);
                                    else
                                        family.DateOfBirth = null;

                                    family.Relationship = relationship;
                                    family.Email = !rdr.IsDBNull(6) ? rdr.GetString(6) : "";

                                    family.PrimaryPhone = !rdr.IsDBNull(7) ? rdr.GetString(7) : "";
                                    family.PrimaryPhoneType = !rdr.IsDBNull(8) ? rdr.GetString(8) : "";

                                    family.AlternativePhone = !rdr.IsDBNull(9) ? rdr.GetString(9) : "";
                                    family.AlternativePhoneType = !rdr.IsDBNull(10) ? rdr.GetString(10) : "";

                                    if (family.DateOfBirth != null)
                                    {
                                        int age = now.Year - family.DateOfBirth.GetValueOrDefault().Year;
                                        if (family.DateOfBirth.GetValueOrDefault() > now.AddYears(-age)) age--;

                                        family.Age = age;
                                    }

                                    returnModel.family.Add(family);
                                }
                            }


                        }
                    }
                }
                else
                    returnModel = (false, "Error: Record not found", new List<FamilyMemberViewModel>());
                    
            }
            catch (Exception ex)
            {
                returnModel = (false, $"Error: {ex.Message}", new List<FamilyMemberViewModel>());
                
            }

            return returnModel;
        }

        public async Task<MemberUpgradeViewModel> GetUpgradeInfoAsync(int rsiId)
        {
            MemberUpgradeViewModel model = new MemberUpgradeViewModel();

            try
            {
                var tmp = await _context.UpgradeAudits.FirstOrDefaultAsync(x => x.RSIId == rsiId);

                if(tmp != null && tmp.UpgradeAuditId > 0)
                {
                    model = new MemberUpgradeViewModel()
                    {
                        CondoWeeks = tmp.CondoWeeks,
                        UpgradeAuditId = tmp.UpgradeAuditId,
                        DownPayment = tmp.DownPayment,
                        FinancedAmount = tmp.FinancedAmount,
                        FinanceTerm = tmp.FinanceTerm,
                        NewPackageId = tmp.NewPackageId,
                        NewRSIOrgId = tmp.NewRSIOrgId,
                        OldPackageId = tmp.OldPackageId,
                        OldRSIOrgId = tmp.OldRSIOrgId,
                        Points = tmp.Points,
                        RCIWeeks = tmp.RCIWeeks,
                        RSIId = tmp.RSIId,
                        UpgradeAgentId = tmp.UpgradeAgentId,
                        UpgradeDate = tmp.UpgradeDate,
                        UpgradePrice = tmp.UpgradePrice,
                        UpgradeProgram = tmp.UpgradeProgram
                    };
                }

                model.Message = "Success";
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new MemberUpgradeViewModel();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }

        public async Task<MemberUpgradeViewModel> AddUpdateUpgradeInfoAsync(MemberUpgradeViewModel model)
        {
            try
            {
                if(model.UpgradeAuditId < 1)
                {
                    UpgradeAuditModel m1 = await _context.UpgradeAudits.FirstOrDefaultAsync(x => x.RSIId == model.RSIId);
                    if (m1 != null && m1.UpgradeAuditId > 0)
                        model.UpgradeAuditId = m1.UpgradeAuditId;
                }

                UpgradeAuditModel m = null;

                if (model.UpgradeAuditId > 0)
                {
                    m = await _context.UpgradeAudits.FirstOrDefaultAsync(x => x.RSIId == model.RSIId);
                    if (m != null)
                    {
                        model.UpgradeAuditId = m.UpgradeAuditId;
                        m.UpgradeProgram = model.UpgradeProgram;
                        m.CondoWeeks = model.CondoWeeks;
                        m.DownPayment = model.DownPayment;
                        m.FinancedAmount = model.FinancedAmount;
                        m.FinanceTerm = model.FinanceTerm;
                        m.NewPackageId = model.NewPackageId;
                        m.NewRSIOrgId = model.NewRSIOrgId;
                        m.OldPackageId = model.OldPackageId;
                        m.OldRSIOrgId = model.OldRSIOrgId;
                        m.Points = model.Points;
                        m.RCIWeeks = model.RCIWeeks;
                        m.UpgradeAgentId = model.UpgradeAgentId;
                        m.UpgradeDate = model.UpgradeDate;
                        m.UpgradePrice = model.UpgradePrice;
                        
                    }
                    else
                    {
                        model.Message = $"Error: Upgrade Id not found";
                    }
                }
                else
                {
                    m = new UpgradeAuditModel()
                    {
                        CondoWeeks = model.CondoWeeks,
                        CreationDate = DateTime.Now,
                        DownPayment = model.DownPayment,
                        FinancedAmount = model.FinancedAmount,
                        FinanceTerm = model.FinanceTerm,
                        NewPackageId = model.NewPackageId,
                        NewRSIOrgId = model.NewRSIOrgId,
                        OldPackageId = model.OldPackageId,
                        OldRSIOrgId = model.OldRSIOrgId,
                        Points = model.Points,
                        RCIWeeks = model.RCIWeeks,
                        RSIId = model.RSIId,
                        UpgradeAgentId = model.UpgradeAgentId,
                        UpgradeDate = model.UpgradeDate,
                        UpgradePrice = model.UpgradePrice,
                        UpgradeProgram = model.UpgradeProgram
                    };

                    await _context.UpgradeAudits.AddAsync(m);
                }

                await _context.SaveChangesAsync();

                model.Message = "Success";
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new MemberUpgradeViewModel();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }

        public async Task<_ListViewModel<ReferralViewModel>> GetReferralsAsync(int rsiId)
        {
            _ListViewModel<ReferralViewModel> model = new _ListViewModel<ReferralViewModel>();

            try
            {
                var tmp = await _context.Referrals.Where(x => x.RSIId == rsiId).ToListAsync();

                foreach(var row in tmp)
                {
                    ReferralViewModel m = new ReferralViewModel()
                    {
                        DateOfBirth = row.DateOfBirth,
                        Email = row.Email,
                        FirstName = row.FirstName,
                        HomePhone = row.Phone1,
                        LastName = row.LastName,
                        MiddleName = row.MiddleName,
                        MobilePhone = row.Phone2,
                        ReferralId = row.ReferralId,
                        Relationship = row.Relationship
                    };

                    model.Rows.Add(m);
                }

                model.TotalCount = model.Rows.Count;
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new _ListViewModel<ReferralViewModel>();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }

        public async Task<(bool isSuccess, string message)> AddUpdateReferralsAsync(int rsiId, List<ReferralViewModel> referrals)
        {
            (bool isSuccess, string message) returnObj = (false, "");

            try
            {
                MemberModel member = await _context.Users.FirstOrDefaultAsync(x => x.MemberId == rsiId);
                ReferralModel tmp = null;
                Random rnd = new Random();
                int padding = rnd.Next(100, 1000);
                string updateId = $"{rsiId.ToString()}_{padding.ToString()}";

                if (member != null && member.MemberId > 0)
                {
                    

                    foreach(var row in referrals)
                    {

                        if (row.ReferralId > 0)
                        {
                            tmp = await _context.Referrals.FirstAsync(x => x.ReferralId == row.ReferralId);

                            if (tmp != null && tmp.ReferralId > 0)
                            {
                                tmp.DateOfBirth = row.DateOfBirth;
                                tmp.Email = row.Email;
                                tmp.FirstName = row.FirstName;
                                tmp.LastName = row.LastName;
                                tmp.MiddleName = row.MiddleName;
                                tmp.Phone1 = row.HomePhone;
                                tmp.Phone2 = row.MobilePhone;
                                tmp.UpdateTimeStamp = updateId;
                                tmp.Relationship = row.Relationship;

                            }
                        }
                        else
                        {
                            tmp = new ReferralModel()
                            {
                                CreationDate = DateTime.Now,
                                DateOfBirth = row.DateOfBirth,
                                Email = row.Email,
                                FirstName = row.FirstName,
                                LastName = row.LastName,
                                MiddleName = row.MiddleName,
                                Phone1 = row.HomePhone,
                                Phone2 = row.MobilePhone,
                                RSIId = rsiId,
                                UpdateTimeStamp = updateId,
                                Relationship = row.Relationship
                            };

                            await _context.Referrals.AddAsync(tmp);
                        }

                        await _context.SaveChangesAsync();
                    }

                    var deleteItems = await _context.Referrals.Where(r => r.RSIId == rsiId && r.UpdateTimeStamp != updateId).ToListAsync();

                    foreach (var rowDelete in deleteItems)
                    {
                        _context.Referrals.Remove(rowDelete);
                    }

                    await _context.SaveChangesAsync();

                    returnObj = (true, "Success");
                }
                else
                    returnObj = (false, $"Error: ({rsiId}) is not found");
            }
            catch (Exception ex)
            {
                returnObj = (false, $"Error: {ex.Message}");
            }

            return returnObj;
        }

        public async Task<(bool isSuccess, string message)> AddUpdateFamilyAsync(int rsiId, List<FamilyMemberViewModel> family)
        {
            (bool isSuccess, string message) returnObj = (false, "");
            try
            {
                MemberModel member = await _context.Users.FirstOrDefaultAsync(x => x.MemberId == rsiId);

                if (member != null && member.MemberId > 0)
                {
                    returnObj = (true, "Success");
                }
                else
                    returnObj = (false, $"Error: ({rsiId}) is not found");
                
            }
            catch (Exception ex)
            {
                returnObj = (false, $"Error: {ex.Message}");
            }

            return returnObj;
        }

        public async Task<MemberInfoViewModel> UpdateMemberAsync(int rsiId, MemberInfoViewModel model)
        {
            try
            {
                MemberModel member = await _context.Users.FirstOrDefaultAsync(x => x.MemberId == rsiId);

                if(member != null && member.MemberId > 0)
                {
                    member.fname = model.PrimaryMember.FirstName;
                    member.MiddleInitial = model.PrimaryMember.MiddleName;
                    member.lname = model.PrimaryMember.LastName;
                    member.Address = model.PrimaryMember.Address1;
                    member.Address2 = model.PrimaryMember.Address2;
                    member.City = model.PrimaryMember.City;
                    member.StateCode = model.PrimaryMember.State;
                    member.PostalCode = model.PrimaryMember.PostalCode;
                    member.CountryCode = model.PrimaryMember.Country;
                    member.phone1 = model.PrimaryMember.HomePhone;
                    member.phone2 = model.PrimaryMember.MobilePhone;
                    member.email = model.PrimaryMember.Email;

                    model.Message = "Success";
                }
                else
                {
                    model.Message = $"Error: ({rsiId}) is not found";
                }
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new MemberInfoViewModel();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }

        public async Task<(bool isSuccess, string message, List<TravelDetailViewModel> travels)> GetTravelInfoAsync(int rsiId)
        {
            (bool isSuccess, string message, List<TravelDetailViewModel> travels) model = (false, "", new List<TravelDetailViewModel>());

            try
            {
                using (var conn = new SqlConnection(SqlHelper.GetConnectionString()))
                {
                    var parameters = new[]
                    {
                            new SqlParameter("@RSIId", rsiId)
                        };

                    var rdr = await SqlHelper.ExecuteReaderAsync(
                           conn,
                           CommandType.StoredProcedure,
                           "[dbo].[CB_TravelListByRSIId]",
                           parameters);
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            TravelDetailViewModel travel = new TravelDetailViewModel()
                            {
                                BookingReference = !rdr.IsDBNull(0) ? rdr.GetInt32(0).ToString() : "",
                            };

                            if (!rdr.IsDBNull(1))
                                travel.DateBooked = rdr.GetDateTime(1);

                            travel.TripType = !rdr.IsDBNull(2) ? rdr.GetString(2) : "";
                            travel.TripCategory = !rdr.IsDBNull(3) ? rdr.GetString(3) : "";
                            travel.AccommodationName = !rdr.IsDBNull(4) ? rdr.GetString(4) : "";
                            travel.AccommodationDescription = !rdr.IsDBNull(5) ? rdr.GetString(5) : "";

                            if (!rdr.IsDBNull(6))
                                travel.ArrivalDate = rdr.GetDateTime(6);

                            if (!rdr.IsDBNull(7))
                                travel.DepartureDate = rdr.GetDateTime(7);

                            model.travels.Add(travel);
                        }
                    }

                    model.isSuccess = true;
                    model.message = "Success";
                }
            }
            catch (Exception ex)
            {
                model.isSuccess = false;
                model.message = ex.Message;
            }

            return model;
        }

        public async Task<MemberInfoViewModel> GetMemberAsync(int rsiId)
        {
            MemberInfoViewModel model = new MemberInfoViewModel();

            try
            {
                var tmp = await (from u in _context.Users
                           join o in _context.Organizations on u.org equals o.OrganizationId
                           join r in _context.RenewalInfo on u.MemberId equals r.RSIId into gj
                           from subr in gj.DefaultIfEmpty()
                           where u.MemberId == rsiId
                           select new
                           {
                               u.fname,
                               u.MiddleInitial,
                               u.lname,
                               u.org,
                               o.OrganizationName,
                               u.email,
                               u.phone2,
                               u.phone1,
                               u.ActivationDate,
                               subr.RenewalFee,
                               subr.DateOfLastPaid,
                               u.Address,
                               u.Address2,
                               u.City,
                               u.StateCode,
                               u.PostalCode,
                               u.CountryCode
                           }).FirstOrDefaultAsync();

                if(tmp != null)
                {
                    model = new MemberInfoViewModel();

                    model.PrimaryMember.FirstName = tmp.fname;
                    model.PrimaryMember.MiddleName = tmp.MiddleInitial;
                    model.PrimaryMember.LastName = tmp.lname;
                    model.PrimaryMember.RSIId = rsiId;
                    model.PrimaryMember.Email = tmp.email;
                    model.PrimaryMember.MobilePhone = tmp.phone2;
                    model.PrimaryMember.HomePhone = tmp.phone1;
                    model.PrimaryMember.Address1 = tmp.Address;
                    model.PrimaryMember.Address2 = tmp.Address2;
                    model.PrimaryMember.City = tmp.City;
                    model.PrimaryMember.State = tmp.StateCode;
                    model.PrimaryMember.PostalCode = tmp.PostalCode;
                    model.PrimaryMember.Country = tmp.CountryCode;
                    
                    DateTime now = DateTime.Today;
                    int age = now.Year - tmp.ActivationDate.GetValueOrDefault().Year;
                    if (tmp.ActivationDate.GetValueOrDefault() > now.AddYears(-age)) age--;

                    model.MembershipInfo.ActivationDate = tmp.ActivationDate.GetValueOrDefault();
                    model.MembershipInfo.YearsAsMember = age;
                    model.MembershipInfo.CondoWeeks = 3;
                    model.MembershipInfo.DuesAmount = tmp.RenewalFee;
                    model.MembershipInfo.LastDuesPayment = tmp.DateOfLastPaid;

                    model.OrganizationInfo.OrganizationId = tmp.org.GetValueOrDefault(0);
                    model.OrganizationInfo.OrganizationName = tmp.OrganizationName;

                    PackageViewModel packages = await _packageService.GetPackageInfoByRSIIdAsync(rsiId);

                    if(packages != null && packages.PackageId > 0)
                    {
                        model.PackageInfo = new PackageInfoViewModel()
                        {
                            PackageId = packages.PackageId,
                            PackageName = packages.PackageName
                        };

                        foreach(var row in packages.Benefits)
                        {
                            switch (row.Category.ToUpper())
                            {
                                case "BRIO":
                                    model.PackageInfo.Benefits.HasBrioResorts = true;
                                    break;
                                case "CONDO":
                                    model.PackageInfo.Benefits.HasCondos = true;
                                    break;
                                case "CRUISE":
                                    
                                    break;
                                case "HOTEL":
                                    model.PackageInfo.Benefits.HasHotels = true;
                                    break;
                                case "AIR":
                                    model.PackageInfo.Benefits.HasFlights = true;
                                    break;
                                case "CAR":
                                    model.PackageInfo.Benefits.HasCars = true;
                                    break;
                                case "GETAWAY":
                                    model.PackageInfo.Benefits.HasFantasyGetaways = true;
                                    model.PackageInfo.Benefits.HasCondoGetaways = true;
                                    model.PackageInfo.Benefits.HasCruiseGetaways = true;
                                    model.PackageInfo.Benefits.HasStaycationGetaways = true;
                                    break;
                                case "LEISUREHUB":
                                    model.PackageInfo.Benefits.HasLeisureHub = true;
                                    break;
                            }
                        }
                    }

                    using (var conn = new SqlConnection(SqlHelper.GetConnectionString()))
                    {
                        var parameters = new[]
                        {
                            new SqlParameter("@RSIId", rsiId)
                        };

                        var rdr = await SqlHelper.ExecuteReaderAsync(
                               conn,
                               CommandType.StoredProcedure,
                               "[dbo].[GetFamilyMembersByRSIId]",
                               parameters);
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                string relationship = !rdr.IsDBNull(5) ? rdr.GetString(5) : "";

                                if (relationship == "Significant_Other" || relationship == "SPOUSE")
                                {
                                    PersonViewModel person = new PersonViewModel()
                                    {
                                        RSIId = unchecked((int)rdr.GetInt64(0)),
                                        FirstName = !rdr.IsDBNull(2) ? rdr.GetString(2) : "",
                                        LastName = !rdr.IsDBNull(3) ? rdr.GetString(3) : "",
                                        Address1 = !rdr.IsDBNull(11) ? rdr.GetString(11) : "",
                                        Address2 = "",
                                        City = !rdr.IsDBNull(12) ? rdr.GetString(12) : "",
                                        State = !rdr.IsDBNull(13) ? rdr.GetString(13) : "",
                                        PostalCode = !rdr.IsDBNull(14) ? rdr.GetString(14) : "",
                                        Country = !rdr.IsDBNull(15) ? rdr.GetString(15) : "",
                                        Email = !rdr.IsDBNull(6) ? rdr.GetString(6) : "",
                                        HomePhone = !rdr.IsDBNull(7) ? rdr.GetString(7) : "",
                                        MobilePhone = !rdr.IsDBNull(9) ? rdr.GetString(9) : ""

                                    };

                                    person.MiddleName = "";
                                    model.SecondaryMember = person;
                                }
                                else
                                {

                                    FamilyMemberViewModel family = new FamilyMemberViewModel
                                    {
                                        MiddleName = "",

                                        FamilyMemberId = rdr.GetInt64(0),
                                        FistName = !rdr.IsDBNull(2) ? rdr.GetString(2) : "",
                                        LastName = !rdr.IsDBNull(3) ? rdr.GetString(3) : ""
                                    };

                                    if (!rdr.IsDBNull(4))
                                        family.DateOfBirth = rdr.GetDateTime(4);
                                    else
                                        family.DateOfBirth = null;

                                    family.Relationship = relationship;
                                    family.Email = !rdr.IsDBNull(6) ? rdr.GetString(6) : "";

                                    family.PrimaryPhone = !rdr.IsDBNull(7) ? rdr.GetString(7) : "";
                                    family.PrimaryPhoneType = !rdr.IsDBNull(8) ? rdr.GetString(8) : "";

                                    family.AlternativePhone = !rdr.IsDBNull(9) ? rdr.GetString(9) : "";
                                    family.AlternativePhoneType = !rdr.IsDBNull(10) ? rdr.GetString(10) : "";

                                    if (family.DateOfBirth != null)
                                    {
                                        age = now.Year - family.DateOfBirth.GetValueOrDefault().Year;
                                        if (family.DateOfBirth.GetValueOrDefault() > now.AddYears(-age)) age--;

                                        family.Age = age;
                                    }

                                    model.FamilyMembers.Add(family);
                                }
                            }

                            
                        }
                    }

                    using (var conn = new SqlConnection(SqlHelper.GetConnectionString()))
                    {
                        var parameters = new[]
                        {
                            new SqlParameter("@RSIId", rsiId)
                        };

                        var rdr = await SqlHelper.ExecuteReaderAsync(
                               conn,
                               CommandType.StoredProcedure,
                               "[dbo].[CB_TravelListByRSIId]",
                               parameters);
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                TravelDetailViewModel travel = new TravelDetailViewModel()
                                {
                                    BookingReference = !rdr.IsDBNull(0) ? rdr.GetInt32(0).ToString() : "",
                                };

                                if (!rdr.IsDBNull(1))
                                    travel.DateBooked = rdr.GetDateTime(1);

                                travel.TripType = !rdr.IsDBNull(2) ? rdr.GetString(2) : "";
                                travel.TripCategory = !rdr.IsDBNull(3) ? rdr.GetString(3) : "";
                                travel.AccommodationName = !rdr.IsDBNull(4) ? rdr.GetString(4) : "";
                                travel.AccommodationDescription = !rdr.IsDBNull(5) ? rdr.GetString(5) : "";

                                if (!rdr.IsDBNull(6))
                                    travel.ArrivalDate = rdr.GetDateTime(6);

                                if (!rdr.IsDBNull(7))
                                    travel.DepartureDate = rdr.GetDateTime(7);

                                model.TravelInfo.Add(travel);
                            }
                        }
                    }

                    model.Message = "Success";
                }
                else
                {
                    model.Message = $"Error: Member Id: {rsiId} is not found";
                }
                           
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new MemberInfoViewModel();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }

        public async Task<_ListViewModel<MemberListViewModel>> GetMembersAsync(MemberSearchViewModel search)
        {
            _ListViewModel<MemberListViewModel> model = new _ListViewModel<MemberListViewModel>();

            try
            {
                var qry = from u in _context.Users
                          join o in _context.Organizations on u.org equals o.OrganizationId
                          select new 
                          {
                              u.email,
                              u.fname,
                              u.lname,
                              u.MemberId,
                              u.MiddleInitial,
                              u.org,
                              o.OrganizationName,
                              u.phone1,
                              u.phone2
                          };

                if (!String.IsNullOrEmpty(search.FistName))
                {
                    if (search.ExactMatch)
                        qry = qry.Where(w => w.fname == search.FistName);
                    else
                        qry = qry.Where(w => w.fname.Contains(search.FistName));
                }

                if (!String.IsNullOrEmpty(search.LastName))
                {
                    if (search.ExactMatch)
                        qry = qry.Where(w => w.lname == search.LastName);
                    else
                        qry = qry.Where(w => w.lname.Contains(search.LastName));
                }

                if (!String.IsNullOrEmpty(search.Email))
                {
                    if (search.ExactMatch)
                        qry = qry.Where(w => w.email == search.Email);
                    else
                        qry = qry.Where(w => w.email.Contains(search.Email));
                }

                if (!String.IsNullOrEmpty(search.Phone))
                {
                    if (search.ExactMatch)
                        qry = qry.Where(w => w.phone1 == search.Phone || w.phone2 == search.Phone);
                    else
                        qry = qry.Where(w => w.phone1.Contains(search.Phone) || w.phone2.Contains(search.Phone));
                }

                if (search.OrganizationId != null && search.OrganizationId > 0)
                {
                    qry = qry.Where(w => w.org == search.OrganizationId);
                }

                if (!String.IsNullOrEmpty(search.CatchAll))
                {
                    if (search.ExactMatch)
                    {
                        //int orgId = 0;

                        qry = qry.Where(
                            w => w.fname == search.CatchAll ||
                                (w.lname != null && w.lname == search.CatchAll) ||
                                w.email == search.CatchAll ||
                                w.phone1 == search.CatchAll ||
                                w.phone2 == search.CatchAll ||
                                //w.org == (int.TryParse(search.CatchAll, out orgId) ? orgId : w.org) ||
                                w.OrganizationName == search.CatchAll
                            );
                    }
                    else
                    {
                        //int orgId = 0;

                        qry = qry.Where(
                            w => w.fname.Contains(search.CatchAll) ||
                                w.lname.Contains(search.CatchAll) ||
                                w.email.Contains(search.CatchAll) ||
                                w.phone1.Contains(search.CatchAll) ||
                                w.phone2.Contains(search.CatchAll) ||
                                //w.org == (int.TryParse(search.CatchAll, out orgId) ? orgId : w.org) ||
                                w.OrganizationName.Contains(search.CatchAll)
                            );
                    }
                }

                string sortColumn = "DEFAULT";

                if (!String.IsNullOrEmpty(search.SortColumn))
                {
                    sortColumn = search.SortColumn.ToUpper();
                }

                string sortDirection = "ASC";

                if (!String.IsNullOrEmpty(search.SortDirection))
                    sortDirection = search.SortDirection;

                if (sortDirection == "ASC")
                {
                    switch (search.SortColumn)
                    {
                        case "EMAIL":
                            qry = qry.OrderBy(o => o.email);
                            break;
                        case "PHONE":
                            qry = qry.OrderBy(o => o.phone1).ThenBy(r => r.phone2);
                            break;
                        case "ORGANIZATIONNAME":
                            qry = qry.OrderBy(o => o.OrganizationName);
                            break;
                        default:
                            qry = qry.OrderBy(o=> o.lname).ThenBy(o=> o.fname);
                            break;

                    }
                }
                else
                {
                    switch (search.SortColumn)
                    {
                        case "EMAIL":
                            qry = qry.OrderByDescending(o => o.email);
                            break;
                        case "PHONE":
                            qry = qry.OrderByDescending(o => o.phone1).ThenByDescending(r => r.phone2);
                            break;
                        case "ORGANIZATIONNAME":
                            qry = qry.OrderByDescending(o => o.OrganizationName);
                            break;
                        default:
                            qry = qry.OrderBy(o=> o.lname).ThenBy(o=> o.fname);
                            //qry = qry.OrderByDescending(o => o.LastName).ThenByDescending(t => t.FirstName);
                            break;
                    }
                }

                model.TotalCount = await qry.CountAsync();

                if(search.NumberOfRows.GetValueOrDefault(0) > 0)
                {
                    qry = qry.Skip(search.StartRowIndex.GetValueOrDefault(0)).Take(search.NumberOfRows.GetValueOrDefault(0));
                }
                
                List<MemberListViewModel> tmp = new List<MemberListViewModel>();

                foreach(var row in qry)
                {
                    MemberListViewModel t = new MemberListViewModel()
                    {
                        Email = row.email,
                        FirstName = row.fname,
                        LastName = row.lname,
                        MemberId = row.MemberId,
                        MiddleName = row.MiddleInitial,
                        OrganizationId = row.org.GetValueOrDefault(0),
                        OrganizationName = row.OrganizationName,
                        Phone1 = row.phone1,
                        Phone2 = row.phone2
                    };

                    tmp.Add(t);
                }


                model.Rows = tmp;

                model.Message = "Success";
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new _ListViewModel<MemberListViewModel>();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }

        
    }
}