using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{

    [Header("ªÛ¡°")]
    public RectTransform shopContents = null;
    public Transform[] shopItems = null;

    private Item item;
    private bool IsCanMove = true;
    //    0.1/0.5/0.85

    private int state = 0;
    private const int rankLimit = 5;
    //  ¥ﬂ ∆¢±Ë∞°∑Á ±‚∏ß
    //  0    1      2
    
    void Start()
    {
        item = new Item();
        item.Setting();
    }

    void Update()
    {
        ShopChoose();
    }
    private void ShopChoose()
    {
        if (GameManager.Instance.timeManager.IsDayTime || !GameManager.Instance.isPanelOn) return;
        if(Input.GetKeyDown(KeyCode.RightArrow)&& state < 2 && IsCanMove)
        {
            IsCanMove = false;
            state++;
            shopContents.DOMoveX(shopContents.position.x - 6, .1f).OnComplete(() => { IsCanMove = true; });
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && state > 0 && IsCanMove)
        {
            IsCanMove = false;
            state--;
            shopContents.DOMoveX(shopContents.position.x + 6, .1f).OnComplete(() => { IsCanMove = true; });
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            shopItems[state].GetComponent<ShopItem>().rank++;
            if (shopItems[state].GetComponent<ShopItem>().rank > rankLimit)
            {
                shopItems[state].GetComponent<ShopItem>().rank = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            shopItems[state].GetComponent<ShopItem>().rank--;
            if (shopItems[state].GetComponent<ShopItem>().rank < 0)
            {
                shopItems[state].GetComponent<ShopItem>().rank = rankLimit;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            item.Perchase();
            GameManager.Instance.RefreshText();
        }
        item.shopRank = shopItems[state].GetComponent<ShopItem>().rank;
        shopItems[state].GetComponent<ShopItem>().price = item.currentPrice;
        item.shopItem = (Item.ShopItem)state;
        item.Setting();
    }
}