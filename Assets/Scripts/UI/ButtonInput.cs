using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ButtonInput : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] TMP_Text minValueText;
        [SerializeField] TMP_Text maxValueText;
        [SerializeField] ValueDepicted _valueDepicted;
        [SerializeField] Slider slider;
        [SerializeField] private float sliderSetting;
        [SerializeField] private BeginTurn turnButton;
        [SerializeField] private GameObject cursor;

        private void Start()
        {
            // move setting to start with
            minValueText.text = "-1 m/s^2";
            maxValueText.text = "1 m/s^2";
            slider.maxValue = 100;
            slider.minValue = -100;
            _valueDepicted.setting = 0;
            turnButton.OnSettingChanged(0);
            // set cursor
            if (sliderSetting == 0)
            {
                cursor.transform.position = transform.position;
            }
        }

        public void ModifySliderSetting()
        {
            // modify text and slider min and max values 
            switch (sliderSetting)
            {
                case 0:
                    minValueText.text = "-1 m/s^2";
                    maxValueText.text = "1 m/s^2";
                    slider.maxValue = 100;
                    slider.minValue = -100;
                    _valueDepicted.setting = 0;
                    turnButton.OnSettingChanged(0);
                    cursor.transform.position = transform.position;
                    break;
                case 1:
                    minValueText.text = "180 degrees counterclockwise";
                    maxValueText.text = "180 degrees clockwise";
                    slider.maxValue = 180;
                    slider.minValue = -180;
                    _valueDepicted.setting = 1;
                    // keyword salad, but it's looking for the z rotation of the player's barrel
                    // to use as the starting value
                    slider.value = 
                        GameManager.Instance.turnManager.playerRef.barrelPivot.transform.localRotation.z;
                    turnButton.OnSettingChanged(1);
                    cursor.transform.position = transform.position;
                    break;
                case 2:
                    minValueText.text = ".5 sec";
                    maxValueText.text = "2 sec";
                    slider.maxValue = 8;
                    slider.minValue = 2;
                    _valueDepicted.setting = 2;
                    turnButton.OnSettingChanged(2);
                    cursor.transform.position = transform.position;
                    break;
                
            }
        }
    }
}