using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    readonly int PELLET_SCORE = 10;
    readonly int SUPPER_PELLET_SCORE = 50;

    public Transform portalReceiver;
    public bool isPortal;
    public bool isPellet;
    public bool isSuperPellet;
    public bool didConsume;

    public int GetScore()
    {
        if (isPellet)
            return PELLET_SCORE;
        if (isSuperPellet)
            return SUPPER_PELLET_SCORE;

        return 0;
    }

    public void Disable()
    {
        if (isPellet || isSuperPellet)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            didConsume = true;
        }
    }

    public void Restart()
    {
        if(isPellet || isSuperPellet)
        {
            GetComponent<SpriteRenderer>().enabled = true;
            didConsume = false;
        }
    }
}
