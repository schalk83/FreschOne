namespace FreschOne.Models
{
    public class PendingStepViewModel
    {

        public int EventID { get; set; }
        public long ProcessInstanceID { get; set; }
        public long StepID { get; set; }


        public long? GroupID { get; set; }

        public string? GroupDescription { get; set; }
        public long? UserID { get; set; }

        public string? FullName { get; set; }
        public DateTime DateAssigned { get; set; }
        public string StepDescription { get; set; }
        public double StepNo { get; set; }
        public int ProcessID { get; set; }

        public string ProcessName { get; set; }

        public string StepType { get; set; } // "Process" or "Approval"
        public bool IsReworkInstance { get; set; }


    }

}
