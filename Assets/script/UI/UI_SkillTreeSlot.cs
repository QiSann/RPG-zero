using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UI_SkillTreeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISaveManager
{
    private UI ui;
    private Image skillImage;//当前技能槽的图片组件

    [SerializeField] private int skillPrice;
    [SerializeField] private string skillName;
    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockedSkillColor;//技能未解锁时的颜色


    public bool unlocked;

    [SerializeField] private UI_SkillTreeSlot[] shouldBeUnlocked;//解锁前置条件
    [SerializeField] private UI_SkillTreeSlot[] shouldBeLocked;//锁定条件



    private void OnValidate()
    {
        gameObject.name = "SkillTreeSlot_UI - " + skillName;
    }


    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => UnlockSkillSlot());//给 Button 组件绑定点击事件，用于触发技能槽解锁逻辑

    }
    private void Start()
    {
        skillImage = GetComponent<Image>();
        ui = GetComponentInParent<UI>();

        skillImage.color = lockedSkillColor;//设置初始颜色为锁定颜色


        ui.skillToolTip.HideToolTip();

        if(unlocked)
        {
            skillImage.color = Color.white;
        }
    }

    public void UnlockSkillSlot()
    {
        if (PlayerManager.instance.HaveEnoughMoney(skillPrice) == false)//检查是否有足够的钱
        {
            return;
        }
        for (int i = 0; i < shouldBeUnlocked.Length; i++)//前置解锁检查
        {
            if (shouldBeUnlocked[i].unlocked == false)
            {
                Debug.Log("不能锁定技能");
                return;
            }
        }

        for (int i = 0; i < shouldBeLocked.Length; i++)//锁定检查
        {
            if (shouldBeLocked[i].unlocked == true)
            {
                Debug.Log("不能解锁技能");
                return;
            }
        }
        if(unlocked == true)
        {
            unlocked = false;
            skillImage.color = Color.gray;
        }
        else
        {
           unlocked = true;
            skillImage.color = Color.white;
        }
            

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(skillDescription, skillName);


        //Vector2 mousePosition = Input.mousePosition;
        //float xLimit = 960;
        //float yLimit = 540;
        //float xOffset = 150;
        //float yOffset = 150;
        //float newXoffset = 0;
        //float newYoffset = 0;


        //if (mousePosition.x > xLimit)
        //    newXoffset = -xOffset;
        //else
        //    newXoffset = xOffset;//鼠标靠近屏幕右侧时，提示框向左偏移；否则向右偏移

        //if (mousePosition.y > yLimit)

        //    newYoffset = -yOffset;
        //else
        //    newYoffset = yOffset;//鼠标靠近屏幕顶部时，提示框向下偏移；否则向上偏移


        //ui.skillToolTip.transform.position = new Vector2(mousePosition.x + newXoffset, mousePosition.y + newYoffset);//更新提示框位置为鼠标位置偏移后的点
        ui.skillToolTip.AdjustPosition();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.HideToolTip();//鼠标离开槽位时，隐藏技能描述提示框
    }

    public void LoadData(GameData _data)
    {
        if(_data.skillTree.TryGetValue(skillName, out bool value))
        {
            unlocked = value;
        }
    }

    public void SaveData(ref GameData _data)
    {
        if(_data.skillTree.TryGetValue(skillName, out bool value))
        {
            _data.skillTree.Remove(skillName);
            _data.skillTree.Add(skillName , unlocked);
        }
        else
        {
            _data.skillTree.Add(skillName , unlocked);
        }

    }
}