using UnityEngine;


//状态机，用于管理玩家的不同状态。通过Initialize方法设置初始状态，通过ChangeState方法在不同状态之间切换。每个状态都有Enter和Exit方法，用于处理状态的进入和退出逻辑
public class PlayerStateMachine
{

    public PlayerState currentState {  get; private set; } //可以公开读取但只能在类内部修改的属性,get前面是默认的public

    //初始化方法
    public void Initialize(PlayerState _startState)
    {  
        currentState = _startState;
        currentState.Enter();
    }
    
    //状态切换
    public void ChangeState(PlayerState _newState)
    {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
}
