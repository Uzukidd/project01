using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlTest : PlayerControl
{

    void UpdateCursor()
    {

        //mousePositionOnScreen = Input.mousePosition;

        //mousePositionInWorld = Camera.main.ScreenToWorldPoint(mousePositionOnScreen);

        //cursor.transform.position = new Vector3(mousePositionInWorld.x, mousePositionInWorld.y, 0);

        if (Vector3.Distance(cursor.transform.position, transform.position) > maxArea)
        {

            Vector3 normalV = (cursor.transform.position - transform.position).normalized;

            cursor.transform.position = transform.position + normalV * maxArea;


        }

    }


    protected void UpdateCursorByControl()
    {

        //print(input.Gyro);

        cursor.transform.position += new Vector3(-input.Gyro.x / 800.0f, -input.Gyro.z / 800.0f, 0.0f);

        if (Vector3.Distance(cursor.transform.position, transform.position) > maxArea)
        {

            Vector3 normalV = (cursor.transform.position - transform.position).normalized;

            cursor.transform.position = transform.position + normalV * maxArea;


        }

    }

    void Update()
    {

        playerAction.setCurrentValue(Clipboard.instance.player1_Scoure);

        playerAction.UpdateScore(Clipboard.instance.player1_failCount);



        if (stop) return;

        if (input.reset) input.Gyro = new Vector3();

        if (input == null) UpdateCursor();
        else UpdateCursorByControl();

        if (Input.GetKey(KeyCode.W))
        {

            cursor.transform.position += new Vector3(0, Time.deltaTime * 5, 0);

        }
        if (Input.GetKey(KeyCode.S))
        {

            cursor.transform.position -= new Vector3(0, Time.deltaTime * 5, 0);

        }
        if (Input.GetKey(KeyCode.A))
        {

            cursor.transform.position -= new Vector3(Time.deltaTime * 5, 0, 0);

        }
        if (Input.GetKey(KeyCode.D))
        {

            cursor.transform.position += new Vector3(Time.deltaTime * 5, 0, 0);

        }

        if (foodHit.getCover)
        {

            fail = false;

            Clipboard.instance.player1_keeping += Time.deltaTime;
            //playerAction.setCurrentValue((int) (playerAction.actionBarValue[1] + Time.deltaTime * 100));

        }
        else
        {
            if (!fail)
            {

                Clipboard.instance.player1_failCount++;

                fail = true;
            }
            //playerAction.setCurrentValue((int)(playerAction.actionBarValue[1] - Time.deltaTime * 100));


        }

    }
}
