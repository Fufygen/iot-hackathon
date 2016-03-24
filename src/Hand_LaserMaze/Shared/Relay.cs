using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Shared
{
    public class Relay<T> : Switch
        where T : class
    {
        public T Device1 { get; }
        public T Device2 { get; }

        private T current = default(T);
        public T Current { get { return current; } set { Set(ref current, value); } }

        public Relay(GpioPin pin, T device1, T device2)
            : base(pin)
        {
            Device1 = device1;
            Device2 = device2;
            SetOff();

            //pin.ValueChanged += Pin_ValueChanged;
        }

        public T GetDevice(bool value)
        {
            return value ? Device1 : Device2;
        }

        public override void On()
        {
            base.On();
            SetOn();
        }

        public override void Off()
        {
            base.Off();
            SetOff();
        }

        private void SetOn()
        {
            Current = Device2;
        }

        private void SetOff()
        {
            Current = Device1;
        }
    }
}
