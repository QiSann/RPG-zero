using System.Collections;
using UnityEngine;
 
public class Enemy_Shady : Enemy
{
 
    [Header("暗影法师特殊信息")]//Shady specific info
    public float battleStateMoveSpeed ;
 
    #region States
 
    public ShadyIdleState idleState { get; private set; }
    public ShadyMoveState moveState { get; private set; }
    public ShadyDeadState deadState { get; private set; }
    public ShadyBattleState battleState { get; private set; }
 
    #endregion
 
 
    protected override void Awake()
    {
        base.Awake();
 
        idleState = new ShadyIdleState(this, stateMachine, "Idle", this);
        moveState = new ShadyMoveState(this, stateMachine, "Move", this);
 
        deadState = new ShadyDeadState(this, stateMachine, "Dead", this);
 
        battleState = new ShadyBattleState(this, stateMachine, "Move", this);
 
    }
 
 
    protected override void Start()
    {
        base.Start();
 
        stateMachine.Initialize(idleState);
    }
 
 
 
    public override void Die()
    {
        base.Die();
 
        stateMachine.ChangeState(deadState);
    }
 
    //public override void AnimationSepcialAttackTrigger()
    //{
    //    GameObject newExplosive = Instantiate(explosivePrefab, transform.position, Quaternion.identity);
 
    //    newExplosive.GetComponent<Explosive_Controller>().SetupExplosive(stats, growSpeed, maxSize, attackDistance);
 
 
    //    cd.enabled = false;
    //    rb.gravityScale = 0;
    //}
 
    public void SelfDestroy() => Destroy(gameObject);
 
}