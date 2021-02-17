using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neighborhood : MonoBehaviour {

    [Header("Set Dynamically")]
    public List<Boid> neighbors;
    private SphereCollider coll;


	// Use this for initialization
	void Start () {
        neighbors = new List<Boid>();
        coll = GetComponent<SphereCollider>();
        coll.radius = Spawner.S.neighborDist / 2;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (coll.radius != Spawner.S.neighborDist / 2)          // If it was not properly set in 'Start()'
        {
            coll.radius = Spawner.S.neighborDist / 2;           // Set it
        }
	}

    void OnTriggerEnter(Collider other)
    {
        Boid b = other.GetComponent<Boid>();
        if (b!=null)
        {
            if (neighbors.IndexOf(b) == -1)                     // Is this boid in the neighbors list?
            {
                neighbors.Add(b);                               // If not, add to the neighbors list
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        Boid b = other.GetComponent<Boid>();
        if (b != null)
        {
            if (neighbors.IndexOf(b) == -1)                     // Is this boid in the neighbors list?
            {
                neighbors.Remove(b);                            // If so, remove from the neighbors list
            }
        }
    }

    public Vector3 avgPos                                       // Average position of neighbors
    {
        get
        {
            Vector3 avg = Vector3.zero;
            if (neighbors.Count == 0) return avg;

            for(int i = 0; i < neighbors.Count; i ++)
            {
                avg += neighbors[i].pos;
            }

            avg /= neighbors.Count;

            return avg;
        }
    }

    public Vector3 avgVel                                       // Average velocity of neighbors
    {
        get
        {
            Vector3 avg = Vector3.zero;
            if (neighbors.Count == 0) return avg;

            for (int i = 0; i < neighbors.Count; i++)
            {
                avg += neighbors[i].rigid.velocity;
            }

            avg /= neighbors.Count;

            return avg;
        }
    }

    public Vector3 avgClosePos
    {
        get
        {
            Vector3 avg = Vector3.zero;
            Vector3 delta;
            int nearCount = 0;
            for(int i = 0; i < neighbors.Count; i++)
            {
                delta = neighbors[i].pos - transform.position;
                if (delta.magnitude <= Spawner.S.collisionDist)
                {
                    avg += neighbors[i].pos;
                    nearCount++;
                }
            }
            if (nearCount == 0) return avg;

            avg /= nearCount;
            return avg;
        }
    }
}
