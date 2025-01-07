using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class TurretGridArea : MonoBehaviour
{
    public TurretGridSettings gridSettings;
    public HashSet<Vector3> gridCells = new HashSet<Vector3>();
    private HashSet<Vector3> prevGridCells = new HashSet<Vector3>();
    public GameObject gridCellPrefab;

    private void OnDrawGizmos()
    {
        if (gridSettings == null) return;

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
        prevGridCells = new HashSet<Vector3>(gridCells);
        gridCells.Clear();
    }

    public void ClearGrid()
    {
        gridCells.Clear();
        prevGridCells.Clear();
    }
}
