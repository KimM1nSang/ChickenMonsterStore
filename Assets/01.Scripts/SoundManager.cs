using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource Fried;
    public AudioSource FriedComplete;
    public AudioSource GunSound;    
    public AudioSource Oil;
    public AudioSource Purchase;
    public AudioSource CantPurchase;
    private void Awake()
    {
        Instance = this;
    }

}
