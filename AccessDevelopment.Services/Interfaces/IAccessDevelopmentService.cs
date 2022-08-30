using AccessDevelopment.Services.Models._ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AccessDevelopment.Services.Interfaces
{
    public interface IAccessDevelopmentService
    {
        Task<ADListReturnViewModel> AddMemberAsync(List<MemberViewModel> model);
        

    }
}
