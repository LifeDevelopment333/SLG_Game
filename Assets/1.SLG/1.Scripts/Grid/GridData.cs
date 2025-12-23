using SLG.RuntimeData;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GridData", menuName = "Scriptable Objects/GridData")]
public class GridData : ScriptableObject
{
    [Header("그리드 속성")]
    public int GridSize = 100;
    public float CellSize = 5;
    public LayerMask TerrainLayer;
    public float RayHeight = 200f;
    public float BuildableSlope = 0.92f;

    // 그리드 생성 툴의 포지션 값을 넣어주세요
    public Vector3 Origin = Vector3.zero;

    private GridCell[] cells;
    public GridCell[] Cells => cells;

    public GridCell GetCell(int x, int z)
    {
        return Cells[x * GridSize + z];
    }

    public int Index(int x, int z)
    {
        return x * GridSize + z;
    }
}