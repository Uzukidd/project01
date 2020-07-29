using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{

    public PlayerAction playerAction = null;




    public GameObject cursor = null;

    public CircleCollider2D cursorHit = null;


    public GameObject area = null;

    public BoxCollider2D areaHit = null;


    public GameObject food = null;

    public Food foodHit = null;



    public Image TimeBar = null;

    public float[] timeBarValue = { 0, 0, 20 };


    public Vector3 mousePositionOnScreen;

    public Vector3 mousePositionInWorld;

    public float maxArea = 1.3f;

    public bool ready = false;

    public bool stop = false;

    public bool fail = true;

    public JoyCon input = null;

    void Start()
    {

        food.TryGetComponent<Food>(out foodHit);

    }

    public void init() {

        food.transform.position = new Vector2();

        food.transform.rotation = new Quaternion();

        cursor.transform.position = new Vector2();

        ready = false;

        stop = false;

        fail = true;

}

    public void setInput(JoyCon joycon)
    {

        input = joycon;

    }

    public void Stop(bool stops)
    {
        stop = stops;

        food.SetActive(!stops);

    }

    public void setCurrentTimeValue(float x, float y)
    {

        TimeBar.fillAmount = 1.0f * x / y;

    }

    protected void UpdateCursorByControl()
    {

        //V/ector2 force = new Vector2(input.Acc.z * 1.5f, - (input.Acc.y - 9.8f) );

        //print(input.Acc.y);

        //cursor.GetComponent<Rigidbody2D>().velocity = force;
        //.AddForce(force);

        cursor.transform.position += new Vector3(input.Gyro.x / 500.0f, input.Gyro.z / 500.0f, 0.0f);

        if (Vector3.Distance(cursor.transform.position, transform.position) > maxArea)
        {

            Vector3 normalV = (cursor.transform.position - transform.position).normalized;

            cursor.transform.position = transform.position + normalV * maxArea;

            //cursor.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);


        }

       

    }

    protected void UpdateCursor()
    {

        mousePositionOnScreen = Input.mousePosition;

        mousePositionInWorld = Camera.main.ScreenToWorldPoint(mousePositionOnScreen);

        cursor.transform.position = new Vector3(mousePositionInWorld.x, mousePositionInWorld.y, 0);

        if (Vector3.Distance(cursor.transform.position, transform.position) > maxArea)
        {

            Vector3 normalV = (cursor.transform.position - transform.position).normalized;

            cursor.transform.position = transform.position + normalV * maxArea;


        }

    }


    void Update()
    {

        playerAction.setCurrentValue(Clipboard.instance.player2_Scoure);

        playerAction.UpdateScore(Clipboard.instance.player2_failCount);

        if (input.reset) input.Gyro = new Vector3();

        if (stop) return;

        if (input == null) UpdateCursor();
        else UpdateCursorByControl();
        if (foodHit.getCover)
        {

            fail = false;

            Clipboard.instance.player2_keeping += Time.deltaTime;

        } else
        {
            if(!fail)
            {

                Clipboard.instance.player2_failCount++;

                fail = true;
            }

        }

    }


}
