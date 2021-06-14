using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerLine<T>
{
    private T[] lineArray;
    public CustomerLine(Vector3 originPos, Func<CustomerLine<T>, int, T> customer)
    {
        lineArray = new T[3];

        for (int index = 0; index < lineArray.GetLength(0); index++)
        {
            lineArray[index] = customer(this, index);
        }
    }
    

}
