using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float time { get; private set; }
    private const float dayTime = 20;
    private bool isDayTime = true;
    public bool IsDayTime { get { return isDayTime; } private set { isDayTime = value; } }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0,0,(time/dayTime) * 360);
    }
    public void SetDayTime(bool isDayTime)
    {
        this.isDayTime = isDayTime;
    }
    public void SubtractTime(float subtractTime)
    {
        time -= subtractTime;
    }
    public void ResetTime()
    {
        time = dayTime;
    }
}
