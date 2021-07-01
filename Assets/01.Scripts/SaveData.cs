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

    public float day = 0;

    public float time = 20;

    public bool isDayTime = false;
    public int dayCheck = 1;

    public int shootNum;

    public bool isCutSceneWatched = false;
    public bool IsHaveData = false;
    public SaveData()
    {
        chickens = new List<Chicken>();
        friedPowders = new List<FriedPowder>();
        oil = new Oil();
    }
}
