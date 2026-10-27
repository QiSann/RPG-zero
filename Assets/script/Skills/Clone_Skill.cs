using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill : Skill
{
    [Header("克隆信息")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]
    [SerializeField] private bool canAttack = true;

    [SerializeField] private bool canCreateCloneOnCounterAttack;

    [Header("克隆复制")]
    [SerializeField] private float chanceToDuplicate;
    [SerializeField] private bool canDuplicateClone;

    [Header("水晶代替克隆")]
    public bool crystalInsteadOfClone;

    public void CreatClone(Transform _clonePosition , Vector3 _offset)
    {
        if(crystalInsteadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            SkillManager.instance.crystal.CurrentCrystalChooseRandomTarget();
            return;
        }


        GameObject newClone = Instantiate(clonePrefab);

        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_clonePosition , cloneDuration , canAttack ,  _offset , FindClosestEnemy(player.transform) , canDuplicateClone , chanceToDuplicate);
    }


    public void CreateCloneOnCounterAttack(Transform _enemyTransform)
    {
        if(canCreateCloneOnCounterAttack)
        {
            StartCoroutine(CreateCloneWithDelay(_enemyTransform , new Vector3(2 * player.facingDir, 0)));
        }
    }
    private IEnumerator CreateCloneWithDelay(Transform _transform , Vector3 _offset)
    {
        yield return new WaitForSeconds(.4f);
            CreatClone(_transform ,_offset );

    }
}
