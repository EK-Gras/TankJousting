using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ValueDepicted : MonoBehaviour
{
    [SerializeField] Slider slider;
    private TMP_Text _text;
    public int setting;
    public float value;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }
    
    

    // Update is called once per frame
    void Update()
    {
        value = slider.value;
        switch (setting)
        {
            case 0:
                _text.text = $"Charge Acceleration: {value/100f}";
                break;
            case 1:
                _text.text = $"Degrees Rotated: {value}";
                break;
            case 2:
                _text.text = $"Time until detonation: {value/4f}";
                break;
        }
    }
}
