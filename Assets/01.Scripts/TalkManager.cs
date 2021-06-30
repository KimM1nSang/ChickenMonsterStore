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
        talkData.Add(1, new string[] { "그 치킨 좀 줘봐", "그런데 말이야","이", "치킨이란게 참 신기해", "나때는 이런 그... 뭐냐 크런키?", "그런게 없었거든","몇개인지 말 안했다고?", "지금 나 늙었다고 무시 하는거야?", "허,참 어이가 없어서", "니가 두발로 걷기 시작할때 말이야", "나는 고오생을 하고 있었어요"}); //라때
        talkData.Add(2, new string[] { "저는 치킨 강도에요 당신의 치킨을 가지러 왔어요" }); //강도
        talkData.Add(3, new string[] { "즐거운 치킨 시간이 또 돌아 왔습니다","아닛 용사잖아?","오호.. 치킨장사를 하는 용사라","반갑습니다! 용사님" }); //시바개
        talkData.Add(4, new string[] { "어디갔지? 작은 요정 쥬쥬....","키링 도착했는데 어디간거야?"}); //만두
        talkData.Add(5, new string[] { "드디어 그곳에서 탈출했어요 먹을것 좀 주세요...","아니 저를 너무 좋아하는 사람이 많아요","너무 무서워" }); //쥬쥬
        talkData.Add(6, new string[] { "야레야레...용사가 여기있었군..","아잇 지금 뭐하는거야" }); //곰돌이
        talkData.Add(7, new string[] { "날 기망 하는거냐!!!!!!" });
        talkData.Add(8, new string[] { "갯수가 틀렸잖아!!!!!!!!!!!!!!" });
        talkData.Add(9, new string[] { "기름 온도가 이게 뭐야!!!!!" });
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
