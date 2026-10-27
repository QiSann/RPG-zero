using UnityEngine;

public class ShockStrike_Controller : MonoBehaviour
{
    [SerializeField] private CharacterStats targetsStats;
    [SerializeField] private float speed;
    private int damage;

    private Animator anim;
    private bool triggered;

    public void Setup(int _damage, CharacterStats _targetStats)
    {
        damage = _damage;
        targetsStats = _targetStats;
    }

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!targetsStats)
        {
            return;
        }

        if (triggered)
        {
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, targetsStats.transform.position, speed * Time.deltaTime);
        transform.right = transform.position - targetsStats.transform.position;
        //https://docs.unity3d.com/cn/current/ScriptReference/Transform-right.html
        //世界空间中变换的红轴

        if (Vector2.Distance(transform.position, targetsStats.transform.position) < .1f)
        {
            //相对于父级变换坐标和旋转
            anim.transform.localRotation = Quaternion.identity;
            anim.transform.localPosition = new Vector3(0, .5f);

            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(3, 3);


            triggered = true;

            Invoke("DamageAmdSelfDestroy", .2f);
            anim.SetTrigger("Hit");

        }
    }

    private void DamageAmdSelfDestroy()
    {
        targetsStats.ApplyShock(true);
        Destroy(gameObject, .4f);
        targetsStats.TakeDamage(1);
    }

}
