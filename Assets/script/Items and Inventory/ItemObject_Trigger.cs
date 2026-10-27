using UnityEngine;

public class ItemObject_Trigger : MonoBehaviour
{
    private ItemObject myItemObject => GetComponentInParent<ItemObject>();
    private void OnTriggerEnter2D(Collider2D collision)//碰到就会拾取
    {

        if (collision.GetComponent<Player>() != null)
        {
            if(collision.GetComponent<CharacterStats>().isDead)
            {
                return;
            }
            myItemObject.PickupItem();
        }
    }
}
