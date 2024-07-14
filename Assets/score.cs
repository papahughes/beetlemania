using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class score : MonoBehaviour
{
    //Score
    public int current_score = 0;
    private int point_value = 1;

    //Chain
    private float chain_timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(chain_timer > 0)
        {
            chain_timer -= Time.deltaTime;
        }
    }

    public void AddPoints()
    {
        //reset points if timer counts down
        if(chain_timer <= 0)
        {
            point_value = 1;
        }

        //continue chain by reseting timer
        chain_timer = 0.5f;

        current_score += point_value;

        point_value *= 2;

        point_value = Mathf.Min(point_value, 9999);

        Debug.Log(current_score);
    }
}
