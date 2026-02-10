using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Scheduler.Pages
{
    public class Process
    {
        public string Name { get; set; } = string.Empty;
        public int ArrivalTime { get; set; }

        public int ProcessTime  { get; set; }
    }
    public class TaskDetailsModel : PageModel
    {

        public int NumberOfTasks { get; set; }

        public List<Process> Processes { get; set; }

        public int TimeQuatum { get; set; }

        public void OnGet()
        {

        }

        public void OnPost() 
        {
        }


        public void OnPostFCFS()
        {
            Console.WriteLine("First Come First Serve");
        }


        public void OnPostRR()
        {
            Console.WriteLine("Round Robin");
        }


        public void OnPostSPN()
        {
            Console.WriteLine("Shortest Process Next");
        }


        public void OnPostSRT()
        {
            Console.WriteLine("Shortest Remaining Time");
        }


        public void OnPostHRRN()
        {
            Console.WriteLine("Highest Response Ratio Next");

        }


        public void OnPostFeedback()
        {
            Console.WriteLine("Feedback");

        }
    }
}
