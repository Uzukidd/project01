using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoyControl : MonoBehaviour
{

    public static JoyControl instance = null;

	public Vector3 velocity;
	
	public bool accControl = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
	{

        

    }
	
	void Update()
	{

		var accDelta = JCManager.Instance.joycons[0].AccDelta;

		var gyroDelta = JCManager.Instance.joycons[0].GyroDelta;

		var stick = JCManager.Instance.joycons[0].stick / 5f;
		//testObject.transform.localPosition += new Vector3(stick.x, stick.y);
		
		//testObject.transform.eulerAngles = 
        
        print(JCManager.Instance.joycons.Count);

	}

	public void Reset()
	{
		//testObject.transform.position = new Vector3(0, 0, 0);
		//testObject.transform.eulerAngles = new Vector3(0, 0, 0);
		JCManager.Instance.joycons[0].Gyro = new Vector3();
		velocity = new Vector3(0, 0, 0);
	}
}
