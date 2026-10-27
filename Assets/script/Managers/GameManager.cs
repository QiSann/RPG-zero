using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance;//单例模式,全局变量
    [SerializeField] private Checkpoint[] checkpoints;
    [SerializeField] private string closestCheckpointId;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;

        checkpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
    }
    public void RestartScene()
    {
        SaveManager.instance.SaveGame();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void LoadData(GameData _data) => StartCoroutine(LoadWithDelay(_data)); //延迟调用PlacePlayerAtClosetCheckpoint方法

    private IEnumerator LoadWithDelay(GameData _data)
    {
        yield return new WaitForSeconds(.1f);

        LoadCheckpoints(_data);
        LoadClosestCheckpoint(_data);
    }
    private void LoadClosestCheckpoint(GameData _data)
    {
        closestCheckpointId = _data.closestCheckpointId; ;//将最近检查点ID存入变量

        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (closestCheckpointId == checkpoint.id)
                PlayerManager.instance.player.transform.position = checkpoint.transform.position;
        }
    }
    private void LoadCheckpoints(GameData _data)
    {
        foreach (KeyValuePair<string, bool> pair in _data.checkpoints)
        {
            foreach (Checkpoint checkpoint in checkpoints)
            {
                if (checkpoint.id == pair.Key && pair.Value)
                {
                    checkpoint.ActivateCheckPoint();
                }
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        if(FindClosestCheckpoint() != null)
        {
            _data.closestCheckpointId = FindClosestCheckpoint().id;//将最近的检查点ID存入数据
        }
        _data.checkpoints.Clear();

        foreach(Checkpoint checkpoint in checkpoints)
        {
            _data.checkpoints.Add(checkpoint.id , checkpoint.activationStatus);
        }
    }

    private Checkpoint FindClosestCheckpoint()
    {
        float closestDistance = Mathf.Infinity;//正无穷
        Checkpoint closestCheckpoint = null;

        foreach (var checkpoint in checkpoints)//遍历所有的检查点
        {
            float distanceToCheckpoint = Vector2.Distance(PlayerManager.instance.player.transform.position, checkpoint.transform.position);//计算玩家和检查点之间的距离

            if (distanceToCheckpoint < closestDistance && checkpoint.activationStatus == true)//如果距离小于最近距离且检查点激活
            {
                closestDistance = distanceToCheckpoint;//更新最近距离
                closestCheckpoint = checkpoint;//更新最近检查点
            }

        }
        return closestCheckpoint;
    }

    public void PauseGame(bool _pause)//游戏暂停
    {
        if(_pause)
        {
            Time.timeScale = 0;//timeScale为0时，所有基于帧率的功能都将被暂停
        }
        else
        {
            Time.timeScale = 1;
        }

    }
}