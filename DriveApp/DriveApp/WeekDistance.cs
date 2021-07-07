using System;
using System.Collections.Generic;
using System.Text;

namespace DriveApp
{
    public enum Week
    {
        Uge1,
        Uge2,
        Uge3,
        Uge4
    };
    class WeekDistance
    {
        public Week Week { get; set; }
        public double Distance { get; set; }
        public WeekDistance(Week _week, double _distance)
        {
            Week = _week;
            Distance = _distance;
        }

        public override string ToString()
        {
            return $"{Week} Kørt: {Math.Truncate(Distance * 1000) / 1000}\n";
        }
    }
}
