using UnityEngine;

public interface IBuilding
{
    /// <summary>
    /// 건물 선택
    /// </summary>
    void Select();

    /// <summary>
    /// 건물 선택 해제
    /// </summary>
    void DeSelect();

    void HoverEnter();

    void HoverExit();

    /// <summary>
    /// 건물 업그레이드
    /// </summary>
    void Upgrade();

    /// <summary>
    /// 건물 제거
    /// </summary>
    void Remove();

    // 사용할지 안할지 미정
    void ReBuild();
}
