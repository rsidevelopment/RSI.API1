using Legacy.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Legacy.Services.Interfaces
{
    public interface IHangFireService
    {
        Task<RSIJobData> NewRSIJobData(object model, int rsiId, string info);
        Task<T> GetModelForJobId<T>(int jobId);
        Task<RSIJobData> GetJobDataByRSIId(int rsiId);
    }
}
