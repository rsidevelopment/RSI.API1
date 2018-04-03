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
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace Legacy.Services
{
    public class MemberServices : IMemberService
    {
        private readonly RSIDbContext _rsiContext;
        private readonly LegacyDbContext _legacyContext;
        private readonly HangFireDbContext _hfContext;
        private readonly IPackageService _packageService;

        public MemberServices(RSIDbContext rsiContext, LegacyDbContext legacyContext, HangFireDbContext hfContext, IPackageService packageService)
        {
            _rsiContext = rsiContext;
            _legacyContext = legacyContext;
            _hfContext = hfContext;
            _packageService = packageService;
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
            var result = _rsiContext.LoadStoredProc("dbo.AddUpdateCRMAuthorizedUsers")
            .WithSqlParam("Id", familyMember.FamilyMemberId)
            .WithSqlParam("RSIId", rsiId)
            .WithSqlParam("FirstName", familyMember.FirstName)
            .WithSqlParam("LastName", familyMember.LastName)
            .WithSqlParam("BirthDate", familyMember.DateOfBirth)
            .WithSqlParam("Relationship", familyMember.Relationship)
            .WithSqlParam("Email", familyMember.Email)
            .WithSqlParam("PrimaryPhone", familyMember.PrimaryPhone)
            .WithSqlParam("SecondaryPhone", familyMember.AlternativePhone)
            .WithSqlParam("Address", string.Empty)
            .WithSqlParam("City", string.Empty)
            .WithSqlParam("State", string.Empty)
            .WithSqlParam("PostalCode", string.Empty)
            .WithSqlParam("Country", string.Empty)
            .ExecuteStoredProcAsync<IdResponse>().Result.FirstOrDefault();

            if (result.Id == 0) throw new Exception($"AddUpdateCRMAuthorizedUsers failed for RSIId: {rsiId} Family Member Id: {familyMember.FamilyMemberId}");
        }
        void AddUpdateCRMAuthorizedUsers(long rsiId, PersonViewModel secondary)
        {
            var result = _rsiContext.LoadStoredProc("dbo.AddUpdateCRMAuthorizedUsers")
            .WithSqlParam("Id", rsiId)
            .WithSqlParam("RSIId", secondary.RSIId)
            .WithSqlParam("FirstName", secondary.FirstName)
            .WithSqlParam("LastName", secondary.LastName)
            .WithSqlParam("BirthDate", secondary.DateOfBirth)
            .WithSqlParam("Relationship", "SPOUSE")
            .WithSqlParam("Email", secondary.Email)
            .WithSqlParam("PrimaryPhone", secondary.HomePhone)
            .WithSqlParam("SecondaryPhone", secondary.MobilePhone)
            .WithSqlParam("Address", $"{secondary.Address1} {secondary.Address2}")
            .WithSqlParam("City", secondary.City)
            .WithSqlParam("State", secondary.State)
            .WithSqlParam("PostalCode", secondary.PostalCode)
            .WithSqlParam("Country", secondary.Country)
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
                    if (!family.Any(f => { return f.FamilyMemberId == fm.FamilyMemberId; }))
                        DeactivateAuthorizedUser(fm.FamilyMemberId);
                }

                foreach (var fm in family)
                {
                    AddUpdateCRMAuthorizedUsers(rsiId, fm);
                }
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
                var jobData = _hfContext.NewRSIJobData(model);

                //UpdateMemberInRSIDb(jobData.jobId, rsiId);
                //UpdateFamilyInRSIDb(jobData.jobId, rsiId);
                BackgroundJob.Enqueue<MemberServices>(x => x.UpdateMemberInLegacyRSIDb(jobData.jobId, rsiId));
                BackgroundJob.Enqueue<MemberServices>(x => x.UpdateMemberInRSIDb(jobData.jobId, rsiId));
                BackgroundJob.Enqueue<MemberServices>(x => x.UpdateFamilyInRSIDb(jobData.jobId, rsiId));

                return new MemberInfoViewModel() { Message = "Success" };
            }
            catch (Exception ex)
            {
                return new MemberInfoViewModel() { Message = $"Error: {ex.Message}" };
            }
        }

        public void UpdateMemberInLegacyRSIDb(int jobId, int rsiId)
        {
            var model = _hfContext.GetModelForJobId<MemberInfoViewModel>(jobId);
            MemberModel member = _legacyContext.Users.FirstOrDefault(x => x.MemberId == rsiId);

            if (member != null && member.MemberId > 0)
            {
                //org/package info
                member.org = model.OrganizationInfo.OrganizationId;
                member.PackageId = model.PackageInfo.PackageId;

                //primary info
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

                //secondary info
                member.FirstName2 = model.SecondaryMember.FirstName;
                member.MiddleInitial2 = model.SecondaryMember.MiddleInitial;
                member.LastName2 = model.SecondaryMember.LastName;
                member.BirthDate2 = model.SecondaryMember.DateOfBirth;

                //member.email2 = model.SecondaryMember.Email;
                member.Family = model.FamilyMemberString;
                member.IsActive = true;
                member.HotelRewards = model.PackageInfo.Points.ToString();

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
        public void UpdateMemberInRSIDb(int jobId, int rsiId)
        {
            var model = _hfContext.GetModelForJobId<MemberInfoViewModel>(jobId);

            #region MemberUpdateVIP
            var result = _rsiContext.LoadStoredProc("dbo.MemberUpdateVIP")
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
            .ExecuteStoredProcAsync<int>().Result;
            #endregion MemberUpdateVIP

            #region MemberUpdateCRM
            result = _rsiContext.LoadStoredProc("dbo.MemberUpdateCRM")
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
            .WithSqlParam("FirstName2", model.SecondaryMember.FirstName)
            .WithSqlParam("MiddleName2", model.SecondaryMember.MiddleName)
            .WithSqlParam("LastName2", model.SecondaryMember.LastName)
            .WithSqlParam("Family", model.FamilyMemberString)
            .WithSqlParam("Address1", model.PrimaryMember.Address1)
            .WithSqlParam("Address2", model.SecondaryMember.Address1)
            .WithSqlParam("City", model.PrimaryMember.City)
            .WithSqlParam("StateCode", model.PrimaryMember.State)
            .WithSqlParam("PostalCode", model.PrimaryMember.PostalCode)
            .WithSqlParam("CountryCode", model.PrimaryMember.Country)
            .WithSqlParam("Phone1", model.PrimaryMember.HomePhone)
            .WithSqlParam("Phone2", model.SecondaryMember.HomePhone)
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
            .ExecuteStoredProcAsync<int>().Result;
            #endregion MemberUpdateCRM

            #region MemberUpdateCB
            result = _rsiContext.LoadStoredProc("dbo.MemberUpdateCB")
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
            .ExecuteStoredProcAsync<int>().Result;
            #endregion MemberUpdateCB
        }
        public void UpdateFamilyInRSIDb(int jobId, int rsiId)
        {
            var updatedMemberInfo = _hfContext.GetModelForJobId<MemberInfoViewModel>(jobId);

            if (string.IsNullOrEmpty(updatedMemberInfo.SecondaryMember.FirstName))
                DeactivateAuthorizedUser(updatedMemberInfo.SecondaryMember.RSIId);
            else
                AddUpdateCRMAuthorizedUsers(rsiId, updatedMemberInfo.SecondaryMember);

            var response = AddUpdateFamilyAsync(rsiId, updatedMemberInfo.FamilyMembers).Result;
            if (!response.isSuccess)
                throw new Exception($"UpdateFamilyInRSIDb Failed for RSIId: {rsiId}  Reason: {response.message} ");
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

        public async Task<MemberInfoViewModel> GetMemberAsync(int rsiId, string clubReference)
        {
            MemberInfoViewModel model = new MemberInfoViewModel();

            try
            {
                var tmp = await (from u in _legacyContext.Users
                           join c in _legacyContext.BrioClubLeads on u.MemberId equals c.rsiId
                           join o in _legacyContext.Organizations on u.org equals o.OrganizationId
                           join r in _legacyContext.RenewalInfo on u.MemberId equals r.RSIId into gj
                           from subr in gj.DefaultIfEmpty()
                           where u.MemberId == rsiId &&
                                 c.clubReference==clubReference
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
                              u.phone2
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
