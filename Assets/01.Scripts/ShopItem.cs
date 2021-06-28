using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public int rank = 0;
    private string strRank;
    public int price;
    public string itemname = "";
    private void Update()
    {
        if (GameManager.Instance.timeManager.IsDayTime) return;

        strRank = ToStringRank(rank);
        gameObject.transform.GetChild(0).GetComponent<Text>().text = strRank;
        gameObject.transform.GetChild(3).GetComponent<Text>().text = price.ToString();
        switch (itemname)
        {
            case "chicken":
                gameObject.transform.GetChild(4).GetComponent<Text>().text = SaveGame.Instance.data.chickens.Count.ToString();
                break;
            case "friedpowder":
                gameObject.transform.GetChild(4).GetComponent<Text>().text = SaveGame.Instance.data.friedPowders.Count.ToString();
                break;
            case "oil":
                gameObject.transform.GetChild(4).GetComponent<Text>().text = ToStringRank(SaveGame.Instance.data.oil.rank);
                break;
        }

    }
    private string ToStringRank(int input)
    {
        switch (input)
        {
            case 5:
                return "A";
            case 4:
                return "B";
            case 3:
                return "C";
            case 2:
                return "D";
            case 1:
                return "E";
            case 0:
                return "F";
            default:
                return "";
        }
    }
}
