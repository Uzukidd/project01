using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct BitsByte
{
	public byte value;

	public static implicit operator BitsByte(byte b)
	{
		return new BitsByte
		{
			value = b
		};
	}

	public bool this[int key]
	{
		get
		{
			return ((int)this.value & 1 << key) != 0;
		}
		set
		{
			if (value)
			{
				this.value |= (byte)(1 << key);
				return;
			}
			this.value &= (byte)(~(byte)(1 << key));
		}
	}
}
