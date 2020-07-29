using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class Report
{
	public BinaryReader Reader
	{
		get
		{
			if (_reader == null)
			{
				var stream = new MemoryStream(dataBuffer);
				_reader = new BinaryReader(stream);
			}
			return _reader;
		}
	}

	public DateTime timeStamp;
	public byte[] dataBuffer;

	private BinaryReader _reader;

	public Report(byte[] dataBuffer, DateTime time)
	{
		this.dataBuffer = dataBuffer;
		this.timeStamp = time;
		this._reader = new BinaryReader(new MemoryStream(dataBuffer));
	}
	
	public bool CheckEmpty(int len)
	{
		return CheckEmpty((int)_reader.BaseStream.Position, len);
	}

	public bool CheckEmpty(int start, int len)
	{
		for (int i = start; i < start + len; i++)
		{
			if (dataBuffer[i] != 0xFF)
				return false;
		}

		return true;
	}

	public void Seek(long i)
	{
		_reader.BaseStream.Position = i;
	}

	public byte ReadByte()
	{
		return _reader.ReadByte();
	}
	
	public byte ReadByte(int i)
	{
		return dataBuffer[i];
	}

	public byte[] ReadBytes(int count)
	{
		return _reader.ReadBytes(count);
	}

	public int ReadInt32()
	{
		return _reader.ReadInt32();
	}

	public uint ReadUInt32()
	{
		return _reader.ReadUInt32();
	}

	public short ReadInt16()
	{
		return _reader.ReadInt16();
	}

	public ushort ReadUInt16()
	{
		return _reader.ReadUInt16();
	}

	public ushort[] Read2UInt12()
	{
		int b1 = ReadByte();
		int b2 = ReadByte();
		int b3 = ReadByte();
		
		return new ushort[] {(ushort) ((b2 << 8) & 0xF00 | b1),
							(ushort) ((b3 << 4) | (b2 >> 4))};
	}

	public Vector3 ReadVector3Int16()
	{
		return new Vector3(ReadInt16(),
							ReadInt16(),
							ReadInt16());
	}
};
