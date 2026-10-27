using TMPro;
using UnityEngine;

public class UI_SkillToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI skillText;//显示技能描述的文本组件
    [SerializeField] private TextMeshProUGUI skillName;

    public void ShowToolTip(string _skillDescprtion, string _skillName)
    {
        skillName.text = _skillName;
        skillText.text = _skillDescprtion;

        AdjustPosition();

        gameObject.SetActive(true);
    }

    public virtual void HideToolTip() => gameObject.SetActive(false);
}