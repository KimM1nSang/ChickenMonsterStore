using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;
using System;

public class PlayerHandler : MonoBehaviour
{

    // ������ ����
    // �����鵵 �����ϴ� ġŲ�� �Ⱦ��糢��

    // ���� ����
    // ���� �մԵ��� �볻�� �����ؼ� �⸧ �µ��� �߸� ���缭 ġŲ�� �ָ� ȭ���� ������ ���� ������ �������� �ִ�.
    // �⸧ �µ��� �߰����� ����� �ʰ� ��� ������ �ָ� �մ��� �ֹ��� ����� ���߾� ġŲ�� Ƣ�� �����ϸ� �ȴ�.
    // �մ� �ϳ��ϳ� ���� �ð��� ������ ���ѽð��� �մ��� �γ��� �̴�.
    // �մ��� ������ ���� �ʰų� ������ ���Կ� �Դٸ� ���� ���� ��ġ ����.

    // �ð� ����
    //  ���� ��, ������ ��� �㿡�� ��� �غ�(����)
    // ����
    //  ��� ����
    //   ���� ���ݿ� ������ ��
    //
    //   ����
    //    A,B,C,D,E,F�� ����
    //
    //    Ȯ��
    //    F = 50;
    //    E = 25;
    //    D = 12.5f;
    //    C = 6.25;
    //    B = 3.125;
    //    A = 1.5625;
    //  
    //    ����
    //    0.1/0.5/0.85
    //
    //   Ƣ���
    //    A,B,C,D,E,F�� Ƣ���
    //   
    //  �⸧ ����
    //   �ѹ��� �絵 ��
    //    A,B,C,D,E,F�� �⸧
    //    �⸧�� ��޿� ���� ġŲ�� �ϼ����� ���̱� ������

    //  ���� ���
    //   A,B,C,D,E,F�� ����
    //   û�� ���
    //    ���� �������� ����
    //   ī���� ���
    //    �ǸŽ� ȹ�� ��ȭ�� ������
    // ����
    //  ���ǿ� ���� �մ��� ���� �󵵰� �޶���

    // ��ġ ����
    // ���� = �⸧ �µ� ����
    // ���� = ġŲ Ƣ���
    // ���� = �մ� ����
    // ��� = ��

    private Transform player;

    [Header("�÷��̾� ��ġ��")]
    [SerializeField] private Transform[] playerPositions;//�÷��̾��� ��ġ����

    private enum PlayerState//�ѱ� ����
    {
        IDLE =0,//���� ���� ���� ����
        GUN = 1//���� �� ����
    }
    private PlayerState playerState = PlayerState.IDLE;
    private enum PlayerDir//�÷��̾��� ��ġ�� ����
    {
        IDLE = 0,
        UP = 1,
        LEFT = 2,
        RiGHT = 3
    }
    private PlayerDir playerDir = PlayerDir.IDLE;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();//�÷��̾� ����

    }
    // Update is called once per frame

    private void LateUpdate()
    {
        CheckplayerDir();//�Է� üũ
        MoveAsplayerDir();//�Է¿� ���� ��ġ �̵�

        if (GameManager.Instance.timeManager.IsDayTime == false) return;
        Working();//�Է¿� ���� �ൿ
    }

    private void Working()
    {
        switch (playerDir)//�÷��̾��� ����(��ġ)�� ���� �ൿ
        {
            case PlayerDir.IDLE:
                //���� or �߻�
                Gun();
                break;
            case PlayerDir.UP:
                //�մ� ����ϱ�(ġŲ ����)
                GameManager.Instance.DisposeChicken();
                break;
            case PlayerDir.LEFT:
                //�⸧ �µ� �ø���(���� �����)
                GameManager.Instance.OilTempUp();
                break;
            case PlayerDir.RiGHT:
                //ġŲ Ƣ���(���� �����)
                GameManager.Instance.FriedChicken();
                break;
        }

        ResetWorkHistory();//���� ����
    }
    private void Gun()
    {
        if (Input.GetKeyDown(KeyCode.Space) && playerState == PlayerState.IDLE && GameManager.Instance.orderState == GameManager.OrderState.BETWEENORDER) 
        {
            //���� ���
            playerState = PlayerState.GUN;
            Debug.Log(playerState);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && GameManager.Instance.bullet > 0 && playerState == PlayerState.GUN && GameManager.Instance.orderState != GameManager.OrderState.ORDER)
        {
            //�߻�
            playerState = PlayerState.IDLE;
            //Debug.Log(playerState);
            GameManager.Instance.bullet--;//�Ѿ� �Ҹ�
            GameManager.Instance.BulletImage[GameManager.Instance.bullet].GetComponent<UnityEngine.UI.Image>().color = new Color(0,0,0); //�Ѿ� �Ҹ� �̹��� ��ȯ
            DOTween.KillAll();//��Ʈ�� ����
            SoundManager.Instance.GunSound.Play();//���� ���
            GameManager.Instance.CameraShaking(2f);//ī�޶� ����
            GameManager.Instance.OrderTextReset();//�ý�Ʈ�� ����
            GameManager.Instance.EndDispose();//�մ��� ���� ��Ų��.

            //���� ó��
            /*if(GameManager.Instance.index >=2)
            {
                GameManager.Instance.index -= GameManager.Instance.customers.Count-2;
            }*/
            

            //�߻� ���� �ִϸ��̼� �ʿ�
        }
        else if(GameManager.Instance.bullet <= 0)
        {
            //�߻� ���� �ִϸ��̼�
        }

    }

    private void ResetWorkHistory()//�÷��̾��� ���¸� �ʱ�� ���� �����
    {
        if (playerDir == PlayerDir.IDLE)//�÷��̾ �߸��� ��ġ �϶�
        {
            GameManager.Instance.ResetWorkHistory();//���� ���൵ ����
        }
        else
        {
            playerState = PlayerState.IDLE;//���� ���� ���� ����
        }
    }

    private void MoveAsplayerDir()//���⿡ ���� ��ġ �̵�
    {
        player.position = playerPositions[(int)playerDir].position;
    }

    private void CheckplayerDir()//�Է¿� ���� �÷��̾��� ��ġ ���� ��ȯ
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool isHorizontal = h != 0 && v == 0 ;
        bool isLeft = h != 0 && h == -1;
        bool isUp = v != 0 && v == 1;
        bool isIdle = h == 0 && v == 0;
        playerDir = isIdle ? PlayerDir.IDLE : isHorizontal ? (isLeft ? PlayerDir.LEFT : PlayerDir.RiGHT) : (isUp ? PlayerDir.UP : PlayerDir.IDLE) ;
    }
}
