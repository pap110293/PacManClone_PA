using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinky : Ghost
{
    public override Vector2? OnChaseModeNextTarget()
    {
        return pacMan.GetPosition() + 4 * pacMan.GetCurrentDirection();
    }
}
