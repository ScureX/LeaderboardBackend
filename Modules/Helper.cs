using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaderboardBackend.Modules
{
    public static class Helper
    {
        public static float SpeedToKmh(double vel)
        {
            return (float)Math.Round((Math.Sqrt(vel) * (0.274176 / 3)), 2);
        }

        public static float SpeedToMph(double vel)
        {
            return (float)Math.Round(Math.Sqrt(vel) * (0.274176 / 3) * (0.621371), 2);
        }
    }
}
