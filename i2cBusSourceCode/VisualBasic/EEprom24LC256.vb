

' See this adddress would be 0x54
' https://learn.sparkfun.com/tutorials/reading-and-writing-serial-eeproms?_ga=2.108951682.187827993.1506340160-1561098805.1424355686&_gac=1.87695722.1505836330.CjwKEAjwgIPOBRDn2eXxsN7S4RcSJABwNV90yvMlGAL86ZfVRkJsPS4LAteI0OhP0edbJN72UPjydBoCEpHw_wcB

' If the user uses “page write buffer” see tutorial above the starting address can not be changed without errors
' See this
' https://forums.ghielectronics.com/t/writing-data-to-eeprom-register-address/20785/55

Imports Microsoft.SPOT
Imports Microsoft.SPOT.Hardware
Imports System.Threading
Imports Microsoft.VisualBasic.Strings

Public Class EEprom24LC256

    ' Set in Initialize
    Private Shared Configuration As I2CDevice.Configuration

    ''' <summary>
    ''' 24Lc256 can hold 32,000 bytes
    ''' Each address will hold 200 bytes
    ''' MAX Text size per address is 66 characters (Need 2 for length and 3 per character... two for address and one for character)
    ''' 160 Addresses possible
    ''' </summary>
    ''' <remarks>1000 is an arbitrary starting point</remarks>
    Structure Address
        
        Public Shared TestWrite As Integer = 1000

    End Structure

    ''' <summary>
    ''' Get the i2c device configuration from i2cBus
    ''' </summary>
    ''' <param name="Config"></param>
    Public Shared Sub Initialize(Config As I2CDevice.Configuration)

        Configuration = Config

    End Sub

    ''' <summary>
    ''' Writes the address in the first 2  bytes
    ''' Writes the text length in the next 2 bytes
    ''' Each additional character is 3 bytes two for address and one for character
    ''' </summary>
    ''' <param name="Address">The starting address to write to</param>
    ''' <param name="Text">The text to write to EEprom</param>
    Public Shared Sub Write(Address As Integer, Text As String)

        Try

            I2cBus.Device.Config = Configuration

            Dim Length As String = String.Empty

            If Text.Length < 10 Then

                Length = " " & Text.Length.ToString

            Else

                Length = Text.Length.ToString

            End If

            Text = Length & Text

            Dim st As String = String.Empty

            For i = 0 To Text.Length - 1

                Dim xActions = New I2CDevice.I2CTransaction(0) {}

                st = Text.Substring(i, 1)

                'The "00" string reserves the room for the 2 bytes address that follows
                Dim buffer As Byte() = System.Text.Encoding.UTF8.GetBytes("00" & st)

                buffer(0) = HighByte(Address)

                buffer(1) = LowByte(Address)

                xActions(0) = I2CDevice.CreateWriteTransaction(buffer)

                Thread.Sleep(5)

                I2cBus.Device.Execute(xActions, 1000)

                Thread.Sleep(5)

                If I2cBus.Device.Execute(xActions, 1000) = 0 Then

                    Debug.Print("Failed to perform I2C transaction")

                End If

                Address += 1

            Next

        Catch ex As Exception

            Debug.Print("Error: Write " & ex.ToString)

        End Try

    End Sub


    ''' <summary>
    ''' Reads at a specified address.
    ''' Extracts the length of the text 
    ''' Reads all the text at this address into a string
    ''' </summary>
    ''' <param name="Address">The address to be read</param>
    ''' <returns>Text from the EEprom</returns>
    Public Shared Function Read(Address As Integer) As String

        Try

            Dim Str As String = String.Empty

            ' Get the text length for the current device
            For i = 2 To 3

                Str += ReadByte(Address)

                Address += 1

            Next

            ' Trim the spaces
            Str = Str.Trim

            ' This is the text length
            Dim StringLength As Integer = CInt(Str)

            ' Get the text to return 
            Str = String.Empty

            For i = 0 To StringLength - 1

                Str += ReadByte(Address + i)

            Next

            Return Str

        Catch ex As Exception

            Return "Error: Read " & ex.ToString

        End Try

    End Function

    ''' <summary>
    ''' Read one byte at the address
    ''' </summary>
    Private Shared Function ReadByte(Address As Integer) As String

        Try

            I2cBus.Device.Config = Configuration

            Dim Data = New Byte(0) {}

            Dim xActions = New I2CDevice.I2CTransaction(0) {}

            xActions(0) = I2CDevice.CreateWriteTransaction(New Byte() {HighByte(Address), LowByte(Address)})

            Thread.Sleep(5)

            I2cBus.Device.Execute(xActions, 1000)

            xActions(0) = I2CDevice.CreateReadTransaction(Data)

            Thread.Sleep(5)

            I2cBus.Device.Execute(xActions, 1000)

            'Convert the byte to string
            Return ChrW(Data(0)).ToString

        Catch ex As Exception

            Return "Error: ReadBtye " & ex.ToString

        End Try

    End Function

    ''' <summary>
    ''' If there is no length number the data has not been saved
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks>Use before calling the Read function</remarks>
    Public Shared Function Exist(Address As Integer) As Boolean

        Dim Str As String = ReadByte(Address + 1)

        Select Case Str
            Case "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
                Exist = True
            Case Else
                Exist = False
        End Select

    End Function

    ''' <summary>
    ''' Get High byte of the address
    ''' </summary>
    ''' <param name="ReadAddress"></param>
    ''' <returns>High Byte</returns>
    ''' <remarks>No byte shifters in VB</remarks>
    Private Shared Function HighByte(ReadAddress As Integer) As Byte

        Dim AddHigh As Byte = CByte(ReadAddress / 2 ^ 8 & &HFF)

        Return AddHigh

    End Function

    ''' <summary>
    ''' Get Low byte of the address
    ''' </summary>
    ''' <param name="ReadAddress"></param>
    ''' <returns>Low byte</returns>
    Private Shared Function LowByte(ReadAddress As Integer) As Byte

        Dim AddrLow As Byte = CByte(ReadAddress And &HFF)

        Return AddrLow

    End Function

End Class