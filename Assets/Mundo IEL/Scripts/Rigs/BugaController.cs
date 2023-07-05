using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.XR.Shared.Rig;
using UnityEngine.InputSystem;
using UnityEngine;

public class BugaController : MonoBehaviour
{
    public IEL_UniversalRig rig;
    public InputAction moveInputAction;

    private void Start()
    {
        moveInputAction.Enable();
    }

    void Update()
    {
        if (rig != null)
        {
            Vector2 moveDirection = moveInputAction.ReadValue<Vector2>();
            Debug.Log(moveDirection);
            rig.Move(moveDirection);
        }
    }

}
