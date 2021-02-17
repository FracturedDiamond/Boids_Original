using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {

    [Header("Set Dynamically")]
    public Rigidbody rigid;
    bool deflect = false;
    private Neighborhood neighborhood;


	// Use this for initialization
	void Awake () {

        neighborhood = GetComponent<Neighborhood>();
        rigid = GetComponent<Rigidbody>();
        pos = Random.insideUnitSphere * Spawner.S.spawnRadius;
        Vector3 vel = Random.onUnitSphere * Spawner.S.veloctity;
        rigid.velocity = vel;
        LookAhead();

        Color randColor = Color.black;
        while(randColor.r + randColor.g + randColor.b <= 1.0f)
        {
            randColor = new Color(Random.value, Random.value, Random.value);
        }

        Renderer[] rends = gameObject.GetComponentsInChildren<Renderer>();
        foreach(Renderer r in rends)
        {
            r.material.color = randColor;
        }

        TrailRenderer tRend = GetComponent<TrailRenderer>();
        tRend.material.SetColor("_TintColor", randColor);
	}

    // Property
    public Vector3 pos
    {
        get { return transform.position; }
        set { transform.position = value;  }
    }
	
    // Look forward
    void LookAhead()
    {
        transform.LookAt(pos + rigid.velocity);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 vel = rigid.velocity;
        Spawner spn = Spawner.S;

        ////////////////////////////////////////////////////////////////////////////////// Collision avoidance
        Vector3 velAvoid = Vector3.zero;
        Vector3 tooClosePos = neighborhood.avgClosePos;
        Vector3 velDeflect = Vector3.zero;

        if (tooClosePos != Vector3.zero)
        {
            velAvoid = pos - tooClosePos;
            velAvoid.Normalize();
            velAvoid *= spn.veloctity;
        }

        ////////////////////////////////////////////////////////////////////////////////// Velocity matching
        Vector3 velAlign = neighborhood.avgVel;
        if (velAlign != Vector3.zero)
        {
            velAlign.Normalize();
            velAlign *= spn.veloctity;
        }

        ////////////////////////////////////////////////////////////////////////////////// Center to the position of the flock
        Vector3 velCenter = neighborhood.avgPos;
        if (velCenter != Vector3.zero)
        {
            velCenter -= transform.position;
            velCenter.Normalize();
            velCenter *= spn.veloctity;
        }
        if (deflect)
        {
            float angle = 30.0f * Mathf.PI / 180.0f;
            velDeflect = new Vector3(vel.x * Mathf.Cos(angle), vel.y, vel.z * Mathf.Sin(angle));
        }
        else
        {
            velDeflect = Vector3.zero;
        }

        ////////////////////////////////////////////////////////////////////////////////// Where is the attractor, where is the boid?
        Vector3 delta = Attractor.POS - pos;
        bool attracted = (delta.magnitude > spn.attractPushDist);
        Vector3 velAttract = delta.normalized * spn.veloctity;

        float fdt = Time.fixedDeltaTime;

        ////////////////////////////////////////////////////////////////////////////////// Boid behaviour
        if (velDeflect != Vector3.zero)
        {
            vel = Vector3.Lerp(vel, velDeflect, spn.deflectPush * fdt);
        }
        else if (velAvoid != Vector3.zero)
        {
            vel = Vector3.Lerp(vel, velAvoid, spn.collAvoid * fdt);
        }
        else
        {
            if (velAlign != Vector3.zero)
            {
                vel = Vector3.Lerp(vel, velAlign, spn.velMatching * fdt);
            }
            if (velCenter != Vector3.zero)
            {
                vel = Vector3.Lerp(vel, velCenter, spn.flockCentering * fdt);
            }
            if (velAttract != Vector3.zero)
            {
                if (attracted)
                {
                    vel = Vector3.Lerp(vel, velAttract, fdt * spn.attractPull);
                }
                else
                {
                    vel = Vector3.Lerp(vel, -velAttract, spn.attractPush * fdt);
                }
            }
        }

        vel = vel.normalized * spn.veloctity;

        rigid.velocity = vel;
        LookAhead();
    }

    void OnTriggerEnter(Collider other)
        {
        if(other.tag == "Obstacle")
        {
            deflect = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Obstacle")
        {
            deflect = false;
        }
    }
}
