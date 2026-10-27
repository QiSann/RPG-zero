using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_VolumeSlider : MonoBehaviour
{
    public Slider slider;
    public string parametr;

    [SerializeField] private AudioMixer audioMixer;

    public void SliderValue(float _value)
    {
        audioMixer.SetFloat(parametr, _value * 100 - 80);
    }

    public void LoadSlider(float _value)
    {
        if(_value >= 0f)
        {
            slider.value = _value;
        }
    }
}
