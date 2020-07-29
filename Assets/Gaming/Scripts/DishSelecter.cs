using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishSelecter : MonoBehaviour
{

    public Dish[] dishP1 = null;

    public Dish[] dishP2 = null;

    public int slter_Player2 = 0;

    public int slter_Player1 = 0;

    public GameManager GMer = null;

    public JoyCon input_Player2 = null;

    public JoyCon input_Player1 = null;

    public bool player2_Button_Up = true;

    public bool player1_Button_Up = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void init() {

        slter_Player2 = 0;

        slter_Player1 = 0;
    
    }

    // Update is called once per frame
    void Update()
    {

        input_Player2 = GMer.player2_Control.input;

        input_Player1 = GMer.player1_Control.input;

        if (player2_Button_Up && input_Player2.next)
        {

            if (!GMer.player2_Control.ready) slter_Player2++;

            player2_Button_Up = false;

        }
        else if (!input_Player2.next && !player2_Button_Up)
        {
            
            player2_Button_Up = true;

        }
        else if (input_Player2.reset && !GMer.player2_Control.ready)
        {

            GMer.player2_Control.ready = true;

            GMer.playerReady();

        }

        if (player1_Button_Up && input_Player1.next)
        {

            if(!GMer.player1_Control.ready) slter_Player1++;

            player1_Button_Up = false;

        }
        else if (!input_Player1.next && !player1_Button_Up)
        {

            player1_Button_Up = true;

        }
        else if (input_Player1.reset && !GMer.player1_Control.ready)
        {

            GMer.player1_Control.ready = true;

            GMer.playerReady();

        }

        slter_Player2 %= dishP2.Length;

        slter_Player1 %= dishP1.Length;

        for(int i = 0; i < dishP1.Length;i++) {

            if (i == slter_Player1) dishP1[i].selected(true);
            else dishP1[i].selected(false);

        }

        for (int i = 0; i < dishP2.Length; i++) {

            if (i == slter_Player2) dishP2[i].selected(true);
            else dishP2[i].selected(false);

        }

    }
}
