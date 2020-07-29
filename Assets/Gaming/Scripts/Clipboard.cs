using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clipboard : MonoBehaviour
{

    public static Clipboard instance = null;

    public int player1_Scoure = 0;

    public int player2_Scoure = 0;

    public int player1_failCount = 0;

    public int player2_failCount = 0;

    public float player1_keeping = 0;

    public float player2_keeping = 0;

    public int round = 0;
    // r0 教程关
    // r1 第一回合
    // ......
    // rn 第n回合

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

    public void clear()
    {

        player1_Scoure = 0;

        player2_Scoure = 0;

    }

    public void allClear()
    {

        clear();

        player1_failCount = 0;

        player2_failCount = 0;

        player1_keeping = 0;

        player2_keeping = 0;

        round = 0;

    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
