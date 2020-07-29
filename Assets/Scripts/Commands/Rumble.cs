using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Rumble : Command
{
	public float highFreq;
	public float amplitude;
	public float lowFreq;
	public float time;
	public bool hasTime;

	public Rumble()
	{
		// Write header
		this.PackByte((byte)CommandType.RUMBLE);
		this.PackByte(GlobalCount);
	}

	public Rumble SetVals(float lowFreq, float highFreq, float amplitude, int time = 0)
	{
		this.highFreq = highFreq;
		this.amplitude = amplitude;
		this.lowFreq = lowFreq;
		this.time = 0;
		this.hasTime = false;
		if (time != 0)
		{
			this.time = time / 1000f;
			hasTime = true;
		}

		this.PackData();

		return this;
	}

	// TODO: Clean
	public void PackData()
	{
		byte[] rumbleData = new byte[8];
		if (amplitude == 0.0f)
		{
			rumbleData[0] = 0x0;
			rumbleData[1] = 0x1;
			rumbleData[2] = 0x40;
			rumbleData[3] = 0x40;
		}
		else
		{
			lowFreq = Mathf.Clamp(lowFreq, 40.875885f, 626.286133f);
			amplitude = Mathf.Clamp(amplitude, 0.0f, 1.0f);
			highFreq = Mathf.Clamp(highFreq, 81.75177f, 1252.572266f);

			UInt16 hf = (UInt16)((Mathf.Round(32f * Mathf.Log(highFreq * 0.1f, 2)) - 0x60) * 4);
			byte lf = (byte)(Mathf.Round(32f * Mathf.Log(lowFreq * 0.1f, 2)) - 0x40);
			byte hfAmp;
			if (amplitude == 0)
			{
				hfAmp = 0;
			}
			else if (amplitude < 0.117)
			{
				hfAmp = (byte)((((Mathf.Log(amplitude * 1000, 2) * 32) - 0x60) / (5 - Mathf.Pow(amplitude, 2))) - 1);
			}
			else if (amplitude < 0.23)
			{
				hfAmp = (byte)(((Mathf.Log(amplitude * 1000, 2) * 32) - 0x60) - 0x5c);
			}
			else
			{
				hfAmp = (byte)((((Mathf.Log(amplitude * 1000, 2) * 32) - 0x60) * 2) - 0xf6);
			}

			UInt16 lfAmp = (UInt16)(Mathf.Round(hfAmp) * .5);
			byte parity = (byte)(lfAmp % 2);
			if (parity > 0)
			{
				--lfAmp;
			}

			lfAmp = (UInt16)(lfAmp >> 1);
			lfAmp += 0x40;
			if (parity > 0)
			{
				lfAmp |= 0x8000;
			}

			rumbleData = new byte[8];
			rumbleData[0] = (byte)(hf & 0xff);
			rumbleData[1] = (byte)((hf >> 8) & 0xff);
			rumbleData[2] = lf;
			rumbleData[1] += hfAmp;
			rumbleData[2] += (byte)((lfAmp >> 8) & 0xff);
			rumbleData[3] += (byte)(lfAmp & 0xff);
		}
		for (int i = 0; i < 4; ++i)
		{
			rumbleData[4 + i] = rumbleData[i];
		}

		//Debug.Log(string.Format("Encoded hex freq: {0:X2}", encoded_hex_freq));
		//Debug.Log(string.Format("lf_amp: {0:X4}", lf_amp));
		//Debug.Log(string.Format("hf_amp: {0:X2}", hf_amp));
		//Debug.Log(string.Format("lowFreq: {0:F}", lowFreq));
		//Debug.Log(string.Format("hf: {0:X4}", hf));
		//Debug.Log(string.Format("lf: {0:X2}", lf));

		this.PackBytes(rumbleData);
	}
}
