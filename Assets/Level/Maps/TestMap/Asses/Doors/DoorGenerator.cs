using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System.Globalization;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.ProBuilder;

[ExecuteAlways]
public class Door : MonoBehaviour
{
    [Header("Door Parameters")]
    public int numberOfDoors = 20;
    public float totalHeight = 3f;
    public float totalWidth = 2f;
    public float doorWidth = 1.5f;
    public float depth = 1f;
    public float minDoorHeight = 0.5f;
    public float maxDoorHeight = 2f;

    
    [SerializeField]
    private Material material = null;

    void OnValidate()
    {
        if (totalHeight <= 0 || totalWidth <= 0 || depth <= 0 || numberOfDoors <= 0 || minDoorHeight < 0 || maxDoorHeight <= 0)
        {
            return;
        }
        if (maxDoorHeight > totalHeight)
        {
            maxDoorHeight = totalHeight;
        }
        if (material == null)
        {
            material = BuiltinMaterials.defaultMaterial;
        }
        GenerateDoors();
    }

    void GenerateDoors()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this == null) return;
            foreach (var door in GetComponentsInChildren<ProBuilderMesh>())
            {
                DestroyImmediate(door.gameObject);
            }
            for (int i = 0; i < numberOfDoors; i++)
            {
                var doorHeight = minDoorHeight + (maxDoorHeight - minDoorHeight) / (numberOfDoors - 1) * i;
                var door = ShapeGenerator.GenerateDoor(PivotLocation.FirstVertex, totalWidth, totalHeight, totalHeight - doorHeight, (totalWidth - doorWidth) / 2, depth);
                door.transform.SetParent(transform);
                door.AddComponent<MeshCollider>();
                door.transform.localPosition = new Vector3(totalWidth * i, 0, 0);
                var stepHeightString = doorHeight.ToString("F3", CultureInfo.InvariantCulture);
                door.name = $"Door_h{stepHeightString}";
                door.SetMaterial(door.faces, material);
                var text = new GameObject("Text").AddComponent<TextMesh>();
                text.text = $"Door: {stepHeightString}u";
                text.characterSize = 0.05f;
                text.fontSize = 50;
                text.anchor = TextAnchor.MiddleCenter;
                text.alignment = TextAlignment.Center;
                text.color = Color.black;
                text.transform.SetParent(door.transform);
                text.transform.localPosition = new Vector3(totalWidth / 2, doorHeight + .25f, 0);
                text.transform.rotation = transform.rotation;
            }
            // add cubes around
            var casing = new ProBuilderMesh[3];
            casing[0] = ShapeGenerator.GenerateCube(PivotLocation.FirstVertex, new Vector3(1, totalHeight, depth));
            casing[1] = ShapeGenerator.GenerateCube(PivotLocation.FirstVertex, new Vector3(1, totalHeight, depth));
            casing[2] = ShapeGenerator.GenerateCube(PivotLocation.FirstVertex, new Vector3(2 + (numberOfDoors + 1) * totalWidth, 1, depth));
            for (int i = 0; i < casing.Length; i++)
            {
                casing[i].name = $"Casing_{i}";
                casing[i].transform.SetParent(transform);
                casing[i].SetMaterial(casing[i].faces, material);
                casing[i].AddComponent<MeshCollider>();
            }
            casing[0].transform.localPosition = new Vector3(-1, 0, 0);
            casing[1].transform.localPosition = new Vector3(totalWidth * (numberOfDoors + 1), 0, 0);
            casing[2].transform.localPosition = new Vector3(-1, totalHeight, 0);    
        };

        #endif
        
    }
}
