using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // 1. 뉴 인풋 시스템 네임스페이스 추가!

public enum PlateResult
{
    FH, HR, BB, SO, HBP, OUT
}

public class GameManager : MonoBehaviour
{
    // 데이터 타입은 부모인 'Team'으로 받지만, 
    // 실제 인스펙터에는 HomeTeam과 AwayTeam을 각각 드래그앤드롭할 수 있습니다.
    public TeamManager homeTeam;
    public TeamManager awayTeam;// 테스트용 예시 스탯 (나중에 선수 클래스로 분리하면 좋습니다)

    [Header("현재 경기 상태")]
    public int currentInning = 1;
    public bool isTopInning = true;
    public int outCount = 0;
    public bool isGameOver = false;    // 게임이 끝났는지 확인하는 플래그 추가

    [Header("주자 상황")]
    public bool isRunnerOn1st = false;
    public bool isRunnerOn2nd = false;
    public bool isRunnerOn3rd = false;

    private TeamManager AttackingTeam => isTopInning ? awayTeam : homeTeam;
    private TeamManager DefendingTeam => isTopInning ? homeTeam : awayTeam;

    void Update()
    {
        // 게임이 이미 끝났다면 더 이상 스페이스바 입력을 받지 않음
        if (isGameOver) return;

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            PlayNextAtBat();
        }
    }

    void PlayNextAtBat()
    {
        PlayerData batter = AttackingTeam.GetCurrentBatter();
        PlayerData pitcher = DefendingTeam.currentPitcher;

        if (batter == null || pitcher == null) return;

        PlateResult result = SimulateAtBat(batter.statChance, pitcher.statChance);
        ProcessPlateResult(result, batter.playerName);

        // 게임 오버가 아니라면 다음 타자로 진행
        if (!isGameOver)
        {
            AttackingTeam.AdvanceToNextBatter();
        }
    }

    void ProcessPlateResult(PlateResult result, string batterName)
    {
        string inningPart = isTopInning ? "초" : "말";
        Debug.Log($"[{currentInning}회{inningPart}] {AttackingTeam.teamName} 공격 | 타자: {batterName} -> 결과: {result}");

        switch (result)
        {
            case PlateResult.FH:
                AdvanceRunners(1);
                isRunnerOn1st = true;
                break;
            case PlateResult.HR:   // 홈런
                Debug.Log($"🔥 {batterName} 선수의 대형 홈런!! 🔥");
                AdvanceRunners(4); // 1. 기존 루상의 주자들 전원 홈인 처리

                AttackingTeam.score++; // 2. ★ 핵심: 홈런을 친 타자 본인도 홈인 (1점 추가)! ★

                Debug.Log($"득점! 현재 스코어 - {awayTeam.teamName} {awayTeam.score} : {homeTeam.score} {homeTeam.teamName}");
                break;
            case PlateResult.BB:
            case PlateResult.HBP:
                ProcessWalk();
                break;
            case PlateResult.SO:
            case PlateResult.OUT:
                outCount++;
                Debug.Log($"아웃! 현재 아웃카운트: {outCount}");
                break;
        }

        // ★ 규칙 1: 9회말 "끝내기" 체크
        // 9회말(또는 그 이상 연장말)에 홈팀이 득점하여 어웨이팀 점수를 앞서는 순간 즉시 경기 종료
        if (!isTopInning && currentInning >= 9 && homeTeam.score > awayTeam.score)
        {
            EndGame("홈팀 끝내기 승리!");
            return;
        }

        if (outCount >= 3)
        {
            ChangeHalfInning();
        }
    }

    void ChangeHalfInning()
    {
        outCount = 0;
        isRunnerOn1st = false;
        isRunnerOn2nd = false;
        isRunnerOn3rd = false;

        if (isTopInning)
        {
            // === [초 -> 말 교대 시점] ===
            // ★ 규칙 2: 9회초가 끝났을 때 홈팀이 이미 이기고 있다면 9회말을 할 필요 없이 경기 종료!
            if (currentInning >= 9 && homeTeam.score > awayTeam.score)
            {
                EndGame("홈팀 9회초 종료 승리! (9회말 미진행)");
                return;
            }

            isTopInning = false;
            Debug.Log($"=== {currentInning}회말 {homeTeam.teamName} 공격 시작 ===");
        }
        else
        {
            // === [말 -> 다음이닝 초 교대 시점] ===
            // ★ 규칙 3: 9회말이 끝났을 때 (점수가 안 났거나, 어웨이가 여전히 이기고 있는 경우)
            if (currentInning >= 9)
            {
                if (awayTeam.score > homeTeam.score)
                {
                    EndGame("어웨이팀 승리!");
                }
                else if (awayTeam.score == homeTeam.score)
                {
                    EndGame("9회 무승부 종료! (연장 없음)");
                }
                return;
            }

            isTopInning = true;
            currentInning++;
            Debug.Log($"=== {currentInning}회초 {awayTeam.teamName} 공격 시작 ===");
        }
    }

    // 경기 종료를 처리하는 함수
    void EndGame(string reason)
    {
        isGameOver = true;
        Debug.LogWarning($"====== 경기 종료 : {reason} ======");
        Debug.LogWarning($"최종 스코어 -> {awayTeam.teamName} {awayTeam.score} : {homeTeam.score} {homeTeam.teamName}");
    }

    // (AdvanceRunners, ProcessWalk, SimulateAtBat 함수는 아래에 그대로 유지)
    void AdvanceRunners(int bases)
    {
        for (int i = 0; i < bases; i++)
        {
            if (isRunnerOn3rd)
            {
                isRunnerOn3rd = false;
                AttackingTeam.score++;
                Debug.Log($"득점! 현재 스코어 - {awayTeam.teamName} {awayTeam.score} : {homeTeam.score} {homeTeam.teamName}");
            }
            if (isRunnerOn2nd) { isRunnerOn2nd = false; isRunnerOn3rd = true; }
            if (isRunnerOn1st) { isRunnerOn1st = false; isRunnerOn2nd = true; }
        }
    }

    void ProcessWalk()
    {
        if (!isRunnerOn1st) isRunnerOn1st = true;
        else if (!isRunnerOn2nd) isRunnerOn2nd = true;
        else if (!isRunnerOn3rd) isRunnerOn3rd = true;
        else
        {
            AttackingTeam.score++;
            Debug.Log($"밀어내기 득점! 현재 스코어 - {awayTeam.teamName} {awayTeam.score} : {homeTeam.score} {homeTeam.teamName}");
        }
    }

    public PlateResult SimulateAtBat(StatChance batter, StatChance pitcher)
    {
        float finalFH = batter.fhChance + pitcher.fhChance;
        float finalHR = batter.hrChance + pitcher.hrChance;
        float finalBB = batter.bbChance + pitcher.bbChance;
        float finalSO = batter.soChance + pitcher.soChance;
        float finalHBP = batter.hbpChance + pitcher.hbpChance;
        float finalOUT = batter.outChance + pitcher.outChance;

        float totalSum = finalFH + finalHR + finalBB + finalSO + finalHBP + finalOUT;
        float randomValue = Random.Range(0f, totalSum);
        float cumulative = 0f;

        cumulative += finalFH; if (randomValue <= cumulative) return PlateResult.FH;
        cumulative += finalHR; if (randomValue <= cumulative) return PlateResult.HR;
        cumulative += finalBB; if (randomValue <= cumulative) return PlateResult.BB;
        cumulative += finalSO; if (randomValue <= cumulative) return PlateResult.SO;
        cumulative += finalHBP; if (randomValue <= cumulative) return PlateResult.HBP;

        return PlateResult.OUT;
    }
}
