using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public int _index = 0;
    private static int index = 0;
    public AudioSource CustomerCome;
    private void Awake()
    {
        CustomerCome = gameObject.GetComponent<AudioSource>();
    }

    public static void Create(Transform customerPrefab, Vector3 originPos)
    {
        Transform createCustomer = Instantiate(customerPrefab, originPos, Quaternion.identity);

        Customer c = createCustomer.GetComponent<Customer>();
        //c._index = GameManager.Instance.customers.Count;
        c._index = index;

        index++;
        if (index >3)
        {
            index--;
        }

        c.SettingCustomer();

        //index++;
        c.MovePosition();
        GameManager.Instance.customers.AddLast(c);
    }
    private enum CustomerType
    {
        NORMAL,
        LATTE,
        ROBBER
    }
    private CustomerType customerType = CustomerType.NORMAL;

    private Vector3[] positions = new Vector3[4] { new Vector3(0, 1f, 1.5f), new Vector3(0, 2.5f, 1.5f), new Vector3(0, 4f, 1.5f), new Vector3(0, 5.5f, 1.5f) };


    public void MovePosition()
    {
        //transform.position = positions[_index];
        transform.DOMove(positions[_index], .1f).OnComplete(()=>
        {
            GameManager.Instance.CameraShaking(.3f);
            CustomerCome.Play();
            if (_index == 0)
            {
                GameManager.Instance.MenuOrder();
            }
            PullCustomers();
        });
    }
    public void SettingCustomer()
    {
        int _customerType = UnityEngine.Random.Range(0, 100);
        if(_customerType < 80)
        {
            customerType = CustomerType.NORMAL;
        }
        else if(_customerType < 95)
        {
            customerType = CustomerType.LATTE;
        }
        else
        {
            customerType = CustomerType.ROBBER;
        }
        Debug.Log(customerType);
        //customerType = (CustomerType)(UnityEngine.Random.Range(0, Enum.GetNames(typeof(CustomerType)).Length));
        waitingTime = UnityEngine.Random.Range(500, 1000);
    }
    //ÀÎ³»½É
    public float waitingTime { get; private set; }

    //ÅðÀå
    public void ExitTheStore()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Vector3 exitPosition = transform.position - new Vector3(1f, 0, 0);
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(exitPosition, .5f));
        seq.Join(sr.DOFade(0f, 1f));
        seq.AppendCallback(() =>
        {
            //GameManager.Instance.PullCustomer();

            Destroy(gameObject);
            //GameManager.Instance.CreateCustomer();
            //GameManager.Instance.MenuOrder();
        });

    }
    private void PullCustomers()
    {
        if (GameManager.Instance.customers.First.Value.gameObject.transform.position != positions[0])
        {
            GameManager.Instance.PullCustomer();
        }
    }
}
