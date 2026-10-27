using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//放在创建仓库的empyty对象上，就是仓库的运行函数
public class Inventory : MonoBehaviour,ISaveManager
{
    public static Inventory instance;//单例模式

    public List<ItemData> startingItems;
    //两种关键的存贮结构
    //List：存储玩家的装备、仓库物品、储藏室物品等
    //Dictionary：存储每个物品的数据以及物品在仓库中的具体信息，例如物品的堆叠数量
    public List<InventoryItem> equipment;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;

    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;


    //UI物品槽管理：通过以下代码将游戏中的物品槽和UI界面上的物品槽关联起来
    [Header("仓库UI")]//Inventory UI
    [SerializeField] private Transform inventorySlotParent;//位置
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSlotParent;
    [SerializeField] private Transform statSlotParent;


    //物品和材料的存贮位置分开
    private UI_ItemSlot[] inventoryItemSlot;
    private UI_ItemSlot[] stashItemSlot;//储藏室
    private UI_EquipmentSlot[] equipmentSlot;//装备
    private UI_StatSlot[] statSlot;

    [Header("物品冷却")]
    private float lastTimeUsedFlask;
    private float lastTimeUsedArmor;

    [Header("数据库")]
    public List<ItemData> itemDataBase;
    public List<InventoryItem> loadedItems;
    public List<ItemData_Equipment> loadedEquipment;
    public float flaskCooldown {  get; private set; }
    private float armorCooldown;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);//防止从一个地方到另一个地方
    }
    private void Start()//初始实例化
    {
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();


        //同时获取UI中对应的物品槽
        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipmentSlot = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();
        statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        AddStartingItems();
    }
    private void AddStartingItems()
    {
        foreach (ItemData_Equipment item in loadedEquipment)
        {
            EquipItem(item);
        }

        if (loadedItems.Count > 0)
        {
            foreach (InventoryItem item in loadedItems)
            {
                for (int i = 0; i < item.stackSize; i++)
                {
                    AddItem(item.data);
                }
            }

            return;
        }

        for (int i = 0; i < startingItems.Count; i++)
        {
            if(startingItems[i] != null)
            {
                AddItem(startingItems[i]);
            }
        }
    }
    public void EquipItem(ItemData _item)//把一个物品装备到角色身上
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;//把 _item 对象转换为 ItemData_Equipment 类型,表示它是一个装备物品
        InventoryItem newItem = new InventoryItem(newEquipment); //把 ItemData 对象转换为 InventoryItem 对象

        ItemData_Equipment oldEquipment = null;//要删除的物品


        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)//遍历装备字典
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)//如果装备类型相同
                oldEquipment = item.Key;//删除该装备
        }


        if (oldEquipment != null)
        {
            UnequipItem(oldEquipment);
            AddItem(oldEquipment);//把要删除的物品放回仓库
        }

        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers();

        RemoveItem(_item);

        UpdataSlotsUI();
    }
    public void UnequipItem(ItemData_Equipment itemToRemove)//移除装备函数
    {
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifiers();
        }
    }
    private void UpdataSlotsUI()//更新UI物体的数量
    {
        // 更新装备槽
        for (int i = 0; i < equipmentSlot.Length; i++)//将装备物品槽与一个装备字典中的物品进行匹配，并根据匹配结果更新物品槽的内容
        {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)//遍历装备字典
            {
                if (item.Key.equipmentType == equipmentSlot[i].slotType)//这个条件用于确保物品能放入正确的槽位中
                    equipmentSlot[i].UpdataSlot(item.Value);
            }
        }


        // 清空并更新仓库和储藏室的物品槽
        for (int i = 0; i < inventoryItemSlot.Length; i++)//仓库物品槽
        {
            inventoryItemSlot[i].CleanUpSlot();
        }
        for (int i = 0; i < stashItemSlot.Length; i++)//储藏室中的物品槽
        {
            stashItemSlot[i].CleanUpSlot();
        }


        // 重新填充仓库和储藏室
        for (int i = 0; i < inventory.Count; i++)
        {
            inventoryItemSlot[i].UpdataSlot(inventory[i]);
        }
        for (int i = 0; i < stash.Count; i++)
        {
            stashItemSlot[i].UpdataSlot(stash[i]);
        }
        for (int i = 0; i < statSlot.Length; i++)
        {
            statSlot[i].UpdateStatValueUI();
        }
    }
    public void AddItem(ItemData _item)//据物品类型，将物品添加到仓库（inventory）或者储藏室（stash）
    {
        if (_item.itemType == ItemType.Equipment && CanAddItem())
        {
            AddToInventory(_item);
        }
        else if (_item.itemType == ItemType.Material)
        {
            AddToStash(_item);
        }

        UpdataSlotsUI();
    }
    private void AddToStash(ItemData _item)
    {
        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
        }
    }
    private void AddToInventory(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))//字典中检查仓库中是否有这个物品，具体查找的是ItemData，out InventoryItem value如果找，返回与该键相关联的值
        {
            value.AddStack();//如果物品已经存在于库存中，则增加其堆叠数量
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);//如果物品不存在，则创建一个新的 InventoryItem
            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }
    }//检查物品是否已经在仓库里，如果存在则增加堆叠数量，如果不存在则创建新的物品对象并加入仓库
    public void RemoveItem(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            if (value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(_item);
            }
            else
            {
                value.RemoveStack();
            }
        }

        if (stashDictionary.TryGetValue(_item, out InventoryItem stashValue))
        {
            if (stashValue.stackSize <= 1)//如果物品的堆叠数量小于等于1，则从库存中删除该物品
            {
                stash.Remove(stashValue);
                stashDictionary.Remove(_item);
            }
            else
            {
                stashValue.RemoveStack();//否则就减少堆寨数量
            }
        }
        UpdataSlotsUI();
    }
    public bool CanAddItem()
    {
        if (inventory.Count >= inventoryItemSlot.Length)
        {
            Debug.Log("仓库已满");
            return false;
        }
        return true;
    }
    public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requiredMaterials)//判断是否可以合成的函数
    {
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();

        for (int i = 0; i < _requiredMaterials.Count; i++)
        {
            if (stashDictionary.TryGetValue(_requiredMaterials[i].data, out InventoryItem stashValue))//如果储藏室中没有所需材料
            {
                if (stashValue.stackSize < _requiredMaterials[i].stackSize)//数量是否足够
                {
                    Debug.Log("没有足够的材料");
                    return false;
                }
                else
                {
                    materialsToRemove.Add(stashValue);
                }
            }
            else
            {
                Debug.Log("没有足够的材料");
                return false;
            }
        }

        for (int i = 0; i < materialsToRemove.Count; i++)//使用了就从临时仓库中移除
        {
            RemoveItem(materialsToRemove[i].data);
        }

        AddItem(_itemToCraft);
        Debug.Log("这里是你的物品" + _itemToCraft.name);

        return true;

    }
    public List<InventoryItem> GetEquipmentList() => equipment;
    public List<InventoryItem> GetStashList() => stash;
    public ItemData_Equipment GetEquipment(EquipmentType _type)
    {
        ItemData_Equipment equipedItem = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)//遍历装备字典
        {
            if (item.Key.equipmentType == _type)//如果装备类型相同
                equipedItem = item.Key;
        }

        return equipedItem;

    }
    public void UseFlask()//使用药水的函数
    {
        ItemData_Equipment currentFlask = GetEquipment(EquipmentType.Flask);//获取当前的药水

        if (currentFlask == null)
            return;


        bool canUseFlask = Time.time > lastTimeUsedFlask + flaskCooldown;//判断是否可以使用药水

        if (canUseFlask)
        {
            flaskCooldown = currentFlask.itemCooldown;
            currentFlask.Effect(null);
            lastTimeUsedFlask = Time.time;
        }
        else
            Debug.Log("药水冷却中");
    }
    public bool CanUseArmor()
    {
        ItemData_Equipment currentArmor = GetEquipment(EquipmentType.Armor);

        if (Time.time > lastTimeUsedArmor + armorCooldown)
        {
            armorCooldown = currentArmor.itemCooldown;
            lastTimeUsedArmor = Time.time;
            return true;
        }

        return false;
    }

    public void LoadData(GameData _data)
    {
        foreach (KeyValuePair<string, int> pair in _data.inventory)
        {
            foreach (var item in itemDataBase)
            {
                if (item != null && item.itemId == pair.Key)
                {
                    InventoryItem itemToLoad = new InventoryItem(item);
                    itemToLoad.stackSize = pair.Value;

                   loadedItems.Add(itemToLoad);
                }
            }
        }

        foreach(string loadedItemId in _data.equipmentId)
        {
            foreach (var item in itemDataBase)
            {
                if(item != null && item.itemId == loadedItemId)
                {
                    loadedEquipment.Add(item as ItemData_Equipment);
                }
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.inventory.Clear();
        _data.equipmentId.Clear();

        foreach (KeyValuePair<ItemData, InventoryItem> pair in inventoryDictionary)
        {
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        foreach (KeyValuePair<ItemData, InventoryItem> pair in stashDictionary)
        {
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> pair in equipmentDictionary)
        {
            _data.equipmentId.Add(pair.Key.itemId);
        }
    }

    #if UNITY_EDITOR
    [ContextMenu("填充数据")]
    private void FillUpItemDataBase() => itemDataBase = new List<ItemData>(GetItemDataBase());
    private List<ItemData> GetItemDataBase()//获取物品数据库
    {
        List<ItemData> itemDatabase = new List<ItemData>();
        string[] assetNames = AssetDatabase.FindAssets("", new[] { "Assets/Data/Items" });

        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOpath);
            itemDatabase.Add(itemData);
        }

        return itemDatabase;

    }
#endif
}