using AccessDevelopment.Services.Models._ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AccessDevelopment.Services.Interfaces
{
    public interface IAccessDevelopmentService
    {
        Task<bool> HasAccessDevelopment(int rsiId);
        Task<ADListReturnViewModel> AddMemberAsync(List<MemberViewModel> model);
        Task<ADListReturnViewModel> GetMembers(int pageNumber = 1, int recordsPerPage = 10, string token = null);
        Task<ListViewModel<ADCSVFileViewModel>> GetCSV(string url, string token = null);

    }
}
