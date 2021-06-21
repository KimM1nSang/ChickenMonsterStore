using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{

    [Header("ªÛ¡°")]
    public Transform shopContents = null;
    private Item item;
    //    0.1/0.5/0.85

    private int state = 0;
    private int rank = 0;
    private const int rankLimit = 6;
    //  ¥ﬂ ∆¢±Ë∞°∑Á ±‚∏ß
    //  0    1      2
    
    void Start()
    {
        item = new Item();
    }

    void Update()
    {
        ShopChoose();
    }
    private void ShopChoose()
    {
        if (GameManager.Instance.IsDayTime) return;
        if(Input.GetKeyDown(KeyCode.RightArrow)&& state < 2)
        {
            state++;
            shopContents.DOMove(shopContents.position - )
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && state > 0)
        {
            state--;
        }
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            rank++;
            if(rank >= rankLimit)
            {
                rank = 0;
            }
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            rank--;
            if(rank <=0)
            {
                rank = rankLimit;
            }
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            item.Perchase();
        }

        item.shopRank = rank;
        item.shopItem = (Item.ShopItem)state;
    }
}