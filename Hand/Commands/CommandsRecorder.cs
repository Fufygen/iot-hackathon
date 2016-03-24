using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Hand.Commands
{
    public class CommandsRecorder
    {
        int _counter = 0;

        private Stopwatch _watch;
        public CommandsRecorder()
        {
            _watch = new Stopwatch();
        }

        private Direction _direction;
        private int _motorId;

        public void Start(IMotor motor, Direction direction)
        {
            _counter += 1;
            _motorId = motor.Id;
            _direction = direction;

            _watch.Restart();
        }

        public MotorCommand Stop()
        {
            _watch.Stop();
            if (_watch.Elapsed > TimeSpan.Zero)
            {
                return new MotorCommand() { MotorId = _motorId, Direction = _direction, Duration = _watch.Elapsed };
            }

            return null;
        }

        public void Reset()
        {
            _watch.Stop();
            _watch.Reset();
            _motorId = 0;
            _direction = Direction.Stopped;
        }
    }

}
