using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Shared
{
    public class Motor : BindableBase, IMotor
    {
        private readonly GpioPin _motorA;
        private readonly GpioPin _motorB;

        private readonly GpioPinValue _stoppedValue;
        private readonly GpioPinValue _runningValue;

        public Direction CurrentDirection
        {
            get; private set;
        } = Direction.Stopped;


        public event EventHandler<bool> IsMovingChanged;
        private bool isMoving = false;
        public bool IsMoving { get { return isMoving; } private set { isMoving = value; IsMovingChanged?.Invoke(this, value); } }
        public int Id { get; }

        public Motor(int id, GpioPin motorA, GpioPin motorB, GpioPinValue stoppedValue = GpioPinValue.Low, GpioPinValue runningValue = GpioPinValue.High)
        {
            Id = id;

            if (stoppedValue == runningValue)
            {
                throw new ArgumentException();
            }

            _motorA = motorA;
            _motorB = motorB;

            _stoppedValue = stoppedValue;
            _runningValue = runningValue;
        }

        public void Forward()
        {
            Stop();

            CurrentDirection = Direction.Forward;
            IsMoving = true;
            _motorA.Write(_runningValue);
        }

        public void Backward()
        {
            Stop();

            CurrentDirection = Direction.Backward;
            IsMoving = true;

            _motorB.Write(_runningValue);
        }

        public void Stop()
        {
            _motorA.Write(_stoppedValue);
            _motorB.Write(_stoppedValue);
            IsMoving = false;

            CurrentDirection = Direction.Stopped;
        }

        //private int CalculatePosition(Direction direction, TimeSpan duration)
        //{
        //    return ((int)direction) * Convert.ToInt32(duration.TotalMilliseconds); 
        //}
    }
}
