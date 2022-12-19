using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker
{
    public class Message
    {
        public int Id { get; set; }
        public int BugId { get; set; }
        public String Title { get; set; }
        public String Text { get; set; }
        public DateTime CreationDate { get; set; }
        public int CreatorId { get; set; }
    }
}
