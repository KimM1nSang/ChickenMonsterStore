using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public int rank = 0;
    private string strRank;
    private void Update()
    {
        if (GameManager.Instance.timeManager.IsDayTime) return;

        switch (rank)
        {
            case 0:
                strRank = "A";
                break;

            case 1:
                strRank = "B";
                break;

            case 2:
                strRank = "C";
                break;

            case 3:
                strRank = "D";
                break;

            case 4:
                strRank = "E";
                break;

            case 5:
                strRank = "F";
                break;
        }
        gameObject.GetComponentInChildren<Text>().text = strRank;

    }
}
