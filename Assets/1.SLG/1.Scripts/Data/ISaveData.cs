using SLG.Builder;
using System;
using System.Resources;
using UnityEditor.Overlays;
using UnityEngine;

public interface ISaveData<T>
{
    T SaveData();
}