using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderBox : MonoBehaviour
{
    void Update()
    {
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector3(gameObject.transform.GetChild(0).GetComponent<Text>().text.Length*55, gameObject.GetComponent<RectTransform>().sizeDelta.y);
    }
}
