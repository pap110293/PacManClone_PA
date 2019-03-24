using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clyde : Ghost
{
    private Plinky plinkyGhost;

    public override Vector2? OnChaseModeNextTarget()
    {
        Vector2 pacManPosition = pacMan.GetPosition();
        pacManPosition += 2 * pacMan.GetCurrentDirection();

        Vector2 plinkyPosition;
        plinkyPosition = FindObjectOfType<Plinky>().transform.position;

        Vector2 vector = (pacManPosition - plinkyPosition) * 2;

        Vector2 target = plinkyPosition + vector;

        return target;
    }
}
