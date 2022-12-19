using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker
{
    public class Log
    {
        public int Id { get; set; }
        public int BugId { get; set; }
        public string Text { get; set; }
        public DateTime CreationDate { get; set; }

    }
}
