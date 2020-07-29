using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Subcommand : Command
{
	public byte[] DefaultBytes = { 0x0, 0x1, 0x40, 0x40, 0x0, 0x1, 0x40, 0x40 };
	
	public Subcommand(SubcommandType type)
	{
		// Write header
		this.PackByte((byte)CommandType.SUBCOMMAND);
		this.PackByte(GlobalCount);

		this.PackBytes(DefaultBytes);
		this.PackByte((byte)type);
	}
}
