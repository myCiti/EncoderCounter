 using System;
using System.Device.Gpio;
using System.IO;

namespace Indexa
{
    class Program
    {
        static void Main(string[] args)
        {
            var gpio = new GpioController();

            // readPin function
            bool readPin(int p) => gpio.Read(p) == PinValue.High;

            const int indexPin = 4;
            const int pulsePin = 17;
            int pulseCounter = 0;
            //int cycleCounter = 0;
            int state = 0;

            gpio.OpenPin(indexPin, PinMode.Input);
            gpio.OpenPin(pulsePin, PinMode.Input);

            // var stopWatch = Stopwatch.StartNew();
            
            while (true)
            {
                switch (state)
                {
                    case 0: if (readPin(indexPin)) state = 1; break;
                    case 1: if (!readPin(indexPin)) state = 2; break;
                    case 2:
                        if (readPin(indexPin))
                        {
                            state = 1;
                            saveData(pulseCounter);
                        }
                        else if (readPin(pulsePin))
                        {
                            state = 3; ++pulseCounter;
                        } 
                        break;
                    case 3: 
                        if (readPin(indexPin))
                        {
                            state = 1;
                            saveData(pulseCounter);
                        }
                        else if (!readPin(pulsePin)) state = 2; break;
                }
            }

            void saveData(int counter)
            {
                Console.WriteLine($"{counter}");
                File.AppendAllLinesAsync("data.csv", new[] { $"{counter}" });
                pulseCounter = 0;

                /* when comenting the line File.AppendAllLinesAsync("data.csv", new[] { $"{counter}" });
                 *  we read on the terminal 2495 +/- 5 pulses
                 * 
                 */
            }
        }
    }
}
