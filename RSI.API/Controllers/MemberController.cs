using Legacy.Services.Interfaces;
using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Member;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RSI.API.Controllers
{
    [Route("api/[controller]"), Authorize]
    public class MemberController : Controller
    {
        private readonly IMemberService _context;

        public MemberController(IMemberService context)
        {
            _context = context;
        }

        
        // GET api/<controller>/5
        [HttpGet]
        public async Task<_ListViewModel<MemberListViewModel>> Get(int startRowIndex, int numberOfRows, string firstName, string lastName, string email, string phone, int organizationId, string catchAll
            , string sortColumn = "DEFAULT", string sortDirection = "ASC"
            , bool exactMatch = true, string clubReference="BRIO")
        {
            var model = new _ListViewModel<MemberListViewModel>();

            try
            {
                MemberSearchViewModel search = new MemberSearchViewModel()
                {
                    CatchAll = catchAll,
                    Email = email,
                    ExactMatch = exactMatch,
                    FistName = firstName,
                    LastName = lastName,
                    NumberOfRows = numberOfRows,
                    OrganizationId = organizationId,
                    Phone = phone,
                    SortColumn = sortColumn,
                    SortDirection = sortDirection,
                    StartRowIndex = startRowIndex,
                    ClubReference = clubReference
                };

                model = await _context.GetMembersAsync(search);
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new _ListViewModel<MemberListViewModel>();

                if (model.Message.Length > 0)
                    model.Message += " | ";
                else
                    model.Message = "Error: ";

                model.Message += ex.Message;
            }

            return model;
        }

        [HttpGet("{rsiId}")]
        public async Task<MemberInfoViewModel> Get(int rsiId, string clubReference = "BRIO")
        {
            var model = new MemberInfoViewModel();

            try
            {
                model = await _context.GetMemberAsync(rsiId, clubReference);
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new MemberInfoViewModel();

                if (model.Message.Length > 0)
                    model.Message += " | ";
                else
                    model.Message = "Error: ";

                model.Message += ex.Message;
            }

            return model;
        }
        
        [HttpGet("{id}/family")]
        public async Task<List<FamilyMemberViewModel>> Family(int id, string clubReference = "BRIO")
        {
            (bool isSuccess, string message, List<FamilyMemberViewModel> family) returnModel = (false, "", new List<FamilyMemberViewModel>());

            try
            {
                var model = new List<FamilyMemberViewModel>();

                returnModel = await _context.GetFamilyAsync(id, clubReference);
            }
            catch (Exception ex)
            {

                throw;
            }

            return returnModel.family;
        }

        [HttpPost("{id}/family")]
        public async Task<JObject> Family(int id, [FromBody]JArray value)
        {
            dynamic returnObj = new JObject();

            try
            {
                var family = value.ToObject<List<FamilyMemberViewModel>>();
                (bool isSuccess, string message) = await _context.AddUpdateFamilyAsync(id, family);
                returnObj.is_success = isSuccess;
                returnObj.message = message;
            }
            catch (Exception ex)
            {
                returnObj.is_success = false;
                returnObj.message = $"Error: {ex.Message}";
            }

            return returnObj;
        }

        [HttpGet("{id}/upgrade")]
        public async Task<MemberUpgradeViewModel> GetUpgrade(int id, string clubReference = "BRIO")
        {
            var model = new MemberUpgradeViewModel();

            try
            {
                model = await _context.GetUpgradeInfoAsync(id, clubReference);
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new MemberUpgradeViewModel();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }

        [HttpPost("{id}/upgrade")]
        public async Task<MemberUpgradeViewModel> AddUpdateUpgrade(int id, [FromBody] JObject value)
        {
            var model = new MemberUpgradeViewModel();

            try
            {
                model = value.ToObject<MemberUpgradeViewModel>();
                model.RSIId = id;

                model = await _context.AddUpdateUpgradeInfoAsync(model);
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new MemberUpgradeViewModel();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }

        [HttpGet("{id}/referral")]
        public async Task<_ListViewModel<ReferralViewModel>> Referral(int id)
        {
            var model = new _ListViewModel<ReferralViewModel>();

            try
            {
                model = await _context.GetReferralsAsync(id);

                if(model == null)
                {
                    model = new _ListViewModel<ReferralViewModel>
                    {
                        Message = "Error: No data returned"
                    };
                }
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new _ListViewModel<ReferralViewModel>();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }

        [HttpGet("{rsiId}/travel")]
        public async Task<_ListViewModel<TravelDetailViewModel>> Travel(int rsiId)
        {
            var model = new _ListViewModel<TravelDetailViewModel>();

            try
            {
                (bool isSuccess, string message, List<TravelDetailViewModel> travels) = await _context.GetTravelInfoAsync(rsiId);
                model.Message = message;
                model.Rows = travels;
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new _ListViewModel<TravelDetailViewModel>();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }

        [HttpPost("{id}/referral")]
        public async Task<_ListViewModel<ReferralViewModel>> UpdateReferral(int id, [FromBody]JArray value)
        {
            var model = new _ListViewModel<ReferralViewModel>();

            try
            {
                List<ReferralViewModel> referral = value.ToObject<List<ReferralViewModel>>();
                (bool isSuccess, string message) = await _context.AddUpdateReferralsAsync(id, referral);

                model = await _context.GetReferralsAsync(id);

                if (model == null)
                {
                    model = new _ListViewModel<ReferralViewModel>
                    {
                        Message = "Error: No data returned"
                    };
                }
                else
                {
                    model.Message = message;
                }
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new _ListViewModel<ReferralViewModel>();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]JObject value)
        { 
            try
            {
                var model = value.ToObject<MemberInfoViewModel>();
                model.PrimaryMember.RSIId = id;
                model = await _context.UpdateMemberAsync(id, model);
                
                if (model.IsSuccess)
                    return new NoContentResult();
                else
                    return NotFound();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
