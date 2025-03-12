using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerUpgradeUIWindow : Window
{
    PlayerUpgradeTree upgradeTree;
    [SerializeField] PlayerUpgradeDetailsUI detailsUI;
    [SerializeField] GameObject ViewportContent;
    [SerializeField] GameObject nodePrefab;
    [SerializeField] GameObject treeRootPoint;
    int levelsSpacing = 150;



    [SerializeField] private float lineOffsetX = 0;
    [SerializeField] private float lineOffsetY = 0;
    List<GameObject> displayedNodes;
    List<GameObject> linePoints;

    public PlayerUpgradeTree UpgradeTree { get => upgradeTree; set => upgradeTree = value; }

    public void DisplayUpgradeTree()
    {
        displayedNodes = new List<GameObject>();
        linePoints = new List<GameObject>();
        detailsUI.UpgradeManager = player.GetComponentInChildren<PlayerUpgradeManager>();

        GameObject rootNode = Instantiate(nodePrefab, treeRootPoint.transform.position, Quaternion.identity, treeRootPoint.transform);
        PlayerUpgradeNodeUI nodeUIScript = rootNode.GetComponent<PlayerUpgradeNodeUI>();
        nodeUIScript.Node = upgradeTree.nodes[0];
        nodeUIScript.UpgradeName.text = upgradeTree.nodes[0].description;
        displayedNodes.Add(rootNode);
        DisplayChildren(nodeUIScript);
        DrawLines(linePoints.Count);
    }

    private void DisplayChildren(PlayerUpgradeNodeUI parent)
    {
        int offset = 0;
        foreach (string childrenID in parent.Node.childNodes)
        {
            UpgradeNode child = UpgradeTree.nodes.FirstOrDefault(x => x.id == childrenID);

            if (child == null)
            {
                return;
            }
            Vector3 position = parent.transform.position + new Vector3(offset, levelsSpacing);
            offset += 150;
            GameObject nodeUI = Instantiate(nodePrefab, position, Quaternion.identity, parent.transform);
            PlayerUpgradeNodeUI nodeUIScript = nodeUI.GetComponent<PlayerUpgradeNodeUI>();
            nodeUIScript.DisplayData(child, parent.transform);
            nodeUIScript.Buton.onClick.AddListener(delegate { detailsUI.DisplayNodeInfo(child); });
            while (!CorrectPlacement(nodeUI, parent.gameObject, offset))
            {
                
            }



            linePoints.Add(nodeUIScript.LineInPoint);
            linePoints.Add(parent.LineOutPoint);
            displayedNodes.Add(nodeUI);
            DisplayChildren(nodeUIScript);
        }
    }

    bool CorrectPlacement(GameObject nodeUI, GameObject parent, int offset)
    {
        foreach (GameObject node in displayedNodes)
        {
            if (Vector3.Distance(node.transform.position, nodeUI.transform.position) < 20f && nodeUI.transform.position.x > 50f)
            {
                parent.transform.position += new Vector3(offset, 0);
                return false;
            }
        }
        return true;
    }

    private void DrawLines(int n)
    {
        for (int i = 0; i< n; i+=2)
        {
            GameObject line = new GameObject();
            line.transform.parent = linePoints[i].transform;
            UILineRenderer render = line.AddComponent<UILineRenderer>();
            render.thickness = 3;
            render.points = new Vector2[] { linePoints[i].transform.position + new Vector3(lineOffsetX, lineOffsetY), linePoints[i+1].transform.position + new Vector3(lineOffsetX, lineOffsetY) };
        }
    }

    public void ScaleTree(Vector2 value)
    {
        Debug.Log(value);
    }
        
    


}
