using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerUpgradeUIWindow : Window
{
    PlayerUpgradeTree upgradeTree;
    [SerializeField] GameObject nodePrefab;
    [SerializeField] GameObject treeRootPoint;
    int levelsSpacing = 100;


    [SerializeField] private float lineOffsetX = 0;
    [SerializeField] private float lineOffsetY = 0;
    List<GameObject> displayedNodes;
    List<Vector2> linePoints;

    public PlayerUpgradeTree UpgradeTree { get => upgradeTree; set => upgradeTree = value; }

    public void DisplayUpgradeTree()
    {
        displayedNodes = new List<GameObject>();
        GameObject rootNode = Instantiate(nodePrefab, treeRootPoint.transform.position, Quaternion.identity, treeRootPoint.transform);
        PlayerUpgradeNodeUI nodeUIScript = rootNode.GetComponent<PlayerUpgradeNodeUI>();
        nodeUIScript.Node = upgradeTree.nodes[0];
        nodeUIScript.UpgradeName.text = upgradeTree.nodes[0].description;
        displayedNodes.Add(rootNode);
        DisplayChildren(nodeUIScript);
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
            offset += 120;
            GameObject nodeUI = Instantiate(nodePrefab, position, Quaternion.identity, parent.transform);
            PlayerUpgradeNodeUI nodeUIScript = nodeUI.GetComponent<PlayerUpgradeNodeUI>();
            nodeUIScript.Node = child;
            nodeUIScript.UpgradeName.text = child.description;
            nodeUIScript.ParentTransform = parent.transform;

            while (!CorrectPlacement(nodeUI, parent.gameObject, offset))
            {
                
            }

            GameObject line = new GameObject();
            line.transform.parent = parent.transform;
            UILineRenderer render = line.AddComponent<UILineRenderer>();
            render.thickness = 3;
            render.points = new Vector2[] { nodeUIScript.LineInPoint.transform.position + new Vector3(lineOffsetX, lineOffsetY), parent.LineOutPoint.transform.position + new Vector3(lineOffsetX, lineOffsetY) };


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

    private void DrawLines()
    {

    }
        
    


}
