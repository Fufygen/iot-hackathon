using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Shared
{
    public class Switch : BindableBase
    {
        private readonly GpioPin _pin;
        private GpioPinValue _value;

        public bool IsOn { get; private set; }

        public Switch(GpioPin pin)
        {
            _pin = pin;
            Number = pin.PinNumber;

            Off();
        }

        public int Number { get; }

        public void Swap()
        {
            var value = _value;
            if (value == GpioPinValue.High)
            {
                Off();
            }
            else
            {
                On();
            }
        }

        public virtual void On()
        {
            _pin.Write(_value = GpioPinValue.High);
            IsOn = true;
        }
        public virtual void Off()
        {
            _pin.Write(_value = GpioPinValue.Low);
            IsOn = false;
        }
    }
}
