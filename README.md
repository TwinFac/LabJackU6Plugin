![](docs/images/logo_white.png)
www.twinfac.com

# LabJack U6 Sample Plugin for TwinFac

## Examples

### Toggle FIO2

```js
function toggleFIO2() {
    labJackU6.invoke("SetDigitalOutput", `{'outputNumber':2, 'state': ${state}}`).then(result => {
        state = !state;
        setTimeout(() => { toggleFIO2(); }, 1000);
    });
}
```

### Get HW Version

```js
function getHWVersion() {

    labJackU6.invoke("GetHWVersion", "").then(result => {
        var hwVersionResult = JSON.parse(result);
        var hwVersion = JSON.parse(hwVersionResult.data);
        console.log(`HW Version: ${hwVersion.HWVersion}`)
   });
}
```

### Sweep DAC0 from 0 - 5V reading value using AIN0

```js
var dacValue = 0;

function readAnalog() {
    labJackU6.invoke('GetAnalogInput', "{'channel':0}").then(result => {
        var analogResult = JSON.parse(result);
        var analogValue = JSON.parse(analogResult.data);
        console.log(`AIN0 = ${analogValue.AnalogValue}`);
        labJackU6.invoke('SetDAC', `{'channel':0, 'value':${dacValue}}`).then(result => { 
            dacValue = dacValue + 0.5;

            if (dacValue > 5) {
                dacValue = 0;
            }
        });
    });
}

function startAnalogRead() {
    labJackU6.invoke('ConfigureAnalogInput', "{'channel':0, 'range':103, 'settlingTime':0, 'resolution':0}").then(result => {
        labJackU6.invoke('SetDAC', `{'channel':0, 'value':${dacValue}}`).then(result => { });
        setInterval(() => { readAnalog(); }, 1000);
    });
}
```
