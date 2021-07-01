using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float day { get { return SaveGame.Instance.data.day; } private set { SaveGame.Instance.data.day = value; } }
    public float time { get { return SaveGame.Instance.data.time; } private set { SaveGame.Instance.data.time = value; } }

    private const float dayTime = 20;
    public bool IsDayTime { get { return SaveGame.Instance.data.isDayTime; } private set { SaveGame.Instance.data.isDayTime = value; } }

    public int dayCheck { get { return SaveGame.Instance.data.dayCheck; } set { SaveGame.Instance.data.dayCheck = value; } }

    public int dayTimeScale = 1;

    void Update()
    {
        gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0,0,(time/dayTime) * 360);
    }
    public void SetDayTime(bool isDayTime)
    {
        SaveGame.Instance.data.isDayTime = isDayTime;
    }
    public void SubtractTime(float subtractTime)
    {
        time -= subtractTime * dayTimeScale;
    }
    public void ResetTime()
    {
        time = dayTime;
    }
    public void DayPlus()
    {
        day++;
    }
}
