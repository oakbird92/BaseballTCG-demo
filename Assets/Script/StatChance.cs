using UnityEngine;
using System;

[Serializable]
public class StatChance
{        
    [Range(0, 500)] public float fhChance;  // 안타 확률 (예: 15%)
    [Range(0, 500)] public float hrChance;  // 홈런 확률
    [Range(0, 500)] public float bbChance;  // 볼넷 확률
    [Range(0, 500)] public float soChance;   // 삼진 확률
    [Range(0, 500)] public float hbpChance; // 데드볼 확률
    [Range(0, 1000)] public float outChance;// 범타 확률
}
