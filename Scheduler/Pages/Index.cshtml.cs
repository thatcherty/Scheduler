using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Scheduler.Pages
{
    public enum Algorithms
    {
        FCFS,
        RR,
        SPN,
        SRT,
        HRRN,
        Feedback
    }
        

    public class IndexModel : PageModel
    {
        public Algorithms SelectedAlgorithm { get; set; }

        public List<string> AlgoDescriptions { get; set; } = new List<string> { };
        public void OnGet()
        {
            AlgoDescriptions.Add("First Come First Serve");
            AlgoDescriptions.Add("Round Robin");
            AlgoDescriptions.Add("Shortest Process Next");
            AlgoDescriptions.Add("Shortest Remaining Time");
            AlgoDescriptions.Add("Highest Response Ratio Next");
            AlgoDescriptions.Add("Feedback");
        }

        public void OnPost()
        {
            
        }

    }
}
