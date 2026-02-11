using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using System.Transactions;

namespace Scheduler.Pages
{
    public class Process
    {
        public string Name { get; set; } = string.Empty;
        public int ArrivalTime { get; set; }

        public int ProcessTime  { get; set; }

        public int FinishTime { get; set; }

        public int Turnaround {  get; set; }

        public int TT { get; set; }
    }
    public class TaskDetailsModel : PageModel
    {
        Algorithms SelectedAlgo { get; set; }

        [BindProperty]
        public int NumberOfTasks { get; set; }

        public List<Process> Processes { get; set; }

        [BindProperty]
        public int TimeQuatum { get; set; }

        public bool HasQuantum { get; set; }
        public bool ListProcesses { get; set; }
        public bool ListDetails { get; set; }


        public async Task<IActionResult> OnGet(Algorithms algo)
        {
            Console.WriteLine("Made it to the Task Page");


            SelectedAlgo = algo;

            ListDetails = true;

            Console.WriteLine(ListDetails);

            if (SelectedAlgo == Algorithms.FCFS || SelectedAlgo == Algorithms.Feedback)
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

            ListProcesses = true;
            ListDetails = false;
            return Page();
        }

        public async Task<IActionResult> OnPostSubmitTasks()
        {
            Console.WriteLine("Time to list the tasks");

            foreach (Process proc in Processes)
            {
                Console.WriteLine(proc.Name, proc.ArrivalTime, proc.ProcessTime);
            }

            return Page();
        }

        public async void OnGetFCFS()
        {
            Console.WriteLine("First Come First Serve");



        }


        public async void OnGetRR()
        {
            Console.WriteLine("Round Robin");
            HasQuantum = true;


            
        }


        public async void OnGetSPN()
        {
            Console.WriteLine("Shortest Process Next");


            
        }


        public async void OnGetSRT()
        {
            Console.WriteLine("Shortest Remaining Time");


            
        }


        public async void OnGetHRRN()
        {
            Console.WriteLine("Highest Response Ratio Next");


            
        }


        public async void OnGetFeedback()
        {
            Console.WriteLine("Feedback");
            HasQuantum = true;


            
        }
    }
}
