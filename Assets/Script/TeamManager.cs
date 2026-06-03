using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    [Header("팀 정보")]
    public string teamName;
    public int score = 0;

    [Header("라인업 설정")]
    // 1번부터 9번 타자까지 순서대로 담을 리스트 (인스펙터에서 드래그앤드롭 가능)
    public List<PlayerData> battingOrder = new List<PlayerData>();

    // 이 팀의 현재 투수
    public PlayerData currentPitcher;

    // 현재 몇 번째 타자가 타석에 설 차례인지 기억하는 인덱스 (0 = 1번타자, 1 = 2번타자...)
    [HideInInspector] public int currentBatterIndex = 0;

    // 다음 타자를 데려오는 함수 (9번타자 다음엔 다시 1번타자로)
    public PlayerData GetCurrentBatter()
    {
        if (battingOrder.Count == 0) return null;
        return battingOrder[currentBatterIndex];
    }

    public void AdvanceToNextBatter()
    {
        currentBatterIndex = (currentBatterIndex + 1) % battingOrder.Count;
    }

    // 공통 기능: 점수 획득
    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log($"{teamName} 팀이 {amount}점을 득점했습니다. 총점: {score}");
    }

    // 가상 메서드: 홈/어웨이에서 다르게 행동할 핵심 로직
    public virtual void PlayInning()
    {
        // 공통적인 이닝 시작 처리
    }
    
    public enum PlateResult
    {
        FH,  // 안타 (Field Hit - 일반 안타)
        HR,  // 홈런
        BB,  // 볼넷
        SO,   // 삼진
        HBP, // 데드볼
        OUT  // 범타
    }
}
