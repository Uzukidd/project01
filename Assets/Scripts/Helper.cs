using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Helper
{
	public static string ByteArrayToString(byte[] ba)
	{
		StringBuilder hex = new StringBuilder(ba.Length * 2);
		foreach (byte b in ba)
			hex.AppendFormat("{0:x2} ", b);
		return hex.ToString();
	}
	
	public static string ArrayToString(Array sa)
	{
		StringBuilder stringBuilder = new StringBuilder(sa.Length * 2);
		foreach (byte s in sa)
			stringBuilder.AppendFormat("{0} ", s);
		return stringBuilder.ToString();
	}
}
