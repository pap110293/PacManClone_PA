using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plinky : Ghost
{
    public override Vector2? OnChaseModeNextTarget()
    {
        return pacMan.GetPosition();
    }
}
