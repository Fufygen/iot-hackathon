// Copyright (c) Microsoft. All rights reserved.
using System;
using System.Linq;
using System.Text;
using Windows.ApplicationModel.Background;
using Windows.Devices.Pwm;
using Windows.ApplicationModel.AppService;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;


// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace SamplePwmConsumer
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const double PWM_FREQUENCY = 50d;
        private const double DUTY_CYCLE_PERCENTAGE = 0d;
        BackgroundTaskDeferral _deferral;

        HttpServer _httpServer;

        PinsController _controller;

        PwmPin _pin1;
        PwmPin _pin2;
        PwmPin _pin3;
        PwmPin _pin4;
        GpioPin _pin5;
        GpioController _gpioController;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            _controller = new PinsController();
            _gpioController = GpioController.GetDefault();
            try
            {
                bool initialized = await _controller.InitAsync(PWM_FREQUENCY);

                if (initialized)
                {
                    _pin1 = _controller.OpenPin(6);
                    _pin1.Start();

                    _pin2 = _controller.OpenPin(13);
                    _pin2.Start();

                    _pin3 = _controller.OpenPin(19);
                    _pin3.Start();

                    _pin4 = _controller.OpenPin(26);
                    _pin4.Start();

                    _pin5 = _gpioController.OpenPin(4);
                    _pin5.SetDriveMode(GpioPinDriveMode.Output);
                    _pin5.Write(GpioPinValue.Low);
                    
                }

                _httpServer = new HttpServer(6000);
                _httpServer.StartServer();

                await ListenAsync();

            }
            catch (Exception ex)
            {
            }

            Stop();

            _deferral.Complete();
            _deferral = null;
        }


        private void Stop()
        {
            _pin1.SetActiveDutyCyclePercentage(0);
            _pin2.SetActiveDutyCyclePercentage(0);
            _pin3.SetActiveDutyCyclePercentage(0);
            _pin4.SetActiveDutyCyclePercentage(0);
            _pin5.Write(GpioPinValue.Low);
        }

        private async Task ListenAsync()
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            int counter = 0;

            EventHandler<string> handler = async (s, msg) =>
            {
                int i = counter += 1;

                bool handleNext = HandleMessage(msg);

                if (handleNext)
                {
                    await Task.Delay(1000);
                    if (i == counter)
                    {
                        tcs.SetResult(false);
                    }
                }
                else
                {
                    tcs.SetResult(false);
                }
            };

            _httpServer.MessageReceived += handler;

            await tcs.Task;

            _httpServer.MessageReceived -= handler;
        }

        private bool HandleMessage(string message)
        {
            char cmd = message[0];
            String valuestr = message.Substring(1);
            double value = 0;

            switch (cmd)
            {
                case 'a':
                    value = Double.Parse(valuestr);
                    if (value < 0)
                    {
                        _pin1.SetActiveDutyCyclePercentage(0);
                        _pin2.SetActiveDutyCyclePercentage(Math.Abs(value));
                    }
                    else
                    {
                        _pin1.SetActiveDutyCyclePercentage(Math.Abs(value));
                        _pin2.SetActiveDutyCyclePercentage(0);
                    }
                    break;
                case 's':
                    value = Double.Parse(valuestr);
                    if (value < 0)
                    {
                        _pin3.SetActiveDutyCyclePercentage(0);
                        _pin4.SetActiveDutyCyclePercentage(Math.Abs(value));
                    }
                    else
                    {
                        _pin3.SetActiveDutyCyclePercentage(Math.Abs(value));
                        _pin4.SetActiveDutyCyclePercentage(0);
                    }
                    break;
                case 'f':
                    _pin5.Write(GpioPinValue.High);
                    break;
                case 'g':
                    _pin5.Write(GpioPinValue.Low);
                    break;
                default:
                    return false;
            }
            return true;
        }

        ~StartupTask()
        {
            _pin1?.Stop();
            _pin2?.Stop();


            _pin1?.Dispose();
            _pin2?.Dispose();
            _httpServer?.Dispose();
        }
    }
}
