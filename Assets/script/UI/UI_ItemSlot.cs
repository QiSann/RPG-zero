using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

//物品数量的UI，放在ItemSlot上
// p105添加了Grad Layout Group，自动排列


public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler , IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image itemImage;// 物品图标
    [SerializeField] protected TextMeshProUGUI itemText;// 显示堆叠数量的文本

    protected UI ui;
    public InventoryItem item;// 当前物品槽中所显示的物品

    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();
    }

    public void UpdataSlot(InventoryItem _newitem)//改成public然后注释掉Start
    {
        item = _newitem;

        itemImage.color = Color.white;//获取了物品，itemslot透明变成白色，变得可见

        if (item != null)//列表中的物体不是空的
        {
            itemImage.sprite = item.data.icon;//获得图标

            if (item.stackSize > 1)
            {
                itemText.text = item.stackSize.ToString();//显示堆叠数量
            }
            else
            {
                itemText.text = "";//不显示数量
            }
        }
    }

    public void CleanUpSlot()//清空物品槽
    {
        item = null;// 将物品槽内物品设为空
 
        itemImage.sprite = null;// 清除物品图标
        itemImage.color = Color.clear;// 让物品槽变得不可见
 
        itemText.text = ""; // 清空堆叠数量的显示
    }



    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (item != null)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                Inventory.instance.RemoveItem(item.data);
                return;
            }
            if (item.data.itemType == ItemType.Equipment)// 如果点击的物品是装备类型
            {
                Inventory.instance.EquipItem(item.data);//装备物品
            }

            ui.itemToolTip.HideToolTip();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item == null)
        {
            return;
        }

        Vector2 mousePosition = Input.mousePosition;

        float xOffset = 0;
        float yOffset = 0;


        if (mousePosition.x > 600)
            xOffset = -150;
        else
            xOffset = 150;//鼠标靠近屏幕右侧时，提示框向左偏移；否则向右偏移

        if (mousePosition.y > 320)

            yOffset = -150;
        else
            yOffset = 150;//鼠标靠近屏幕顶部时，提示框向下偏移；否则向上偏移

        ui.itemToolTip.ShowToolTip(item.data as ItemData_Equipment);
        ui.itemToolTip.transform.position = new Vector2(mousePosition.x + xOffset , mousePosition.y + yOffset);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null)
        {
            return;
        }
        ui.itemToolTip.HideToolTip();
    }

}