using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public Queue<Chicken> chickens;
    public Queue<FriedPowder> friedPowders;
    public Oil oil { get; set; }
    
    public SaveData()
    {
        chickens = new Queue<Chicken>();
        friedPowders = new Queue<FriedPowder>();
        oil = new Oil();
    }
}
