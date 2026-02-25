using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace Scheduler.Pages
{
    public class OutputModel : PageModel
    {
        public List<Process>? Processes;

        // consider passing this value in to avoid recalculating
        public int TotalServiceTime;

        private readonly IMemoryCache _cache;

        public OutputModel(IMemoryCache cache)
        {
            _cache = cache;
        }
        public async Task<IActionResult> OnGet(string key)
        {

            Processes = _cache.Get<List<Process>>(key) ?? new();

            // consider passing this value in to avoid recalculating
            foreach (Process p in Processes)
            {
                TotalServiceTime += p.ServiceTime;
            }

            return Page();
        }
    }
}
