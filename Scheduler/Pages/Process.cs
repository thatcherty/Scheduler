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
    public class Process
    {
        public static int TotalServiceTime { get; set; }

        public static Algorithms SelectedAlgo { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ArrivalTime { get; set; }

        public int ServiceTime { get; set; } 

        public int RemainingTime { get; set; }
        public double ResponseRatio { get; set; }

        public int WaitTime { get; set; }

        public int FinishTime { get; set; }

        public int Turnaround { get; set; }

        public double TT { get; set; }

        public bool Queued { get; set; } = false;

        public List<bool>? IsRunning { get; set; }

    }



}
