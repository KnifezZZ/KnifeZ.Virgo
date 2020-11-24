using System.Collections.Generic;
using System.Net;

namespace KnifeZ.Virgo.Core.FDFS
{
    public class FDFSManager
    {
        public static bool Initialize(List<IPEndPoint> trackers)
        {
            return ConnectionManager.Initialize(trackers);
        }
    }
}
