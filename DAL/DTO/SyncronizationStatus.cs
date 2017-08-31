using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public enum SyncronizationStatus
    {
        New,
        Dirty,
        Deleted,
        Syncronized,
        SentToCloud
    }
}
