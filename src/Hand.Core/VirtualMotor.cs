using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hand.Core
{
    public class VirtualMotor : IMotor
    {
        private readonly Motor _motor;
        private readonly Switch _relay;

        public bool Value { get; }

        public bool IsMoving
        {
            get; private set;
        }

        public int Id { get; }

        public VirtualMotor(int id, Switch relay, Motor motor, bool value)
        {
            Id = id;
            _motor = motor;
            _relay = relay;
            Value = value;
            Setup();
        }

        private void Setup()
        {
            _motor.IsMovingChanged += VirtualMotor_IsMovingChanged;
        }

        private void VirtualMotor_IsMovingChanged(object sender, bool e)
        {
            if (_relay.IsOn == Value)
            {
                IsMoving = e;
            }
        }

        public void Backward()
        {
            if (_relay.IsOn != Value)
            {
                _motor.Stop();
                _relay.Swap();
            }
            _motor.Backward();
        }

        public void Forward()
        {
            if (_relay.IsOn != Value)
            {
                _motor.Stop();
                _relay.Swap();
            }
            _motor.Forward();
        }

        public void Stop()
        {
            if (_relay.IsOn != Value)
            {
                _motor.Stop();
                _relay.Swap();
            }
            _motor.Stop();
        }
    }
}
