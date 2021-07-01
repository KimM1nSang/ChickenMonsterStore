using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public Dictionary<int, Sprite[]> endingSpritesDict = new Dictionary<int, Sprite[]>();
    public Sprite[] endingSprites;
    private void Start()
    {
        GenerateData();
    }
    public Sprite GetImage(int id, int talkIndex)
    {
        if (talkIndex == endingSpritesDict[id].Length)
        {
            return null;
        }
        return endingSpritesDict[id][talkIndex];
    }
    private void GenerateData()
    {
        //0
        //1
        //2
        //3
        //4
        //5
        //6
        //7
        //8
        endingSpritesDict.Add(0, new Sprite[8] { endingSprites[0], endingSprites[1], endingSprites[2], endingSprites[3], endingSprites[4], endingSprites[5], endingSprites[6], endingSprites[7] });
        endingSpritesDict.Add(1, new Sprite[] { });
        endingSpritesDict.Add(2, new Sprite[] { });
        endingSpritesDict.Add(3, new Sprite[] { });
        endingSpritesDict.Add(4, new Sprite[] { });
        endingSpritesDict.Add(5, new Sprite[] { });
        endingSpritesDict.Add(6, new Sprite[] { });
        endingSpritesDict.Add(7, new Sprite[] { });
        endingSpritesDict.Add(8, new Sprite[] { });
    }

}
