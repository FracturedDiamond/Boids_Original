using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour {

    static public Vector3 POS = Vector3.zero;

    [Header("Set in Inspector")]
    public float radius = 10;
    public float xPhase = 0.5f;
    public float yPhase = 0.4f;
    public float zPhase = 0.1f;
    public GameObject obstacleRef;
    public int numObstacles = 3;

    [Header("Set Dynamically")]
    public List<GameObject> obstacles;
    public int numActive = 0;

    // Use this for initialization
    void Awake () {

        obstacles = new List<GameObject>();

        for (int i = 0; i < numObstacles; i++)
        {
            GameObject go = Instantiate<GameObject>(obstacleRef);
            go.SetActive(false);
            obstacles.Add(go);
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 tPos = Vector3.zero;
        Vector3 scale = transform.localScale;
        tPos.x = Mathf.Sin(xPhase * Time.time) * radius * scale.x;
        tPos.y = Mathf.Sin(yPhase * Time.time) * radius * scale.y;
        tPos.z = Mathf.Sin(zPhase * Time.time) * radius * scale.z;

        transform.position = tPos;
        POS = tPos;

        int frames = (int)(Time.frameCount);
        if (frames % 300 == 0 && numActive < numObstacles)
        {
            obstacles[numActive].SetActive(true);
            obstacles[numActive].transform.position = POS;
            numActive++;
        }
    }
}
