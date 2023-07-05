using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Shared.Rig;
using UnityEngine.AI;
using Fusion;

public class IEL_UniversalRig : HardwareRig
{
    public NavMeshAgent agent;

    [Header("---- Player Stats ----")]
    public float moveSpeed = 5f;

    public void Move(Vector2 direction)
    {
        Vector3 finalDirection = (headset.transform.forward * direction.y + headset.transform.right * direction.x).normalized;
        // agent.Move(finalDirection * moveSpeed * Time.deltaTime);
        // agent.Move(finalDirection * moveSpeed * Time.deltaTime);
        agent.velocity = finalDirection * moveSpeed;
        agent.updateRotation = false;


        Teleport(transform.position + finalDirection);
    }

    public override void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public override void Teleport(Vector3 position)
    {
        // agent.SetDestination(position);
    }
}
