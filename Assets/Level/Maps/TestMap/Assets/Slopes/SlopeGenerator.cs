using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System.Globalization;
using TMPro;
using Unity.VisualScripting;

[ExecuteAlways]
public class SlopeGenerator : MonoBehaviour
{
    [Header("Slope Parameters")]
    public int numberOfSlopes = 10;
    public float height = 8f;
    public float width = 8f;
    public float depth = 8f;
    public float minSlopeSlopeDegree = 5f;
    public float maxSlopeSlopeDegree = 45f;
    // allow change of default material
    [SerializeField]
    private Material material = null;

    void OnValidate()
    {
        if (height <= 0 || width <= 0 || depth <= 0 || numberOfSlopes < 0 || minSlopeSlopeDegree < 0 || maxSlopeSlopeDegree <= 0)
        {
            return;
        }
        if (material == null)
        {
            material = BuiltinMaterials.defaultMaterial;
        }
        GenerateSlopes();
    }

    void GenerateSlopes()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this == null) return;
            foreach (var slope in GetComponentsInChildren<ProBuilderMesh>())
            {
                DestroyImmediate(slope.gameObject);
            }
            for (int i = 0; i < numberOfSlopes; i++)
            {
                float slopeDegree = minSlopeSlopeDegree + (maxSlopeSlopeDegree - minSlopeSlopeDegree) / (numberOfSlopes - 1) * i;
                float instancedHeight = height / Mathf.Sin(slopeDegree * Mathf.Deg2Rad);
                var slope = ShapeGenerator.GeneratePlane(PivotLocation.FirstVertex, width, instancedHeight, 1, 1, Axis.Up);
                slope.transform.SetParent(transform);
                slope.AddComponent<MeshCollider>();
                slope.transform.localPosition = new Vector3(width * i, height, 0);
                slope.transform.localRotation = Quaternion.Euler(0, 90, -slopeDegree);
                var stepHeightString = slopeDegree.ToString("F1", CultureInfo.InvariantCulture);
                slope.name = $"Slope_{stepHeightString}_deg";
                slope.SetMaterial(slope.faces, material);
                var text = new GameObject("Text").AddComponent<TextMesh>();
                text.text = $"Slope: {stepHeightString}\u00B0";
                text.characterSize = 0.1f;
                text.fontSize = 80;
                text.anchor = TextAnchor.MiddleCenter;
                text.alignment = TextAlignment.Center;
                text.color = Color.black;
                text.transform.SetParent(slope.transform);
                text.transform.localPosition = new Vector3(0, 0, width / 2);
                text.transform.rotation = transform.rotation;
            }
        };

        #endif
        
    }
}
