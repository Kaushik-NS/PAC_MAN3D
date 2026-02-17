using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum GhostType
{
    Blinky,   // red
    Pinky,    // ambush
    Inky,     // hybrid/random
    Clyde     // patrol + chase switch
}

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;


    private float timer;
    public float moveSpeed = 0.5f;
    private Vector3 currentDirection;

    public Transform player;
    public float repathRate = 0.2f;   // how often to update path
    float repathTimer;

    public GhostType ghostType;
    public float ChaseDistance = 6f;
    public Transform RandomTarget;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;
        //SetNewDestination();
    }


    void Update()
    {
        if (!GameManager.Instance.IsGameStarted) return;


        // Update path frequently so enemy keeps chasing
        repathTimer += Time.deltaTime;
        if (repathTimer >= repathRate)
        {
            Vector3 target = FindTheTarget();
            agent.SetDestination(target);
            repathTimer = 0f;
        }

        // Read next NavMesh corner
        if (agent.hasPath && agent.path.corners.Length > 1)
        {
            Vector3 nextCorner = agent.path.corners[1];
            Vector3 direction = nextCorner - transform.position;
            direction.y = 0;

            // Axis-only movement (Pacman style)
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
                currentDirection = new Vector3(Mathf.Sign(direction.x), 0, 0);
            else
                currentDirection = new Vector3(0, 0, Mathf.Sign(direction.z));
        }

        // Move manually
        transform.position += currentDirection * moveSpeed * Time.deltaTime;

        // Rotate toward movement direction
        if (currentDirection != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(currentDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                12f * Time.deltaTime   // rotation speed
            );
        }


        // Stay on NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 0.5f, NavMesh.AllAreas))
            transform.position = hit.position;

        agent.nextPosition = transform.position;
    }

    Vector3 FindTheTarget()
    {
        switch (ghostType)
        {
            //BLINKY — Aggressively chases Pac-Man
            case GhostType.Blinky:
                return player.position;

            //PINKY — Predictive / ambush behavior
            case GhostType.Pinky:
                Vector3 ahead = player.position + player.forward * 4f;
                return ahead;

            //INKY — Random or hybrid logic
            case GhostType.Inky: 
                if (Random.value < 0.30f) //30% chance to chase directly
                    return player.position;
                else
                    return player.position + Random.insideUnitSphere * 5f;

            //CLYDE — Patrols or chasing when player is near
            case GhostType.Clyde:
                float dist = Vector3.Distance(transform.position, player.position);

                // FAR → patrol
                if (dist > ChaseDistance)
                {
                    return player.position + Random.insideUnitSphere * 10f;
                }
                else
                {
                    return player.position;
                }
            default:
                return player.position;
        }
    }

//void SetNewDestination()
//    {
//        for (int i = 0; i < 10; i++)   // try multiple times
//        {
//            Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
//            randomDirection += transform.position;
//            randomDirection.y = 0;

//            NavMeshHit hit;
//            if (NavMesh.SamplePosition(randomDirection, out hit, 2f, NavMesh.AllAreas))
//            {
//                if (Vector3.Distance(transform.position, hit.position) > 2f)
//                {
//                    agent.SetDestination(hit.position);
//                    return;
//                }
//            }
//        }
//    }

}
