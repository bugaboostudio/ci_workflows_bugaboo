using UnityEngine;

public abstract class AbstractState<T> : MonoBehaviour where T : FiniteStateMachine<T>
{
    public T fsm;

    public abstract void OnEnterState();
    public abstract void OnExitState();
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
}

public abstract class FiniteStateMachine<T> : MonoBehaviour where T : FiniteStateMachine<T>
{
    public AbstractState<T> initialState;
    [SerializeField]
    private AbstractState<T> currentState;

    public AbstractState<T> CurrentState { get => currentState; }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        ChangeState(initialState);
    }

    public void ChangeState(AbstractState<T> newState)
    {
        newState.fsm = (T)this;
        if (currentState != null)
        {
            currentState.OnExitState();
        }

        currentState = newState;

        if (currentState != null)
        {
            currentState.OnEnterState();
        }
    }

    private void Update()
    {
        currentState.UpdateState();
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState();
    }
}
