using System.Collections.Generic;
using UnityEngine;

using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System.Globalization;
using TMPro;
using Unity.VisualScripting;

[ExecuteAlways]
public class StairGenerator : MonoBehaviour
{
    [Header("Stair Parameters")]
    public int numberOfStairs = 3;
    public float height = 8f;
    public float width = 8f;
    public float depth = 8f;
    public float minStepHeight = 1f;
    public float maxStepHeight = 2f;


    void OnValidate()
    {
        if (numberOfStairs < 0 || height <= 0 || width <= 0 || depth <= 0 || minStepHeight <= 0 || maxStepHeight <= 0)
        {
            return;
        }
        GenerateStairs();
    }

    void GenerateStairs()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this == null) return;
            foreach (var stair in GetComponentsInChildren<ProBuilderMesh>())
            {
                DestroyImmediate(stair.gameObject);
            }
            for (int i = 0; i < numberOfStairs; i++)
            {
                float stepHeight = minStepHeight + (maxStepHeight - minStepHeight) / (numberOfStairs - 1) * i;
                int stepCount = Mathf.CeilToInt(height / stepHeight);
                float instancedHeight = stepHeight * stepCount;
                var stair = ShapeGenerator.GenerateStair(PivotLocation.FirstVertex, new Vector3(width, instancedHeight, depth), stepCount, true);
                stair.transform.SetParent(transform);
                stair.AddComponent<MeshCollider>();
                stair.transform.localPosition = new Vector3(width * i, height - instancedHeight, 0);
                var stepHeightString = stepHeight.ToString("F2", CultureInfo.InvariantCulture);
                stair.name = $"Stair_h{stepHeightString}";
                stair.SetMaterial(stair.faces, BuiltinMaterials.defaultMaterial);
                var text = new GameObject("Text").AddComponent<TextMesh>();
                text.text = $"Step height: {stepHeightString}";
                text.characterSize = 0.1f;
                text.fontSize = 80;
                text.anchor = TextAnchor.MiddleCenter;
                text.alignment = TextAlignment.Center;
                text.color = Color.black;
                text.transform.SetParent(stair.transform);
                text.transform.localPosition = new Vector3(width / 2, instancedHeight - height / 1.5f, 0);
                text.transform.rotation = transform.rotation;
            }
        };

        #endif
        
    }
}
