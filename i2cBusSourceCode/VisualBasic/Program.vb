Option Explicit On
Option Strict On

Imports Microsoft.SPOT
Imports Microsoft.SPOT.Hardware
Imports System.Threading

Namespace i2cBusTest

    Public Module Module1

        Sub Main()

            ' Set i2c device configutation for the EEprom 24LC256 device
            Dim EEpromCon As I2CDevice.Configuration = I2cBus.AddNewDeviceConfiguration(&H54)

            ' Set the shared I2CDevice.Configuration property 
            EEprom24LC256.Initialize(EEpromCon)

            ' Set i2c device configutation for the SS1306 OLED device
            Dim OLEDCon As I2CDevice.Configuration = I2cBus.AddNewDeviceConfiguration(&H3C)

            ' Set the shared I2CDevice.Configuration property
            OLED.Initialize(OLEDCon)

            EEprom24LC256.Write(EEprom24LC256.Address.TestWrite, "Hello")

            OLED.Write(0, 0, "I2c Test", True)

            Dim ReturnString As String = String.Empty

            If EEprom24LC256.Exist(EEprom24LC256.Address.TestWrite) Then

                ReturnString = EEprom24LC256.Read(EEprom24LC256.Address.TestWrite)

                Debug.Print("Exist " & ReturnString)

            End If

            OLED.Write(0, 2, "First String: " & ReturnString, True)

            EEprom24LC256.Write(EEprom24LC256.Address.TestWrite, "World!")

            If EEprom24LC256.Exist(EEprom24LC256.Address.TestWrite) Then

                ReturnString = EEprom24LC256.Read(EEprom24LC256.Address.TestWrite)

                Debug.Print("The address exists " & ReturnString)

            End If

            OLED.Write(0, 4, "Second String: " & ReturnString, True)

        End Sub

    End Module

End Namespace