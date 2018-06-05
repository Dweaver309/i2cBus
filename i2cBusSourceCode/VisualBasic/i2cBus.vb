
Imports Microsoft.SPOT.Hardware

''' <summary>
''' Sets to current i2c device to execute the current action
''' The address for the 24LC256 EEprom device is &H54
''' The address for the SS1306 OLED device is &H3C
''' </summary>
Public Class I2cBus

    ' Used to set the current i2c device to execute i2c action
    Public Shared Device As I2CDevice = New I2CDevice(New I2CDevice.Configuration(0, 0))

    ''' <summary>
    ''' Adds a new i2c device to the bus
    ''' </summary>
    ''' <returns>i2c Device Configuration</returns>
    Public Shared Function AddNewDeviceConfiguration(Address As Byte, Optional ClockRateKHZ As Integer = 400) As I2CDevice.Configuration
        Return New I2CDevice.Configuration(Address, ClockRateKHZ)
    End Function

End Class