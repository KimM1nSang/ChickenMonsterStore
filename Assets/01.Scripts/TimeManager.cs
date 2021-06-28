using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float day { get { return SaveGame.Instance._data.day; } private set { SaveGame.Instance._data.day = value; } }
    public float time { get { return SaveGame.Instance._data.time; } private set { SaveGame.Instance._data.time = value; } }

    private const float dayTime = 20;
    public bool IsDayTime { get { return SaveGame.Instance._data.isDayTime; } private set { SaveGame.Instance._data.isDayTime = value; } }

    public int dayTimeScale = 1;

    void Update()
    {
        gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0,0,(time/dayTime) * 360);
    }
    public void SetDayTime(bool isDayTime)
    {
        this.IsDayTime = isDayTime;
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
