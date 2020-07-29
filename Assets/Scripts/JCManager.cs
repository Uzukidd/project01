using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class JCManager : MonoBehaviour
{
	public static JCManager Instance { get; private set; }

	public const ushort VENDOR_TOP = 0x57e;
	public const ushort VENDOR_ID = 0x057e;
	public const ushort PRODUCT_L_ID = 0x2006;
	public const ushort PRODUCT_R_ID = 0x2007;

	public List<JoyCon> joycons;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		joycons = new List<JoyCon>();

		HIDapi.hid_init();
		
		IntPtr ptr = HIDapi.hid_enumerate(VENDOR_TOP, 0x0);
		IntPtr top_ptr = ptr;

		if (ptr == IntPtr.Zero)
		{
			ptr = HIDapi.hid_enumerate(VENDOR_ID, 0x0);
			if (ptr == IntPtr.Zero)
			{
				HIDapi.hid_free_enumeration(ptr);
				Debug.Log("No Joy-Cons found!");
			}
		}

		while (ptr != IntPtr.Zero)
		{
			var enumerate = (hid_device_info)Marshal.PtrToStructure(ptr, typeof(hid_device_info));
			Debug.Log(enumerate.product_id);

			if (enumerate.product_id == PRODUCT_L_ID || enumerate.product_id == PRODUCT_R_ID)
			{
				var type = JoyConType.NONE;
				if (enumerate.product_id == PRODUCT_L_ID)
				{
					type = JoyConType.LEFT;
					Debug.Log("Left Joy-Con connected.");
				}
				else if (enumerate.product_id == PRODUCT_R_ID)
				{
					type = JoyConType.RIGHT;
					Debug.Log("Right Joy-Con connected.");
				}

				IntPtr handle = HIDapi.hid_open_path(enumerate.path);
				HIDapi.hid_set_nonblocking(handle, 1);
				joycons.Add(new JoyCon(type, handle, true));
			}

			ptr = enumerate.next;
		}

		HIDapi.hid_free_enumeration(top_ptr);

		//TODO: Move to other place
		Attach();
		
	}

	void Attach()
	{
		for (int i = 0; i < joycons.Count; ++i)
		{
			Debug.Log(i);
			var jc = joycons[i];
			byte LEDs = 0x0;
			LEDs |= (byte)(0x1 << i);
			jc.Attach(LEDs);
			jc.Begin();
		}
	}

	void Update()
	{
		foreach (var jc in joycons)
		{
			jc.Loop();
		}
	}
	
	void OnDestroy()
	{
		foreach (var jc in joycons)
		{
			jc.Destroy();
		}
	}
}
