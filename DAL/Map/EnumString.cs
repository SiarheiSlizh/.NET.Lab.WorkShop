using DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Map
{
    public static class EnumString
    {
        public static SyncronizationStatus MapToSyncStatus(this string status)
        {
            switch(status)
            {
                case "New": return SyncronizationStatus.New;
                case "Dirty": return SyncronizationStatus.Dirty;
                case "Deleted": return SyncronizationStatus.Deleted;
                case "Syncronized": return SyncronizationStatus.Syncronized;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
