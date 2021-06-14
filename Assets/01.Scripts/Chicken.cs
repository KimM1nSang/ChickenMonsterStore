using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : MonoBehaviour
{
	private int amount;

	public int Amount
	{
		get { return amount; }
		set { amount = value; }
	}

	private bool isGood;

	public bool IsGood { get; set; }

	private int maxAmount = 3;
	public void RandomAmount()
	{
		if(Random.Range(0, 99) == 0)
		{
			amount = Random.Range(10, 15);
		}
		else
		{
			amount = Random.Range(1, maxAmount);
		}
	}
}
