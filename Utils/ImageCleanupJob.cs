using Quartz;
using System;
using System.Threading.Tasks;
using WinFormApp.Services;

namespace WinFormApp.Utils
{
    public class ImageCleanupJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                Console.WriteLine($"[Quartz] Running image cleanup at {DateTime.Now}");
                await ImageService.Instance.CleanUpUnusedImages();
                Console.WriteLine("[Quartz] Image cleanup completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Quartz] Cleanup failed: {ex.Message}");
            }
        }
    }
}
