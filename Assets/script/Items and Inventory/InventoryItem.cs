using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public ItemData data;//调用物品数据
    public int stackSize;


    public InventoryItem(ItemData _newItemData)//获取物品数据的函数
    {
        data = _newItemData;//获取物体的数据，data目前有名字和图表
        AddStack();//增加堆叠数量
    }

    public void AddStack() => stackSize++;//stack堆栈
    public void RemoveStack() => stackSize--;

}

