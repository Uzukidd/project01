using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAction : MonoBehaviour
{

    public Image actionBar = null;

    public Text scoure = null;

    public float[] actionBarValue = { 0, 0, 6 };//状态条， 最小值，目前值，最大值

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateScore(int x)
    {

        scoure.text = "" + x;

    }

    public void UpdateScore(string x)
    {

        scoure.text = "" + x;

    }

    // Update is called once per frame
    void Update()
    {

        actionBar.fillAmount = actionBarValue[1] / actionBarValue[2];

    }

    public void setCurrentValue(int x)
    {

        actionBarValue[1] = Mathf.Max(Mathf.Min(x, actionBarValue[2]), actionBarValue[0]);

    }

}
