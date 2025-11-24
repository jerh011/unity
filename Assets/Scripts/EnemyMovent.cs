using UnityEngine;
using UnityEngine.AI;

public class EnemyMovent : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Verifica que el agente y el jugador existan antes de mover al enemigo
        if (navMeshAgent != null && player != null)
        {
            navMeshAgent.SetDestination(player.position);
        }
    }
}
