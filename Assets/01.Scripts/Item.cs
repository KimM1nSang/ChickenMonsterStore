using System;
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
	public int currentPrice = 0;

	public void Setting()
	{
		switch (shopItem)
		{
			case ShopItem.chicken:
				Chicken chicken = new Chicken();
				int rank1 = chicken.rank;
				chicken.rank = shopRank;
				currentPrice = chicken.price * (chicken.rank + 1);
				chicken.rank = rank1;
				break;
			case ShopItem.friedPowder:
				FriedPowder friedPowder = new FriedPowder();
				int rank2 = friedPowder.rank;
				friedPowder.rank = shopRank;
				currentPrice = friedPowder.price * (friedPowder.rank + 1);
				friedPowder.rank = rank2;
				break;
			case ShopItem.oil:
				int rank3 = SaveGame.Instance.data.oil.rank;
				SaveGame.Instance.data.oil.rank = shopRank;
				currentPrice = SaveGame.Instance.data.oil.price * (SaveGame.Instance.data.oil.rank + 1);
				SaveGame.Instance.data.oil.rank = rank3;
				break;
		}
	}
	public void Perchase()
	{
		switch (shopItem)
		{
			case ShopItem.chicken:
				Chicken chicken = new Chicken();
				int rank1 = chicken.rank;
				chicken.rank = shopRank;
				currentPrice = chicken.price * (chicken.rank + 1);
				chicken.currentPrice = currentPrice;
				if (SaveGame.Instance.data.money >= currentPrice)
				{
					DecreaseMoney(currentPrice);
					SaveGame.Instance.data.chickens.Add(chicken);
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
				currentPrice = friedPowder.price * (friedPowder.rank + 1);
				friedPowder.currentPrice = currentPrice;
				if (SaveGame.Instance.data.money >=currentPrice)
				{
					DecreaseMoney(currentPrice);
					for (int i = 0; i < 5; i++)
					{
						SaveGame.Instance.data.friedPowders.Add(friedPowder);
					}
				}
				else
				{
					friedPowder.rank = rank2;
				}
				break;
			case ShopItem.oil:
				int rank3 = SaveGame.Instance.data.oil.rank;
				SaveGame.Instance.data.oil.rank = shopRank;
				currentPrice = SaveGame.Instance.data.oil.price * (SaveGame.Instance.data.oil.rank + 1);
				if (SaveGame.Instance.data.money >= currentPrice&& rank3 != shopRank && rank3 <= shopRank)
				{
					DecreaseMoney(currentPrice);
				}
				else
				{
					SaveGame.Instance.data.oil.rank = rank3;
				}
				break;
		}
		//Debug.Log(currentPrice);
	}
	private void DecreaseMoney(int decrease)
	{
		SaveGame.Instance.data.money -= decrease;
	}
	public int rank = 0;
	public int price = 10;
}

[Serializable]
public class Chicken : Item
{

	private bool isGood;

	public bool IsGood { get; set; }


	public Chicken()
	{
		price = 3;
	}
}

[Serializable]
public class FriedPowder : Item
{
	public FriedPowder()
	{
		price = 2;
	}
}

[Serializable]
public class Oil : Item
{
	public Oil()
	{
		price = 40;
	}
}