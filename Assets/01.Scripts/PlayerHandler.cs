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

    // ��ġ ����
    // ���� = �⸧ �µ� ����
    // ���� = ġŲ Ƣ���
    // ���� = �մ� ����
    // ��� = ��

    private Transform player;

    [Header("�÷��̾� ��ġ��")]
    [SerializeField] private Transform[] playerPositions;

    private enum PlayerState
    {
        IDLE =0,
        GUN = 1
    }
    private PlayerState playerState = PlayerState.IDLE;
    private enum PlayerDir
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
        player = GameObject.Find("Player").GetComponent<Transform>();

    }
    // Update is called once per frame

    private void LateUpdate()
    {
        if (Time.timeScale == 0) return;
        CheckplayerDir();
        MoveAsplayerDir();
        Working();
    }

    private void Working()
    {
        switch (playerDir)
        {
            case PlayerDir.IDLE:
                Gun();
                break;
            case PlayerDir.UP:
                //�մ� ����ϱ�
                GameManager.Instance.DisposeChicken();
                break;
            case PlayerDir.LEFT:
                //�⸧ �µ� �ø���
                GameManager.Instance.OilTempUp();
                break;
            case PlayerDir.RiGHT:
                //ġŲ Ƣ���
                GameManager.Instance.FriedChicken();
                break;
        }

        ResetWorkHistory();
    }
    private void Gun()
    {
        if (Input.GetKeyDown(KeyCode.Space) && playerState == PlayerState.IDLE && GameManager.Instance.orderState == GameManager.OrderState.BETWEENORDER) 
        {
            playerState = PlayerState.GUN;
            Debug.Log(playerState);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && GameManager.Instance.bullet > 0 && playerState == PlayerState.GUN && GameManager.Instance.orderState != GameManager.OrderState.ORDER)
        {
            playerState = PlayerState.IDLE;
            Debug.Log(playerState);
            GameManager.Instance.bullet--;
            GameManager.Instance.BulletImage[GameManager.Instance.bullet].GetComponent<UnityEngine.UI.Image>().color = new Color(0,0,0); 
            DOTween.KillAll();
            GameManager.Instance.CameraShaking(2f);
            GameManager.Instance.orderState = GameManager.OrderState.ORDER;
            GameManager.Instance.OrderTextReset();
            GameManager.Instance.EndDispose();
        }
        else if(GameManager.Instance.bullet <= 0)
        {
            //�߻� ���� �ִϸ��̼�
        }

    }

    private void ResetWorkHistory()
    {
        if (playerDir == PlayerDir.IDLE)
        {
            GameManager.Instance.ResetWorkHistory();
        }
        else
        {
            playerState = PlayerState.IDLE;
        }
    }

    private void MoveAsplayerDir()
    {
        player.position = playerPositions[(int)playerDir].position;
    }

    private void CheckplayerDir()
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
