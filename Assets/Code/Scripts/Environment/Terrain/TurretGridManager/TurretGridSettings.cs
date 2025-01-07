using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretGridSettings", menuName = "Custom Settings/Grid Settings")]
public class TurretGridSettings : ScriptableObject
{
    [Header("Grid Configuration")]
    public float cellSize = 1.0f;
    [Range(0.0f, 1.0f)]
    public float cubeFill = 0.7f;
    public float cubeHeight = 0.2f;
    public Vector3 globalOffset = Vector3.zero;
    public Vector3 cubeSize => new Vector3(cellSize * cubeFill, cubeHeight, cellSize * cubeFill);
    public Material gridMaterial;
    public GridType gridType;
    public Color gridColor = Color.red;
    public Boolean displayLessAssets = true;
}
public enum GridType
{
    Square,
    Circle
}
