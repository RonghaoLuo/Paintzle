using UnityEngine;
using UnityEngine.AI;

public class StateNpc : MonoBehaviour, IPoolable
{
    [SerializeField] protected NavMeshAgent myAgent;
    //[SerializeField] private Transform target;
    [SerializeField] protected Vector3 destination;
    [SerializeField] protected PoolableType type;

    protected NpcState currentState;
    protected RoomKeeper currentRoom;

    public PoolableType Type => type;

    public GameObject GameObject => gameObject;

    public void ChangeState(NpcState newState)
    {
        if (currentState != null)
        {
            currentState.OnStateExit();
        }

        currentState = newState;
        currentState.OnStateEnter();
    }

    //private void FollowTarget()
    //{
    //    myAgent.SetDestination(target.position);
    //}

    public void SetAgentDestination()
    {
        myAgent.SetDestination(destination);
    }

    public void SetAgentDestination(Vector3 destination)
    {
        this.destination = destination;
        SetAgentDestination();
    }

    public bool IsMoving()
    {
        return myAgent.remainingDistance > myAgent.stoppingDistance;// || !myAgent.isStopped;
                                                                    // && myAgent.velocity.magnitude > 5f;
    }

    public virtual void OnSpawn()
    {
        gameObject.SetActive(true);

        // reset any visual or owner info here
    }

    public void OnDespawn()
    {
        // Stop particle systems, disable effects
        gameObject.SetActive(false);
    }

    public void SetPosition(Vector3 position)
    {
        myAgent.Warp(position);
    }

    public void OnPoolInitialize()
    {
        //Debug.Log("tries to initialize a state npc");
        NpcManager.Instance.RegisterNpc(gameObject, this);
    }

    public void SetCurrentRoom(RoomKeeper room)
    {
        currentRoom = room;
    }

    public RoomKeeper GetCurrentRoom()
    {
        return currentRoom;
    }
}
