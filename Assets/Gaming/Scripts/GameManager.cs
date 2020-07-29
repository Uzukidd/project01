using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public PlayerAction player1_Action = null;

    public PlayerControl player1_Control = null;

    public bool player1_Win = false;


    public PlayerAction player2_Action = null;

    public PlayerControl player2_Control = null;

    public bool player2_Win = false;



    public DishSelecter dishSelecter = null;

    public Text bigTitle = null;

    public Text counter = null;

    public Clipboard CBD = null;



    public float time_last = 10;

    public float time_max;

    public float time_unit = 1.0f;

    public bool stop = true;


    public int maxFail = 5;

    public bool gaming = false;//是否处于游戏中


    bool gyinfo;

    Gyroscope go;

    public Text goText;

    void Start()
    {

        initCBD();

        initControl();

        player1_Control.Stop(true);

        player2_Control.Stop(true);

        playerReady();

        startCounter();


        gyinfo = SystemInfo.supportsGyroscope;

        go = Input.gyro;

        go.enabled = true;

    }

    public void initControl()
    {

        time_max = time_last;

        foreach(JoyCon jc in JCManager.Instance.joycons)
        {

            if(jc.Type == JoyConType.LEFT)
            {

                player1_Control.setInput(jc);

                jc.Gyro = new Vector3();

            }
            else if(jc.Type == JoyConType.RIGHT)
            {

                player2_Control.setInput(jc);

                jc.Gyro = new Vector3();

            }

        }
            

    }

    public void initCBD()
    {

        CBD = Clipboard.instance;

        CBD.player1_failCount = 0;

        CBD.player2_failCount = 0;

        CBD.player1_keeping = 0;

        CBD.player2_keeping = 0;

    }

    public void playerReady() {

        stop = !(player2_Control.ready && player1_Control.ready);

        player1_Control.Stop(stop);

        player2_Control.Stop(stop);

        gaming = !stop;

    }

    public void setCounter(int x)
    {

        if (counter == null) return;

        if(x < 10) counter.text = "0" + x;
        else counter.text = "" + x;

    }

    public void setCounter(string x)
    {

        counter.text = "" + x;

    }

    void restartGame()
    {

        print("restart");

        stop = true;

        gaming = false;

        initCBD();

        player1_Win = player2_Win = false;

        player2_Control.ready = player1_Control.ready = false;

        player1_Control.Stop(true);

        player2_Control.Stop(true);

        time_last = 30;

        bigTitle.enabled = false;

        dishSelecter.init();

        startCounter();

        //SceneManager.LoadScene(3);

    }

    public void updateTime()
    {


        player1_Control.setCurrentTimeValue(time_last, time_max);

        player2_Control.setCurrentTimeValue(time_last, time_max);


    }

    public void UpdateScoure()
    {

        player1_Action.UpdateScore(CBD.player1_Scoure);

        player2_Action.UpdateScore(CBD.player2_Scoure);

    }

    public void startCounter()
    {



        if (time_last <= 0)
        {

            //时间结束
            gameOver();

            return;

        } else if(player1_Control.stop && player2_Control.stop && gaming)
        {
            //双方完成

            gameOver();

            return;

        }



        if (CBD.round != 0) setCounter("回合" + CBD.round);
        else setCounter("教程回合");

        Invoke("startCounter", time_unit);

    }

    public void gameOver()
    {

        print("restart");

        player1_Win = player1_Win || CBD.player1_failCount < 5 && CBD.player1_keeping >= 15.0;

        player2_Win = player2_Win || CBD.player2_failCount < 5 && CBD.player2_keeping >= 15.0;



        CBD.player1_Scoure += player1_Win ? 2 : 0;

        CBD.player2_Scoure += player2_Win ? 2 : 0;



        if (player1_Win && player2_Win)
        {
            //同时胜利
            bigTitle.text = "平 局";
        }else
        if (player1_Win)
        {
            //玩家1胜利
            bigTitle.text = "玩家1获胜";
        }else
        if (player2_Win)
        {
            //玩家2胜利
            bigTitle.text = "玩家2获胜";
        } else
        {
            //同时失败
            bigTitle.text = "失 败";
        }

        player1_Win = player2_Win = false;

        bigTitle.enabled = true;

        CBD.round++;

        print("restart");


        if (CBD.player1_Scoure >= 6 && CBD.player2_Scoure <= 6) {

            ending(0);

        } else if (CBD.player2_Scoure >= 6 && CBD.player1_Scoure <= 6) {

            ending(1);

        } else if (CBD.player1_Scoure >= 6 && CBD.player2_Scoure >= 6) {

            ending(2);

        } else {

            //restartGame();

            print("restart");

            //restartGame();

            Invoke("restartGame", 3);

        }

    }

    void ending(int action) {

        SceneManager.LoadScene(0);

    }

    void Update()
    {

        //UpdateScoure();

        if (gyinfo) {

            Vector3 a = go.attitude.eulerAngles;
            a = new Vector3(-a.x, -a.y, a.z);

            goText.text = string.Format("x:{0} y:{1} z:{2}", (int)a.x, (int)a.y, (int)a.z);
        } else {

            goText.text = string.Format("x:{0} y:{1} z:{2}", 0, 0, 0);

        }




        if (stop) return;



        if (CBD.round != 0)//没有处于教程关
        {

            updateTime();

            time_last -= Time.deltaTime;

        }





        if (CBD.player1_failCount >= maxFail)
        {
            //玩家1失败
            //player1_Win = true;
            player1_Control.Stop(true);

        }
        if (CBD.player2_failCount >= maxFail)
        {
            //玩家2失败
            player2_Control.Stop(true);

        }



        if (CBD.round == 0)//教程关
        {

            if (CBD.player1_keeping >= 15.0)
            {
                //玩家1完成
                player1_Win = true;
                player1_Control.Stop(true);

            }
            if (CBD.player2_keeping >= 15.0)
            {
                //玩家2完成
                player2_Win = true;
                player2_Control.Stop(true);

            }
        }

    }
}
