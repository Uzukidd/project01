using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SPIReportReader
{
	public static StickCalibration ReadStickCalibration(Report report, JoyConType type)
	{
		report.Seek(20);

		var cal = new StickCalibration();

		var temp = new ushort[2];
		if (type == JoyConType.LEFT)
		{
			temp = report.Read2UInt12();
			cal.xMax = temp[0];
			cal.yMax = temp[1];
			temp = report.Read2UInt12();
			cal.xCenter = temp[0];
			cal.yCenter = temp[1];
			temp = report.Read2UInt12();
			cal.xMin = temp[0];
			cal.xMin = temp[1];
		}
		else
		{
			temp = report.Read2UInt12();
			cal.xCenter = temp[0];
			cal.yCenter = temp[1];
			temp = report.Read2UInt12();
			cal.xMin = temp[0];
			cal.xMin = temp[1];
			temp = report.Read2UInt12();
			cal.xMax = temp[0];
			cal.yMax = temp[1];
		}

		//cal.xMin = 0x0;
		//cal.xCenter = 0x7ff;
		//cal.xMax = 0xfff;
		//cal.yMin = 0x0;
		//cal.yCenter = 0x7ff;
		//cal.yMax = 0xfff;

		return cal;
	}

	public static StickParameter ReadStickParameter(Report report, JoyConType type)
	{
		report.Seek(20);

		var para = new StickParameter();

		report.Read2UInt12();
		var temp = report.Read2UInt12();
		para.deadzone = temp[0];
		para.rangeRatio = temp[1];

		return para;
	}
	
	public static SixAxisCalibration ReadSixAxisCalibration(Report report)
	{
		report.Seek(20);

		var cal = new SixAxisCalibration();
		
		cal.accOrigin = new Vector3(report.ReadInt16(), report.ReadInt16(), report.ReadInt16());
		cal.accCoeff = new Vector3(report.ReadInt16(), report.ReadInt16(), report.ReadInt16());
		cal.gyroOrigin = new Vector3(report.ReadInt16(), report.ReadInt16(), report.ReadInt16());
		cal.gyroCoeff = new Vector3(report.ReadInt16(), report.ReadInt16(), report.ReadInt16());

		return cal;
	}
}
