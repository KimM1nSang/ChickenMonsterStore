using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource Fried;
    public AudioSource GunSound;
    public AudioSource Oil;
    private void Awake()
    {
        Instance = this;
    }

}
