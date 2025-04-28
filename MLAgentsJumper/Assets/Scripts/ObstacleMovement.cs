using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    public int speed;
    public bool xMov = true;

    void Start()
    {
        speed = Random.Range(4, 10); // Random speed every episode
    }

    void Update()
    {
        if(xMov) this.transform.Translate(Time.deltaTime * speed, 0, 0); // Movement if X direction
        else this.transform.Translate(0, 0, -Time.deltaTime * speed); // Movement if Z direction
    }
}
