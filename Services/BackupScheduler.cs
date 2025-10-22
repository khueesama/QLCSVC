using System;
using System.Configuration;
using System.IO;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace QLCSVCWinApp.Services
{
    // Job: gọi BackupService
    public class BackupJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var folder = ConfigurationManager.AppSettings["Backup:Folder"] ?? "";
            // Có thể bật zip nếu muốn: true
            BackupService.BackupOnce(folder, zipAfter: false);
            return Task.CompletedTask;
        }
    }

    public static class BackupScheduler
    {
        private static IScheduler _scheduler;

        public static bool IsEnabled()
        {
            var val = (ConfigurationManager.AppSettings["Backup:Enabled"] ?? "false").Trim();
            return val.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        public static void StartIfEnabled()
        {
            if (!IsEnabled()) return;

            var cron = ConfigurationManager.AppSettings["Backup:Cron"] ?? "0 0 2 ? * * *";
            _ = StartInternal(cron).ConfigureAwait(false);
        }

        public static async Task StartInternal(string cron)
        {
            if (_scheduler != null && !_scheduler.IsShutdown) return;

            var factory = new StdSchedulerFactory();
            _scheduler = await factory.GetScheduler();
            await _scheduler.Start();

            var job = JobBuilder.Create<BackupJob>()
                .WithIdentity("backupJob", "system")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("backupTrigger", "system")
                .WithCronSchedule(cron)
                .StartNow()
                .Build();

            await _scheduler.ScheduleJob(job, trigger);
        }

        public static async Task Stop()
        {
            if (_scheduler != null && !_scheduler.IsShutdown)
                await _scheduler.Shutdown();
        }
    }
}
