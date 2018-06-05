// See this adddress would be 0x54
// https://learn.sparkfun.com/tutorials/reading-and-writing-serial-eeproms?_ga=2.108951682.187827993.1506340160-1561098805.1424355686&_gac=1.87695722.1505836330.CjwKEAjwgIPOBRDn2eXxsN7S4RcSJABwNV90yvMlGAL86ZfVRkJsPS4LAteI0OhP0edbJN72UPjydBoCEpHw_wcB

// If the user uses “page write buffer” see tutorial above the starting address can not be changed without errors
// See this
// https://forums.ghielectronics.com/t/writing-data-to-eeprom-register-address/20785/55

using System;

using Microsoft.VisualBasic;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;
using Microsoft.VisualBasic;


public class EEprom24LC256
{

    // Set in Initialize
    private static I2CDevice.Configuration Configuration;

    /// <summary>
    ///     ''' 24Lc256 can hold 32,000 bytes
    ///     ''' Each address will hold 200 bytes
    ///     ''' MAX Text size per address is 66 characters (Need 2 for length and 3 per character... two for address and one for character)
    ///     ''' 160 Addresses possible
    ///     ''' </summary>
    ///     ''' <remarks>1000 is an arbitrary starting point</remarks>
   public struct Address
    {
       
        public static int TestWrite = 1000;
    }

    /// <summary>
    ///     ''' Get the i2c device configuration from i2cBus
    ///     ''' </summary>
    ///     ''' <param name="Config"></param>
    public static void Initialize(I2CDevice.Configuration Config)
    {
        Configuration = Config;
    }

    /// <summary>
    ///     ''' Writes the address in the first 2  bytes
    ///     ''' Writes the text length in the next 2 bytes
    ///     ''' Each additional character is 3 bytes two for address and one for character
    ///     ''' </summary>
    ///     ''' <param name="Address">The starting address to write to</param>
    ///     ''' <param name="Text">The text to write to EEprom</param>
    public static void Write(int Address, string Text)
    {
        try
        {
            I2cBus.Device.Config = Configuration;

            string Length = string.Empty;

            if (Text.Length < 10)
                Length = " " + Text.Length.ToString();
            else
                Length = Text.Length.ToString();

            Text = Length + Text;

            string st = string.Empty;

            for (var i = 0; i <= Text.Length - 1; i++)
            {
                var xActions = new I2CDevice.I2CTransaction[1] ;

                st = Text.Substring(i, 1);

                // The "00" string reserves the room for the 2 bytes address that follows
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes("00" + st);

                buffer[0] = HighByte(Address);

                buffer[1] = LowByte(Address);

                xActions[0] = I2CDevice.CreateWriteTransaction(buffer);

                Thread.Sleep(5);

                I2cBus.Device.Execute(xActions, 1000);

                Thread.Sleep(5);

                if (I2cBus.Device.Execute(xActions, 1000) == 0)
                    Debug.Print("Failed to perform I2C transaction");

                Address += 1;
            }
        }
        catch (Exception ex)
        {
            Debug.Print("Error: Write " + ex.ToString());
        }
    }


    /// <summary>
    ///     ''' Reads at a specified address.
    ///     ''' Extracts the length of the text 
    ///     ''' Reads all the text at this address into a string
    ///     ''' </summary>
    ///     ''' <param name="Address">The address to be read</param>
    ///     ''' <returns>Text from the EEprom</returns>
    public static string Read(int Address)
    {
        try
        {
            string Str = string.Empty;

            // Get the text length for the current device
            for (var i = 2; i <= 3; i++)
            {
                Str += ReadByte(Address);

                Address += 1;
            }

            // Trim the spaces
            Str = Str.Trim();

            // This is the text length
            int StringLength = System.Convert.ToInt32(Str);

            // Get the text to return 
            Str = string.Empty;

            for (var i = 0; i <= StringLength - 1; i++)

                Str += ReadByte(Address + i);

            return Str;
        }
        catch (Exception ex)
        {
            return "Error: Read " + ex.ToString();
        }
    }

    /// <summary>
    ///     ''' Read one byte at the address
    ///     ''' </summary>
    private static string ReadByte(int Address)
    {
        try
        {
            I2cBus.Device.Config = Configuration;

            var Data = new byte[1] ;

            var xActions = new I2CDevice.I2CTransaction[1] ;

            xActions[0] = I2CDevice.CreateWriteTransaction(new byte[] { HighByte(Address), LowByte(Address) });

            Thread.Sleep(5);

            I2cBus.Device.Execute(xActions, 1000);

            xActions[0] = I2CDevice.CreateReadTransaction(Data);

            Thread.Sleep(5);

            I2cBus.Device.Execute(xActions, 1000);

            // Convert the byte to string
            return Strings.ChrW(Data[0]).ToString();
        }
        catch (Exception ex)
        {
            return "Error: ReadBtye " + ex.ToString();
        }
    }

    /// <summary>
    ///     ''' If there is no length number the data has not been saved
    ///     ''' </summary>
    ///     ''' <returns>True or False</returns>
    ///     ''' <remarks>Use before calling the Read function</remarks>
    public static bool Exist(int Address)
    {
        string Str = ReadByte(Address + 1);

        switch (Str)
        {
            case "0":
            case "1":
            case "2":
            case "3":
            case "4":
            case "5":
            case "6":
            case "7":
            case "8":
            case "9":
                {
                    return true;
                   
                }

            default:
               {
                    return false;
                   
               }
        }
    }

    /// <summary>
    ///     ''' Get High byte of the address
    ///     ''' </summary>
    ///     ''' <param name="ReadAddress"></param>
    ///     ''' <returns>High Byte</returns>
    ///     ''' <remarks>No byte shifters in VB</remarks>
    private static byte HighByte(int ReadAddress)
    {
        
        byte AddHigh = (byte)(ReadAddress >> 8);
       
        return AddHigh;
    }

    /// <summary>
    ///     ''' Get Low byte of the address
    ///     ''' </summary>
    ///     ''' <param name="ReadAddress"></param>
    ///     ''' <returns>Low byte</returns>
    private static byte LowByte(int ReadAddress)
    {
        byte AddrLow = (Byte)(ReadAddress + 0xFF);

        return AddrLow;
    }
}
