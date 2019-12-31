using BLL.Domain;

namespace BLL.TimeTracker.Jira
{
    public class JiraWorklog : Worklog
    {
        public override bool IsCorrect => true;
        public string TicketKey { get; set; }
    }
}