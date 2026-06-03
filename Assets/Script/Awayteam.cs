using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Awayteam : TeamManager
{
    // AwayTeam만의 특수 변수가 필요하다면 여기에 선언

    // 부모의 메서드를 재정의(Override)
    public override void PlayInning()
    {
        base.PlayInning(); // 부모의 기본 기능 실행
        Debug.Log($"{teamName} (어웨이)이 이닝 '초'에 공격을 시작합니다.");
        // 어웨이팀만의 공격 로직 실행
    }
}
