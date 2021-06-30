using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData;

    private void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        GenerateData();
    }
    private void GenerateData()
    {
        talkData.Add(1, new string[] { "�� ġŲ �� ���", "�׷��� ���̾�","��", "ġŲ�̶��� �� �ű���", "������ �̷� ��... ���� ũ��Ű?", "�׷��� �����ŵ�","����� �� ���ߴٰ�?", "���� �� �ľ��ٰ� ���� �ϴ°ž�?", "��,�� ���̰� ���", "�ϰ� �ι߷� �ȱ� �����Ҷ� ���̾�", "���� ������� �ϰ� �־����"}); //��
        talkData.Add(2, new string[] { "���� ġŲ �������� ����� ġŲ�� ������ �Ծ��" }); //����
        talkData.Add(3, new string[] { "��ſ� ġŲ �ð��� �� ���� �Խ��ϴ�","�ƴ� ����ݾ�?","��ȣ.. ġŲ��縦 �ϴ� ����","�ݰ����ϴ�! ����" }); //�ùٰ�
        talkData.Add(4, new string[] { "�����? ���� ���� ����....","Ű�� �����ߴµ� ��𰣰ž�?"}); //����
        talkData.Add(5, new string[] { "���� �װ����� Ż���߾�� ������ �� �ּ���...","�ƴ� ���� �ʹ� �����ϴ� ����� ���ƿ�","�ʹ� ������" }); //����
        talkData.Add(6, new string[] { "�߷��߷�...��簡 �����־���..","���� ���� ���ϴ°ž�" }); //������
        talkData.Add(7, new string[] { "�� ��� �ϴ°ų�!!!!!!" });
        talkData.Add(8, new string[] { "������ Ʋ���ݾ�!!!!!!!!!!!!!!" });
        talkData.Add(9, new string[] { "�⸧ �µ��� �̰� ����!!!!!" });
    }

    public string GetTalk(int id, int talkIndex)
    {
        if(talkIndex == talkData[id].Length)
        {
            return null;
        }
        return talkData[id][talkIndex];
    }
}
