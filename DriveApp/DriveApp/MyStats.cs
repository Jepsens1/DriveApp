using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace DriveApp
{
    public class MyStats
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public DateTime Timecreated { get; set; }
        public double Distance { get; set; }
    }
}
