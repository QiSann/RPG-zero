using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private int possibleItemDrop;
    [SerializeField] private ItemData[] possibleDrop;
    private List<ItemData> dropList = new List<ItemData>();

    [SerializeField] private GameObject dropPrefab;


    public virtual void GenerateDrop()
    {
        for (int i = 0; i < possibleDrop.Length; i++)
        {
            if (Random.Range(0, 100) <= possibleDrop[i].dropChance)//生成掉落列表
                dropList.Add(possibleDrop[i]);

        }
        if(dropList.Count >= possibleItemDrop)
        {
            for (int i = 0; i < possibleItemDrop; i++)// 从掉落列表中随机选择物品并生成掉落
            {
                ItemData randomItem = dropList[Random.Range(0, dropList.Count - 1)];

                dropList.Remove(randomItem);
                DropItem(randomItem);
            }
        }
        else
        {
            if(dropList.Count == 0)
            {
                return;
            }
            for (int i = 0; i < possibleItemDrop; i++)// 从掉落列表中随机选择物品并生成掉落
            {
                ItemData randomItem = dropList[Random.Range(0, dropList.Count - 1)];
                DropItem(randomItem);
            }
        }

    }


    public void DropItem(ItemData _itemData)//敌人死亡时调用这个函数就行
    {
        GameObject newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);

        Vector2 randomVelocity = new Vector2(Random.Range(-5, 5), Random.Range(15, 20));//随机掉落方向

        newDrop.GetComponent<ItemObject>().SetupItem(_itemData, randomVelocity);
    }

}