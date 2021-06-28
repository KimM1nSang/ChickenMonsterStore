using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public List<Chicken> chickens;
    public List<FriedPowder> friedPowders;
    public Oil oil;

    public int money;

    public int reputation;

    public float day;

    public float time = 20;

    public bool isDayTime = false;

    public SaveData()
    {
        isDayTime = false;
        chickens = new List<Chicken>();
        friedPowders = new List<FriedPowder>();
        oil = new Oil();
    }
}
