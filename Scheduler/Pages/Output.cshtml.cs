using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace Scheduler.Pages
{
    public class OutputModel : PageModel
    {
        public List<Process>? Processes;

        private readonly IMemoryCache _cache;

        public OutputModel(IMemoryCache cache)
        {
            _cache = cache;
        }
        public async Task<IActionResult> OnGet(string key)
        {

            Processes = _cache.Get<List<Process>>(key) ?? new();

            foreach (Process p in Processes)
            {
                Process.TTMean += Math.Round(p.TT, 2);
                Process.TurnaroundMean += Math.Round(p.Turnaround, 2);
            }

            Process.TTMean /= Processes.Count;
            Process.TurnaroundMean /= Processes.Count;

            return Page();
        }
    }
}
