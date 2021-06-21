using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public enum ShopItem
    {
        chicken = 0,
        friedPowder = 1,
        oil = 2
    }
    public ShopItem shopItem = ShopItem.chicken;

	public int shopRank = 0;

	public void Perchase()
	{
		switch (shopItem)
		{
			case ShopItem.chicken:
				Chicken chicken = new Chicken();
				chicken.rank = shopRank;
				GameManager.Instance.data.chickens.Enqueue(chicken);
				break;
			case ShopItem.friedPowder:
				FriedPowder friedPowder = new FriedPowder();
				friedPowder.rank = shopRank;
				GameManager.Instance.data.friedPowders.Enqueue(friedPowder);
				break;
			case ShopItem.oil:
				GameManager.Instance.data.oil.rank = shopRank;
				break;
		}
	}
	public int rank = 0;

}

public class Chicken : Item
{

	private bool isGood;

	public bool IsGood { get; set; }


	public Chicken()
	{

	}
}

public class FriedPowder : Item
{
	public FriedPowder()
	{

	}
}

public class Oil : Item
{
	public Oil()
	{

	}
}