using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class JoyCon
{
	public const int ReportLen = 49;

	public JoyConType Type { get; private set; }
	public bool IMU { get; private set; }
	public State State { get; private set; }

	private IntPtr handle;

	#region Button & Stick
	private bool[] buttonsDown = new bool[23];
	private bool[] buttonsUp = new bool[23];
	private bool[] buttons = new bool[23];
	private bool[] _down = new bool[23];

    public bool reset = false;

	public bool next = false;

	private StickCalibration stickCal;
	private StickParameter stickPara;
	float deadZone;
	private Vector2 stickPrecal;

	public Vector2 stick;
	#endregion

	#region IMU
	public Vector3 accCalCoeff;
	public Vector3 gyroCalCoeff;
	public SixAxisCalibration sixAxisCal;
	
	public Vector3[] AccDelta;
	public Vector3[] GyroDelta;

	public Vector3 Acc;
	public Vector3 Gyro;
	#endregion

	#region IR 
	
	#endregion

	private Thread PollThreadObj;
	private bool end;
	private Queue<Report> reports;

	public JoyCon(JoyConType type, IntPtr handle, bool imu)
	{
		this.Type = type;
		this.handle = handle;
		this.IMU = imu;

		this.State = State.NOT_ATTACHED;
		reports = new Queue<Report>();
	}
	
	public void Attach(byte leds = 0x0)
	{
		// Input report mode
		new Subcommand(SubcommandType.SetInputReportMode).PackByte(0x3f).Send(handle);
		GetCalibrationData();
		
		// Connect
		new Subcommand(SubcommandType.BluetoothManualPairing).PackByte(0x01).Send(handle);
		new Subcommand(SubcommandType.BluetoothManualPairing).PackByte(0x02).Send(handle);
		new Subcommand(SubcommandType.BluetoothManualPairing).PackByte(0x03).Send(handle);

		new Subcommand(SubcommandType.SetPlayerLights).PackByte(leds).Send(handle);
		new Subcommand(SubcommandType.EnableIMU).PackBool(IMU).Send(handle);
		new Subcommand(SubcommandType.SetInputReportMode).PackByte(0x30).Send(handle);
		new Subcommand(SubcommandType.EnableVibration).PackBool(true).Send(handle);

		Debug.Log("Done with init.");
		State = State.ATTACHED;
	}

	public void Detach()
	{
		end = true;

		if (State > State.NO_JOYCONS)
		{
			new Subcommand(SubcommandType.SetPlayerLights).PackByte(0x0).Send(handle);
			new Subcommand(SubcommandType.EnableIMU).PackBool(false).Send(handle);
			new Subcommand(SubcommandType.EnableVibration).PackBool(false).Send(handle);
			new Subcommand(SubcommandType.SetInputReportMode).PackByte(0x3f).Send(handle);
		}

		if (State > State.DROPPED)
		{
			HIDapi.hid_close(handle);
		}

		State = State.NOT_ATTACHED;

		//Debug.Log("Detached.");
	}

	public void EnableIR(IRImageConfig config)
	{
		// Set input report to x31
		new Subcommand(SubcommandType.SetInputReportMode).PackByte(0x31).Send(handle);
		// Enable MCU
		new Subcommand(SubcommandType.SetNFCOrIRMCUState).PackByte(0x01).Send(handle);
		// Request MCU mode status
		CommandHelper.RequestMCUModeStatus(handle);
		// Set MCU mode
		CommandHelper.SetMCUMode((byte)MCUMode.IR, handle);
		// Request MCU mode status
		CommandHelper.RequestMCUModeStatus(handle);
		// Set IR mode and number of packets for each data blob. Blob size is packets * 300 bytes.

		// Write to registers for the selected IR mode

		// Write to registers for the selected IR mode

		// Stream or Capture images from NIR Camera

		// Disable MCU

		// Set input report back to x3f

	}

	// TODO: Remove Thread
	public void Begin()
	{
		if (PollThreadObj == null)
		{
			PollThreadObj = new Thread(Poll);
			PollThreadObj.Start();
		}
	}

	public void Loop()
	{
		if (State > State.NO_JOYCONS)
		{
			while (reports.Count > 0)
			{
				Report report;
				lock (reports)
				{
					report = reports.Dequeue();
				}
				ProcessReport(report);
			}
		}
	}

	#region Calibration Data
	private void GetCalibrationData()
	{
		try
		{
			// Get stick's calibration infomation
			var report = CommandHelper.GetStickCalibration(this.Type, this.handle);
			this.stickCal = SPIReportReader.ReadStickCalibration(report, this.Type);
			Debug.Log(string.Format(
				"Stick calibration data: xMax:{0} xCenter:{1} xMin:{2} yMax:{3} yCenter:{4} yMin:{5} ",
				stickCal.xMax, stickCal.xCenter, stickCal.xMin, stickCal.yMax, stickCal.yCenter, stickCal.yMin));

		}
		catch (Exception ex)
		{
			SetDefaultStickCalibratrion();

		}

		try
		{
			// Get stick's parameter infomation, mainly deadzone
			var report = CommandHelper.GetStickParameters(Type, handle);
			this.stickPara = SPIReportReader.ReadStickParameter(report, Type);
			this.deadZone = stickPara.deadzone;
			Debug.Log(string.Format("Deadzone: {0}", deadZone));
		}
		catch (Exception ex)
		{
			SetDefaultStickParameter();
			Debug.LogError(ex);
		}

		try
		{
			// Get 6-axis sensor's calibration infomation
			var report = CommandHelper.GetSixAxisCalibration(handle);
			sixAxisCal = SPIReportReader.ReadSixAxisCalibration(report);
			for (int i = 0; i < 3; i++)
			{
				accCalCoeff[i] = (float)(1.0 / (sixAxisCal.accCoeff[i] - sixAxisCal.accOrigin[i])) * 4.0f * 9.8f;
				gyroCalCoeff[i] = (float)((sixAxisCal.gyroCoeff[i] / 10) / (sixAxisCal.gyroCoeff[i] - sixAxisCal.gyroOrigin[i]));
			}
			//Debug.Log(string.Format("6-Axis calibration data: accOrigin:{0} accCoeff:{1} gyroOrigin:{2} gyroCoeff:{3} ",
				//sixAxisCal.accOrigin, sixAxisCal.accCoeff, sixAxisCal.gyroOrigin, sixAxisCal.gyroCoeff));
		}
		catch (Exception ex)
		{
			SetDefaultSixAxisCalibratrion();
			Debug.LogError(ex);
		}
	}

	private void SetDefaultStickCalibratrion()
	{
		this.stickCal = new StickCalibration();
		stickCal.xCenter = 0x7FFF;
		stickCal.xMin = 0x0000;
		stickCal.xMax = 0xFFFF;
		stickCal.yCenter = 0x7FFF;
		stickCal.yMin = 0x0000;
		stickCal.yMax = 0xFFFF;
	}
	private void SetDefaultStickParameter()
	{
		this.stickPara = new StickParameter();
		stickPara.deadzone = 0xAE;
	}
	private void SetDefaultSixAxisCalibratrion()
	{
		this.sixAxisCal = new SixAxisCalibration();

	}
	#endregion

	#region Message
	private void Poll()
	{
		int attempts = 0;
		while (!end && State > State.NO_JOYCONS)
		{
			int a = ReceiveRaw();
			if (a > 0)
			{
				State = State.IMU_DATA_OK;
				attempts = 0;
			}
			else if (attempts > 1000)
			{
				State = State.DROPPED;
				Debug.Log("Connection lost. Is the Joy-Con connected?");
				break;
			}
			else
			{
				Thread.Sleep(5);
			}
			++attempts;
		}
		Debug.Log("End poll loop.");
	}

	private int ReceiveRaw()
	{
		if (handle == IntPtr.Zero)
			return -2;

		HIDapi.hid_set_nonblocking(handle, 0);
		byte[] rawBuf = new byte[ReportLen];
		int ret = HIDapi.hid_read(handle, rawBuf, new UIntPtr(ReportLen));
		if (ret > 0)
		{
			lock (reports)
			{
				reports.Enqueue(new Report(rawBuf, DateTime.Now));
			}
		}
		return ret;
	}

	private void ProcessReport(Report report)
	{
		ProcessButtonsAndStick(report);

		if (IMU && State >= State.IMU_DATA_OK)
		{
			ProcessIMU(report);
		}
	}
	#endregion

	#region Button&Stick Process
	private void ProcessButtonsAndStick(Report report)
	{
		// Read sticks
		this.stick = InputReportReader.ReadSticks(report, this);

		// Read buttons
		lock (buttons)
		{
			lock (_down)
			{
				for (int i = 0; i < buttons.Length; ++i)
				{
					_down[i] = buttons[i];
				}
			}

			InputReportReader.ReadButtons(report, buttons);

			lock (buttonsUp)
			{
				lock (buttonsDown)
				{
					for (int i = 0; i < buttons.Length; ++i)
					{
						buttonsUp[i] = (_down[i] & !buttons[i]);
						buttonsDown[i] = (!_down[i] & buttons[i]);
						if (buttonsDown[i])
                        {
                            if(i == (int)JoyConButton.L || i == (int)JoyConButton.R)
                            {
                                reset = true;
                            } else if(i == (int)JoyConButton.ZL || i == (int)JoyConButton.ZR)
							{

								next = true;
							
							}
                        }
							//Debug.Log(string.Format("Button {0} down.", (JoyConButton)i));
						if (buttonsUp[i])
                        {
                            if (i == (int)JoyConButton.L || i == (int)JoyConButton.R)
                            {
                                reset = false;
                            } else if(i == (int)JoyConButton.ZL || i == (int)JoyConButton.ZR)
							{

								next = false;

							}


                        }
							//Debug.Log(string.Format("Button {0} up.", (JoyConButton)i));
					}
				}
			}
		}
	}

	public Vector2 CenterSticks(Vector2 stick)
	{
		Vector2 vec;
		// Apply Joy-Con center deadzone. 0xAE translates approx to 15%. Pro controller has a 10% deadzone.
		float deadZoneCenter = 0.15f;
		// Add a small ammount of outer deadzone to avoid edge cases or machine variety.
		float deadZoneOuter = 0.10f;

		vec.x = -(stick.x - stickCal.xCenter) / (stickCal.xMax - stickCal.xCenter);
		vec.y = -(stick.y - stickCal.yCenter) / (stickCal.yMax - stickCal.yCenter);

		// Interpolate zone between deadzones
		float mag = vec.magnitude;
		if (mag > deadZoneCenter)
		{
			// scale such that output magnitude is in the range [0.0f, 1.0f]
			float legalRange = 1.0f - deadZoneOuter - deadZoneCenter;
			float normalizedMag = Mathf.Min(1.0f, (mag - deadZoneCenter) / legalRange);
			float scale = normalizedMag / mag;
			stick = vec * scale;
		}
		else
		{
			// stick is in the inner dead zone
			stick = Vector2.zero;
		}

		return stick;
	}
	#endregion

	#region IMU Process
	private bool _firstImuPacket = true;
	private DateTime _lastTime;
	private Vector3 _lastRotate;
	private void ProcessIMU(Report report)
	{
		this.AccDelta = InputReportReader.ReadAcc(report, this);
		this.GyroDelta = InputReportReader.ReadGyro(report, this);
		this.Acc = AccDelta[2];

		if (_firstImuPacket)
		{
			_lastTime = DateTime.Now;
			_lastRotate = new Vector3(0, 0, 0);
			_firstImuPacket = false;
		}

		UpdateGyro(report, GyroDelta);
	}

	// Need help in calculating sensor state
	private void UpdateGyro(Report report, Vector3[] gyroDelta)
	{
		var timeDelta = (report.timeStamp - _lastTime).Seconds;

		var delta = new Vector3();
		delta += (_lastRotate + gyroDelta[0]) * timeDelta / 2;
		delta += gyroDelta[0] * 0.005f;
		delta += gyroDelta[1] * 0.005f;

		_lastTime = report.timeStamp;
		_lastRotate = gyroDelta[2];

		if (delta.magnitude > 0.1)
		{
			this.Gyro += delta;
		}
	}
	#endregion

	#region IR Process
	private void ProcessIR(Report report)
	{

	}
	#endregion

	public void Destroy()
	{
		Detach();
		PollThreadObj.Abort();
	}
}
