using LabJack.LabJackUD;
using TwinFac.LabJackUD.models;

namespace TwinFac.LabJackUD
{
    public class U6Plugin
    {
        private U6? _u6;
        private bool _connected = false;

        /// <summary>
        /// Default constructor
        /// </summary>
        public U6Plugin()
        {
            LastError = "";
        }

        /// <summary>
        /// Requests the LabJack UD driver version
        /// </summary>
        /// <returns>Formatted string with driver versio number</returns>
        public string GetDriverVersion()
        {
            var dblDriverVersion = LJUD.GetDriverVersion();
            return String.Format("{0:0.000}", dblDriverVersion);
        }

        /// <summary>
        /// Connects to the U6
        /// </summary>
        /// <returns>Connection result</returns>
        public bool Connect()
        {
            if (_u6 == null && !Connected)
            {
                try
                {
                    _u6 = new U6(LJUD.CONNECTION.USB, "0", true);
                    _connected = true;
                    return true;
                }
                catch (LabJackUDException e)
                {
                    LastError = $"Error connecting with '{e.Message}'";
                    return false;
                }
            }

            LastError = "U6 already connected";
            return false;
        }

        /// <summary>
        /// Sets the specified digital output to high or low as requested
        /// </summary>
        /// <param name="output">The number of the output to be set</param>
        /// <param name="state">True = high, False = low</param>
        /// <returns>Result of setting the output</returns>
        public bool SetDigitalOutput(int output, bool state)
        {
            if (!Connected)
            {
                LastError = "U6 not connected";
                return false;
            }

            try
            {
                LJUD.AddRequest(_u6.ljhandle, LJUD.IO.PUT_DIGITAL_BIT, output, state == true ? 1 : 0, 0, 0);
                LJUD.GoOne(_u6.ljhandle);
                return true;
            }
            catch (LabJackUDException e)
            {
                LastError = $"Error setting digital output with '{e.Message}'";
                return false;
            }
        }

        /// <summary>
        /// Reads the value of the specified digital input
        /// </summary>
        /// <param name="input">The number of the input to read</param>
        /// <returns>The value of the read input</returns>
        public int GetDigitalInput(int input)
        {
            if (!Connected)
            {
                LastError = "U6 not connected";
                return -99;
            }

            LJUD.IO ioType = 0;
            LJUD.CHANNEL channel = 0;
            double dblValue = 0;
            int dummyInt = 0;
            double dummyDouble = 0;

            try
            {
                LJUD.AddRequest(_u6.ljhandle, LJUD.IO.GET_DIGITAL_BIT, input, 0, 0, 0);
                LJUD.GoOne(_u6.ljhandle);
                LJUD.GetFirstResult(_u6.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);

                if (ioType == LJUD.IO.GET_DIGITAL_BIT)
                {
                    return (int)dblValue;
                }
                else
                {
                    LastError = "IO Read returned unexpcted value";
                    return -99;
                }
            }
            catch (LabJackUDException e)
            {
                LastError = $"Error getting digital input with '{e.Message}'";
                return -99;
            }
        }

        /// <summary>
        /// Configure the specified analog channel before reading its value
        /// 
        /// Range values
        ///     AUTO = 0
        ///     +/- 20V = 1
        ///     +/- 10V = 2
        ///     +/- 5V = 3
        ///     +/- 4V = 4
        ///     +/- 2.5V = 5
        ///     +/- 2V = 6
        ///     +/- 1.25V = 7
        ///     +/- 1V = 8
        ///     +/- 0.625V = 9
        ///     +/- 0.1V = 10
        ///     +/- 0.01V = 11
        ///     + 20V = 101
        ///     + 10V = 102
        ///     + 5V = 103
        ///     + 4V = 104
        ///     + 2.5V = 105
        ///     + 2V = 106
        ///     + 1.25V = 107
        ///     + 1V = 108
        ///     + 0.625V = 109
        ///     + 0.5V = 110
        ///     + 0.3125V = 111
        ///     + 0.25V = 112
        ///     + 0.025V = 113
        ///     + 0.0025V = 114
        /// </summary>
        /// <param name="channel">The analog channel to configure</param>
        /// <param name="range">The range configuration (see summary)</param>
        /// <param name="resolution">Resolution of the analog inputs (pass a non-zero value for quick sampling).</param>
        /// <param name="settlingTime">Settleing time 0=5us, 1=10us, 2=100us, 3=1ms, 4=10ms</param>
        /// <returns></returns>
        public bool ConfigureAnalogInput(int channel, int range, int resolution, int settlingTime)
        {
            if (!Connected)
            {
                LastError = "U6 not connected";
                return false;
            }

            try
            {
                LJUD.ePut(_u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.AIN_RESOLUTION, resolution, 0);
                LJUD.ePut(_u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.AIN_SETTLING_TIME, settlingTime, 0);
                LJUD.AddRequest(_u6.ljhandle, LJUD.IO.PUT_AIN_RANGE, channel, (double)range, 0, 0);
                LJUD.GoOne(_u6.ljhandle);
                return true;
            }
            catch (LabJackUDException e)
            {
                LastError = $"Error configuring analog input with '{e.Message}'";
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="analogChannel"></param>
        /// <returns></returns>
        public double GetAnalogInput(int analogChannel)
        {
            if (!Connected)
            {
                LastError = "U6 not connected";
                return -99;
            }

            LJUD.IO ioType = 0;
            LJUD.CHANNEL channel = 0;
            double dblValue = 0;
            int dummyInt = 0;
            double dummyDouble = 0;

            try
            {
                LJUD.AddRequest(_u6.ljhandle, LJUD.IO.GET_AIN, analogChannel, 0, 0, 0);
                LJUD.GoOne(_u6.ljhandle);
                LJUD.GetFirstResult(_u6.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);

                if (ioType == LJUD.IO.GET_AIN)
                {
                    return dblValue;
                }
                else
                {
                    LastError = "IO Read returned unexpcted value";
                    return -99;
                }
            }
            catch (LabJackUDException e)
            {
                LastError = $"Error reading analog value with '{e.Message}'";
                return -99;
            }
        }

        /// <summary>
        /// Sets the specified DAC to the requesed value
        /// </summary>
        /// <param name="channel">DAC channel to be set</param>
        /// <param name="value">Value to set the DAC channel to</param>
        /// <returns>Result of setting the DAC channel</returns>
        public bool SetDAC(int channel, double value)
        {
            if (!Connected)
            {
                LastError = "U6 not connected";
                return false;
            }
            
            try
            {
                LJUD.AddRequest(_u6.ljhandle, LJUD.IO.PUT_DAC, channel, value, 0, 0);
                LJUD.GoOne(_u6.ljhandle);
                return true;
            }
            catch (LabJackUDException e)
            {
                LastError = $"Error setting DAC with '{e.Message}'";
                return false;
            }
        }

        /// <summary>
        /// Requests the bootloader version of the connected U6
        /// </summary>
        /// <returns>Bootloader version</returns>
        public string GetBootloaderVersion()
        {
            if (Connected)
            {
                var version = _u6.bootloaderversion;
                return String.Format("{0:0.000}", version);
            }
            else
            {
                LastError = "U6 not connected";
                return "";
            }
        }

        /// <summary>
        /// Requests the HW version of the connected U6
        /// </summary>
        /// <returns>HW Version</returns>
        public string GetHWVersion()
        {
            if (Connected)
            {
                double dblValue = 0;
                LJUD.eGet(_u6.ljhandle, LJUD.IO.GET_CONFIG, LJUD.CHANNEL.HARDWARE_VERSION, ref dblValue, 0);
                return String.Format("{0:0.000}", dblValue);
            }
            else
            {
                LastError = "U6 not connected";
                return "";
            }
        }

        /// <summary>
        /// Requests the FW version of the connected U6
        /// </summary>
        /// <returns>FW Version</returns>
        public string GetFWVersion()
        {
            if (Connected)
            {
                double dblValue = 0;
                LJUD.eGet(_u6.ljhandle, LJUD.IO.GET_CONFIG, LJUD.CHANNEL.FIRMWARE_VERSION, ref dblValue, 0);
                return String.Format("{0:0.000}", dblValue);
            }
            else
            {
                LastError = "U6 not connected";
                return "";
            }
        }

        public string LastError { get; set; }

        public bool Connected => _connected;
    }
}
