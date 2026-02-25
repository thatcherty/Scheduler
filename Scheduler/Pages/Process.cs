namespace Scheduler.Pages
{
    public class Process
    {
        public string Name { get; set; } = string.Empty;
        public int ArrivalTime { get; set; }

        public int ServiceTime { get; set; }

        public int RemainingTime { get; set; }

        public int WaitTime { get; set; }

        public int FinishTime { get; set; }

        public int Turnaround { get; set; }

        public double TT { get; set; }

        public List<bool>? IsRunning { get; set; }

    }

    public enum Algorithms
    {
        FCFS,
        RR,
        SPN,
        SRT,
        HRRN,
        Feedback
    }

}
