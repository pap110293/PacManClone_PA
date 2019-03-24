using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inky : Ghost
{
    public float distanceToPacMan = 8.0f;
    public override Vector2? OnChaseModeNextTarget()
    {
        float distance = Vector2.Distance(pacMan.GetPosition(), GetPostition());

        if(distance < 8)
        {
            return pacMan.GetPosition();
        }
        else
        {
            return ghostHouse.transform.position;
        }
    }
}
