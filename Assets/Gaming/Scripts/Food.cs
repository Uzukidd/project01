using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{

    public GameObject[] mark = null;

    public int markInt = 0;

    public bool getCover = false;

    public int Speed = 9;

    public Rigidbody2D RB2D = null;
    
    void Start()
    {

        TryGetComponent<Rigidbody2D>(out RB2D);

        

    }

    // Update is called once per frame
    void Update()
    {

        //SpeedLimit();

        gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, mark[markInt].transform.position, Time.deltaTime * 2);

        if(Vector2.Distance(transform.position, mark[markInt].transform.position) < 0.1) {

            markInt++;

            markInt %= mark.Length;

        }


    }

    private void SpeedLimit()
    {

         RB2D.AddForce(new Vector2(Random.Range(-100, 100) % Speed, (Random.Range(0, 200) - 100) % Speed), ForceMode2D.Force);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        getCover = true;

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        getCover = false;
    }
}
