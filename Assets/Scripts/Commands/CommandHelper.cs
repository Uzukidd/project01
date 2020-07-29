using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CommandHelper
{
	public static Report ReadSPI(int addr, byte size, IntPtr handle)
	{
		var sc = new Subcommand(SubcommandType.SPIFlashRead)
			.PackInt32(addr)
			.PackByte(size);

		var report = new Report(sc.Send(handle), DateTime.Now);

		report.Seek(15);
		if (report.ReadInt32() != addr || report.ReadByte() != size)
		{
			Debug.Log(string.Format("Invalid SPI report: {0}", Helper.ByteArrayToString(report.dataBuffer)));
			return null;
		}

		return report;
	}

	public static readonly int[] StickUserCalibrationAddr = new int[2] {0x8012, 0x801d};
	public static readonly int[] StickFactoryCalibrationAddr = new int[2] { 0x603d, 0x6046 };
	public static Report GetStickCalibration(JoyConType type, IntPtr handle)
	{
		int calAddr = StickUserCalibrationAddr[(int)type];
		var report = ReadSPI(calAddr, 9, handle);

		if (!report.CheckEmpty(9))
		{
			Debug.Log("Use user stick calibration.");
			return report;
		}

		calAddr = StickFactoryCalibrationAddr[(int) type];
		report = ReadSPI(calAddr, 9, handle);

		if (!report.CheckEmpty(9))
		{
			Debug.Log("Use factory stick calibration.");
			return report;
		}
		
		Debug.LogError("No stick calibration.");
		return null;
	}

	public static readonly int[] StickParameterAddr = new int[2] { 0x6086, 0x6098 };
	public static Report GetStickParameters(JoyConType type, IntPtr handle)
	{
		int paraAddr = StickParameterAddr[(int) type];
		var report = ReadSPI(paraAddr, 16, handle);

		return report;
	}

	public static readonly int SixAxisUserCalibrationAddr = 0x8028;
	public static readonly int SixAxisFactoryCalibrationAddr = 0x6020;
	public static Report GetSixAxisCalibration(IntPtr handle)
	{
		int calAddr = SixAxisUserCalibrationAddr;
		var report = ReadSPI(calAddr, 24, handle);

		if (!report.CheckEmpty(24))
		{
			Debug.Log("Use user 6-axis calibration.");
			return report;
		}

		calAddr = SixAxisFactoryCalibrationAddr;
		report = ReadSPI(calAddr, 24, handle);

		if (!report.CheckEmpty(24))
		{
			Debug.Log("Use factory 6-axis calibration.");
			return report;
		}

		Debug.LogError("No 6-axis calibration.");
		return null;
	}

	public static void RequestMCUModeStatus(IntPtr handle)
	{
		bool mcuReady = false;
		while (!mcuReady)
		{
			var result = new Subcommand(SubcommandType.BluetoothManualPairing).Send(handle);
			if (result[0] == 0x31)
			{
				//if (result[49] == 0x01 && result[56] == 0x06) // MCU state is Initializing
				// *(u16*)result[52]LE x04 in lower than 3.89fw, x05 in 3.89
				// *(u16*)result[54]LE x12 in lower than 3.89fw, x18 in 3.89
				// result[56]: mcu mode state
				if (result[49] == 0x01 && result[56] == 0x01) // MCU state is Standby
					mcuReady = true;
			}
		}
	}

	public static void SetMCUMode(byte mode, IntPtr handle)
	{
		bool mcuReady = false;
		while (!mcuReady)
		{
			var result = new Subcommand(SubcommandType.SetNFCOrIRMCUConfiguration)
								.PackByte(0x21)
								.PackByte(0x00)
								.PackByte(mode).Send(handle);
			if (result[0] == 0x21)
			{
				if (result[15] == 0x01 && result[22] == 0x01) // MCU state is Standby
					mcuReady = true;
			}
		}
	}
}