using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public int index = 0; //�մ��� ����
    public AudioSource CustomerCome;
    private void Awake()
    {
        CustomerCome = gameObject.GetComponent<AudioSource>();
    }

    //����
    public static void Create(Transform customerPrefab, Vector3 originPos, int index)
    {
        Transform createCustomer = Instantiate(customerPrefab, originPos, Quaternion.identity);//����

        Customer c = createCustomer.GetComponent<Customer>();

        c.index = index;//���� ����

        c.SettingCustomer();//���� �Լ� ȣ��

        //index++;
        c.MovePosition();//������ �´� ��ġ �̵�
        GameManager.Instance.customers.Enqueue(c);//������ �մ��� ť�� ����
    }
    public enum CustomerType //�մ��� Ÿ��
    {
        NORMAL,
        LATTE,
        ROBBER
    }
    public CustomerType customerType = CustomerType.NORMAL;

    //�̵� ��ġ �̸� ���صα�(�ϵ��ڵ�)
    private Vector3[] positions = new Vector3[4] { new Vector3(0, 1f, 1.5f), new Vector3(0, 2.5f, 1.5f), new Vector3(0, 4f, 1.5f), new Vector3(0, 5.5f, 1.5f) };


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

        if (_customerType < 100)
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

        Debug.Log(customerType);
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
    public void ExitTheStore() //���� ����
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();//�������� �޾ƿ�
        Vector3 exitPosition = transform.position - new Vector3(1f, 0, 0);//�̵� ��ġ

        //��Ʈ�� ������
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(exitPosition, .5f));
        seq.Join(sr.DOFade(0f, 1f));
        seq.AppendCallback(() =>//Ʈ���� ��������
        {
            GameManager.Instance.PullCustomer();//���� �Ŵ����� PullCustomer �Լ� ȣ��
            Destroy(gameObject);//�� ������Ʈ ����
        });

    }
}
