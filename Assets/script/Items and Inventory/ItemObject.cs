using UnityEngine;

//创建一个物体sprite，放入这个脚本
public class ItemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ItemData itemData;//就是你放入的图标

    private void SetupVisuals()//设置物品的图标和名称
    {
        if (itemData == null)
            return;

        GetComponent<SpriteRenderer>().sprite = itemData.icon;//在编辑器中显示图标
        gameObject.name = "Item object - " + itemData.itemName;//直接在编辑器中显示名字
    }


    public void SetupItem(ItemData _itemData, Vector2 _velocity)//配置物品的数据和掉落的初始速度
    {
        itemData = _itemData;
        rb.linearVelocity = _velocity;

        SetupVisuals();
    }
    public void PickupItem()
    {
        if(!Inventory.instance.CanAddItem() && itemData.itemType == ItemType.Equipment)
        {
            rb.linearVelocity = new Vector2(0, 7);
            PlayerManager.instance.player.fx.CreatePopUpText("库存已满");
            return;
        }

        AudioManager.instance.PlaySFX(8, transform);
        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }
}