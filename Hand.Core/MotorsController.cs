using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hand.Core
{
    public class MotorsController
    {
        private const int MOTOR_1_MINUS = 23;
        private const int MOTOR_1_PLUS = 24;

        private const int MOTOR_1_SWITCH = 17;

        private const int MOTOR_2_MINUS = 19;
        private const int MOTOR_2_PLUS = 26;

        private const int MOTOR_2_SWITCH = 27;

        private const int MOTOR_3_MINUS = 20;
        private const int MOTOR_3_PLUS = 21;


        public MotorsController()
        {
            var pins = PinSource.Default;

            var motor1 = new Motor(0, pins.GetPin(MOTOR_1_MINUS), pins.GetPin(MOTOR_1_PLUS));

            var motor2 = new Motor(0, pins.GetPin(MOTOR_2_MINUS), pins.GetPin(MOTOR_2_PLUS));



            Switch rm1 = new Switch(pins.GetPin(MOTOR_1_SWITCH));
            Switch rm2 = new Switch(pins.GetPin(MOTOR_2_SWITCH));

            Hand = new Motor(5, pins.GetPin(MOTOR_3_MINUS), pins.GetPin(MOTOR_3_PLUS), Windows.Devices.Gpio.GpioPinValue.High, Windows.Devices.Gpio.GpioPinValue.Low);

            Motor1 = new VirtualMotor(1, rm1, motor1, true);
            Motor2 = new VirtualMotor(2, rm2, motor2, false);
            Motor3 = new VirtualMotor(3, rm2, motor2, true);
            Motor4 = new VirtualMotor(4, rm1, motor1, false);
        }

        public IMotor Motor1 { get; }
        public IMotor Motor2 { get; }
        public IMotor Motor3 { get; }
        public IMotor Motor4 { get; }

        public IMotor Hand { get; }

        public IEnumerable<IMotor> Motors
        {
            get
            {
                yield return Motor1;
                yield return Motor2;
                yield return Motor3;
                yield return Motor4;
                yield return Hand;
            }
        }
    }
}
