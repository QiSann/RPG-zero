using UnityEngine;


//用于设置背景随着摄像机移动
public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;

    [SerializeField] private float parallaxEffect;

    private float xPosition;
    private float length;

    void Start()
    {
        cam = GameObject.Find("Main Camera");

        //获取 SpriteRenderer 组件的边界在 X 轴上的尺寸大小
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        xPosition = transform.position.x;
    }

    void Update()
    {
        float distanceToMove =  cam.transform.position.x * parallaxEffect;
        float distanceMove =cam.transform.position.x * (1 - parallaxEffect);

        transform.position = new Vector3(xPosition + distanceToMove , transform.position.y);

        //实现无限背景
        if(distanceMove > xPosition +length)
        {
            xPosition = xPosition + length;
        }
        else if(distanceMove < xPosition - length)
        {
            xPosition = xPosition - length;
        }
    }
}
