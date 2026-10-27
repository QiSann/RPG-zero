using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int currency;
    public int hp;

    public SerializableDictionary<string, bool> skillTree;//技能树
    public SerializableDictionary<string, int> inventory;//物品的名字和数量
    public List<string> equipmentId;


    public SerializableDictionary<string, bool> checkpoints;
    public string closestCheckpointId;

    public SerializableDictionary<string, float> volumeSettings;
    public GameData()
    {
        this.currency = 0;
        this.hp = 0;
        skillTree = new SerializableDictionary<string, bool>();
        inventory = new SerializableDictionary<string, int>();
        equipmentId = new List<string>();

        closestCheckpointId = string.Empty;
        checkpoints = new SerializableDictionary<string, bool>();

        volumeSettings = new SerializableDictionary<string, float>();
    }

}