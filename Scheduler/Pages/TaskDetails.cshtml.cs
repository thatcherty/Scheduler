using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Transactions;

namespace Scheduler.Pages
{
    public class TaskDetailsModel : PageModel
    {
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

            Process.SelectedAlgo = algo;

            ListDetails = true;

            if (Process.SelectedAlgo == Algorithms.RR || Process.SelectedAlgo == Algorithms.Feedback)
            {
                HasQuantum = true;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitDetails()
        {
            Processes = new List<Process>(new Process[NumberOfTasks]);

            // update visible
            ListProcesses = true;
            ListDetails = false;
            return Page();
        }

        public RedirectToPageResult OnPostSubmitTasks()
        {
            // calculate TotalServiceTime
            // considering idle CPU time
            // sort processes by arrival time
            Processes.Sort((p1, p2) => p1.ArrivalTime.CompareTo(p2.ArrivalTime));

            Process.TotalServiceTime = 0;

            for (int i = 0; i < Processes.Count; i++)
            {
                Process.TotalServiceTime = Math.Max(Process.TotalServiceTime, Processes[i].ArrivalTime) + Processes[i].ServiceTime;
            }

            // initialize list to track if process
            // ran at specified time
            foreach (Process p in Processes)
            {
                p.IsRunning = new List<bool>();
                p.RemainingTime = p.ServiceTime;

                for (int i = 0; i < Process.TotalServiceTime; i++)
                {
                    p.IsRunning.Add(false);
                }
            }


            // select algorithm
            switch (Process.SelectedAlgo)
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

        public string? get_key()
        {
            // get identifier for list to pass to next page
            var key = Guid.NewGuid().ToString("N");
            _cache.Set(key, Processes, TimeSpan.FromMinutes(10));
            return key;
        }

        public string FCFS()
        {
            TotalTime = 0;

            foreach (Process p in Processes)
            {
                TotalTime = Math.Max(TotalTime, p.ArrivalTime) + p.ServiceTime;

                p.FinishTime = TotalTime;

                p.Turnaround = p.FinishTime - p.ArrivalTime;

                p.TT = (double)p.Turnaround / p.ServiceTime;

                // flag time indices where process was running
                for (int i = p.ArrivalTime; i < TotalTime; i++)
                {
                    if (i >= p.FinishTime - p.ServiceTime && i < p.FinishTime)
                    {
                        p.IsRunning[i] = true;
                    }
                    
                }
            }



            return get_key();
        }

        public string RR()
        {
            TotalTime = 0;
            int curr = -1;

            // pass index of the waiting processes
            // from the existing processes list
            Queue<int> waiting = new Queue<int>();

            for (int i = 0; i < Process.TotalServiceTime; i++)
            {
                TotalTime++;
                if (Processes[waiting.Peek()].ArrivalTime == i)
                {
                    // if no process running
                    if (curr == -1)
                    {
                        curr = waiting.Dequeue();
                        Processes[curr].RemainingTime--;

                    }
                }
            }

            return get_key();
        }

        public string SPN()
        {
            // currently running process index
            int curr = -1;
            TotalTime = 0;

            PriorityQueue<int, int> heap = new PriorityQueue<int, int>();

            for (int i = 0; i < Process.TotalServiceTime; i++)
            {
                
                // first process starts
                if (curr == -1)
                {
                    curr = 0;
                    Processes[curr].RemainingTime--;
                    Processes[curr].IsRunning[i] = true;
                    Processes[curr].Queued = true;

                }
                // let process finish running
                else if (Processes[curr].RemainingTime > 0)
                {
                    Processes[curr].RemainingTime--;
                    Processes[curr].IsRunning[i] = true;
                }
                // record stats
                // select next process
                else
                {
                    // only update stats if process just finished
                    if (Processes[curr].Queued)
                    {
                        Processes[curr].FinishTime = TotalTime;
                        Processes[curr].Turnaround = Processes[curr].FinishTime - Processes[curr].ArrivalTime;
                        Processes[curr].TT = (double)Processes[curr].Turnaround / Processes[curr].ServiceTime;
                    }

                    // ensure stats are not updated again
                    // in the event the next available process
                    // has not arrived yet
                    Processes[curr].Queued = false;

                    for (int j = 0; j < Processes.Count; j++)
                    {
                        // select only processes
                        //  - not completed
                        //  - not queued
                        //  - that have arrived
                        if (Processes[j].RemainingTime > 0 && !Processes[j].Queued && Processes[j].ArrivalTime <= TotalTime)
                        {
                            Processes[j].Queued = true;
                            heap.Enqueue(j, Processes[j].ServiceTime);
                        }
                    }

                    // select shortest process from top of min-heap
                    // assuming it is not empty (account for CPU idle time)
                    if (heap.Count > 0)
                    {
                        curr = heap.Dequeue();
                        Processes[curr].RemainingTime--;
                        Processes[curr].IsRunning[i] = true;
                    }
                }
                TotalTime++;
            }

            // add calculations to final process
            if (curr > -1)
            {
                Processes[curr].FinishTime = TotalTime;
                Processes[curr].Turnaround = Processes[curr].FinishTime - Processes[curr].ArrivalTime;
                Processes[curr].TT = (double)Processes[curr].Turnaround / Processes[curr].ServiceTime;
            }

            return get_key();
        }

        public string SRT()
        {
            // currently running process index
            int curr = -1;

            PriorityQueue<int, int> heap = new PriorityQueue<int, int>();

            // track which processes have arrived
            int LastArrivedIDX = 0;

            for (int i = 0; i < Process.TotalServiceTime; i++)
            {
                // initial process
                if (curr == -1)
                {
                    curr = 0;
                    Processes[curr].RemainingTime--;
                    Processes[curr].IsRunning[i] = true;
                    Processes[curr].Queued = true;
                }
                else
                {
                    // simulate process arrival
                    if (Processes.Count > LastArrivedIDX+1 && Processes[LastArrivedIDX + 1].ArrivalTime == i)
                    {
                        heap.Enqueue(LastArrivedIDX + 1, Processes[LastArrivedIDX + 1].RemainingTime);
                        Processes[LastArrivedIDX + 1].Queued = true;
                        LastArrivedIDX++;
                    }

                    // udpate if current process just completed
                    if (Processes[curr].RemainingTime == 0 && Processes[curr].Queued)
                    {
                        Processes[curr].FinishTime = i;
                        Processes[curr].Turnaround = Processes[curr].FinishTime - Processes[curr].ArrivalTime;
                        Processes[curr].TT = (double)Processes[curr].Turnaround / Processes[curr].ServiceTime;
                        Processes[curr].Queued = false;

                    }
                    else if (Processes[curr].Queued)
                    {
                        heap.Enqueue(curr, Processes[curr].RemainingTime);
                    }

                    // select shortest process from top of min-heap
                    // assuming it is not empty (account for CPU idle time)
                    if (heap.Count > 0)
                    {
                        curr = heap.Dequeue();
                        Processes[curr].RemainingTime--;
                        Processes[curr].IsRunning[i] = true;
                    }
                    // if heap is empty, but there is still a running process
                    else if (Processes[curr].Queued)
                    {
                        Processes[curr].RemainingTime--;
                        Processes[curr].IsRunning[i] = true;
                    }
                }
            }

            // add calculations to final process
            if (curr > -1)
            {
                Processes[curr].FinishTime = Process.TotalServiceTime;
                Processes[curr].Turnaround = Processes[curr].FinishTime - Processes[curr].ArrivalTime;
                Processes[curr].TT = (double)Processes[curr].Turnaround / Processes[curr].ServiceTime;
            }

            return get_key();
        }


        public string HRRN()
        {


            return get_key();
        }


        public string Feedback()
        {


            return get_key();
        }


    }
}
