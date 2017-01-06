using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace BlinkyPOC
{
    public sealed class StartupTask : IBackgroundTask
    {
        //This is the one from the example but i'm not using it on the board
        //private const int LED_PIN = 12;

        //Yellow
        private const int YELLOW_LED_PIN = 26;
        //Yellow
        private const int RED_LED_PIN = 19;
        //Yellow
        private const int GREEN_LED_PIN = 13;


        private GpioPinValue pinValue;

        private GpioPin yellowPin = null;
        private GpioPin redPin = null;
        private GpioPin greenPin = null;

        private static object objectLockObj = new object();

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // 
            // TODO: Insert code to perform background work
            //
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            //
            var state = new TimingStateModel();
            var aTimer = new Timer(Timer_Tick, state, 0, 500);

            InitGPIO(state);


            state.Wait();
            aTimer.Dispose();
        }

        private void Timer_Tick(object sender)
        {
            lock (objectLockObj)
            {
                if (redPin != null && yellowPin != null && greenPin != null)
                {
                    if (pinValue == GpioPinValue.High)
                    {
                        //Grounded out, should turn on
                        pinValue = GpioPinValue.Low;
                    }
                    else
                    {
                        //Default: high , no light
                        pinValue = GpioPinValue.High;
                    }
                    redPin.Write(pinValue);
                    greenPin.Write(pinValue);
                    yellowPin.Write(pinValue);
                }
            }
        }

        private void InitGPIO(TimingStateModel state)
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                state.Finish();
                return;
            }

            yellowPin = gpio.OpenPin(YELLOW_LED_PIN);
            redPin = gpio.OpenPin(RED_LED_PIN);
            greenPin = gpio.OpenPin(GREEN_LED_PIN);
            pinValue = GpioPinValue.High;
            redPin.Write(pinValue);
            greenPin.Write(pinValue);
            yellowPin.Write(pinValue);
            redPin.SetDriveMode(GpioPinDriveMode.Output);
            greenPin.SetDriveMode(GpioPinDriveMode.Output);
            yellowPin.SetDriveMode(GpioPinDriveMode.Output);
        }
    }

    public sealed class TimingStateModel
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);


        public TimingStateModel()
        {
            semaphore.Wait();
        }

        public void Finish()
        {
            semaphore.Release();
        }

        public void Wait()
        {
            semaphore.Wait();
        }
    }
}
