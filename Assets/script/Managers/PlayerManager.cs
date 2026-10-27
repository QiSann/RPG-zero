using UnityEngine;


//实现一个单例模式的PlayerManager类。主要目的是在整个Unity项目中提供一个全局唯一的PlayerManager实例，方便其他脚本访问和管理玩家相关的功能
public class PlayerManager : MonoBehaviour , ISaveManager
{
    public static PlayerManager instance;
    public Player player;

    public int currency;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            //将当前PlayerManager实例赋值给静态变量instance。这样，其他脚本可以通过PlayerManager.instance访问这个唯一的实例
            instance = this;
        }
    }

    public bool HaveEnoughMoney(int _price)//是否有钱去买技能
    {
        if (_price > currency)
        {
            Debug.Log("没有足够的钱");
            return false;
        }
        else
        {
            currency -= _price;
        }

        return true;

    }

    public int GetCurrency()
    {
        return currency;
    }

    public void LoadData(GameData _data)
    {
        currency = _data.currency;
    }

    public void SaveData(ref GameData _data)
    {
        _data.currency = currency;
    }
}
