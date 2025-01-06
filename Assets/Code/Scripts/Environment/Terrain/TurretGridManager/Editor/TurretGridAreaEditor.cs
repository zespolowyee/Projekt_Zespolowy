using UnityEngine;
using UnityEditor;
using System;
using System.Security.Permissions;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine.UIElements;
using Unity.Mathematics;

[CustomEditor(typeof(TurretGridArea))]
public class TurretGridAreaEditor : Editor
{
    private TurretGridArea gridArea;
    private TurretGridSettings gridSettings;
    private bool isPainting = false;
    public float brushSize = 4.0f;
    private void OnEnable()
    {
        gridArea = (TurretGridArea)target;
        gridSettings = gridArea.gridSettings;
    }

    private void OnSceneGUI()
    {
        if (gridSettings == null) return;
        Event e = Event.current;

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftShift)
        {
            isPainting = true;
        }
        if (e.type == EventType.KeyUp && e.keyCode == KeyCode.LeftShift)
        {
            isPainting = false;
        }
        
        if (isPainting)
        {
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.R)
            {
                gridArea.ClearGrid();
                e.Use();
            }
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.B)
            {
                gridArea.BakeGrid();
                e.Use();
            }
            if (e.type == EventType.ScrollWheel)
            {
                brushSize *= 1 - e.delta.x * .1f;
                brushSize = Mathf.Clamp(brushSize, 0.1f, 50.0f);
                e.Use();
            }
            if (isPointingOnPrefab())
            {
                return;
            }
            var selectedCells = getCellsInBrushRange(GetMouseWorldPosition());
            DrawBrush(selectedCells);
            if (e.type == EventType.MouseDrag || e.type == EventType.MouseDown)
            {
                // if left mouse button is pressed
                if (e.button == 1)
                    gridArea.gridCells.ExceptWith(selectedCells);
                // if right mouse button is pressed
                if (e.button == 0)
                    gridArea.gridCells.UnionWith(selectedCells);
                e.Use();
            }
        }
        SceneView.RepaintAll();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance: math.INFINITY))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    private void DrawBrush(List<Vector3> selectedCells)
    {
        Handles.color = new Color(gridSettings.gridColor.r, gridSettings.gridColor.g, gridSettings.gridColor.b, 0.5f);
        foreach (var cell in selectedCells)
        {
            Handles.DrawWireCube(cell + new Vector3(gridSettings.cellSize / 2, 0, gridSettings.cellSize / 2), new Vector3(gridSettings.cellSize, 0.1f, gridSettings.cellSize));
        }
    }

    private List<Vector3> getCellsInBrushRange(Vector3 mouseWorldPosition)
    {
        List<Vector3> cells = new List<Vector3>();
        for (float x = -brushSize + gridSettings.globalOffset.x; x < brushSize + gridSettings.globalOffset.x; x += gridSettings.cellSize)
        {
            for (float z = -brushSize + gridSettings.globalOffset.z; z < brushSize + gridSettings.globalOffset.z; z += gridSettings.cellSize)
            {
                var cellCenter = mouseWorldPosition + new Vector3(x, 0, z);
                cellCenter = new Vector3(
                    Mathf.Floor(cellCenter.x / gridSettings.cellSize) * gridSettings.cellSize + gridSettings.cellSize / 2,
                    cellCenter.y,
                    Mathf.Floor(cellCenter.z / gridSettings.cellSize) * gridSettings.cellSize + gridSettings.cellSize / 2
                );
                // round to .001 to properly detect collisions in hashset
                cellCenter = new Vector3(
                    Mathf.Round(cellCenter.x * 1000) / 1000,
                    Mathf.Round(cellCenter.y * 1000) / 1000,
                    Mathf.Round(cellCenter.z * 1000) / 1000
                );
                if (gridSettings.gridType == GridType.Circle &&
                        Vector3.Distance(mouseWorldPosition, cellCenter) > brushSize)
                        {
                    continue;
                }
                cells.Add(cellCenter);
            }
        }
        return cells;
    }
    private bool isPointingOnPrefab()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        return Physics.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("TurretGrid"));
    }
}