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
        // static declarations
        public static int TotalServiceTime { get; set; }
        public static int TimeQuantum { get; set; }
        public static Algorithms SelectedAlgo { get; set; }
        public static double TurnaroundMean { get; set; } = 0;
        public static double TTMean { get; set; } = 0;


        // Common
        public string Name { get; set; } = string.Empty;
        public int ArrivalTime { get; set; }
        public int ServiceTime { get; set; }
        public int RemainingTime { get; set; }
        public bool Queued { get; set; } = false;


        // RR and FCFS
        public int TimeInQuantum { get; set; }

        // FCFS
        public int NextQueue { get; set; }

        // HRRN
        public double ResponseRatio { get; set; }


        // Metrics
        public int WaitTime { get; set; }

        public int FinishTime { get; set; }

        public double Turnaround { get; set; }

        public double TT { get; set; }


        // Display running time
        public List<bool>? IsRunning { get; set; }

    }



}
