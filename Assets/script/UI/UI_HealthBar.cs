using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    private Entity entity;
    private CharacterStats myStats;
    private RectTransform myTransform;
    private Slider slider;

    private void Start()
    {
        myTransform = GetComponent<RectTransform>();
        entity = GetComponentInParent<Entity>();
        slider = GetComponentInChildren<Slider>();
        myStats = GetComponentInParent<CharacterStats>();


        entity.onFlipped += FlipUI;//; 这行代码的意思是将 FlipUI 方法订阅到 entity 对象的 onFlipped 事件
        myStats.onHealthChanged += UpdateHealthUI;

        UpdateHealthUI();
    }

    private void UpdateHealthUI()//更新血条值
    {
        slider.maxValue = myStats.GetMaxHealthValue();
        slider.value = myStats.currentHealth;
    }

    private void FlipUI() => myTransform.Rotate(0, 180, 0);
    private void OnDisable()
    {
        entity.onFlipped -= FlipUI;//这个即与+=相反，取消订阅
        myStats.onHealthChanged -= UpdateHealthUI;

    }
}
