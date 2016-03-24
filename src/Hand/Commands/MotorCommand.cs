using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Hand.Commands
{

    public class MotorCommand
    {
        public int MotorId { get; set; }
        public Direction Direction { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
