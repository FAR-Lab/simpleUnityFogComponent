using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))]
public class RandomMovingAgent : MonoBehaviour {

    public NavMeshAgent agent;

    float[] portals = { 0, 2, -2, 4, -4, 6, -6, 8, -8 };

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();

        agent.destination = getRandomDestination(Vector3.zero);

        Vector3 start = getRandomDestination(agent.destination);
        agent.transform.position =
            Vector3.Lerp(start, agent.destination, Random.Range(0f, 1f));

    }

    float scale = 2.5f;

    public Vector3 getRandomDestination(Vector3 position) {
        float cx = position.x;
        float xd = cx < -1 ? 8.5f :
                            cx > 1 ? -8.5f :
                                     Random.Range(0f, 2f) < 1 ? -8.5f : 8.5f;
        float zd = portals[Random.Range(0, portals.Length)];

        return new Vector3(scale * xd, agent.transform.position.y, scale * zd);
    }

    public void setRandomDestination() {
        agent.destination = getRandomDestination(agent.transform.position);
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            //setRandomDestination();
        } else {
            if ((agent.destination - agent.transform.position).magnitude < 0.5) {
                setRandomDestination();
            }
        }
	}
}
