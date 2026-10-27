using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CraftList : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Transform craftSlotParent;//合成槽父物体
    [SerializeField] private GameObject craftSlotPrefab;//合成槽预制体

    [SerializeField] private List<ItemData_Equipment> craftEquipment;//合成装备列表


    void Start()
    {
        transform.parent.GetChild(0).GetComponent<UI_CraftList>().SetupCraftList();
        SetupDefaultCraftWindow();
    }


    public void SetupCraftList()
    {
        for (int i = 0; i < craftSlotParent.childCount; i++)
        {
            Destroy(craftSlotParent.GetChild(i).gameObject);
        }


        for (int i = 0; i < craftEquipment.Count; i++)
        {

            GameObject newSlot = Instantiate(craftSlotPrefab, craftSlotParent);
            newSlot.GetComponent<UI_CraftSlot>().SetupCraftSlot(craftEquipment[i]);

        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetupCraftList();
    }


    public void SetupDefaultCraftWindow()
    {
        if (craftEquipment[0] != null)
            GetComponentInParent<UI>().craftWindow.SetCraftWindow(craftEquipment[0]);
    }
}