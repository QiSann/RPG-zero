using Unity.VisualScripting;
using UnityEngine;

//状态基类
public class PlayerState
{
    protected PlayerStateMachine stateMachine; //对状态机的引用，用于在状态之间切换
    protected Player player; //对玩家对象的引用，用于访问玩家的相关属性和方法
    protected Rigidbody2D rb;

    protected float xInput;
    protected float yInput;
    private string animBoolName; //用于控制动画状态

    protected float stateTimer; //计时器
    protected bool triggerCalled;

    //初始化
    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
    {
        this.stateMachine = _stateMachine;
        this.player = _player;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        player.anim.SetBool(animBoolName,true);
        rb = player.rb;
        triggerCalled = false;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;

        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        player.anim.SetFloat("yVelocity",rb.linearVelocity.y);
    }

    public virtual void Exit() 
    {
        player.anim.SetBool(animBoolName,false);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }
}
