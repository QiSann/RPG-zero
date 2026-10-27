using System.Collections;
using UnityEngine;

public class PlayerStats : CharacterStats , ISaveManager
{
    Player player;
    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }

    protected override void Update()
    {
        base.Update();
    }
    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();
        player.Die();

        GetComponent<PlayerItemDrop>().GenerateDrop();
    }
    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

        ItemData_Equipment currentArmor = Inventory.instance.GetEquipment(EquipmentType.Armor);
        if (currentArmor != null)
        {
            currentArmor.Effect(player.transform);
        }
    }

    public void LoadData(GameData _data)
    {
        StartCoroutine(LoadHP(_data.hp));
    }

    private IEnumerator LoadHP(int _hp)
    {
        yield return new WaitForSeconds(.2f);
        if(_hp <= 0)
        {
            currentHealth = GetMaxHealthValue();
        }
        else
        {
            currentHealth = _hp;
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.hp = currentHealth;
    }
}
