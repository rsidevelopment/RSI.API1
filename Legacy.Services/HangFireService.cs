using Legacy.Services.Data;
using Legacy.Services.Interfaces;
using Legacy.Services.Models;
using Legacy.Services.Models._ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Legacy.Services
{


    public class HangFireService : IHangFireService
    {
        private readonly HangFireDbContext _context;
        public HangFireService(HangFireDbContext context)
        {
            _context = context;
        }

        public async Task<RSIJobData> NewRSIJobData(object model, int rsiId, string info)
        {
            var jobData = new RSIJobData()
            {
                data = JsonConvert.SerializeObject(model),
                RSIId = rsiId,
                info = info
            };

            _context.RSIJobData.Add(jobData);
            await _context.SaveChangesAsync();

            return jobData;
        }

        public async Task<T> GetModelForJobId<T>(int jobId)
        {
            var jobData = await _context.RSIJobData.FirstOrDefaultAsync(x => x.jobId == jobId);
            return JsonConvert.DeserializeObject<T>(jobData.data);
        }

        public async Task<RSIJobData> GetJobDataByRSIId(int rsiId)
        {
            var jobData = await
                (from j in _context.RSIJobData
                 where j.RSIId == rsiId
                 orderby j.jobId descending
                 select new RSIJobData()
                 {
                     jobId = j.jobId,
                     data = j.data,
                     RSIId = j.RSIId,
                     info = j.info
                 }).ToListAsync();

            return (jobData.Count > 0) ? jobData[0] : new RSIJobData();
        }
    }
}