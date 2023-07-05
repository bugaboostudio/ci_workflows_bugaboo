using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_GameController_Preparation : AbstractState<GameController>
{
    public override async void OnEnterState()
    {
        await fsm.connectionManager.Connect();
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void OnExitState()
    {

    }

    public override void UpdateState()
    {

    }
}
