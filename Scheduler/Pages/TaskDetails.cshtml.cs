using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Threading.Tasks;
using System.Transactions;

namespace Scheduler.Pages
{
    public class TaskDetailsModel : PageModel
    {
        Algorithms SelectedAlgo { get; set; }

        [BindProperty]
        public int NumberOfTasks { get; set; }

        [BindProperty]
        public List<Process>? Processes { get; set; }

        [BindProperty]
        public int TimeQuatum { get; set; }

        public int TotalTime { get; set; }

        public bool HasQuantum { get; set; }
        public bool ListProcesses { get; set; }
        public bool ListDetails { get; set; }

        public string? Key {  get; set; }

        // enable use of cache for list transfer
        private readonly IMemoryCache _cache;

        public TaskDetailsModel(IMemoryCache cache)
        {
            _cache = cache;
        }


        public async Task<IActionResult> OnGet(Algorithms algo)
        {
            Console.WriteLine("Made it to the Task Page");

            SelectedAlgo = algo;

            ListDetails = true;

            Console.WriteLine(ListDetails);

            if (SelectedAlgo == Algorithms.RR || SelectedAlgo == Algorithms.Feedback)
            {
                HasQuantum = true;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitDetails()
        {
            if (HasQuantum)
            {
                Console.WriteLine("This needs a time quantum");
            }
            else
            {
                Console.WriteLine("This does not need a time quantum");
            }

            Console.WriteLine(NumberOfTasks);

            Processes = new List<Process>(new Process[NumberOfTasks]);

            ListProcesses = true;
            ListDetails = false;
            return Page();
        }

        public RedirectToPageResult OnPostSubmitTasks()
        {
            Console.WriteLine("Time to list the tasks");

            foreach (Process proc in Processes)
            {
                Console.WriteLine($"{proc.Name} {proc.ArrivalTime} {proc.ServiceTime}");
            }
            
            // select algorithm
            switch (SelectedAlgo)
            {
                case Algorithms.FCFS:
                    Key = FCFS();
                    break;
                case Algorithms.RR:
                    Key = RR();
                    break;
                case Algorithms.SPN:
                    Key = SPN();
                    break;
                case Algorithms.SRT:
                    Key = SRT();
                    break;
                case Algorithms.HRRN:
                    Key = HRRN();
                    break;
                case Algorithms.Feedback:
                    Key = Feedback();
                    break;
                default:
                    break;
            }

            return RedirectToPage("/Output", new { Key });

        }

        public string FCFS()
        {
            TotalTime = 0;
            int TotalServiceTime = 0;

            // sort processes by arrival time
            Processes.Sort((p1, p2) => p1.ArrivalTime.CompareTo(p2.ArrivalTime));


            // assuming that the total of all processes service times
            // will never be exceeded due to a late arrival
            foreach (Process p in Processes)
            {
                TotalServiceTime += p.ServiceTime;
            }

            foreach (Process p in Processes)
            {
                p.IsRunning = new List<bool>();

                TotalTime += p.ServiceTime;

                p.FinishTime = TotalTime;

                p.Turnaround = p.FinishTime - p.ArrivalTime;

                p.TT = (double)p.Turnaround / p.ServiceTime;

                // initialize list to track what time index a process ran
                for (int i = 0; i < TotalServiceTime; i++)
                {
                    p.IsRunning.Add(false);

                    if (i >= p.FinishTime - p.ServiceTime && i < p.FinishTime)
                    {
                        p.IsRunning[i] = true;
                    }
                }

                Console.WriteLine($"{p.Name}: {p.FinishTime}");
            }

            // get identifier for list to pass to next page
            var key = Guid.NewGuid().ToString("N");
            _cache.Set(key, Processes, TimeSpan.FromMinutes(10));

            return key;
        }

        public string RR()
        {
            TotalTime = 0;



            // get identifier for list to pass to next page
            var key = Guid.NewGuid().ToString("N");
            _cache.Set(key, Processes, TimeSpan.FromMinutes(10));

            return key;
        }

        public string SPN()
        {
            // get identifier for list to pass to next page
            var key = Guid.NewGuid().ToString("N");
            _cache.Set(key, Processes, TimeSpan.FromMinutes(10));

            return key;
        }

        public string SRT()
        {
            // get identifier for list to pass to next page
            var key = Guid.NewGuid().ToString("N");
            _cache.Set(key, Processes, TimeSpan.FromMinutes(10));

            return key;
        }


        public string HRRN()
        {
            // get identifier for list to pass to next page
            var key = Guid.NewGuid().ToString("N");
            _cache.Set(key, Processes, TimeSpan.FromMinutes(10));

            return key;
        }


        public string Feedback()
        {
            // get identifier for list to pass to next page
            var key = Guid.NewGuid().ToString("N");
            _cache.Set(key, Processes, TimeSpan.FromMinutes(10));

            return key;
        }


    }
}
