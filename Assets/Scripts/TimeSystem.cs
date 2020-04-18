using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimeSystem : Singleton<TimeSystem>
{
    public Text textTime;
    public int timePassedinSeconds;
    private int minute;

    void Start()
    {
        textTime = GetComponent<Text>();
        StartCoroutine(UpdatePerSecond());
    }

    public void ResetTime()
    {
        minute = 0;
        timePassedinSeconds = 0;
        textTime.text = "Time: " + minute + "m " + timePassedinSeconds.ToString("00") + "s";
        StopAllCoroutines();
        StartCoroutine(UpdatePerSecond());
    }

    IEnumerator UpdatePerSecond()
    {
        yield return new WaitForSeconds(1.0f);
        timePassedinSeconds++;

        if (timePassedinSeconds >= 60)
        {
            timePassedinSeconds -= 60;
            minute++;
        }

        textTime.text = "Time: " + minute + "m " + timePassedinSeconds.ToString("00") + "s";
        StartCoroutine(UpdatePerSecond());
    }
}
