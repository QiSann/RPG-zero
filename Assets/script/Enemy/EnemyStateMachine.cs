using UnityEngine;

public class EnemyStateMachine
{
    public EnemyState currentState {  get; private set; }

    public void Initialize(EnemyState _StartState)
    {
        currentState = _StartState;
        currentState.Enter();
    }

    public void ChangeState(EnemyState _newState)
    {
        currentState.Exit();
        currentState =_newState;
        currentState.Enter();
    }
}
