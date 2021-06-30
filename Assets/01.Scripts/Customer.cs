using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public int index = 0; //�մ��� ����
    public AudioSource CustomerCome;

    public bool isAngry = false;
    private void Awake()
    {
        CustomerCome = gameObject.GetComponent<AudioSource>();
    }

    public enum CustomerType //�մ��� Ÿ��
    {
        NORMAL = 0,
        LATTE = 1,
        ROBBER = 2,
        SHIBADOGE = 3,
        MANDU = 4,
        JUJU = 5,
        TEDDYBEAR = 6
    }
    public CustomerType customerType = CustomerType.NORMAL;
    //����
    public static void Create(Transform customerPrefab, Vector3 originPos,bool isEventCustomer)
    {
        Transform createCustomer = Instantiate(customerPrefab, originPos, Quaternion.identity);//����

        Customer c = createCustomer.GetComponent<Customer>();

        
        c.index = Mathf.Clamp(GameManager.Instance.customers.Count, 0, 2);//���� ����

        if (isEventCustomer)
        {
            int eventNum = GameManager.Instance.eventNum;
            
            c.SettingEventCustomer(eventNum);
            c.gameObject.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.eventCustomerImage[eventNum-1];
        }
        else
        {
            //Debug.Log(c.index);

            c.SettingCustomer();//���� �Լ� ȣ��
            int rand = UnityEngine.Random.Range(0, GameManager.Instance.customerSprites.Length);
            c.gameObject.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.customerSprites[rand];
            //index++;
        }

        c.MovePosition();//������ �´� ��ġ �̵�
        GameManager.Instance.customers.Enqueue(c);//������ �մ��� ť�� ����
    }
    

    //�̵� ��ġ �̸� ���صα�(�ϵ��ڵ�)
    private Vector3[] positions = new Vector3[4] { new Vector3(0, 1f, 1.5f), new Vector3(0, 2.5f,1.6f), new Vector3(0, 4f, 1.7f), new Vector3(0, 5.5f, 1.8f) };


    public void MovePosition()
    {
        if(index >-1) //�ε����� -1 �ʰ� �϶�, ������ �մ��� �̵��� ���ܽ�Ŵ
        {
            transform.DOMove(positions[index], .1f).OnComplete(() => //�̵� �� �Ʒ��� ���� ����
            {
                GameManager.Instance.CameraShaking(.3f);//ī�޶� ����
                CustomerCome.Play();//���� ���
                if (index == 0)//���� ����� �մ��϶�(ù��° �մ��϶�)
                {
                    GameManager.Instance.MenuOrder();//�ֹ��� �޴´�
                }
            });
        }
    }
    public void SettingCustomer()//���� ����
    {
        int _customerType = UnityEngine.Random.Range(0, 200);//�մ��� Ÿ���� �������� ����

        if (_customerType < 160)
        {
            customerType = CustomerType.NORMAL;
            WaitingTimeSetting(UnityEngine.Random.Range(800,1000), UnityEngine.Random.Range(100, 200));
        }
        else if (_customerType < 195)
        {
            customerType = CustomerType.LATTE;
            WaitingTimeSetting(UnityEngine.Random.Range(1200, 2000),50);
        }
        else
        {
            customerType = CustomerType.ROBBER;
            WaitingTimeSetting(UnityEngine.Random.Range(800, 1000), UnityEngine.Random.Range(200, 250));
        }

        //Debug.Log(customerType);
    }

    public void SettingEventCustomer(int eventNum)
    {
        customerType = (CustomerType)(eventNum + 2);
        WaitingTimeSetting(1000,50);
    }

    private void WaitingTimeSetting(int watingTime,int downWaitingTime)
    {
        WaitingTime = watingTime;
        DownWaitingTime = downWaitingTime;
    }
    //�γ���
    public float WaitingTime { get; private set; }
    public float DownWaitingTime { get; private set; }

    //����
    public void ExitTheStore(Vector3 exitpos) //���� ����
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();//�������� �޾ƿ�
        Vector3 exitPosition = transform.position - exitpos;//�̵� ��ġ

        //��Ʈ�� ������
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(exitPosition, .5f));
        seq.Join(sr.DOFade(0f, 1f));
        seq.AppendCallback(() =>//Ʈ���� ��������
        {
            if (SaveGame.Instance.data.isDayTime)
            {
                GameManager.Instance.PullCustomer();//���� �Ŵ����� PullCustomer �Լ� ȣ��
            }
            else
            {
                Destroy(gameObject);//�� ������Ʈ ����
            }
            //Debug.Log(index);
            //gameObject.SetActive(false);
        });

    }
}
