using System;

namespace BLL
{
    public interface IToggl2TempoSynchronizer
    {
        SyncResult Sync(DateTime startTime, DateTime endTime);
    }
}