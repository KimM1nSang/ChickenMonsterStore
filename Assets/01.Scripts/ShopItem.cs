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

        switch (rank)
        {
            case 5:
                strRank = "A";
                break;

            case 4:
                strRank = "B";
                break;

            case 3:
                strRank = "C";
                break;

            case 2:
                strRank = "D";
                break;

            case 1:
                strRank = "E";
                break;

            case 0:
                strRank = "F";
                break;
        }
        gameObject.transform.GetChild(0).GetComponent<Text>().text = strRank;
        gameObject.transform.GetChild(3).GetComponent<Text>().text = price.ToString();
        switch (itemname)
        {
            case "chicken":
                gameObject.transform.GetChild(4).GetComponent<Text>().text = GameManager.Instance.data.chickens.Count.ToString();
                break;
            case "friedpowder":
                gameObject.transform.GetChild(4).GetComponent<Text>().text = GameManager.Instance.data.friedPowders.Count.ToString();
                break;
            case "oil":
                gameObject.transform.GetChild(4).GetComponent<Text>().text = GameManager.Instance.data.oil.rank.ToString();
                break;
        }

    }
}
