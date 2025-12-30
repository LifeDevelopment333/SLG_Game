using UnityEngine;

public interface IBuilding
{
    int Level { get; }

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
}
