using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    public Node[] neighbors;
    public Vector2[] validDirection;
    public bool isGhostCase;

    // Use this for initialization
    void Awake () {
        validDirection = new Vector2[neighbors.Length];
        for (int i = 0; i < neighbors.Length; i++)
        {
            Node neighbor = neighbors[i];
            validDirection[i] = (neighbor.transform.position - transform.position).normalized;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //private void OnDrawGizmos()
    //{
    //    if (neighbors.Length > 0)
    //    {
    //        foreach (Node node in neighbors)
    //        {
    //            if (node == null) continue;
    //            var isLinked = false;
    //            if (node.neighbors.Length > 0)
    //            {
    //                foreach (var item in node.neighbors)
    //                {
    //                    isLinked = transform == item.transform;
    //                    if (isLinked)
    //                        break;
    //                }
    //            }
    //            if (isLinked)
    //            {
    //                Gizmos.color = Color.red;
    //                Gizmos.DrawLine(transform.position, node.transform.position);
    //            }
    //        }
    //    }
    //}
}
