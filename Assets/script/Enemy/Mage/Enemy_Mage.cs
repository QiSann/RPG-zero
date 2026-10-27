using UnityEngine;

public class Enemy_Mage : Enemy
{
    #region State

    public MageIdleState idleState { get; private set; }
    public MageMoveState moveState { get; private set; }
    public MageBattleState battleState { get; private set; }
    public MageAttackState attackState { get; private set; }
    public MageStunnedState stunnedState { get; private set; }
    public MageDeadState deadState { get; private set; }

    #endregion

    [Header("Mage Settings")]
    [SerializeField] private GameObject magicProjectilePrefab;
    [SerializeField] public float retreatDistance = 3f; // 保持距离
    [SerializeField] private float spellCastDuration = 1.5f;

    public GameObject GetMagicProjectile() => magicProjectilePrefab;
    public float GetSpellCastDuration() => spellCastDuration;

    protected override void Awake()
    {
        base.Awake();

        idleState = new MageIdleState(this, stateMachine, "Idle", this);
        moveState = new MageMoveState(this, stateMachine, "Move", this);
        battleState = new MageBattleState(this, stateMachine, "Battle", this);
        attackState = new MageAttackState(this, stateMachine, "Attack", this);
        stunnedState = new MageStunnedState(this, stateMachine, "Stunned", this);
        deadState = new MageDeadState(this, stateMachine, "Dead", this);
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.U))
        {
            stateMachine.ChangeState(stunnedState);
        }
    }

    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            stateMachine.ChangeState(stunnedState);
            return true;
        }
        return false;
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }
}
