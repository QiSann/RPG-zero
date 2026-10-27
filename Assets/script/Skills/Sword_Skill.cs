using UnityEngine;
using UnityEngine.UI;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}
public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;

    [Header("弹射信息")]
    [SerializeField] private UI_SkillTreeSlot bounceUnlockButton;//解锁按钮
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;

    [Header("穿刺信息")]
    [SerializeField] private UI_SkillTreeSlot pierceUnlockButton;//解锁按钮
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    [Header("旋转攻击信息")]
    [SerializeField] private UI_SkillTreeSlot spinUnlockButton;//解锁按钮
    [SerializeField] private float hitCooldown = .35f;
    [SerializeField] private float maxTravelDistance = 7;
    [SerializeField] private float spinDuration = 2;
    [SerializeField] private float spinGravity = 1;

    [Header("技能信息")]
    [SerializeField] private UI_SkillTreeSlot swordUnlockButton;//解锁按钮
    public bool swordUnlocked { get; private set; }//解锁
    [SerializeField] private GameObject swordPrefab;// 剑的预制体
    [SerializeField] private Vector2 launchForce;// 剑的发射力量
    [SerializeField] private float swordGravity;// 剑的重力
    [SerializeField] private float FreezeTimeDuration;
    [SerializeField] private float returnSpeed;


    private Vector2 finalDir;// 最终的方向向量

    [Header("用于瞄准的点")]
    [SerializeField] private int numberOfDots; //表示要生成的点的数量
    [SerializeField] private float spaceBetweenDots; //相邻两个点之间的间距
    [SerializeField] private GameObject dotPrefab; //点的预制体
    [SerializeField] private Transform dotsParent; //点的父对象

    private GameObject[] dots;


    protected override void Start()
    {
        base.Start();
        GenereateDots();// 在开始时生成瞄准点
        SetupGravity();

        swordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        bounceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBounceSword);
        pierceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPierceSword);
        spinUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSpinSword);

    }
    #region Unlock region  //解锁区域

    protected override void CheckUnlock()
    {
        base.CheckUnlock();
        UnlockSword();
        UnlockBounceSword();
        UnlockPierceSword();
        UnlockSpinSword();
    }
    private void UnlockSword()//解锁投掷武器
    {
        if (swordUnlockButton.unlocked)
        {
            swordType = SwordType.Regular;
            swordUnlocked = true;
        }
    }


    private void UnlockBounceSword()
    {
        if (bounceUnlockButton.unlocked)
            swordType = SwordType.Bounce;
    }


    private void UnlockPierceSword()
    {
        if (pierceUnlockButton.unlocked)
            swordType = SwordType.Pierce;
    }

    private void UnlockSpinSword()
    {
        if (spinUnlockButton.unlocked)
            swordType = SwordType.Spin;
    }
    #endregion

    private void SetupGravity()
    {
        switch(swordType)
        {
            case SwordType.Regular:
                break;
            case SwordType.Bounce:
                swordGravity = bounceGravity;break;
            case SwordType.Pierce:
                swordGravity = pierceGravity;break;
            case SwordType.Spin:
                swordGravity = spinGravity; break;
        }
    }
    protected override void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            // 计算最终的方向向量，乘以发射力量,这里normalized是将数据单位化
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
            }
        }
    }

    public void CreatSword()
    {
        // 实例化剑的预制体
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        switch (swordType)
        {
            case SwordType.Regular:
                break;
            case SwordType.Bounce:
                newSwordScript.SetupBounce(true, bounceAmount , bounceSpeed); break;
            case SwordType.Pierce:
                newSwordScript.SetupPierce(pierceAmount); break;
            case SwordType.Spin:
                newSwordScript.SetupSpin(true, maxTravelDistance, spinDuration , hitCooldown); break;
        }
        /**
        if (swordType == SwordType.Bounce)
        {
            newSwordScript.SetupBounce(true, bounceAmount);
        }
        else if(swordType == SwordType.Pierce)
        {
            newSwordScript.SetupPierce(pierceAmount);
        }
        */

            // 设置剑的参数：方向和重力
            newSwordScript.SetupSword(finalDir, swordGravity, player , FreezeTimeDuration , returnSpeed);

        player.AssignNewSword(newSword);

        DotsActive(false);
    }

    #region Aim region
    //计算从玩家位置到鼠标位置的方向向量，用于确定剑的飞行方向
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        //鼠标的坐标（屏幕坐标转世界坐标）
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;

        return direction;
    }
    //// 控制瞄准点的显示和隐藏
    public void DotsActive(bool _isActive)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }
    // 生成瞄准点
    private void GenereateDots()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            // 实例化点预制体，并设置其父对象,Quaternion.identity表示一个没有任何旋转的四元数
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    //匀加速公式 d=vt + 1/2at^2 这里的t是用点在数组里的位置近似表示时间
    private Vector2 DotsPosition(float t)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y) * t + .5f * (Physics2D.gravity * swordGravity) * (t * t);

        return position;
    }
    #endregion
}
