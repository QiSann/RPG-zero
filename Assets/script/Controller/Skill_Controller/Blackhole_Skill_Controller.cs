using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;// 热键预制体
    [SerializeField] private List<KeyCode> keyCodeList;//可用热键列表

    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;
    private float blackholeTimer;

    private bool canGrow = true;
    private bool canShrink;
    private bool canCreateHotKeys = true;
    private bool cloneAttackReleased;
    private bool playerCanDisapear = true;

    private int amountOfAttacks = 4;
    private float cloneAttackCooldown = .3f;
    private float cloneAttackTimer;

    private List<Transform> target = new List<Transform>();// 目标列表
    private List<GameObject> createdHotKey = new List<GameObject>();

    public bool playerCanExitState {  get; private set; }

    public void SetupBlackhole(float _maxSize , float _growSpeed , float _shrinkSpeed , int _amountOfAttacks , float _cloneAttackCooldown , float _blackholeDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountOfAttacks;
        cloneAttackCooldown = _cloneAttackCooldown;
        blackholeTimer = _blackholeDuration;

        if(SkillManager.instance.clone.crystalInsteadOfClone)
        {
            playerCanDisapear = false;
        }
    }

    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackholeTimer -= Time.deltaTime;

        if(blackholeTimer < 0 )
        {
            blackholeTimer = Mathf.Infinity;

            if(target.Count > 0)
            {
                ReleaseCloneAttack();
            }
            else
            {
                FinishiBlackHoleAbility();
            }
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            ReleaseCloneAttack();
        }

        CloneAttackLogic();

        if (canGrow && !canShrink)
        {
            // 逐渐放大到最大尺寸
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);
            if (transform.localScale.x < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void ReleaseCloneAttack()
    {
        if(target.Count <= 0 )
        {
            return;
        }

        DestroyHotKeys();
        cloneAttackReleased = true;
        canCreateHotKeys = false;

        if(playerCanDisapear)
        {
            playerCanDisapear = false;
            PlayerManager.instance.player.fx.MakeTransprent(true);
        }
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer <= 0 && cloneAttackReleased && amountOfAttacks > 0)
        {
            cloneAttackTimer = cloneAttackCooldown;

            int randomIndex = Random.Range(0, target.Count);

            float xOffset;

            if (Random.Range(0, 100) > 50)
            {
                xOffset = 2;
            }
            else
            {
                xOffset = -2;
            }

            if(SkillManager.instance.clone.crystalInsteadOfClone)
            {
                SkillManager.instance.crystal.CreateCrystal();
                SkillManager.instance.crystal.CurrentCrystalChooseRandomTarget();
            }
            else
            {
                SkillManager.instance.clone.CreatClone(target[randomIndex], new Vector3(xOffset, 0));
            }

            amountOfAttacks--;
            if (amountOfAttacks <= 0)
            {
                cloneAttackReleased = false;

                Invoke("FinishiBlackHoleAbility" , 1f);
            }
        }
    }

    private void FinishiBlackHoleAbility()
    {
        DestroyHotKeys();

        playerCanExitState = true;
        canShrink = true;
        cloneAttackReleased = false;

        //layerManager.instance.player.ExitBlackHoleAbility();
        //这个函数是用于切换player的状态
    }

    private void DestroyHotKeys()
    {
        if(createdHotKey.Count <= 0)
        {
            return;
        }
        for(int i = 0; i < createdHotKey.Count; i++)
        {
            Destroy(createdHotKey[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>()  != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);

            CreatHotKey(collision);
        }
    }

    //这个写法和下面注释的写法意思是一样的
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(false);
        }
    }

    //private void OnTriggerExit2D(Collider2D collision) => collision.GetComponent<Enemy>()?.FreezeTime(false);
 

    private void CreatHotKey(Collider2D collision)
    {
        if(keyCodeList.Count <= 0)
        {
            Debug.Log("没有键位给你用辣,预设的五个全用完辣");
            return;
        }
        if(!canCreateHotKeys)
        {
            return;
        }

        // 实例化热键UI,Quaternion.identity为对象与世界轴或父轴完全对齐
        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);
        createdHotKey.Add(newHotKey);

        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);

        Blackhole_Hotkey_Controller newHotKeyScript = newHotKey.GetComponent<Blackhole_Hotkey_Controller>();

        newHotKeyScript.SetupHotKey(choosenKey, collision.transform, this);
        //target.Add(collision.transform);
    }

    public void AddEnemyToList(Transform _enemyTransform) => target.Add(_enemyTransform);
}
