using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    public Enemy_Skeleton skeleton { get; private set; }
 
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            //将当前PlayerManager实例赋值给静态变量instance。这样，其他脚本可以通过PlayerManager.instance访问这个唯一的实例
            instance = this;
        }
    }

    private void Start()
    {
        skeleton = GetComponent<Enemy_Skeleton>();
    }
}
