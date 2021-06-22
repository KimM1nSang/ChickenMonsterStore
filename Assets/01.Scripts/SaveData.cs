using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public Queue<Chicken> chickens { get; set; }
    public Queue<FriedPowder> friedPowders { get; set; }
    public Oil oil { get; set; }
    public int money { get; set; }
    
    public SaveData()
    {
        chickens = new Queue<Chicken>();
        friedPowders = new Queue<FriedPowder>();
        oil = new Oil();
    }
}
