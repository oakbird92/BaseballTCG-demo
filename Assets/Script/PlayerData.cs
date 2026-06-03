using UnityEngine;

// 이 에셋을 유니티 메뉴에서 쉽게 만들 수 있게 해줍니다.
[CreateAssetMenu(fileName = "NewPlayer", menuName = "Baseball/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("선수 기본 정보")]
    public string playerName;
    public int backNumber; // 등번호
    public bool isPitcher; // 투수 여부 (true면 투수, false면 타자)

    [Header("타석 확률 스탯")]
    public StatChance statChance;
}
