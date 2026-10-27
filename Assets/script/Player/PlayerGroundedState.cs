using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(Input.GetKeyDown(KeyCode.R))
        {
            if(player.skill.blackhole.cooldownTimer > 0)
            {
                player.fx.CreatePopUpText("技能冷却中");
                return;
            }

            stateMachine.ChangeState(player.blackholeState);
        }

        if(Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword() && SkillManager.instance.sword.swordUnlocked)
        {
            stateMachine.ChangeState(player.aimSwordState);
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            stateMachine.ChangeState(player.counterAttackState);
        }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            stateMachine.ChangeState(player.primaryAttackState);
        }

        //不在地上
        if(!player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.airState);
        }
        //按下空格并且在地上
        if(Input.GetKeyDown(KeyCode.Space)  &&  player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.jumpState);
        }
    }

    private bool HasNoSword()
    {
        if(!player.sword)
        {
            return true;
        }

        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();

        return false;
    }
}
