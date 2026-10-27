using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//玩家死亡掉落装备函数
public class PlayerItemDrop : ItemDrop
{
    [Header("玩家掉落")]//player drop
    [SerializeField] private float chanceToLooseItems;
    [SerializeField] private float chanceToLooseMaterials;

    public override void GenerateDrop()//玩家产生掉落的函数
    {
        //先检查玩家身上有什么物品
        Inventory inventory = Inventory.instance;

        //掉落的列表
        //List<InventoryItem> itemsToUnequip = new List<InventoryItem>();//装备掉落
        List<InventoryItem> materulsToLoose = new List<InventoryItem>();//材料掉落


        //然后判断是否掉落
        //foreach (InventoryItem item in inventory.GetEquipmentList())
        //{
        //    if (Random.Range(0, 100) <= chanceToLooseItems)//掉落概率
        //    {
        //        DropItem(item.data);
        //        itemsToUnequip.Add(item);
        //    }
        //}


        //for (int i = 0; i < itemsToUnequip.Count; i++)
        //{
        //    inventory.UnequipItem(itemsToUnequip[i].data as ItemData_Equipment);
        //}



        foreach (InventoryItem item in inventory.GetStashList())
        {
            if (Random.Range(0, 100) <= chanceToLooseMaterials)//掉落概率
            {
                DropItem(item.data);
                materulsToLoose.Add(item);
            }
        }


        for (int i = 0; i < materulsToLoose.Count; i++)
        {
            inventory.RemoveItem(materulsToLoose[i].data);
        }

    }
}