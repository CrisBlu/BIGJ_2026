using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GuardNavMesh : MonoBehaviour
{
    [SerializeField] NavMeshAgent guardAgent;
    [SerializeField] Guard guard;
    [SerializeField] Transform[] waypoints;
    [SerializeField] float waypointReachedDistance = 0.5f;
    [SerializeField] float chaseStopDistance = 1.5f;
    [SerializeField] float pauseDuration = 2f;
    int currentWaypoint = 0;
    bool hasBeenAlerted = false;
    
    enum GuardState { Patrolling, Chasing, Pausing }
    GuardState state = GuardState.Patrolling;

    private void Awake()
    {
        guardAgent = GetComponent<NavMeshAgent>();
    }
    
    void Start()
    {
        GoToWaypoint(currentWaypoint);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case GuardState.Patrolling: Patrol(); break;
            case GuardState.Chasing:   Chase();  break;
        }
    }
    
    void Patrol()
    {
        if (guard.IsAlerted)
        {
            state = GuardState.Chasing;
            return;
        }

        if (!guardAgent.pathPending && guardAgent.remainingDistance <= waypointReachedDistance)
            AdvanceWaypoint();
    }

    void Chase()
    {

        if (guard.LastSeenPlayerPos == Vector3.negativeInfinity) return;

        float dist = Vector3.Distance(transform.position, guard.LastSeenPlayerPos);

        

        
        
        if (dist > chaseStopDistance)
            guardAgent.SetDestination(guard.LastSeenPlayerPos);
        else
            StartCoroutine(PauseAndResume());
    }
    
    IEnumerator PauseAndResume()
    {
        state = GuardState.Pausing;
        guardAgent.ResetPath();

        yield return new WaitForSeconds(pauseDuration);

        state = GuardState.Patrolling;
        GoToWaypoint(currentWaypoint);
    }

    void GoToWaypoint(int index)
    {
        if (waypoints.Length == 0) return;
        guardAgent.SetDestination(waypoints[index].position);
    }
    void AdvanceWaypoint()
    {
        if(waypoints.Length == 0)
             return; 
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        GoToWaypoint(currentWaypoint);
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);
    }
}
