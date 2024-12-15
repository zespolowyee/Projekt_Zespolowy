// using UnityEngine;

// public class GridManager : MonoBehaviour
// {
//     public TurretGridArea turretGridArea; // Reference to the painted grid area
//     public TurretGridSettings gridSettings; // Reference to the grid settings

//     private void Start()
//     {
//         CreateGrid();
//     }

//     void CreateGrid()
//     {
//         if (gridSettings == null || turretGridArea == null) return;

//         foreach (var cellPosition in turretGridArea.gridCells)
//         {
//             if (turretGridArea.gridCellPrefab != null)
//             {
//                 Instantiate(
//                     turretGridArea.gridCellPrefab,
//                     cellPosition + new Vector3(gridSettings.cellSize / 2, 0, gridSettings.cellSize / 2),
//                     Quaternion.identity
//                 );
//             }
//         }
//     }
// }