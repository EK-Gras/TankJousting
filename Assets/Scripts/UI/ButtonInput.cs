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

        public void ModifySliderSetting()
        {
            // modify text and slider min and max values 
            switch (sliderSetting)
            {
                case 0:
                    titleText.text = "charge speed";
                    minValueText.text = "-1 m/s";
                    maxValueText.text = "1 m/s";
                    slider.maxValue = 100;
                    slider.minValue = -100;
                    _valueDepicted.setting = 0;
                    turnButton.OnSettingChanged(0);
                    break;
                case 1:
                    titleText.text = "barrel angle";
                    minValueText.text = "-180 degrees";
                    maxValueText.text = "180 degrees";
                    slider.maxValue = 180;
                    slider.minValue = -180;
                    _valueDepicted.setting = 1;
                    // keyword salad, but it's looking for the z rotation of the player's barrel
                    // to use as the starting value
                    slider.value = 
                        GameManager.Instance.turnManager.playerRef.barrelPivot.transform.localRotation.z;
                    turnButton.OnSettingChanged(1);
                    break;
                case 2:
                    titleText.text = "fuse timer";
                    minValueText.text = ".5 sec";
                    maxValueText.text = "2 sec";
                    slider.maxValue = 8;
                    slider.minValue = 2;
                    _valueDepicted.setting = 2;
                    turnButton.OnSettingChanged(2);
                    break;
            }
        }
    }
}