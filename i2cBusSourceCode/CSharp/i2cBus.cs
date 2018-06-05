
using Microsoft.SPOT.Hardware;

/// <summary>

/// ''' Sets to current i2c device to execute the current action

/// ''' The address for the 24LC256 EEprom device is &H54

/// ''' The address for the SS1306 OLED device is &H3C

/// ''' </summary>
public class I2cBus
{

    // Used to set the current i2c device to execute i2c action
    public static I2CDevice Device = new I2CDevice(new I2CDevice.Configuration(0, 0));

    /// <summary>
    ///     ''' Adds a new i2c device to the bus
    ///     ''' </summary>
    ///     ''' <returns>i2c Device Configuration</returns>
    public static I2CDevice.Configuration AddNewDeviceConfiguration(byte Address, int ClockRateKHZ = 400)
    {
        return new I2CDevice.Configuration(Address, ClockRateKHZ);
    }
}
