using System;
using UnityEngine;
using UnityEngine.UI;

public class BeginTurn : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] private float sliderSetting; 
    
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        _button.enabled = false;
    }

    public void OnSettingChanged(float setting)
    {
        // allow turn to be taken
        _button.enabled = true;
        
        // set slider setting
        sliderSetting = setting;
    }
    
    public void TurnProxy()
    {
        switch (sliderSetting)
        {
            case 0:
                GameManager.Instance.turnManager.TurnTrigger(TurnManager.TurnAction.CHARGE, slider.value/100f);
                break;
            case 1:
                GameManager.Instance.turnManager.TurnTrigger(TurnManager.TurnAction.ANGLE, slider.value);
                break;
            case 2:
                GameManager.Instance.turnManager.TurnTrigger(TurnManager.TurnAction.LANDMINE, slider.value/4f);
                break;
        }
    }
}
