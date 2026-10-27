using Unity.VisualScripting;
using UnityEngine;

public class UI_ToolTip : MonoBehaviour
{
    [SerializeField] private float xLimit = 960;
    [SerializeField] private float yLimit = 540;

    [SerializeField] private float xOffset = 150;
    [SerializeField] private float yOffset = 150;
    public virtual void AdjustPosition()
    {

        Vector2 mousePosition = Input.mousePosition;

        float newXoffset = 0;
        float newYoffset = 0;


        if (mousePosition.x > xLimit)
            newXoffset = -xOffset;
        else
            newXoffset = xOffset;//鼠标靠近屏幕右侧时，提示框向左偏移；否则向右偏移

        if (mousePosition.y > yLimit)

            newYoffset = -yOffset;
        else
            newYoffset = yOffset;//鼠标靠近屏幕顶部时，提示框向下偏移；否则向上偏移


        transform.position = new Vector2(mousePosition.x + newXoffset, mousePosition.y + newYoffset);//更新提示框位置为鼠标位置偏移后的点
    }
}
