
' Code is from adafruit origianlly https://github.com/adafruit/Adafruit_SSD1306/
' Translated by David Weaver

' OLED Driver has a very small memory footprint that requires about 2.5K

Imports System.Threading
Imports Microsoft.SPOT
Imports Microsoft.SPOT.Hardware

Public Class OLED

    ' Set in Initialize
    Private Shared Configuration As I2CDevice.Configuration

    Private Const BufferSize As Integer = 1024

    Private Const Width As Integer = 128

    Private Const Height As Integer = 64

    Public Shared DisplayBuffer As Byte() = New Byte(BufferSize - 1) {}

    '  SSD1306 Commands
    Enum Cmds As Byte
        DisplayOff = &HAE
        DisplayClockDiv = &HD5
        DisplayRatio = &H80
        Multiplex = &HA8
        DisplayOffSet = &HD3
        NoOffSet = &H0
        StartLine = &H40
        ChargePump = &H8D
        VCCState = &H14
        MemoryMode = &H20
        LowColumn = &H0
        SegRemap = &HA1
        ComScanDec = &HC8
        SetComPins = &HDA
        DisableLRRemap = &H12
        SetContrast = &H81
        NoExternalVcc = &HCF
        PreCharge = &HD9
        InternalDC = &HF1
        ComDetect = &HD8
        SetComDetect = &H40
        DisplayResume = &HA4
        NormalDisplay = &HA6
        DeactivateScroll = &H2E
        DisplayOn = &HAF
        ColumnAddress = &H21
        Reset = &H0
        PageAddress = &H22
        PageEndAddress = &H37
    End Enum

    ''' <summary>
    ''' Set SSD1306 Defaults for I2c
    ''' The i2c configuration is set in the i2cbus 
    ''' </summary>
    Public Shared Sub Initialize(Config As I2CDevice.Configuration)

        Configuration = Config

        Command(Cmds.DisplayOff)
        Command(Cmds.DisplayClockDiv)
        Command(Cmds.DisplayRatio)
        Command(Cmds.Multiplex)
        Command(Height - 1)
        Command(Cmds.DisplayOffSet)
        Command(Cmds.NoOffSet)
        Command(Cmds.StartLine)
        Command(Cmds.ChargePump)
        Command(Cmds.VCCState)
        Command(Cmds.MemoryMode)
        Command(Cmds.LowColumn)
        Command(Cmds.SegRemap)
        Command(Cmds.ComScanDec)
        Command(Cmds.SetComPins)
        Command(Cmds.DisableLRRemap)
        Command(Cmds.SetContrast)
        Command(Cmds.NoExternalVcc)
        Command(Cmds.PreCharge)
        Command(Cmds.InternalDC)
        Command(Cmds.ComDetect)
        Command(Cmds.SetComDetect)
        Command(Cmds.DisplayResume)
        Command(Cmds.NormalDisplay)
        Command(Cmds.DeactivateScroll)
        Command(Cmds.DisplayOn)

    End Sub

    ''' <summary>
    ''' Puts one font character in the display buffer
    ''' At the horizonal and Line location
    ''' </summary>
    Private Shared Sub DrawCharacter(Horizonal As Integer, Line As Integer, ASCII As Integer)

        For i As Integer = 0 To 4

            DisplayBuffer(Horizonal + (Line * 128)) = Font((ASCII * 5) + i)

            Horizonal += 1

        Next

    End Sub

    ''' <summary>
    ''' Called from optional Center in Write
    ''' </summary>
    Private Shared Function CenterString(Str As String) As String

        Try
            Dim MaxStringLength As Integer = 21

            Dim spacesNeeded As Integer = CInt((MaxStringLength - Str.Length) / 2)

            Dim Spaces As String = String.Empty

            For i = 1 To spacesNeeded
                Spaces += " "
            Next

            Return Spaces & Str

        Catch ex As Exception

            Debug.Print("CenterString Error")

        End Try

        Return Str

    End Function

    ''' <summary>
    ''' Adds the string to the display
    ''' Calls DrawCharacter for each character
    ''' </summary>
    Public Shared Sub Write(Horizonal As Integer, Line As Integer, Str As String, Optional Center As Boolean = False)

        If Center Then Str = CenterString(Str)

        For Counter As Integer = 0 To Str.Length - 1

            Dim Asc As Integer = Microsoft.VisualBasic.Strings.AscW(Str.Substring(Counter, 1))

            DrawCharacter(Horizonal, Line, Asc)

            Horizonal += 6

            '  6 Pixels wide
            If Horizonal + 6 >= Width Then

                Horizonal = 0

                '  New Line
                Line += 1

            End If

            '  Max lines
            If Line >= Height / 8 Then

                Return

            End If

        Next

        Display()

    End Sub

    ''' <summary>
    ''' Writes the text to the device 
    ''' </summary>
    ''' <param name="WriteBuffer">The array of bytes that will be sent to the device.</param>
    ''' <param name="TransactionTimeout">The amount of time the system will wait before resuming execution of the transaction.</param>
    Public Shared Sub WriteToDevice(WriteBuffer As Byte(), Optional TransactionTimeout As Integer = 1000)

        Try

            ' Set i2c device configuration
            I2cBus.Device.Config = Configuration

            ' Create an i2c write transaction to be sent to the device.
            Dim XActions As I2CDevice.I2CTransaction() = New I2CDevice.I2CTransaction() {I2CDevice.CreateWriteTransaction(WriteBuffer)}

            ' the i2c data is sent here to the device.
            Dim transferred As Integer = I2cBus.Device.Execute(XActions, TransactionTimeout)

            Thread.Sleep(50)

            ' Make sure the data was sent.
            If transferred <> WriteBuffer.Length Then

                Debug.Print("Could not write to device.")

            End If

        Catch ex As Exception

            Debug.Print("Write Error: " & ex.ToString)

        End Try

    End Sub

    ''' <summary>
    ''' Clears the display
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub ClearScreen()
        DisplayBuffer(0) = 0
        DisplayBuffer(1) = 0
        DisplayBuffer(2) = 0
        DisplayBuffer(3) = 0
        DisplayBuffer(4) = 0
        DisplayBuffer(5) = 0
        DisplayBuffer(6) = 0
        DisplayBuffer(7) = 0
        DisplayBuffer(8) = 0
        DisplayBuffer(9) = 0
        DisplayBuffer(10) = 0
        DisplayBuffer(11) = 0
        DisplayBuffer(12) = 0
        DisplayBuffer(13) = 0
        DisplayBuffer(14) = 0
        DisplayBuffer(15) = 0

        Array.Copy(DisplayBuffer, 0, DisplayBuffer, 16, 16)
        Array.Copy(DisplayBuffer, 0, DisplayBuffer, 32, 32)
        Array.Copy(DisplayBuffer, 0, DisplayBuffer, 64, 64)
        Array.Copy(DisplayBuffer, 0, DisplayBuffer, 128, 128)
        Array.Copy(DisplayBuffer, 0, DisplayBuffer, 256, 256)
        Array.Copy(DisplayBuffer, 0, DisplayBuffer, 512, 512)

        Display()

    End Sub

    ''' <summary>
    ''' Clear one line on the device screen
    ''' </summary>
    Public Shared Sub ClearLine(Horizonal As Integer, Line As Integer)

        ' 21 spaces
        ' font is 6 pixels wide sceen is 128 pixel
        Dim Str As String = "                     "

        Write(Horizonal, Line, Str)

    End Sub

    ''' <summary>
    ''' <summary>
    ''' Creates the I2c transaction for the display setup
    ''' </summary>
    Private Shared Sub Command(Cmd As Integer)

        Dim xActions As I2CDevice.I2CTransaction() = New I2CDevice.I2CTransaction(0) {}

        ' Create the write buffer with one byte
        Dim Buff As Byte() = New Byte(1) {}

        Buff(1) = CByte(Cmd)

        WriteToDevice(Buff)

    End Sub

    ''' <summary>
    ''' Sends the Write and Clear I2c transactions to the display
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub Display()

        Command(Cmds.ColumnAddress)

        Command(Cmds.Reset)

        Command(Width - 1)

        Command(Cmds.PageAddress)

        Command(Cmds.Reset)

        Command(Cmds.PageEndAddress)

        Dim img As Byte() = New Byte((1024)) {}


        img(0) = Cmds.StartLine

        Array.Copy(DisplayBuffer, 0, img, 1, 1024)

        ' Send the bytes to the device
        WriteToDevice(img)


    End Sub

    ''' <summary>
    '''  5x7 font
    ''' </summary>
    Private Shared ReadOnly Font As Byte() = New Byte() {&H0, &H0, &H0, &H0, &H0, &H3E,
             &H5B, &H4F, &H5B, &H3E, &H3E, &H6B,
             &H4F, &H6B, &H3E, &H1C, &H3E, &H7C,
             &H3E, &H1C, &H18, &H3C, &H7E, &H3C,
             &H18, &H1C, &H57, &H7D, &H57, &H1C,
             &H1C, &H5E, &H7F, &H5E, &H1C, &H0,
             &H18, &H3C, &H18, &H0, &HFF, &HE7,
             &HC3, &HE7, &HFF, &H0, &H18, &H24,
             &H18, &H0, &HFF, &HE7, &HDB, &HE7,
             &HFF, &H30, &H48, &H3A, &H6, &HE,
             &H26, &H29, &H79, &H29, &H26, &H40,
             &H7F, &H5, &H5, &H7, &H40, &H7F,
             &H5, &H25, &H3F, &H5A, &H3C, &HE7,
             &H3C, &H5A, &H7F, &H3E, &H1C, &H1C,
             &H8, &H8, &H1C, &H1C, &H3E, &H7F,
             &H14, &H22, &H7F, &H22, &H14, &H5F,
             &H5F, &H0, &H5F, &H5F, &H6, &H9,
             &H7F, &H1, &H7F, &H0, &H66, &H89,
             &H95, &H6A, &H60, &H60, &H60, &H60,
             &H60, &H94, &HA2, &HFF, &HA2, &H94,
             &H8, &H4, &H7E, &H4, &H8, &H10,
             &H20, &H7E, &H20, &H10, &H8, &H8,
             &H2A, &H1C, &H8, &H8, &H1C, &H2A,
             &H8, &H8, &H1E, &H10, &H10, &H10,
             &H10, &HC, &H1E, &HC, &H1E, &HC,
             &H30, &H38, &H3E, &H38, &H30, &H6,
             &HE, &H3E, &HE, &H6, &H0, &H0,
             &H0, &H0, &H0, &H0, &H0, &H5F,
             &H0, &H0, &H0, &H7, &H0, &H7,
             &H0, &H14, &H7F, &H14, &H7F, &H14,
             &H24, &H2A, &H7F, &H2A, &H12, &H23,
             &H13, &H8, &H64, &H62, &H36, &H49,
             &H56, &H20, &H50, &H0, &H8, &H7,
             &H3, &H0, &H0, &H1C, &H22, &H41,
             &H0, &H0, &H41, &H22, &H1C, &H0,
             &H2A, &H1C, &H7F, &H1C, &H2A, &H8,
             &H8, &H3E, &H8, &H8, &H0, &H80,
             &H70, &H30, &H0, &H8, &H8, &H8,
             &H8, &H8, &H0, &H0, &H60, &H60,
             &H0, &H20, &H10, &H8, &H4, &H2,
             &H3E, &H51, &H49, &H45, &H3E, &H0,
             &H42, &H7F, &H40, &H0, &H72, &H49,
             &H49, &H49, &H46, &H21, &H41, &H49,
             &H4D, &H33, &H18, &H14, &H12, &H7F,
             &H10, &H27, &H45, &H45, &H45, &H39,
             &H3C, &H4A, &H49, &H49, &H31, &H41,
             &H21, &H11, &H9, &H7, &H36, &H49,
             &H49, &H49, &H36, &H46, &H49, &H49,
             &H29, &H1E, &H0, &H0, &H14, &H0,
             &H0, &H0, &H40, &H34, &H0, &H0,
             &H0, &H8, &H14, &H22, &H41, &H14,
             &H14, &H14, &H14, &H14, &H0, &H41,
             &H22, &H14, &H8, &H2, &H1, &H59,
             &H9, &H6, &H3E, &H41, &H5D, &H59,
             &H4E, &H7C, &H12, &H11, &H12, &H7C,
             &H7F, &H49, &H49, &H49, &H36, &H3E,
             &H41, &H41, &H41, &H22, &H7F, &H41,
             &H41, &H41, &H3E, &H7F, &H49, &H49,
             &H49, &H41, &H7F, &H9, &H9, &H9,
             &H1, &H3E, &H41, &H41, &H51, &H73,
             &H7F, &H8, &H8, &H8, &H7F, &H0,
             &H41, &H7F, &H41, &H0, &H20, &H40,
             &H41, &H3F, &H1, &H7F, &H8, &H14,
             &H22, &H41, &H7F, &H40, &H40, &H40,
             &H40, &H7F, &H2, &H1C, &H2, &H7F,
             &H7F, &H4, &H8, &H10, &H7F, &H3E,
             &H41, &H41, &H41, &H3E, &H7F, &H9,
             &H9, &H9, &H6, &H3E, &H41, &H51,
             &H21, &H5E, &H7F, &H9, &H19, &H29,
             &H46, &H26, &H49, &H49, &H49, &H32,
             &H3, &H1, &H7F, &H1, &H3, &H3F,
             &H40, &H40, &H40, &H3F, &H1F, &H20,
             &H40, &H20, &H1F, &H3F, &H40, &H38,
             &H40, &H3F, &H63, &H14, &H8, &H14,
             &H63, &H3, &H4, &H78, &H4, &H3,
             &H61, &H59, &H49, &H4D, &H43, &H0,
             &H7F, &H41, &H41, &H41, &H2, &H4,
             &H8, &H10, &H20, &H0, &H41, &H41,
             &H41, &H7F, &H4, &H2, &H1, &H2,
             &H4, &H40, &H40, &H40, &H40, &H40,
             &H0, &H3, &H7, &H8, &H0, &H20,
             &H54, &H54, &H78, &H40, &H7F, &H28,
             &H44, &H44, &H38, &H38, &H44, &H44,
             &H44, &H28, &H38, &H44, &H44, &H28,
             &H7F, &H38, &H54, &H54, &H54, &H18,
             &H0, &H8, &H7E, &H9, &H2, &H18,
             &HA4, &HA4, &H9C, &H78, &H7F, &H8,
             &H4, &H4, &H78, &H0, &H44, &H7D,
             &H40, &H0, &H20, &H40, &H40, &H3D,
             &H0, &H7F, &H10, &H28, &H44, &H0,
             &H0, &H41, &H7F, &H40, &H0, &H7C,
             &H4, &H78, &H4, &H78, &H7C, &H8,
             &H4, &H4, &H78, &H38, &H44, &H44,
             &H44, &H38, &HFC, &H18, &H24, &H24,
             &H18, &H18, &H24, &H24, &H18, &HFC,
             &H7C, &H8, &H4, &H4, &H8, &H48,
             &H54, &H54, &H54, &H24, &H4, &H4,
             &H3F, &H44, &H24, &H3C, &H40, &H40,
             &H20, &H7C, &H1C, &H20, &H40, &H20,
             &H1C, &H3C, &H40, &H30, &H40, &H3C,
             &H44, &H28, &H10, &H28, &H44, &H4C,
             &H90, &H90, &H90, &H7C, &H44, &H64,
             &H54, &H4C, &H44, &H0, &H8, &H36,
             &H41, &H0, &H0, &H0, &H77, &H0,
             &H0, &H0, &H41, &H36, &H8, &H0,
             &H2, &H1, &H2, &H4, &H2, &H3C,
             &H26, &H23, &H26, &H3C, &H1E, &HA1,
             &HA1, &H61, &H12, &H3A, &H40, &H40,
             &H20, &H7A, &H38, &H54, &H54, &H55,
             &H59, &H21, &H55, &H55, &H79, &H41,
             &H21, &H54, &H54, &H78, &H41, &H21,
             &H55, &H54, &H78, &H40, &H20, &H54,
             &H55, &H79, &H40, &HC, &H1E, &H52,
             &H72, &H12, &H39, &H55, &H55, &H55,
             &H59, &H39, &H54, &H54, &H54, &H59,
             &H39, &H55, &H54, &H54, &H58, &H0,
             &H0, &H45, &H7C, &H41, &H0, &H2,
             &H45, &H7D, &H42, &H0, &H1, &H45,
             &H7C, &H40, &HF0, &H29, &H24, &H29,
             &HF0, &HF0, &H28, &H25, &H28, &HF0,
             &H7C, &H54, &H55, &H45, &H0, &H20,
             &H54, &H54, &H7C, &H54, &H7C, &HA,
             &H9, &H7F, &H49, &H32, &H49, &H49,
             &H49, &H32, &H32, &H48, &H48, &H48,
             &H32, &H32, &H4A, &H48, &H48, &H30,
             &H3A, &H41, &H41, &H21, &H7A, &H3A,
             &H42, &H40, &H20, &H78, &H0, &H9D,
             &HA0, &HA0, &H7D, &H39, &H44, &H44,
             &H44, &H39, &H3D, &H40, &H40, &H40,
             &H3D, &H3C, &H24, &HFF, &H24, &H24,
             &H48, &H7E, &H49, &H43, &H66, &H2B,
             &H2F, &HFC, &H2F, &H2B, &HFF, &H9,
             &H29, &HF6, &H20, &HC0, &H88, &H7E,
             &H9, &H3, &H20, &H54, &H54, &H79,
             &H41, &H0, &H0, &H44, &H7D, &H41,
             &H30, &H48, &H48, &H4A, &H32, &H38,
             &H40, &H40, &H22, &H7A, &H0, &H7A,
             &HA, &HA, &H72, &H7D, &HD, &H19,
             &H31, &H7D, &H26, &H29, &H29, &H2F,
             &H28, &H26, &H29, &H29, &H29, &H26,
             &H30, &H48, &H4D, &H40, &H20, &H38,
             &H8, &H8, &H8, &H8, &H8, &H8,
             &H8, &H8, &H38, &H2F, &H10, &HC8,
             &HAC, &HBA, &H2F, &H10, &H28, &H34,
             &HFA, &H0, &H0, &H7B, &H0, &H0,
             &H8, &H14, &H2A, &H14, &H22, &H22,
             &H14, &H2A, &H14, &H8, &HAA, &H0,
             &H55, &H0, &HAA, &HAA, &H55, &HAA,
             &H55, &HAA, &H0, &H0, &H0, &HFF,
             &H0, &H10, &H10, &H10, &HFF, &H0,
             &H14, &H14, &H14, &HFF, &H0, &H10,
             &H10, &HFF, &H0, &HFF, &H10, &H10,
             &HF0, &H10, &HF0, &H14, &H14, &H14,
             &HFC, &H0, &H14, &H14, &HF7, &H0,
             &HFF, &H0, &H0, &HFF, &H0, &HFF,
             &H14, &H14, &HF4, &H4, &HFC, &H14,
             &H14, &H17, &H10, &H1F, &H10, &H10,
             &H1F, &H10, &H1F, &H14, &H14, &H14,
             &H1F, &H0, &H10, &H10, &H10, &HF0,
             &H0, &H0, &H0, &H0, &H1F, &H10,
             &H10, &H10, &H10, &H1F, &H10, &H10,
             &H10, &H10, &HF0, &H10, &H0, &H0,
             &H0, &HFF, &H10, &H10, &H10, &H10,
             &H10, &H10, &H10, &H10, &H10, &HFF,
             &H10, &H0, &H0, &H0, &HFF, &H14,
             &H0, &H0, &HFF, &H0, &HFF, &H0,
             &H0, &H1F, &H10, &H17, &H0, &H0,
             &HFC, &H4, &HF4, &H14, &H14, &H17,
             &H10, &H17, &H14, &H14, &HF4, &H4,
             &HF4, &H0, &H0, &HFF, &H0, &HF7,
             &H14, &H14, &H14, &H14, &H14, &H14,
             &H14, &HF7, &H0, &HF7, &H14, &H14,
             &H14, &H17, &H14, &H10, &H10, &H1F,
             &H10, &H1F, &H14, &H14, &H14, &HF4,
             &H14, &H10, &H10, &HF0, &H10, &HF0,
             &H0, &H0, &H1F, &H10, &H1F, &H0,
             &H0, &H0, &H1F, &H14, &H0, &H0,
             &H0, &HFC, &H14, &H0, &H0, &HF0,
             &H10, &HF0, &H10, &H10, &HFF, &H10,
             &HFF, &H14, &H14, &H14, &HFF, &H14,
             &H10, &H10, &H10, &H1F, &H0, &H0,
             &H0, &H0, &HF0, &H10, &HFF, &HFF,
             &HFF, &HFF, &HFF, &HF0, &HF0, &HF0,
             &HF0, &HF0, &HFF, &HFF, &HFF, &H0,
             &H0, &H0, &H0, &H0, &HFF, &HFF,
             &HF, &HF, &HF, &HF, &HF, &H38,
             &H44, &H44, &H38, &H44, &H7C, &H2A,
             &H2A, &H3E, &H14, &H7E, &H2, &H2,
             &H6, &H6, &H2, &H7E, &H2, &H7E,
             &H2, &H63, &H55, &H49, &H41, &H63,
             &H38, &H44, &H44, &H3C, &H4, &H40,
             &H7E, &H20, &H1E, &H20, &H6, &H2,
             &H7E, &H2, &H2, &H99, &HA5, &HE7,
             &HA5, &H99, &H1C, &H2A, &H49, &H2A,
             &H1C, &H4C, &H72, &H1, &H72, &H4C,
             &H30, &H4A, &H4D, &H4D, &H30, &H30,
             &H48, &H78, &H48, &H30, &HBC, &H62,
             &H5A, &H46, &H3D, &H3E, &H49, &H49,
             &H49, &H0, &H7E, &H1, &H1, &H1,
             &H7E, &H2A, &H2A, &H2A, &H2A, &H2A,
             &H44, &H44, &H5F, &H44, &H44, &H40,
             &H51, &H4A, &H44, &H40, &H40, &H44,
             &H4A, &H51, &H40, &H0, &H0, &HFF,
             &H1, &H3, &HE0, &H80, &HFF, &H0,
             &H0, &H8, &H8, &H6B, &H6B, &H8,
             &H36, &H12, &H36, &H24, &H36, &H6,
             &HF, &H9, &HF, &H6, &H0, &H0,
             &H18, &H18, &H0, &H0, &H0, &H10,
             &H10, &H0, &H30, &H40, &HFF, &H1,
             &H1, &H0, &H1F, &H1, &H1, &H1E,
             &H0, &H19, &H1D, &H17, &H12, &H0,
             &H3C, &H3C, &H3C, &H3C, &H0, &H0,
             &H0, &H0, &H0}
End Class