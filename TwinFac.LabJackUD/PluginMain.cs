using Newtonsoft.Json;
using TwinFac.LabJackUD.models;

namespace TwinFac.LabJackUD
{
    /// <summary>
    /// Entry point for LabJack U6 TwinFac Plugin
    /// 
    /// Only supports connecting to a single U6
    /// 
    /// Functions in this class will be accessable from the Javascript as labJackU6.functionName
    /// 
    /// e.g.
    /// labJackU6.invoke("connect", "").then(result => {
    ///     const result = JSON.parse(result);
    ///     const connectedResult = JSON.parse(result.data);
    /// });
    /// </summary>
    public class PluginMain
    {
        private U6Plugin _u6Plugin = new U6Plugin();

        /// <summary>
        /// Requests the last error reported by the U6
        /// </summary>
        /// <param name="methodParams"></param>
        /// <returns>The last error message</returns>
        public string GetLastError(string methodParams)
        {
            return JsonConvert.SerializeObject(new { lastError = _u6Plugin.LastError });
        }

        /// <summary>
        /// Requests the LabJack UD driver version
        /// </summary>
        /// <param name="methodParams"></param>
        /// <returns>Serialized JSON containing the version number of the installed LabJack UD driver installed</returns>
        public string GetDriverVersion(string methodParams)
        {
            return JsonConvert.SerializeObject(new { DriverVersion = _u6Plugin.GetDriverVersion() });
        }

        /// <summary>
        /// Connects to a LabJack U6 connected via USB
        /// </summary>
        /// <param name="methodParams"></param>
        /// <returns>Serialized JSON containing the connection result</returns>
        public string Connect(string methodParams)
        {
            var connectResult = _u6Plugin.Connect();
            return JsonConvert.SerializeObject(new { Connected = connectResult });
        }

        /// <summary>
        /// Requests the Bootloader version of the connected U6
        /// </summary>
        /// <param name="methodParams"></param>
        /// <returns>Serialized JSON containing the bootloader version</returns>
        public string GetBooloaderVersion(string methodParams)
        {
            return JsonConvert.SerializeObject(new { BootloaderVersion = _u6Plugin.GetBootloaderVersion() });
        }

        /// <summary>
        /// Requests the Hardware version of the connected U6
        /// </summary>
        /// <param name="methodParams"></param>
        /// <returns>Serialized JSON containing the hardware version</returns>
        public string GeHWVersion(string methodParams)
        {
            return JsonConvert.SerializeObject(new { HWVersion = _u6Plugin.GetHWVersion() });
        }

        /// <summary>
        /// Requests the Firmware version of the connected U6
        /// </summary>
        /// <param name="methodParams"></param>
        /// <returns>Serialized JSON containing the firmware version</returns>
        public string GetFWVersion(string methodParams)
        {
            return JsonConvert.SerializeObject(new { HWVersion = _u6Plugin.GetFWVersion() });
        }

        /// <summary>
        /// Sets the specified digital output to high or low as requested
        /// </summary>
        /// <param name="methodParams">Serialized JSON object with details of output to set e.g. to set FIO1 to high "{'outputNumber':1, 'state': true}" </param>
        /// <returns>Result of setting the output</returns>
        public string SetDigitalOutput(string methodParams)
        {
            var outputToSet = JsonConvert.DeserializeObject<OutputSet>(methodParams);
            var setResult = _u6Plugin.SetDigitalOutput(outputToSet.OutputNumber, outputToSet.State);
            return JsonConvert.SerializeObject(new { OutputResult = setResult });
        }

        /// <summary>
        /// Returns the value of the requested digital input
        /// </summary>
        /// <param name="methodParams">Serialized JSON object with the details of the input to query e.g. to get FIO2 "{'input': 2}"</param>
        /// <returns>Result value of the input</returns>
        public string GetDigitalInput(string methodParams)
        {
            var inputToGet = JsonConvert.DeserializeObject<InputGet>(methodParams);
            var getResult = _u6Plugin.GetDigitalInput(inputToGet.Input);
            return JsonConvert.SerializeObject(new { InputResult = getResult });
        }

        /// <summary>
        /// Sets the specified DAC to the requesed value
        /// </summary>
        /// <param name="methodParams">Serialized JSON object with the details of the DAC to set e.g. to set DAC0 to 2.5V "{'channel':0, 'value':2.5}"</param>
        /// <returns>Result of setting the DAC</returns>
        public string SetDAC(string methodParams)
        {
            var dacToSet = JsonConvert.DeserializeObject<DACSet>(methodParams);
            var setResult = _u6Plugin.SetDAC(dacToSet.Channel, dacToSet.Value);
            return JsonConvert.SerializeObject(new { DACResult = setResult });
        }

        /// <summary>
        /// Configure the specified analog channel before reading its value
        /// 
        /// Range values:
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
        ///     
        /// Settling Time Values:
        ///     0=5us
        ///     1=10us
        ///     2=100us
        ///     3=1ms
        ///     4=10ms
        ///     
        /// </summary>
        /// <param name="methodParams">Serialized JSON object with the details of the analog channel to configure e.g to set AIN2 to 0-5V "{'channel':2, 'range':103, 'settlingTime':0, 'resolution':0}"</param>
        /// <returns>Result of configuring the analog channel</returns>
        public string ConfigureAnalogInput(string methodParams)
        {
            var analogConfig = JsonConvert.DeserializeObject<AnalogConfig>(methodParams);
            var configResult = _u6Plugin.ConfigureAnalogInput(analogConfig.Channel, analogConfig.Range, analogConfig.Resolution, analogConfig.SettlingTime);
            return JsonConvert.SerializeObject(new { AnalogConfigResult = configResult });
        }

        /// <summary>
        /// Reads the value of the requested analog channel
        /// </summary>
        /// <param name="methodParams">Serialized JSON object with the details of the analog channel to read e.g to read AIN2 "{'channel':2}"</param>
        /// <returns></returns>
        public string GetAnalogInput(string methodParams)
        {
            var analogGet = JsonConvert.DeserializeObject<AnalogGet>(methodParams);
            var analogValue = _u6Plugin.GetAnalogInput(analogGet.Channel);
            return JsonConvert.SerializeObject(new { AnalogValue = analogValue });
        }

        public string NameSpace => "labJackU6";
    }
}