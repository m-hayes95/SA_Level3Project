using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    // Protected will make this state public to any classes that implement it
    protected State currentState;

    public void InitializeState(State startState)
    {
        currentState = startState;
        currentState.Enter();
    }
    public void ChangeState(State newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
