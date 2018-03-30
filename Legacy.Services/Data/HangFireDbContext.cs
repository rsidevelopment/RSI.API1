using Legacy.Services.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Legacy.Services.Data
{
    public class HangfireActivator : Hangfire.JobActivator
    {
        private readonly IServiceProvider _serviceProvider;

        public HangfireActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override object ActivateJob(Type type)
        {
            return _serviceProvider.GetService(type);
        }
    }

    public class HangFireDbContext : DbContext
    {
        public HangFireDbContext(DbContextOptions<HangFireDbContext> options)
            : base(options)
        { }
        
        public DbSet<RSIJobData> RSIJobData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public RSIJobData NewRSIJobData(object model)
        {
            var jobData = new RSIJobData()
            {
                data = JsonConvert.SerializeObject(model)
            };

            RSIJobData.Add(jobData);
            SaveChanges();

            return jobData;
        }

        public T GetModelForJobId<T>(int jobId)
        {
            var jobData = RSIJobData.FirstOrDefault(x => x.jobId == jobId);
            return JsonConvert.DeserializeObject<T>(jobData.data);
        }
    }
}
