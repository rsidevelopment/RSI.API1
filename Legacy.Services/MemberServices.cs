﻿using AccessDevelopment.Services.Interfaces;
using AccessDevelopment.Services.Models._ViewModels;
using Hangfire;
using Legacy.Services.Data;
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
using System.Threading.Tasks;

namespace Legacy.Services
{
    public class MemberServices : IMemberService
    {
        private readonly RSIDbContext _rsiContext;
        private readonly LegacyDbContext _legacyContext;
        private readonly IHangFireService _hfService;
        private readonly IPackageService _packageService;
        private readonly IAccessDevelopmentService _accessDevelopmentService;

        public MemberServices(RSIDbContext rsiContext, LegacyDbContext legacyContext, IHangFireService hfContext, IPackageService packageService, IAccessDevelopmentService accessDevelopmentService)
        {
            _rsiContext = rsiContext;
            _legacyContext = legacyContext;
            _hfService = hfContext;
            _packageService = packageService;
            _accessDevelopmentService = accessDevelopmentService;
        }

        public async Task<(bool isSuccess, string message, List<FamilyMemberViewModel> family)> GetFamilyAsync(int rsiId, string clubReference)
        {
            (bool isSuccess, string message, List<FamilyMemberViewModel> family) returnModel = (false, "", new List<FamilyMemberViewModel>());

            try
            {
                int auth = await (from u in _legacyContext.Users
                           join c in _legacyContext.BrioClubLeads on u.MemberId equals c.rsiId
                          where u.MemberId == rsiId && (c.clubReference == clubReference ||
                          clubReference == string.Empty)
                         select u.MemberId).FirstOrDefaultAsync();

                DateTime now = DateTime.Today;
                //int age = now.Year - tmp.ActivationDate.GetValueOrDefault().Year;
                //if (tmp.ActivationDate.GetValueOrDefault() > now.AddYears(-age)) age--;

                MemberModel member = await _legacyContext.Users.FirstOrDefaultAsync(x => x.MemberId == rsiId);
                if (member != null && member.MemberId > 0 && member.MemberId == auth)
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
                                        FirstName = !rdr.IsDBNull(2) ? rdr.GetString(2) : "",
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

        public async Task<MemberUpgradeViewModel> GetUpgradeInfoAsync(int rsiId, string clubReference)
        {
            MemberUpgradeViewModel model = new MemberUpgradeViewModel();

            try
            {
                int auth = await (from u in _legacyContext.Users
                                  join c in _legacyContext.BrioClubLeads on u.MemberId equals c.rsiId
                                 where u.MemberId == rsiId && c.clubReference == clubReference
                                 select u.MemberId).FirstOrDefaultAsync();

                var tmp = await _legacyContext.UpgradeAudits.FirstOrDefaultAsync(x => x.RSIId == rsiId);

                if(tmp != null && tmp.UpgradeAuditId > 0 && auth == rsiId)
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
                        UpgradeProgram = tmp.UpgradeProgram,
                        IsUnlimitedPoints = tmp.IsUnlimitedPoints
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
                    UpgradeAuditModel m1 = await _legacyContext.UpgradeAudits.FirstOrDefaultAsync(x => x.RSIId == model.RSIId);
                    if (m1 != null && m1.UpgradeAuditId > 0)
                        model.UpgradeAuditId = m1.UpgradeAuditId;
                }

                UpgradeAuditModel m = null;

                if (model.UpgradeAuditId > 0)
                {
                    m = await _legacyContext.UpgradeAudits.FirstOrDefaultAsync(x => x.RSIId == model.RSIId);
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
                        m.IsUnlimitedPoints = model.IsUnlimitedPoints;
                        
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

                    await _legacyContext.UpgradeAudits.AddAsync(m);
                }

                await _legacyContext.SaveChangesAsync();

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
                var tmp = await _legacyContext.Referrals.Where(x => x.RSIId == rsiId).ToListAsync();

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
                MemberModel member = await _legacyContext.Users.FirstOrDefaultAsync(x => x.MemberId == rsiId);
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
                            tmp = await _legacyContext.Referrals.FirstAsync(x => x.ReferralId == row.ReferralId);

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

                            await _legacyContext.Referrals.AddAsync(tmp);
                        }

                        await _legacyContext.SaveChangesAsync();
                    }

                    var deleteItems = await _legacyContext.Referrals.Where(r => r.RSIId == rsiId && r.UpdateTimeStamp != updateId).ToListAsync();

                    foreach (var rowDelete in deleteItems)
                    {
                        _legacyContext.Referrals.Remove(rowDelete);
                    }

                    await _legacyContext.SaveChangesAsync();

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

        class IdResponse
        {
            public long Id { get; set; }
        }
        void DeactivateAuthorizedUser(long rsiId)
        {
            if (rsiId == 0) return;

            var result = _rsiContext.LoadStoredProc("dbo.DeactivateAuthorizedUser")
            .WithSqlParam("Id", rsiId)
            .ExecuteStoredProcAsync<IdResponse>().Result.FirstOrDefault();

            if (result.Id != rsiId) throw new Exception($"DeactivateAuthorizedUser failed for RSIId: {rsiId}");
        }
        void AddUpdateCRMAuthorizedUsers(long rsiId, FamilyMemberViewModel familyMember)
        {
            if (familyMember.Relationship != "SPOUSE")
            {
                var result = _rsiContext.LoadStoredProc("dbo.AddUpdateCRMAuthorizedUsers")
                .WithSqlParam("@Id", familyMember.FamilyMemberId)
                .WithSqlParam("@RSIId", rsiId)
                .WithSqlParam("@FirstName", familyMember.FirstName)
                .WithSqlParam("@LastName", familyMember.LastName)
                .WithSqlParam("@BirthDate", familyMember.DateOfBirth)
                .WithSqlParam("@Relationship", familyMember.Relationship)
                .WithSqlParam("@Email", familyMember.Email)
                .WithSqlParam("@PrimaryPhone", familyMember.PrimaryPhone)
                .WithSqlParam("@SecondaryPhone", familyMember.AlternativePhone)
                .WithSqlParam("@Address", string.Empty)
                .WithSqlParam("@City", string.Empty)
                .WithSqlParam("@State", string.Empty)
                .WithSqlParam("@PostalCode", string.Empty)
                .WithSqlParam("@Country", string.Empty)
                .ExecuteStoredProcAsync<IdResponse>().Result.FirstOrDefault();

                if (result.Id == 0) throw new Exception($"AddUpdateCRMAuthorizedUsers failed for RSIId: {rsiId} Family Member Id: {familyMember.FamilyMemberId}");
            }
        }
        void AddUpdateCRMAuthorizedUsers(long rsiId, PersonViewModel secondary)
        {
            var result = _rsiContext.LoadStoredProc("dbo.AddUpdateCRMAuthorizedUsers")
            .WithSqlParam("@Id", secondary.RSIId)
            .WithSqlParam("@RSIId", rsiId)
            .WithSqlParam("@FirstName", secondary.FirstName)
            .WithSqlParam("@LastName", secondary.LastName)
            .WithSqlParam("@BirthDate", secondary.DateOfBirth)
            .WithSqlParam("@Relationship", "SPOUSE")
            .WithSqlParam("@Email", secondary.Email)
            .WithSqlParam("@PrimaryPhone", secondary.HomePhone)
            .WithSqlParam("@SecondaryPhone", secondary.MobilePhone)
            .WithSqlParam("@Address", $"{secondary.Address1} {secondary.Address2}")
            .WithSqlParam("@City", secondary.City)
            .WithSqlParam("@State", secondary.State)
            .WithSqlParam("@PostalCode", secondary.PostalCode)
            .WithSqlParam("@Country", secondary.Country)
            .ExecuteStoredProcAsync<IdResponse>().Result.FirstOrDefault();

            if (result.Id == 0) throw new Exception($"AddUpdateCRMAuthorizedUsers failed for RSIId: {rsiId} Secondary Member Id: {secondary.RSIId}");
        }
        public async Task<(bool isSuccess, string message)> AddUpdateFamilyAsync(int rsiId, List<FamilyMemberViewModel> family)
        {
            (bool isSuccess, string message) returnObj = (true, "");
            try
            {
                MemberModel member = await _legacyContext.Users.FirstOrDefaultAsync(x => x.MemberId == rsiId);

                if (member == null || member.MemberId == 0)
                    return (false, $"Error: ({rsiId}) is not found");

                var previousFamily = await GetFamilyAsync(rsiId, string.Empty);
                foreach (var fm in previousFamily.family)
                {
                    if (fm.Relationship != "SPOUSE" || fm.Relationship != "Significant_Other")
                    {
                        if (!family.Any(f => { return f.FamilyMemberId == fm.FamilyMemberId; }))
                            DeactivateAuthorizedUser(fm.FamilyMemberId);
                    }
                }

                foreach (var fm in family)
                {
                    if (fm.Relationship != "SPOUSE" || fm.Relationship != "Significant_Other")
                        AddUpdateCRMAuthorizedUsers(rsiId, fm);
                }
            }
            catch (Exception ex)
            {
                returnObj = (false, $"Error: {ex.Message}");
            }

            return returnObj;
        }

        public async Task<MemberInfoViewModel> AddMemberAsync(MemberInfoViewModel model)
        {
            try
            {
                MemberModel member = new MemberModel();

                if (model.OrganizationInfo != null)
                {
                    member.org = model.OrganizationInfo.OrganizationId;
                    if (model.PackageInfo != null)
                    {
                        member.PackageId = model.PackageInfo.PackageId;
                        member.HotelRewards = model.PackageInfo.Points.ToString();
                        
                        if (model.PrimaryMember != null)
                        {
                            int ct = _legacyContext.Users.Count(w => (
                                w.email == model.PrimaryMember.Email ||
                                w.UserName == model.PrimaryMember.Email ||
                                w.phone1 == model.PrimaryMember.HomePhone ||
                                w.phone1 == model.PrimaryMember.MobilePhone ||
                                w.phone2 == model.PrimaryMember.HomePhone ||
                                w.phone2 == model.PrimaryMember.MobilePhone) && 
                                w.org == model.OrganizationInfo.OrganizationId);

                            if (ct < 1)
                            {
                                member.SalesDate = DateTime.Now;
                                member.TemplateAddDate = DateTime.Now;
                                member.SiteBlockReason = "";
                                member.ActivationDate = DateTime.Now;
                                member.CreationDate = DateTime.Now;
                                member.CreatorId = 0;
                                member.AddProvider = false;
                                member.AddRep = false;
                                member.Admin = 0;
                                member.AmountPaid = "0";
                                member.BlockedReason = "";
                                member.BulkNumber = false;
                                member.CheckInfo = false;
                                member.Comments = "";
                                member.Company = "";
                                member.CondoRewards = "0";
                                member.Currency = "USD";
                                member.ExpiryDate = DateTime.Now.AddYears(10);
                                member.Guest = false;
                                member.IsCompedAccount = false;
                                member.KeepMe = true;
                                member.Language = "EN";
                                member.MembershipLengthInDays = (365 * 10);
                                member.MembershipRenewalAmount = 0;
                                member.MemberStatus = "M";
                                member.Military = false;
                                member.Military2 = false;
                                member.MiniVacs = false;
                                member.MultipleOrgs = false;
                                member.NDR = false;
                                member.OptOut = false;
                                member.org = model.OrganizationInfo.OrganizationId;
                                member.OrigionalCondoRewards = "0";
                                member.OrigionalCruiseRewards = "0";
                                member.OrigionalHotelRewards = model.PackageInfo.Points.ToString();
                                member.Password = "Changeme%123";
                                member.Prompts = false;
                                member.Provider = false;
                                member.ProviderAdd = false;
                                member.ProviderEdit = false;
                                member.Reject = false;
                                member.RejectReason = "";
                                member.RenewalDate = DateTime.Now.AddYears(10);
                                member.RenewalLength = (365 * 10);
                                member.RenewalPriceEdit = false;
                                member.RenewalSkipBilling = false;
                                member.RenewalSoftConfirm = false;
                                member.Rep = false;
                                member.ResortLookup = false;
                                member.ReturnedToDR = false;
                                member.SoapCreationLocation = "APIV1";
                                member.SoapCreatorReference = "";
                                member.Tickets = false;
                                member.TransferOffered = false;
                                member.unitcost = "0";
                                member.UserLevel = 3;
                                member.UserName = model.PrimaryMember.Email;
                                member.ViewOnly = false;

                                member.fname = model.PrimaryMember.FirstName;
                                member.MiddleInitial = model.PrimaryMember.MiddleInitial;
                                member.lname = model.PrimaryMember.LastName;
                                member.BirthDate = model.PrimaryMember.DateOfBirth;

                                member.Address = model.PrimaryMember.Address1;
                                member.Address2 = model.PrimaryMember.Address2;
                                member.City = model.PrimaryMember.City;
                                member.StateCode = model.PrimaryMember.State;
                                member.PostalCode = model.PrimaryMember.PostalCode;
                                member.CountryCode = model.PrimaryMember.Country;

                                member.phone1 = model.PrimaryMember.HomePhone;
                                member.phone2 = model.PrimaryMember.MobilePhone;
                                member.email = model.PrimaryMember.Email;


                                if (model.SecondaryMember != null)
                                {
                                    member.FirstName2 = model.SecondaryMember.FirstName;
                                    member.MiddleInitial2 = model.SecondaryMember.MiddleInitial;
                                    member.LastName2 = model.SecondaryMember.LastName;
                                    member.BirthDate2 = model.SecondaryMember.DateOfBirth;
                                    member.email2 = model.SecondaryMember.Email;
                                }

                                member.Family = model.FamilyMemberString;
                                member.IsActive = true;

                                await _legacyContext.Users.AddAsync(member);
                                await _legacyContext.SaveChangesAsync();
                            }
                            else
                            {
                                model.Message = "Error: Either the email or phone number is already in our system";
                            }
                        }
                        else
                        {
                            model.Message = $"Error: Primary member not found";
                        }
                    }
                    else
                    {
                        model.Message = "Error: Package Info not found";
                    }
                }
                else
                {
                    model.Message = "Error: Organization Info not found";
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

        public async Task<MemberInfoViewModel> UpdateMemberAsync(int rsiId, MemberInfoViewModel model)
        {
            try
            {
                //var jobData = await _hfService.NewRSIJobData(model, rsiId, 
                //    (model.PackageInfo != null) ? model.PackageInfo.PackageId.ToString() : "0");

                UpdateMemberInLegacyRSIDb(model, rsiId);
                await UpdateMemberInRSIDbAsync(model, rsiId);
                UpdateFamilyInRSIDb(model, rsiId);
                //BackgroundJob.Enqueue<MemberServices>(x => x.UpdateMemberInLegacyRSIDb(jobData.jobId, rsiId));
                //BackgroundJob.Enqueue<MemberServices>(x => x.UpdateMemberInRSIDb(jobData.jobId, rsiId));
                //BackgroundJob.Enqueue<MemberServices>(x => x.UpdateFamilyInRSIDb(jobData.jobId, rsiId));

                return new MemberInfoViewModel() { Message = "Success" };
            }
            catch (Exception ex)
            {
                return new MemberInfoViewModel() { Message = $"Error: {ex.Message}" };
            }
        }

        [Queue("rsi_api")]
        //public void UpdateMemberInLegacyRSIDb(int jobId, int rsiId)
        public void UpdateMemberInLegacyRSIDb(MemberInfoViewModel model, int rsiId)
        {
            //var model = _hfService.GetModelForJobId<MemberInfoViewModel>(jobId).Result;
            MemberModel member = _legacyContext.Users.FirstOrDefault(x => x.MemberId == rsiId);

            if (member != null && member.MemberId > 0)
            {
                //org/package info
                if (model.OrganizationInfo != null) member.org = model.OrganizationInfo.OrganizationId;
                if (model.PackageInfo != null)
                {
                    member.PackageId = model.PackageInfo.PackageId;
                    member.HotelRewards = model.PackageInfo.Points.ToString();
                }

                //primary info
                if (model.PrimaryMember != null)
                {
                    member.fname = model.PrimaryMember.FirstName;
                    member.MiddleInitial = model.PrimaryMember.MiddleInitial;
                    member.lname = model.PrimaryMember.LastName;
                    member.BirthDate = model.PrimaryMember.DateOfBirth;

                    member.Address = model.PrimaryMember.Address1;
                    member.Address2 = model.PrimaryMember.Address2;
                    member.City = model.PrimaryMember.City;
                    member.StateCode = model.PrimaryMember.State;
                    member.PostalCode = model.PrimaryMember.PostalCode;
                    member.CountryCode = model.PrimaryMember.Country;

                    member.phone1 = model.PrimaryMember.HomePhone;
                    member.phone2 = model.PrimaryMember.MobilePhone;
                    member.email = model.PrimaryMember.Email;
                }

                //secondary info
                if (model.SecondaryMember != null)
                {
                    member.FirstName2 = model.SecondaryMember.FirstName;
                    member.MiddleInitial2 = model.SecondaryMember.MiddleInitial;
                    member.LastName2 = model.SecondaryMember.LastName;
                    member.BirthDate2 = model.SecondaryMember.DateOfBirth;
                    //member.email2 = model.SecondaryMember.Email;
                }

                member.Family = model.FamilyMemberString;
                member.IsActive = true;

                #region Unmapped MemberModel fields
                //member.MemberId = model. ;
                //member.UserName = model. ;
                //member.Password = model. ;
                //member.Language = model. ;
                //member.Admin = model. ;
                //member.UserLevel = model. ;
                //member.Guest = model. ;
                //member.Prompts = model. ;
                //member.CreationDate = model. ;
                //member.CreatorId = model. ;
                //member.LastLogin = model. ;
                //member.LoginIp = model. ;
                //member.Extension1 = model. ;
                //member.Extension2 = model. ;
                //member.Comments = model. ;
                //member.AddRep = model. ;
                //member.AddProvider = model. ;
                //member.Rep = model. ;
                //member.Provider = model. ;
                //member.RenewalDate = model. ;
                //member.ActivationDate = model. ;
                //member.DR = model. ;
                //member.SentToDR = model. ;
                //member.DateSentToDR = model. ;
                //member.Company = model. ;
                //member.MemberStatus = model. ;
                //member.Currency = model. ;
                //member.Partner = model. ;
                //member.AmountPaid = model. ;
                //member.ViewOnly = model. ;
                //member.MultipleOrgs = model. ;
                //member.OrganizationNames = model. ;
                //member.SiteBlockReason = model. ;
                //member.ReturnedToDR = model. ;
                //member.DateReturnedToDR = model. ;
                //member.BulkNumber = model. ;
                //member.IsTemplate = model. ;
                //member.Reject = model. ;
                //member.RejectReason = model. ;
                //member.CheckInfo = model. ;
                //member.TemplateAddDate = model. ;
                //member.CruiseRewards = model. ;
                //member.CondoRewards = model. ;
                //member.OrganizationCompany = model. ;
                //member.PrimaryMember = model. ;
                //member.ResortLookup = model. ;
                //member.unitcost = model. ;
                //member.ApprovalCode = model. ;
                //member.MiniVacPackage = model. ;
                //member.MiniVacUpgrades = model. ;
                //member.MiniVacs = model. ;
                //member.SalesDate = model. ;
                //member.InternalTracking = model. ;
                //member.ProviderAdd = model. ;
                //member.ProviderEdit = model. ;
                //member.ProviderNumbers = model. ;
                //member.OrigionalHotelRewards = model. ;
                //member.OrigionalCondoRewards = model. ;
                //member.OrigionalCruiseRewards = model. ;
                //member.Tickets = model. ;
                //member.BlockedReason = model. ;
                //member.Fax1 = model. ;
                //member.FaxExtension1 = model. ;
                //member.Fax2 = model. ;
                //member.FaxExtension2 = model. ;
                //member.SignupIP = model. ;
                //member.FirstLogin = model. ;
                //member.ReferralId = model. ;
                //member.RenewalMessageDate = model. ;
                //member.RenewalSoftConfirm = model. ;
                //member.RenewalError = model. ;
                //member.RenewalSoftConfirmDate = model. ;
                //member.FinancingOption = model. ;
                //member.OptOut = model. ;
                //member.NDR = model. ;
                //member.RenewalLength = model. ;
                //member.RenewalTeamKeyId = model. ;
                //member.RenewalPriceEdit = model. ;
                //member.RenewalSkipBilling = model. ;
                //member.Military = model. ;
                //member.Military2 = model. ;
                //member.Phone3 = model. ;
                //member.TransferOffered = model. ;
                //member.TransferOfferedId = model. ;
                //member.TransferOfferedDate = model. ;
                //member.InactiveDate = model. ;
                //member.PasswordQuestion = model. ;
                //member.PasswordAnswer = model. ;
                //member.ContactContactId = model. ;
                //member.ContactSubscriptionId = model. ;
                //member.ContactSendId = model. ;
                //member.MailMonkeyUploadDate = model. ;
                //member.ExpiryDate = model. ;
                //member.MailMonkeyCode = model. ;
                //member.MailMonkeyMessage = model. ;
                //member.DRUserId = model. ;
                //member.OutsideRenewalUserId = model. ;
                //member.MembershipLengthInDays = model. ;
                //member.MembershipRenewalAmount = model. ;
                //member.IsCompedAccount = model. ;
                //member.SoapCreatorReference = model. ;
                //member.SoapCreationLocation = model. ;
                //member.ExpiryOverrideDate = model. ;
                //member.KLLId = model. ;
                //member.DeleteMe = model. ;
                //member.KeepMe = model. ;
                #endregion Unmapped MemberModel fields

                _legacyContext.SaveChanges();
            }
            else
            {
                throw new Exception($"Error: ({rsiId}) is not found");
            }
        }
        [Queue("rsi_api")]
        //public void UpdateMemberInRSIDb(int jobId, int rsiId)
        public async Task UpdateMemberInRSIDbAsync(MemberInfoViewModel model, int rsiId)
        {
            //var model = _hfService.GetModelForJobId<MemberInfoViewModel>(jobId).Result;

            if (model.OrganizationInfo == null) model.OrganizationInfo = new OrganizationInfoViewModel();
            if (model.PackageInfo == null) model.PackageInfo = new PackageInfoViewModel();
            if (model.SecondaryMember == null) model.SecondaryMember = new PersonViewModel();

            #region MemberUpdateVIP
            
            var result = await _rsiContext.LoadStoredProc("dbo.MemberUpdateVIP")
            .WithSqlParam("RSIId", rsiId)
            .WithSqlParam("RSIOrganizationId", model.OrganizationInfo.OrganizationId)
            .WithSqlParam("DistributorId", null)
            .WithSqlParam("PackageId", model.PackageInfo.PackageId)
            .WithSqlParam("AffiliateId", null)
            .WithSqlParam("RSICreatorId", null)
            .WithSqlParam("ClubReference", null)
            .WithSqlParam("CreatorIP", null)
            .WithSqlParam("Username", null)
            .WithSqlParam("Password", null)
            .WithSqlParam("FirstName", model.PrimaryMember.FirstName)
            .WithSqlParam("MiddleName", model.PrimaryMember.MiddleName)
            .WithSqlParam("LastName", model.PrimaryMember.LastName)
            .WithSqlParam("FirstName2", model.SecondaryMember.FirstName)
            .WithSqlParam("MiddleName2", model.SecondaryMember.MiddleName)
            .WithSqlParam("LastName2", model.SecondaryMember.LastName)
            .WithSqlParam("Family", model.FamilyMemberString)
            .WithSqlParam("Phone1", model.PrimaryMember.HomePhone)
            .WithSqlParam("Phone2", model.PrimaryMember.MobilePhone)
            .WithSqlParam("Email1", model.PrimaryMember.Email)
            .WithSqlParam("Email2", model.SecondaryMember.Email)
            .WithSqlParam("Address1", model.PrimaryMember.Address1)
            .WithSqlParam("Address2", model.PrimaryMember.Address2)
            .WithSqlParam("City", model.PrimaryMember.City)
            .WithSqlParam("StateCode", model.PrimaryMember.State)
            .WithSqlParam("PostalCode", model.PrimaryMember.PostalCode)
            .WithSqlParam("CountryCode", model.PrimaryMember.Country)
            .WithSqlParam("CondoRewards", 0)
            .WithSqlParam("CruiseRewards", 0)
            .WithSqlParam("HotelRewards", model.PackageInfo.Points.ToString())
            .WithSqlParam("SalesAmount", 0)
            .WithSqlParam("Note", null)
            .WithSqlParam("BlockedReason", null)
            .WithSqlParam("SalesDate", null)
            .WithSqlParam("DateOfBirth", model.PrimaryMember.DateOfBirth)
            .WithSqlParam("BlockedDate", null)
            .WithSqlParam("ExpirationDate", null)
            .WithSqlParam("IsEmailOptOut", null)
            .WithSqlParam("IsActive", true)
            .WithSqlParam("IsGuest", null)
            .WithSqlParam("IsComped", null)
            .ExecuteStoredProcAsync<int>();
            #endregion MemberUpdateVIP

            #region MemberUpdateCRM
            
            result = await _rsiContext.LoadStoredProc("dbo.MemberUpdateCRM")
            .WithSqlParam("RSIId", rsiId)
            .WithSqlParam("RSIModifierId", null)
            .WithSqlParam("RSIOrganizationId", model.OrganizationInfo.OrganizationId)
            .WithSqlParam("PackageId", model.PackageInfo.PackageId)
            .WithSqlParam("CreatorIP", null)
            .WithSqlParam("Username", null)
            .WithSqlParam("Password", null)
            .WithSqlParam("Title", null)
            .WithSqlParam("FirstName", model.PrimaryMember.FirstName)
            .WithSqlParam("MiddleName", model.PrimaryMember.MiddleName)
            .WithSqlParam("LastName", model.PrimaryMember.LastName)
            .WithSqlParam("FirstName2", null)
            .WithSqlParam("MiddleName2", null)
            .WithSqlParam("LastName2", null)
            .WithSqlParam("Family", null)
            .WithSqlParam("Address1", model.PrimaryMember.Address1)
            .WithSqlParam("Address2", model.PrimaryMember.Address2)
            .WithSqlParam("City", model.PrimaryMember.City)
            .WithSqlParam("StateCode", model.PrimaryMember.State)
            .WithSqlParam("PostalCode", model.PrimaryMember.PostalCode)
            .WithSqlParam("CountryCode", model.PrimaryMember.Country)
            .WithSqlParam("Phone1", model.PrimaryMember.HomePhone)
            .WithSqlParam("Phone2", model.PrimaryMember.MobilePhone)
            .WithSqlParam("Email1", model.PrimaryMember.Email)
            .WithSqlParam("BlockedReason", null)
            .WithSqlParam("CondoRewards", 0)
            .WithSqlParam("CruiseRewards", 0)
            .WithSqlParam("HotelRewards", model.PackageInfo.Points.ToString())
            .WithSqlParam("UnlimitedRewards", 0)
            .WithSqlParam("SalesDate", null)
            .WithSqlParam("BlockedDate", null)
            .WithSqlParam("DateOfBirth", model.PrimaryMember.DateOfBirth)
            .WithSqlParam("ExpirationDate", null)
            .WithSqlParam("IsActive", true)
            .WithSqlParam("IsMilitary", null)
            .ExecuteStoredProcAsync<int>();
            #endregion MemberUpdateCRM

            #region MemberUpdateCB
            result = await _rsiContext.LoadStoredProc("dbo.MemberUpdateCB")
            .WithSqlParam("RSIId", rsiId)
            .WithSqlParam("RSIOrganizationId", model.OrganizationInfo.OrganizationId)
            .WithSqlParam("OrganizationName", model.OrganizationInfo.OrganizationName)
            .WithSqlParam("FirstName", model.PrimaryMember.FirstName)
            .WithSqlParam("MiddleName", model.PrimaryMember.MiddleName)
            .WithSqlParam("LastName", model.PrimaryMember.LastName)
            .WithSqlParam("FirstName2", model.SecondaryMember.FirstName)
            .WithSqlParam("MiddleName2", model.SecondaryMember.MiddleName)
            .WithSqlParam("LastName2", model.SecondaryMember.LastName)
            .WithSqlParam("Address1", model.PrimaryMember.Address1)
            .WithSqlParam("Address2", model.SecondaryMember.Address1)
            .WithSqlParam("City", model.PrimaryMember.City)
            .WithSqlParam("StateCode", model.PrimaryMember.State)
            .WithSqlParam("PostalCode", model.PrimaryMember.PostalCode)
            .WithSqlParam("CountryCode", model.PrimaryMember.Country)
            .WithSqlParam("NOTES", null)
            .WithSqlParam("IsActive", true)
            .WithSqlParam("Phone1", model.PrimaryMember.HomePhone)
            .WithSqlParam("Phone2", model.SecondaryMember.HomePhone)
            .WithSqlParam("Email1", model.PrimaryMember.Email)
            .WithSqlParam("Email2", model.SecondaryMember.Email)
            .WithSqlParam("Username", null)
            .WithSqlParam("Password", null)
            .WithSqlParam("BirthDate1", model.PrimaryMember.DateOfBirth)
            .WithSqlParam("BirthDate2", model.SecondaryMember.DateOfBirth)
            .WithSqlParam("ExpirationDate", null)
            .WithSqlParam("PROFILEID", 0, ParameterDirection.Output)
            .ExecuteStoredProcAsync<int>();
            #endregion MemberUpdateCB
        }
        [Queue("rsi_api")]
        //public void UpdateFamilyInRSIDb(int jobId, int rsiId)
        public void UpdateFamilyInRSIDb(MemberInfoViewModel updatedMemberInfo, int rsiId)
        {
            //var updatedMemberInfo = _hfService.GetModelForJobId<MemberInfoViewModel>(jobId).Result;

            if (updatedMemberInfo.SecondaryMember != null && updatedMemberInfo.SecondaryMember.RSIId > 0)
            {
                if (string.IsNullOrEmpty(updatedMemberInfo.SecondaryMember.FirstName))
                    DeactivateAuthorizedUser(updatedMemberInfo.SecondaryMember.RSIId);
                else
                    AddUpdateCRMAuthorizedUsers(rsiId, updatedMemberInfo.SecondaryMember);
            }
            else if (updatedMemberInfo.SecondaryMember != null && !String.IsNullOrEmpty(updatedMemberInfo.SecondaryMember.FirstName) && !String.IsNullOrEmpty(updatedMemberInfo.SecondaryMember.LastName))
                AddUpdateCRMAuthorizedUsers(rsiId, updatedMemberInfo.SecondaryMember);

            if (updatedMemberInfo.FamilyMembers != null)
            {
                if (updatedMemberInfo.FamilyMembers != null)
                {
                    var response = AddUpdateFamilyAsync(rsiId, updatedMemberInfo.FamilyMembers).Result;
                    if (!response.isSuccess)
                        throw new Exception($"UpdateFamilyInRSIDb Failed for RSIId: {rsiId}  Reason: {response.message} ");
                }
            }
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

        public async Task<MemberInfoViewModel> GetMemberAsync(int rsiId, string clubReference = null)
        {
            MemberInfoViewModel model = new MemberInfoViewModel();

            try
            {
                var t1 = from u in _legacyContext.Users
                          where u.MemberId == rsiId //&&
                          select u;
                int age = 0;
                DateTime now = new DateTime();
                if (clubReference != null)
                {
                    var tmp1 = await (from u in t1
                                      join c in _legacyContext.BrioClubLeads on u.MemberId equals c.rsiId
                                      join o in _legacyContext.Organizations on u.org equals o.OrganizationId
                                      join r in _legacyContext.RenewalInfo on u.MemberId equals r.RSIId into gj
                                      from subr in gj.DefaultIfEmpty()
                                      where c.clubReference == clubReference
                                      select new
                                      {
                                          c.clubReference,
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

                    model = new MemberInfoViewModel();

                    model.PrimaryMember.FirstName = tmp1.fname;
                    model.PrimaryMember.MiddleName = tmp1.MiddleInitial;
                    model.PrimaryMember.LastName = tmp1.lname;
                    model.PrimaryMember.RSIId = rsiId;
                    model.PrimaryMember.Email = tmp1.email;
                    model.PrimaryMember.MobilePhone = tmp1.phone2;
                    model.PrimaryMember.HomePhone = tmp1.phone1;
                    model.PrimaryMember.Address1 = tmp1.Address;
                    model.PrimaryMember.Address2 = tmp1.Address2;
                    model.PrimaryMember.City = tmp1.City;
                    model.PrimaryMember.State = tmp1.StateCode;
                    model.PrimaryMember.PostalCode = tmp1.PostalCode;
                    model.PrimaryMember.Country = tmp1.CountryCode;


                    now = DateTime.Today;
                    age = now.Year - tmp1.ActivationDate.GetValueOrDefault().Year;
                    if (tmp1.ActivationDate.GetValueOrDefault() > now.AddYears(-age)) age--;

                    model.MembershipInfo.ActivationDate = tmp1.ActivationDate.GetValueOrDefault();
                    model.MembershipInfo.YearsAsMember = age;
                    model.MembershipInfo.CondoWeeks = 3;
                    model.MembershipInfo.DuesAmount = tmp1.RenewalFee;
                    model.MembershipInfo.LastDuesPayment = tmp1.DateOfLastPaid;

                    model.OrganizationInfo.OrganizationId = tmp1.org.GetValueOrDefault(0);
                    model.OrganizationInfo.OrganizationName = tmp1.OrganizationName;

                }
                else
                {
                    var tmp = await (from u in t1
                                     join o in _legacyContext.Organizations on u.org equals o.OrganizationId
                                     join r in _legacyContext.RenewalInfo on u.MemberId equals r.RSIId into gj
                                     from subr in gj.DefaultIfEmpty()
                                     select new
                                     {
                                         clubReference = "",
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


                    now = DateTime.Today;
                    age = now.Year - tmp.ActivationDate.GetValueOrDefault().Year;
                    if (tmp.ActivationDate.GetValueOrDefault() > now.AddYears(-age)) age--;

                    model.MembershipInfo.ActivationDate = tmp.ActivationDate.GetValueOrDefault();
                    model.MembershipInfo.YearsAsMember = age;
                    model.MembershipInfo.CondoWeeks = 3;
                    model.MembershipInfo.DuesAmount = tmp.RenewalFee;
                    model.MembershipInfo.LastDuesPayment = tmp.DateOfLastPaid;

                    model.OrganizationInfo.OrganizationId = tmp.org.GetValueOrDefault(0);
                    model.OrganizationInfo.OrganizationName = tmp.OrganizationName;
                }
                

                if(model != null && model.PrimaryMember.RSIId > 0)
                {
                    

                    PackageViewModel packages = await _packageService.GetPackageInfoByRSIIdAsync(rsiId);

                    if (packages != null && packages.PackageId > 0)
                    {
                        model.PackageInfo = new PackageInfoViewModel()
                        {
                            PackageId = packages.PackageId,
                            PackageName = packages.PackageName
                        };

                        foreach (var row in packages.Benefits)
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

                        model.PackageInfo.UpgradingToPackgeId = model.PackageInfo.PackageId;
                        //var jobData = await _hfService.GetJobDataByRSIId(rsiId);
                        //model.PackageInfo.UpgradingToPackgeId = (string.IsNullOrEmpty(jobData.info)) ? 
                        //    model.PackageInfo.PackageId : Convert.ToInt32(jobData.info);
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
                                        FirstName = !rdr.IsDBNull(2) ? rdr.GetString(2) : "",
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
                var qry = from u in _legacyContext.Users
                          join c in _legacyContext.BrioClubLeads on u.MemberId equals c.rsiId
                          join o in _legacyContext.Organizations on u.org equals o.OrganizationId
                          select new 
                          {
                              c.clubReference,
                              u.email,
                              u.fname,
                              u.lname,
                              u.MemberId,
                              u.MiddleInitial,
                              u.org,
                              o.OrganizationName,
                              u.phone1,
                              u.phone2,
                              u.PackageId
                          };

                qry = qry.Where(c => c.clubReference == search.ClubReference);

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
                    int rsiId = 0;
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
                                w.OrganizationName == search.CatchAll ||
                                w.MemberId == (int.TryParse(search.CatchAll, out rsiId) ? rsiId : w.MemberId)
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
                                w.OrganizationName.Contains(search.CatchAll) ||
                                w.MemberId == (int.TryParse(search.CatchAll, out rsiId) ? rsiId : w.MemberId)
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

                //string ids = "";
                
                List<MemberListViewModel> tmp = new List<MemberListViewModel>();
                //int ctIds = 0;
                foreach(var row in qry)
                {
                    int pid = 0;
                    string pname = "";

                    if (row.PackageId.GetValueOrDefault(0) > 0)
                    {
                        pid = row.PackageId.GetValueOrDefault(0);
                    }
                    /*
                        pid = row.PackageId.GetValueOrDefault(0);
                        using (var conn = new SqlConnection(SqlHelper.GetConnectionString()))
                        {
                            var parameters = new[]
                            {
                            new SqlParameter("@RSIId", row.MemberId)
                        };

                            var rdr = await SqlHelper.ExecuteReaderAsync(
                                  conn,
                                  CommandType.StoredProcedure,
                                  "[dbo].[GetPackageIdAndNameByRSIId]",
                                  parameters);

                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                //var pn = tmp.FirstOrDefault(x => x.MemberId == rdr.GetInt32(0));
                                //if (pn != null)
                               // {
                                string fullName = rdr.GetFieldType(0).FullName;
                                if (!rdr.IsDBNull(0) && fullName == "System.Decimal")
                                    pid = Decimal.ToInt32(rdr.GetDecimal(0));
                                else if (!rdr.IsDBNull(0) && fullName == "System.Int32")
                                    pid = rdr.GetInt32(0);
                                if(!rdr.IsDBNull(1))
                                    pname = rdr.GetString(1);
                                //}


                            }
                        }
                    }*/

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
                        Phone2 = row.phone2,
                        PackageId = pid,
                        PackageName = pname
                    };

                    tmp.Add(t);
                    //if (t.PackageId > 0)
                    //{
                        //if (ids.Length > 0)
                           // ids += ",";

                        //ids += row.MemberId;
                    
                    
                        /*if (ctIds > 6)
                        {
                            using (var conn = new SqlConnection(SqlHelper.GetConnectionString()))
                            {
                                var parameters = new[]
                                {
                                    new SqlParameter("@RSIIds", ids)
                                };

                                var rdr = await SqlHelper.ExecuteReaderAsync(
                                       conn,
                                       CommandType.StoredProcedure,
                                       "[dbo].[GetPackageInfosByRSIIds]",
                                       parameters);
                                if (rdr.HasRows)
                                {
                                    while (rdr.Read())
                                    {
                                        if (!rdr.IsDBNull(0) && !rdr.IsDBNull(1) && !rdr.IsDBNull(2))
                                        {
                                            var pn = tmp.FirstOrDefault(x => x.MemberId == rdr.GetInt32(0));
                                            if (pn != null)
                                            {
                                                string fullName = rdr.GetFieldType(1).FullName;
                                                pn.PackageId = rdr.GetInt32(1);
                                                pn.PackageName = rdr.GetString(2);
                                            }
                                        }


                                    }

                                    
                                }
                            }
                            ids = "";
                            ctIds = 0;
                        }
                        else
                        {
                            ctIds++;
                        }*/
                    //}
                }

                /*if(ids.Length > 0)
                {
                    using (var conn = new SqlConnection(SqlHelper.GetConnectionString()))
                    {
                        var parameters = new[]
                        {
                            new SqlParameter("@RSIIds", ids)
                        };

                        var rdr = await SqlHelper.ExecuteReaderAsync(
                               conn,
                               CommandType.StoredProcedure,
                               "[dbo].[GetPackageInfosByRSIIds]",
                               parameters);
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                if (!rdr.IsDBNull(0) && !rdr.IsDBNull(1) && !rdr.IsDBNull(2))
                                {
                                    var pn = tmp.FirstOrDefault(x => x.MemberId == rdr.GetInt32(0));
                                    if (rdr.GetFieldType(1).FullName == "System.Int32")
                                    {
                                        if(!rdr.IsDBNull(1))
                                            pn.PackageId = rdr.GetInt32(1);
                                    }

                                    pn.PackageName = rdr.GetString(2);
                                }
                            }
                        }
                    }
                }
                */

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

        public async Task<AccessDevelopmentMemberAuditModel> GetAccessDevelopmentId(int rsiId, string pid)
        {
            AccessDevelopmentMemberAuditModel returnObj = new AccessDevelopmentMemberAuditModel();

            try
            {
                returnObj = await _legacyContext.AccessDevelopmentMemberAudit.FirstOrDefaultAsync(x => x.RSIId == rsiId && x.ProgramId == pid);

                if (returnObj == null)
                {
                    returnObj = new AccessDevelopmentMemberAuditModel();
                }
                
            }
            catch (Exception ex)
            {
                if (returnObj == null)
                    returnObj = new AccessDevelopmentMemberAuditModel();

                returnObj.IsSuccess = false;
                returnObj.Message = ex.Message;
                returnObj.AccessDevelopmentId = 0;
            }

            return returnObj;
        }

        public async Task<(bool isSuccess, string message, int accesDevelopmentId)> AddAccessDevelopmentAudit(MemberViewModel model)
        {
            AccessDevelopmentMemberAuditModel adModel = new AccessDevelopmentMemberAuditModel();
            (bool isSuccess, string message, int accesDevelopmentId) = (false, "", 0);
            try
            {
                adModel = await GetAccessDevelopmentId(model.RSIId, model.ProgramCustomerIdentifier);

                if (adModel == null || adModel.AccessDevelopmentId < 1)
                {
                    List<MemberViewModel> tmp = new List<MemberViewModel>
                    {
                        model
                    };

                    ADListReturnViewModel returnAD = await _accessDevelopmentService.AddMemberAsync(tmp);
                    if (returnAD.IsSuccess)
                    {
                        AccessDevelopmentMemberAuditModel insert = new AccessDevelopmentMemberAuditModel()
                        {
                            AccessDevelopmentId = returnAD.Items[0].Id,
                            CreationDate = returnAD.Items[0].CreationDate,
                            ImportedDate = returnAD.Items[0].ImportedDate,
                            InvalidMemberCount = returnAD.Items[0].InvalidMemberCount,
                            InvalidMembersCSVLink = returnAD.Items[0].Links.InvalidMembersCSVLink,
                            IsSuccess = returnAD.IsSuccess,
                            Message = returnAD.Message,
                            RSIId = model.RSIId,
                            ShowImportLink = returnAD.Items[0].Links.ShowImportLink,
                            Status = returnAD.Items[0].Status,
                            ValidMemberCount = returnAD.Items[0].ValidMemberCount,
                            ValidMembersCSVLink = returnAD.Items[0].Links.ValidMembersCSVLink,
                            ProgramId = model.ProgramCustomerIdentifier
                            
                        };

                        await _legacyContext.AccessDevelopmentMemberAudit.AddAsync(insert);
                        await _legacyContext.SaveChangesAsync();
                        isSuccess = true;
                        message = "Success";
                        accesDevelopmentId = returnAD.Items[0].Id;



                    }
                    
                }
                else
                {
                    isSuccess = true;
                    message = "Success";
                    accesDevelopmentId = adModel.AccessDevelopmentId;
                }

            }
            catch (Exception ex)
            {
                isSuccess = false;
                message = ex.Message;
               
            }

            return (isSuccess, message, accesDevelopmentId);
        }
    }
}
