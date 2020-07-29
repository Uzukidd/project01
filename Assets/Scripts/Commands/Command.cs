using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

// TODO: Create subclasses for various commands
public class Command
{
	public const int ReportLen = JoyCon.ReportLen;

	public static byte GlobalCount;

	private MemoryStream memoryStream;
	private BinaryWriter writer;

	public Command()
	{
		memoryStream = new MemoryStream();
		writer = new BinaryWriter(memoryStream);
	}

	public byte[] Send(IntPtr handle)
	{
		var data = this.GetByteData();
		HIDapi.hid_write(handle, data, new UIntPtr((uint)data.Length));
		//Debug.Log("Send Data: " + Helper.ByteArrayToString(data));

		if (GlobalCount == 0xF)
			GlobalCount = 0;
		else
			GlobalCount++;

		var responseData = new byte[ReportLen];
		int result = 0;
		int count = 0;
		while (result != 1)
		{
			result = HIDapi.hid_read_timeout(handle, responseData, new UIntPtr((uint)ReportLen), 50);

			// TODO: Check error type
			count++;
			if(count > 8)
				break;
		}
		if (result < 1)
		{
			//Debug.Log("No response.");
		}
		else
		{
			//Debug.Log("Recieve Data: " + Helper.ByteArrayToString(responseData));
		}
		return responseData;
	}

	public Command PackByte(byte num)
	{
		writer.Write(num);
		return this;
	}

	public Command PackBytes(byte[] num)
	{
		writer.Write(num);
		return this;
	}

	public Command PackBool(bool num)
	{
		writer.Write(num);
		return this;
	}

	public Command PackInt16(short num)
	{
		writer.Write(num);
		return this;
	}

	public Command PackUInt16(ushort num)
	{
		writer.Write(num);
		return this;
	}

	public Command PackInt32(int num)
	{
		writer.Write(num);
		return this;
	}

	public Command PackUInt32(uint num)
	{
		writer.Write(num);
		return this;
	}

	public Command PackUInt64(ulong num)
	{
		writer.Write(num);
		return this;
	}

	public Command PackSingle(float num)
	{
		writer.Write(num);
		return this;
	}

	public Command PackString(string str)
	{
		writer.Write(str);
		return this;
	}
	
	public byte[] GetByteData()
	{
		return memoryStream.ToArray().Take(ReportLen).ToArray();
	}
}
