using UnityEngine;
using UnityEngine.UI;

public class Dash_Skill : Skill
{
    [Header("ÉÁ±ÜÎÞµÐÖ¡")]
    public bool dashUnlocked;
    [SerializeField] private UI_SkillTreeSlot dashUnlockedButton;

    [Header("ÉÁ±Ü¿ËÂ¡¹¥»÷")]
    public bool cloneOnDashUnlocked;
    [SerializeField] private UI_SkillTreeSlot cloneOnDashUnlockedButton;

    [Header("Ë«ÖØÉÁ±Ü¿ËÂ¡¹¥»÷")]
    public bool cloneOnArrivalUnlocked;
    [SerializeField] private UI_SkillTreeSlot cloneOnArrivalUnlockedButton;

    public override void UseSkill()
    {
        base.UseSkill();
    }

    protected override void Start()
    {
        base.Start();
        dashUnlockedButton.GetComponent<Button>().onClick.AddListener(UnlockDash);
        cloneOnDashUnlockedButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnDash);
        cloneOnArrivalUnlockedButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnArrival);
    }

    protected override void CheckUnlock()
    {
        base.CheckUnlock();
        UnlockDash();
        UnlockCloneOnDash();
        UnlockCloneOnArrival();
    }
    private void UnlockDash()
    {
        if(dashUnlockedButton.unlocked)
            dashUnlocked = true;
    }

    private void UnlockCloneOnDash()
    {
        if(cloneOnDashUnlockedButton.unlocked)
        {
            cloneOnDashUnlocked = true;
        }
    }

    private void UnlockCloneOnArrival()
    {
        if(cloneOnArrivalUnlockedButton.unlocked)
            cloneOnArrivalUnlocked = true;
    }
    public void CloneOnDashStart()
    {
        if (cloneOnDashUnlocked)
        {
            SkillManager.instance.clone.CreatClone(player.transform, Vector3.zero);
        }
    }
    public void CloneOnDashOver()
    {
        if (cloneOnArrivalUnlocked)
        {
            SkillManager.instance.clone.CreatClone(player.transform, Vector3.zero);
        }
    }
}
