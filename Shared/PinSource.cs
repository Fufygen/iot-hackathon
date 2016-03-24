using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Shared
{
    public class PinSource
    {
        private GpioController _gpio;


        private static PinSource instance = null;
        public static PinSource Default
        {
            get
            {
                if (instance == null)
                {
                    instance = new PinSource();
                    instance.Init();
                }
                return instance;
            }
        }

        private bool Init()
        {
            _gpio = GpioController.GetDefault();
            return _gpio != null;
        }

        public bool IsInitialized {  get { return _gpio != null; } }

        private Dictionary<int, GpioPin> pins = new Dictionary<int, GpioPin>();
        public GpioPin GetPin(int pinNumber)
        {
            GpioPin pin = null;

            if (pins.TryGetValue(pinNumber, out pin) == false)
            {
                try
                {
                    Debug.WriteLine("Opening pin: " + pinNumber);
                    pin = _gpio.OpenPin(pinNumber);
                    pin.Write(GpioPinValue.Low);
                    pin.SetDriveMode(GpioPinDriveMode.Output);
                    pins.Add(pinNumber, pin);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to open pin:");
                    Debug.WriteLine(ex);
                    pin = null;
                }
            }

            return pin;
        }
    }

}
