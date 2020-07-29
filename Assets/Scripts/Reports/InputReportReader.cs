using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class InputReportReader
{
	public const int ButtonCount = 23;

	public static void Read(Report report, JoyCon jc)
	{
		report.Reader.BaseStream.Position = 1L;
	}


	//Standard input report - buttons
	//
	//Y       X      B          A         RSR     RSL       R    ZR
	//Minus   Plus   R Stick    L Stick   Home    Capture 	--   Charging Grip
	//Down    Up     Right      Left      LSR     SL        L    ZL
	public static void ReadButtons(Report report, bool[] buttons)
	{
		report.Seek(3);

		var rightStatus = (BitsByte)report.ReadByte();
		var sharedStatus = (BitsByte)report.ReadByte();
		var leftStatus = (BitsByte)report.ReadByte();
		// Right
		buttons[(int)JoyConButton.Y] = rightStatus[0];
		buttons[(int)JoyConButton.X] = rightStatus[1];
		buttons[(int)JoyConButton.B] = rightStatus[2];
		buttons[(int)JoyConButton.A] = rightStatus[3];
		buttons[(int)JoyConButton.RSR] = rightStatus[4];
		buttons[(int)JoyConButton.RSL] = rightStatus[5];
		buttons[(int)JoyConButton.R] = rightStatus[6];
		buttons[(int)JoyConButton.ZR] = rightStatus[7];
		// Shared
		buttons[(int)JoyConButton.Minus] = sharedStatus[0];
		buttons[(int)JoyConButton.Plus] = sharedStatus[1];
		buttons[(int)JoyConButton.RStick] = sharedStatus[2];
		buttons[(int)JoyConButton.LStick] = sharedStatus[3];
		buttons[(int)JoyConButton.Home] = sharedStatus[4];
		buttons[(int)JoyConButton.Capture] = sharedStatus[5];
		buttons[(int)JoyConButton.ChargingGrip] = sharedStatus[7];
		// Left
		buttons[(int)JoyConButton.Down] = leftStatus[0];
		buttons[(int)JoyConButton.Up] = leftStatus[1];
		buttons[(int)JoyConButton.Right] = leftStatus[2];
		buttons[(int)JoyConButton.Left] = leftStatus[3];
		buttons[(int)JoyConButton.LSR] = leftStatus[4];
		buttons[(int)JoyConButton.LSL] = leftStatus[5];
		buttons[(int)JoyConButton.L] = leftStatus[6];
		buttons[(int)JoyConButton.ZL] = leftStatus[7];
	}

	public static Vector2 ReadSticks(Report report, JoyCon jc)
	{
		if (jc.Type == JoyConType.LEFT)
		{
			report.Seek(6);
		}
		else
		{
			report.Seek(9);
		}

		var temp = report.Read2UInt12();
		return jc.CenterSticks(new Vector2(temp[0], temp[1]));
	}

	public static Vector3[] ReadAcc(Report report, JoyCon jc)
	{
		var result = new Vector3[3];

		for (int i = 0; i < 3; i++)
		{
			report.Seek(13 + i * 12);
			var vec = report.ReadVector3Int16();
			vec.Scale(jc.accCalCoeff);

			if (jc.Type == JoyConType.LEFT)
			{
				result[i] = new Vector3(vec.y, -vec.x, vec.z);
			}
			else
			{
				result[i] = new Vector3(vec.y, vec.x, -vec.z);
			}
		}

		return result;
	}

	public static Vector3[] ReadGyro(Report report, JoyCon jc)
	{
		var result = new Vector3[3];

		for (int i = 0; i < 3; i++)
		{
			report.Seek(19 + i * 12);
			var vec = report.ReadVector3Int16();
			vec -= jc.sixAxisCal.gyroOrigin;
			vec.Scale(jc.gyroCalCoeff);
			
			if (jc.Type == JoyConType.LEFT)
			{
				result[i] = new Vector3(vec.y, -vec.x, vec.z);
			}
			else
			{
				result[i] = new Vector3(-vec.y, -vec.x, -vec.z);
			}
		}

		return result;
	}
}
