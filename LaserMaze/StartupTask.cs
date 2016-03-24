using Glovebox.IoT.Devices.HATs;
using Glovebox.IoT.Devices.Sensors;
using System;
using System.Diagnostics;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;
using Windows.System.Threading;
using System.Threading.Tasks;
using Windows.Media.Capture;
using System.Collections.Generic;
using Windows.Storage.Streams;
using Windows.Media.MediaProperties;
using Microsoft.ProjectOxford.Face;
using System.Linq;
using Windows.Networking.Sockets;
using System.Text;
using System.IO;
using Microsoft.ProjectOxford.Face.Contract;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace LaserMaze
{
    public sealed class StartupTask : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral;
        private const int LED_PIN1 = 6;
        private GpioPin _pin1;
        private const int LED_PIN2 = 12;
        private GpioPin _pin2;
        private const int LED_PIN3 = 13;
        private GpioPin _pin3;
        private const int LED_PIN4 = 16;
        private GpioPin _pin4;
        private MediaCapture mediaCapture;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            InitGPIO();
            await InitCameraAsync();

            ExplorerHatPro explorerHatPro = new ExplorerHatPro();
            await explorerHatPro.InitialiseHatAsync(true, true, true); // initialise ADC, LEDS and motors

            if (!explorerHatPro.IsAdcInitalised)// alternatively initialise the ADS1015 on the board
            {
                await explorerHatPro.InitaliseAdcAsync();
            }

            List<Ldr> sensors = new List<Ldr>();
            sensors.Add(new Ldr(explorerHatPro.Adc.OpenChannel(3)));
            sensors.Add(new Ldr(explorerHatPro.Adc.OpenChannel(2)));
            sensors.Add(new Ldr(explorerHatPro.Adc.OpenChannel(1)));
            sensors.Add(new Ldr(explorerHatPro.Adc.OpenChannel(0)));

            List<GpioPin> leds = new List<GpioPin>() { _pin1, _pin2, _pin3, _pin4 };


            while (true)
            {

                var lightPercentages = sensors.Select(s => s.ReadRatio * 10).ToArray();

                int detections = 0;
                for (int i = 0; i < 4; i += 1)
                {
                    if (lightPercentages[i] < 90)
                    {
                        detections += 1;
                        leds[i].Write(GpioPinValue.High);
                    }
                    else
                    {
                        leds[i].Write(GpioPinValue.Low);
                    }
                }

                if (detections > 2)
                {
                    int femalesCount = await TryFindFemalesAsync();
                    if (femalesCount > 0)
                    {
                        SendFemaleFacesCount(femalesCount);
                    }
                }
            }
        }

        private void InitGPIO()
        {
            _pin1 = GpioController.GetDefault().OpenPin(LED_PIN1);
            _pin1.Write(GpioPinValue.Low);
            _pin1.SetDriveMode(GpioPinDriveMode.Output);

            _pin2 = GpioController.GetDefault().OpenPin(LED_PIN2);
            _pin2.Write(GpioPinValue.Low);
            _pin2.SetDriveMode(GpioPinDriveMode.Output);

            _pin3 = GpioController.GetDefault().OpenPin(LED_PIN3);
            _pin3.Write(GpioPinValue.Low);
            _pin3.SetDriveMode(GpioPinDriveMode.Output);

            _pin4 = GpioController.GetDefault().OpenPin(LED_PIN4);
            _pin4.Write(GpioPinValue.Low);
            _pin4.SetDriveMode(GpioPinDriveMode.Output);
        }

        private async Task InitCameraAsync()
        {
            mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync();
        }




        private async Task<IEnumerable<Face>> TakePhotoAsync()
        {
            IEnumerable<Face> faces = null;
            try
            {
                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
                    await mediaCapture.CapturePhotoToStreamAsync(imageProperties, stream);
                    stream.Seek(0);
                    FaceServiceClient client = new FaceServiceClient(/* INSERT YOUR API KEY */);
                    faces = await client.DetectAsync(imageStream: stream.AsStreamForRead());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Cleanup();
            }
            return faces ?? Enumerable.Empty<Face>();
        }

        private void SendFemaleFacesCount(int count)
        {
            try
            {
                StreamSocket socket = new StreamSocket();

                // TODO configure socket
                byte[] bodyArray = Encoding.UTF8.GetBytes(count.ToString());
                // Show the html 
                using (var outputStream = socket.OutputStream)
                {
                    using (Stream resp = outputStream.AsStreamForWrite())
                    {
                        using (MemoryStream stream = new MemoryStream(bodyArray))
                        {
                            string header = String.Format("HTTP/1.1 200 OK\r\n" +
                                                "Content-Length: {0}\r\n" +
                                                "Connection: close\r\n\r\n",
                                                stream.Length);
                            byte[] headerArray = Encoding.UTF8.GetBytes(header);
                            resp.Write(headerArray, 0, headerArray.Length);
                            stream.CopyTo(resp);
                            resp.Flush();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void Cleanup()
        {
            if (mediaCapture != null)
            {
                mediaCapture.Dispose();
                mediaCapture = null;
            }
        }

        private async Task<int> TryFindFemalesAsync()
        {
            var faces = await TakePhotoAsync();
            if (faces.Any())
            {
                int females = faces.Count(f => f.FaceAttributes.Gender.Equals("female", StringComparison.OrdinalIgnoreCase));
                return females;
            }

            return 0;
        }

        

    }


}
