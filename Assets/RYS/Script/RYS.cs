using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RYS : MonoBehaviour
{
    public static RYS instance;
    private void Awake()
    {
        instance = this;
    }

    public GameObject target;
    public NavMeshAgent agent;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 1.5f;

        agent.isStopped = true;
        
    }
    void Update()
    {
        if (agent.isStopped == false && RightHand.instance.frist_motion == false)
        {
            Agent_Controller();
        }
    }

    void Agent_Controller()
    {
        agent.destination = target.transform.position;

        transform.rotation = Quaternion.Euler(0, 90, 0);

        float remainingDistance = Vector3.Distance(transform.position, target.transform.position);

        if (agent.stoppingDistance >= remainingDistance)
        {
            agent.isStopped = true;
            RightHand.instance.frist_motion = true;

            LobbySetting.instance.Controller_Texture_judgment = true;
            LobbySetting.instance.neon.SetActive(true);
        }
    }
}
