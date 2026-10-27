using System.Text;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif
public enum ItemType
{
    Material,
    Equipment
}
//fileName是创建的名字 ， menuName是创建所在的位置
[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]//在资源管理器中创建新的Item，创建的物品自带这个脚本

public class ItemData : ScriptableObject //在编辑器中创建和编辑的类
{
    public ItemType itemType;
    public string itemName;//定义你的物品的名字
    public Sprite icon;//可以选择图标
    public string itemId;

    [Range(0,100)]
    public float dropChance;
    protected StringBuilder sb = new StringBuilder();//创建一个字符串生成器


    private void OnValidate()
    {
#if UNITY_EDITOR//只在编辑器中运行
        string path = AssetDatabase.GetAssetPath(this);//获取这个物品的路径
        itemId = AssetDatabase.AssetPathToGUID(path);//获取这个物品的GUID
#endif
    }

    public virtual string GetDescription()//虚方法，返回一个字符串
    {
        return "";
    }
}