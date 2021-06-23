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
				int rank1 = chicken.rank;
				chicken.rank = shopRank;
				if(GameManager.Instance.data.money >= chicken.price * (chicken.rank + 1))
				{
					DecreaseMoney(chicken.price * (chicken.rank + 1));
					GameManager.Instance.data.chickens.Enqueue(chicken);
				}
				else
				{
					chicken.rank = rank1;
				}
				break;
			case ShopItem.friedPowder:
				FriedPowder friedPowder = new FriedPowder();
				int rank2 = friedPowder.rank;
				friedPowder.rank = shopRank;
				if (GameManager.Instance.data.money >= friedPowder.price * (friedPowder.rank + 1))
				{
					DecreaseMoney(friedPowder.price * (friedPowder.rank + 1));
					GameManager.Instance.data.friedPowders.Enqueue(friedPowder);
				}
				else
				{
					friedPowder.rank = rank2;
				}
				break;
			case ShopItem.oil:
				int rank3 = GameManager.Instance.data.oil.rank;
				GameManager.Instance.data.oil.rank = shopRank;
				if (GameManager.Instance.data.money >= GameManager.Instance.data.oil.price * (GameManager.Instance.data.oil.rank + 1))
				{
					DecreaseMoney(GameManager.Instance.data.oil.price * (GameManager.Instance.data.oil.rank + 1));
				}
				else
				{
					GameManager.Instance.data.oil.rank = rank3;
				}
				break;
		}
	}
	private void DecreaseMoney(int decrease)
	{
		GameManager.Instance.data.money -= decrease;
	}
	public int rank = 0;
	public int price = 10;

}

public class Chicken : Item
{

	private bool isGood;

	public bool IsGood { get; set; }


	public Chicken()
	{
		price = 2;
	}
}

public class FriedPowder : Item
{
	public FriedPowder()
	{
		price = 1;
	}
}

public class Oil : Item
{
	public Oil()
	{
		price = 40;
	}
}