using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    TMP_Text text;
    public float timer;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.timeScale == 0)
            return;
        timer += Time.deltaTime;
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        text.text = minutes + ":" + seconds.ToString("00");
    }
}