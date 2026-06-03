using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hometeam : TeamManager
{
    // 부모의 메서드를 재정의(Override)
    public override void PlayInning()
    {
        base.PlayInning(); // 부모의 기본 기능 실행
        Debug.Log($"{teamName} (홈)이 이닝 '말'에 공격을 시작합니다.");

        // 홈팀만의 특수 규칙 체크 (예: 9회말 끝내기 찬스 등)
    }

    // 홈팀에게만 있는 특별한 기능 예시
    public void CheckWalkOffWin()
    {
        // 9회말 끝내기 승리 조건 검사
    }
}
