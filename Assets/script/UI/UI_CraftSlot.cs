using UnityEngine.EventSystems;

//函数放在新的创建Craft panel的itemSlot里
public class UI_CraftSlot : UI_ItemSlot//继承物品槽UI
{
    protected override void Start()
    {
        base.Start();
    }

    public void SetupCraftSlot(ItemData_Equipment _data)//设置合成槽
    {

        if (_data == null)
            return;

        item.data = _data;

        itemImage.sprite = _data.icon;//设置图标
        itemText.text = _data.itemName;//设置名字

        if (itemText.text.Length > 12)
            itemText.fontSize = itemText.fontSize * .7f;
        else
            itemText.fontSize = 24;
    }

    private void OnValidate()
    {

    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        //ItemData_Equipment craftData = item.data as ItemData_Equipment;

        //Inventory.instance.CanCraft(craftData, craftData.craftingMaterials);//检查是否可以制作该物品

        ui.craftWindow.SetCraftWindow(item.data as ItemData_Equipment);//设置合成窗口
    }
}