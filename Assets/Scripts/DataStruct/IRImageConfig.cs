using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct IRImageConfig
{
	public byte irResReg;
	public ushort irExposure;
	public byte irLeds; // Leds to enable, Strobe/Flashlight modes
	public ushort irLedsIntensity; // MSByte: Leds 1/2, LSB: Leds 3/4
	public byte irDigitalGain;
	public byte irExLightFilter;
	public uint irCustomRegister; // MSByte: Enable/Disable, Middle Byte: Edge smoothing, LSB: Color interpolation
	public ushort irBufferUpdateTime;
	public byte irHandAnalysisMode;
	public byte irHandAnalysisThreshold;
	public uint irDenoise; // MSByte: Enable/Disable, Middle Byte: Edge smoothing, LSB: Color interpolation
	public byte irFlip;
};