using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class TurretGridArea : MonoBehaviour
{
    public TurretGridSettings gridSettings;
    public HashSet<Vector3> gridCells = new HashSet<Vector3>();
    private HashSet<Vector3> prevGridCells;
    public string gridSavePath = "Assets/Level/Maps/TestMap/Assets/TurretGrid/grid.json";
    public GameObject gridCellPrefab;

    private void OnEnable()
    {
        prevGridCells = new HashSet<Vector3>(PersistentGrid<List<Vector3>>.LoadGridData(gridSavePath) ?? new List<Vector3>());
    }
    private void OnDrawGizmos()
    {
        if (gridSettings == null) return;

        if (prevGridCells == null)
            prevGridCells = new HashSet<Vector3>(PersistentGrid<List<Vector3>>.LoadGridData(gridSavePath) ?? new List<Vector3>());
        Gizmos.color = new Color(Color.magenta.r, Color.magenta.g, Color.magenta.b, 0.5f);
        foreach (var cellPosition in prevGridCells)
        {
            Gizmos.DrawCube(
                cellPosition + new Vector3(gridSettings.cellSize / 2, 0, gridSettings.cellSize / 2),
                gridSettings.cubeSize
            );
        }
        Gizmos.color = new Color(gridSettings.gridColor.r, gridSettings.gridColor.g, gridSettings.gridColor.b, 0.5f);
        var diff = gridCells.Except(prevGridCells);
        foreach (var cellPosition in diff)
        {
            Gizmos.DrawCube(
                cellPosition + new Vector3(gridSettings.cellSize / 2, 0, gridSettings.cellSize / 2),
                gridSettings.cubeSize
            );
        }
    }
    public void BakeGrid()
    {
        gridCells.UnionWith(prevGridCells);
        Debug.Log($"Grid baked. {gridCells.Count} cells in total.");
        var tmp  = new List<Vector3>(gridCells);
        Debug.Log($"Grid baked. {tmp.Count} cells in total.");
        PersistentGrid<List<Vector3>>.SaveGridData(new List<Vector3>(gridCells), gridSavePath);
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.name == gridCellPrefab.name)
                DestroyImmediate(transform.GetChild(i).gameObject);
        }
        foreach (var cellPosition in gridCells)
        {
            if (gridCellPrefab != null)
            {
                Instantiate(
                    gridCellPrefab,
                    cellPosition + new Vector3(gridSettings.cellSize / 2, 0, gridSettings.cellSize / 2),
                    Quaternion.identity,
                    transform
                );
            }
        }
    }
}
