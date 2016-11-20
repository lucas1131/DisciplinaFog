/*using UnityEngine;
using Pathfinding;
using System.Collections;

[RequireComponent (typeof(Seeker))]
[RequireComponent (typeof(Rigidbody2D))]
public class SeekerScript : MonoBehaviour {
    private Seeker seeker;
    private Rigidbody2D rb;

    public Transform target;
    public float updateRate = 2f;
    public float speed = 3f;
    public float nextWaypointDistance = 1f;

    private Path path;
    private int currentWaypoint;
    private bool pathIsDed = false;

	void Start() {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        seeker.StartPath(transform.position, target.position, OnPathComplete);
        if(target != null) StartCoroutine(UpdatePath());
        if (target == null)
            Debug.Log("Já deu bosta, corre");
	}

    public IEnumerator UpdatePath() {
        while (target != null) {
            seeker.StartPath(transform.position, target.position, OnPathComplete);
            yield return new WaitForSeconds(1f / updateRate);
        }
        yield break;
    }
	
    public void OnPathComplete(Path p) {
        if (!p.error) {
            path = p;
            currentWaypoint = 0;
        }
    }

	// Update is called once per frame
	void FixedUpdate () {
	    if (target != null && path != null) {
            if (currentWaypoint >= path.vectorPath.Count) {
                if (!pathIsDed) {
                    pathIsDed = true;
                    Debug.Log("Path is ded");
                }
            } else {
                pathIsDed = false;
                Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
                dir *= speed * Time.fixedDeltaTime;
                dir.z = 0;
                rb.AddForce(dir);

                float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
             
                if (dist < nextWaypointDistance) {
                    currentWaypoint++;
                }
            }
        }
	}
}*/
