using UnityEngine;
using UnityEngine.AI;

public class SimpleNavMeshFollower : MonoBehaviour
{
    #region References

    // NavMeshAgent that controls movement
    public NavMeshAgent agent;

    // Target to follow
    public Transform target;

    #endregion

    #region Runtime Tuning Values

    // Movement speed of the agent
    public float speed = 3.5f;

    // How fast the agent accelerates
    public float acceleration = 8f;

    // How fast the agent rotates toward direction
    public float angularSpeed = 120f;

    // Distance at which agent stops near target
    public float stoppingDistance = 1.5f;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        #region Auto Assign Agent

        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        #endregion

        #region Auto Find Player Target

        // Try to find player by tag and assign it as target
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("No GameObject found with tag 'Player'. AI is blind now. Very cinematic. Very tragic.");
        }

        #endregion
    }

    private void Update()
    {
        #region Apply Real-Time Settings

        // Apply live tuning values every frame (yes, it obeys instantly)
        if (agent != null)
        {
            agent.speed = speed;
            agent.acceleration = acceleration;
            agent.angularSpeed = angularSpeed;
            agent.stoppingDistance = stoppingDistance;
        }

        #endregion

        #region Follow Target

        // Safety check so nothing explodes
        if (agent == null || target == null)
            return;

        // Continuously chase target
        agent.SetDestination(target.position);

        #endregion
    }

    #endregion
}