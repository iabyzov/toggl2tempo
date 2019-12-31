using Microsoft.AspNetCore.Mvc;
using BLL;
using Microsoft.AspNetCore.Authorization;

namespace WebHost.ClientApi.Synchronization
{
    public class SynchronizationController : Controller
    {
        //private IToggl2TempoSynchronizer _toggl2TempoSynchronizer;

        //public SynchronizationController(IToggl2TempoSynchronizer toggl2TempoSynchronizer)
        //{
        //    _toggl2TempoSynchronizer = toggl2TempoSynchronizer;
        //}

        private ITogglToJiraSynchronizer _togglToJiraSynchronizer;

        public SynchronizationController(ITogglToJiraSynchronizer togglToJiraSynchronizer)
        {
            _togglToJiraSynchronizer = togglToJiraSynchronizer;
        }

        [HttpPost]
        [Authorize]
        [Route("api/[controller]/[action]")]
        public SyncResultModel Sync([FromBody] SyncPeriodModel period)
        {
            var result = _togglToJiraSynchronizer.Sync(period.StartTime, period.EndTime);
            return new SyncResultModel
            {
                TempoSentAmount = result.TempoSentAmount,
                TogglAmount = result.TogglAmount
            };
        }
    }
}