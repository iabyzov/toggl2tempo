using BLL.Domain;

namespace BLL.TimeTracker.Tempo
{
    public class TempoWorklog : Worklog
    {
        public string TicketKey { get; set; }

        public string Activity { get; set; }

        public override bool IsCorrect =>
            !string.IsNullOrEmpty(TicketKey)
            && !string.IsNullOrEmpty(Activity)
            && !string.IsNullOrEmpty(Description)
            && Duration > 0;
    }
}
