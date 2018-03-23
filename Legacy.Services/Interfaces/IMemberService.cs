using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Member;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Legacy.Services.Interfaces
{
    public interface IMemberService
    {
        Task<_ListViewModel<MemberListViewModel>> GetMembersAsync(MemberSearchViewModel search);
        Task<MemberInfoViewModel> GetMemberAsync(int rsiId);
        Task<(bool isSuccess, string message, List<FamilyMemberViewModel> family)> GetFamilyAsync(int rsiId);
        Task<MemberInfoViewModel> UpdateMemberAsync(int rsiId, MemberInfoViewModel model);
        Task<(bool isSuccess, string message)> AddUpdateFamilyAsync(int rsiId, List<FamilyMemberViewModel> family);
        Task<(bool isSuccess, string message)> AddUpdateReferralsAsync(int rsiId, List<ReferralViewModel> referrals);
        Task<_ListViewModel<ReferralViewModel>> GetReferralsAsync(int rsiId);
        Task<MemberUpgradeViewModel> GetUpgradeInfoAsync(int rsiId);
        Task<MemberUpgradeViewModel> AddUpdateUpgradeInfoAsync(MemberUpgradeViewModel model);
        Task<(bool isSuccess, string message, List<TravelDetailViewModel> travels)> GetTravelInfoAsync(int rsiId);
    }
}
