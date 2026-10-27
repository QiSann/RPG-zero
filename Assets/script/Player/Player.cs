using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XInput;

public class Player : Entity
{

    [Header("攻击细节")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = .2f;

    public bool isBusy {  get; private set; }
    [Header("运动信息")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float swordReturnImpact;//在player里设置swordReturnImpact作为击退的参数
    private float defaultMoveSpeed;
    private float defaultJumpForce;

    [Header("冲刺信息")]
    public float dashSpeed;
    public float dashDuration;
    private float defaultDashSpeed;
    public float dashDir {  get; private set; }

    public SkillManager skill { get; private set; }
    public GameObject sword {  get; private set; }
    #region state

    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerPrimaryAttackState primaryAttackState { get; private set; }
    public PlayerCounterAttackState counterAttackState { get; private set; }
    public PlayerAimSwordState aimSwordState { get; private set; }
    public PlayerCatchSwordState catchSwordState { get; private set; }
    public PlayerBlackholeState blackholeState { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    #endregion
    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");
        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
        aimSwordState = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSwordState = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackholeState = new PlayerBlackholeState(this, stateMachine, "Jump");
        deadState = new PlayerDeadState(this, stateMachine, "Die");
    }

    protected override void Start()
    {
        base.Start();

        skill = SkillManager.instance;

        stateMachine.Initialize(idleState);

        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
    }

    protected override void Update()
    {
        if(Time.timeScale == 0)
        {
            return;
        }

        base .Update();

        stateMachine.currentState.Update();

        CheckForDashInput();

        if(Input.GetKeyDown(KeyCode.F) && skill.crystal.crystalUnlocked)
        {
            skill.crystal.CanUseSkill();
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Inventory.instance.UseFlask();
        }
    }

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1-_slowPercentage);
        jumpForce = jumpForce * (1-_slowPercentage);
        dashSpeed = dashSpeed * (1-_slowPercentage);
        anim.speed = anim.speed * (1-_slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);

    }
    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;
    }
    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSwordState);
        Destroy(sword);
    }

    //协程使用的时候需要用到play.StartCoroutine("协程的名字（BusyFor）",需要输入的数据(_seconds))
    //可以用于加在爬墙跳上，这样就可以同时按方向键和跳跃键了
    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;

        yield return new WaitForSeconds(_seconds);

        isBusy = false;
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    //设置一个计时器，当冲刺不在冷却时则可以进行冲刺
    private void CheckForDashInput()
    {
        if(IsWallDetected())
        {
            return;
        }


        if (Input.GetKeyDown(KeyCode.LeftShift)  && SkillManager.instance.dash.CanUseSkill() && !isKnocked) 
        {
            dashDir = Input.GetAxisRaw("Horizontal");

            if(dashDir == 0)
            {
                dashDir = facingDir;
            }
            stateMachine.ChangeState(dashState);
        }
    }
    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }

}