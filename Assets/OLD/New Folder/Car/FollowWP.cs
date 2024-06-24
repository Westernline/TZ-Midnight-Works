using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWP : MonoBehaviour
{
    public GameObject[] waypoints;
    int currentWP = 0;
    public float speed = 10.0f;
    public float rotSpeed = 30.0f;

    // Update is called once per frame
    void Update()
    {
        if (waypoints.Length == 0)
            return;

        // Move to the next waypoint if close enough to the current one
        if (Vector3.Distance(this.transform.position, waypoints[currentWP].transform.position) < 3)
        {
            currentWP++;
            if (currentWP >= waypoints.Length)
                currentWP = 0;
        }

        // Rotate towards the current waypoint
        Quaternion lookAtWP = Quaternion.LookRotation(waypoints[currentWP].transform.position - this.transform.position);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookAtWP, rotSpeed * Time.deltaTime);

        // Move forward
        this.transform.position += this.transform.forward * speed * Time.deltaTime;
    }
}
