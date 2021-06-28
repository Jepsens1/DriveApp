using System;
using System.Collections.Generic;
using System.Text;

namespace DriveApp
{
    public enum Days
    {
        Mandag,
        Tirsdag,
        Onsdag,
        Torsdag,
        Fredag,
        Lørdag,
        Søndag
    };
    class DaysDistance
    {
        public Days Day { get; set; }
        public double Distance { get; set; }
        public DaysDistance(Days day, double _distance)
        {
            Day = day;
            Distance = _distance;
        }
        public override string ToString()
        { 
            return $"{Day} Distance: {Math.Truncate(Distance * 1000) / 1000}\n";
        }
    }
}
