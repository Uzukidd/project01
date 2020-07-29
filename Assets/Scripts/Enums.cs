using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum JoyConType
{
	LEFT,
	RIGHT,
	NONE
}

public enum CommandType : byte
{
	SUBCOMMAND = 0x1,
	RUMBLE = 0x10
}

public enum SubcommandType : byte
{
	GetOnlyControllerState = 0x00,
	BluetoothManualPairing = 0x01,
	RequestDeviceInfo = 0x02,
	SetInputReportMode = 0x03,
	TriggerButtonsElapsedTime = 0x04,
	GetPageListState = 0x05,
	SetHCIState = 0x06,
	ResetPairingInfo = 0x07,
	SetShipmentLowPowerState = 0x08,
	SPIFlashRead = 0x10,
	SPIFlashWrite = 0x11,
	SPISectorErase = 0x12,
	ResetNFCOrIRMCU = 0x20,
	SetNFCOrIRMCUConfiguration = 0x21,
	SetNFCOrIRMCUState = 0x22,
	SetPlayerLights = 0x30,
	EnableIMU = 0x40,
	SetIMUSensitivity = 0x41,
	EnableVibration = 0x48
}

public enum DebugType : int
{
	NONE,
	ALL,
	COMMS,
	THREADING,
	IMU,
	RUMBLE,
};

public enum State : uint
{
	NOT_ATTACHED,
	DROPPED,
	NO_JOYCONS,
	ATTACHED,
	INPUT_MODE_0x30,
	IMU_DATA_OK,
};

public enum JoyConButton : int
{
	Y = 0,
	X = 1,
	B = 2,
	A = 3,
	RSR = 4,
	RSL = 5,
	R = 6,
	ZR = 7,
	Minus = 8,
	Plus = 9,
	RStick = 10,
	LStick = 11,
	Home = 12,
	Capture = 13,
	ChargingGrip = 14,
	Down = 15,
	Up = 16,
	Right = 17,
	Left = 18,
	LSR = 19,
	LSL = 20,
	L = 21,
	ZL = 22,
};

public enum MCUMode : byte
{
	Standby = 0,
	NFC = 4,
	IR = 5
}
