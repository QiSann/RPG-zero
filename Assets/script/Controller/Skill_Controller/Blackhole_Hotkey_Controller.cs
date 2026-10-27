using TMPro;
using UnityEngine;

//管理单个热键的显示与检测
public class Blackhole_Hotkey_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode myHotKey;// 当前热键
    private TextMeshProUGUI myText;// 显示热键的文本组件

    private Transform myEnemy;
    private Blackhole_Skill_Controller blackHole;
    
    // 设置热键并更新显示
    public void SetupHotKey(KeyCode _myNewHotKey , Transform _myEnemy , Blackhole_Skill_Controller _blackHole)
    {
        sr = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshProUGUI>();// 获取子对象上的文本组件
        myEnemy = _myEnemy;
        blackHole = _blackHole;


        //TextMeshProUGUI.text为TextMeshProUGUI的文本组件
        myText.text = _myNewHotKey.ToString(); //将_myNewHotKey转化为字符串
        myHotKey = _myNewHotKey;
    }

    private void Update()
    {
        if(Input.GetKeyUp(myHotKey))
        {
            blackHole.AddEnemyToList(myEnemy);

            myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }
}
