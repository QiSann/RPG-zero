using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("水晶幻象")]
    [SerializeField] private UI_SkillTreeSlot unlockCloneInsteadButton;
    [SerializeField] private bool cloneInsteadOfCrystal;

    [Header("水晶")]
    [SerializeField] private UI_SkillTreeSlot unlockCrystalButton;//解锁水晶按钮
    public bool crystalUnlocked {  get; private set; }

    [Header("水晶爆炸")]
    [SerializeField] private UI_SkillTreeSlot unlockExplosiveButton;//解锁爆炸水晶按钮
    [SerializeField] private bool canExplode;

    [Header("水晶移动")]
    [SerializeField] private UI_SkillTreeSlot unlockMovingCrystalButton;//解锁移动水晶按钮
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;

    [Header("多个水晶")]
    [SerializeField] private UI_SkillTreeSlot unlockMultiStackButton;//解锁多重水晶按钮
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();

    protected override void Start()
    {
        base.Start();

        unlockCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        unlockCloneInsteadButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalMirage);
        unlockExplosiveButton.GetComponent<Button>().onClick.AddListener(UnlockExplosiveCrystal);
        unlockMovingCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockMovingCrystal);
        unlockMultiStackButton.GetComponent<Button>().onClick.AddListener(UnlockMultiStackCrystal);

    }


    #region Unlock skill region

    protected override void CheckUnlock()
    {
        base.CheckUnlock();
        UnlockCrystal();
        UnlockCrystalMirage();
        UnlockExplosiveCrystal();
        UnlockMovingCrystal();
        UnlockMultiStackCrystal();
    }
    private void UnlockCrystal()
    {
        if (unlockCrystalButton.unlocked)
            crystalUnlocked = true;
    }

    private void UnlockCrystalMirage()
    {
        if (unlockCloneInsteadButton.unlocked)
            cloneInsteadOfCrystal = true;
    }


    private void UnlockExplosiveCrystal()
    {
        if (unlockExplosiveButton.unlocked)
            canExplode = true;
    }


    private void UnlockMovingCrystal()
    {
        if (unlockMovingCrystalButton.unlocked)
            canMoveToEnemy = true;
    }

    private void UnlockMultiStackCrystal()
    {
        if (unlockMultiStackButton.unlocked)
            canUseMultiStacks = true;
    }
    #endregion

    public override void UseSkill()
    {
        base.UseSkill();

        if(CanUseMultiCrystal())
        {
            return;
        }

        RefilCrystal();

        if(currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            if(canMoveToEnemy)
            {
                return;
            }

            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;
            if(cloneInsteadOfCrystal)
            {
                SkillManager.instance.clone.CreatClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();

            }

        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();

        currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform));
    }

    public void CurrentCrystalChooseRandomTarget() => currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRadomEnemy();
    private bool CanUseMultiCrystal()
    {
        if(canUseMultiStacks)
        {
            if(crystalLeft.Count > 0)
            {
                if(crystalLeft.Count == amountOfStacks)
                {
                    //这里useTimeWindow是用于控制技能持续时间，即当过了这个时间后即使你的水晶没有用完也依旧进入冷却
                    Invoke("ResetAbility", useTimeWindow);
                }

                cooldown = 0;
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn , player.transform.position, Quaternion.identity);

                crystalLeft.Remove(crystalToSpawn);

                newCrystal.GetComponent<Crystal_Skill_Controller>().SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(newCrystal.transform));

                //这里是当水晶用完时直接进入冷却时间
                if (crystalLeft.Count <= 0)
                {
                    //技能冷却
                    //补充水晶
                    cooldown = multiStackCooldown;
                    RefilCrystal();
                }
                return true;
            }

        }
        return false;
    }

    private void RefilCrystal()
    {
        int amountToAdd = amountOfStacks-crystalLeft.Count;

        for(int i = 0; i < amountToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    private void ResetAbility()
    {
        if(cooldownTimer > 0)
        {
            return;
        }

        cooldownTimer = multiStackCooldown;
        RefilCrystal();
    }
}
