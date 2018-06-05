
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;


namespace i2cBusTest
{
    public static class Program
    {
        public static void Main()
        {

            // Set i2c device configutation for the EEprom 24LC256 device
            I2CDevice.Configuration EEpromCon = I2cBus.AddNewDeviceConfiguration(0x54);

            // Set the shared I2CDevice.Configuration property 
            EEprom24LC256.Initialize(EEpromCon);

            // Set i2c device configutation for the SS1306 OLED device
            I2CDevice.Configuration OLEDCon = I2cBus.AddNewDeviceConfiguration(0x3C);

            // Set the shared I2CDevice.Configuration property
            OLED.Initialize(OLEDCon);

            EEprom24LC256.Write(EEprom24LC256.Address.TestWrite, "Hello");

            OLED.Write(0, 0, "I2c Test", true);

            string ReturnString = string.Empty;

            if (EEprom24LC256.Exist(EEprom24LC256.Address.TestWrite))
            {
                ReturnString = EEprom24LC256.Read(EEprom24LC256.Address.TestWrite);

                Debug.Print("Exist " + ReturnString);
            }

            OLED.Write(0, 2, "First String: " + ReturnString, true);

            EEprom24LC256.Write(EEprom24LC256.Address.TestWrite, "World!");

            if (EEprom24LC256.Exist(EEprom24LC256.Address.TestWrite))
            {
                ReturnString = EEprom24LC256.Read(EEprom24LC256.Address.TestWrite);

                Debug.Print("The address exists " + ReturnString);
            }

            OLED.Write(0, 4, "Second String: " + ReturnString, true);
        }
    }
}
