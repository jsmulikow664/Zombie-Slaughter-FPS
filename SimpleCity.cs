using UnityEngine;
using System.Collections;

public class SimpleCity : MonoBehaviour
{

    public Transform[] blocks;
    public Transform[] bases;
    public Transform[] floors;
    public Transform[] roofs;

    public int blockXNum = 10;
    public int blockZNum = 10;
    public float blockSpan = 10f;

    public int buildingXNum = 4;
    public int buildingZNum = 2;
    public float buildingSpan = 10f;

    GameObject cityRoot;

    int numBlocks;
    int numBuildings;

    void Start()
    {
        Generate();
    }

    void Generate()
    {
        if (cityRoot)
            Destroy(cityRoot);

        cityRoot = new GameObject("CityRoot");

        float blockXSize = buildingXNum * buildingSpan;
        float blockZSize = buildingZNum * buildingSpan;

        float blockXOffset = (((blockXNum - 1) * blockSpan) + ((blockXNum - 1) * blockXSize)) * 0.5f;
        float blockZOffset = (((blockZNum - 1) * blockSpan) + ((blockZNum - 1) * blockZSize)) * 0.5f;

        Vector3 blockPos = new Vector3(-blockXOffset, 0, -blockZOffset);

        for (int x = 0; x < blockXNum; ++x)
        {
            for (int z = 0; z < blockZNum; ++z)
            {
                CreateBlock(blockPos);

                blockPos.z += blockZSize + blockSpan;
            }

            blockPos.x += blockXSize + blockSpan;
            blockPos.z = -blockZOffset;
        }
    }

    void CreateBlock(Vector3 position)
    {
        GameObject blockRoot = new GameObject("Block" + ++numBlocks);
        blockRoot.transform.position = position;
        blockRoot.transform.parent = cityRoot.transform;

        Transform block = null;

        if (blocks.Length > 0)
        {
            block = (Transform)Instantiate(blocks[Random.Range(0,blocks.Length)], position, Quaternion.identity);
            block.name = "Block" + ++numBlocks;
        }
        else
        {
            block = new GameObject("Block" + ++numBlocks).transform;
            block.position = position;
        }
        
        block.parent = cityRoot.transform;

        float blockXScale = ((buildingXNum * buildingSpan) + blockSpan) * 0.1f;
        float blockZScale = ((buildingZNum * buildingSpan) + blockSpan) * 0.1f;

        block.localScale = new Vector3(blockXScale, 1, blockZScale);

        float buildingXOffset = ((buildingXNum - 1) * buildingSpan) * 0.5f;
        float buildingZOffset = ((buildingZNum - 1) * buildingSpan) * 0.5f;

        Vector3 buildingPos = new Vector3(position.x - buildingXOffset, 0, position.z - buildingZOffset);

        for (int x = 0; x < buildingXNum; ++x)
        {
            for (int z = 0; z < buildingZNum; ++z)
            {
                CreateBuilding(buildingPos, blockRoot.transform);
                buildingPos.z += buildingSpan;
            }

            buildingPos.x += buildingSpan;
            buildingPos.z = position.z - buildingZOffset;
        }
    }

    void CreateBuilding(Vector3 position, Transform block)
    {
        Vector3 curPosition = position;

        ++numBuildings;

        Transform c_parent = null;

        if (bases.Length > 0)
        {
            Transform b = (Transform)Instantiate(bases[Random.Range(0, bases.Length)], curPosition, Quaternion.identity);
            b.name = "Base" + numBuildings;
            b.parent = block;
            c_parent = b;
        }

        if (floors.Length > 0)
        {
            int numFloors = Random.Range(1, 10);

            for (int i = 0; i < numFloors; ++i)
            {
                curPosition.y += 4;
                Transform f = (Transform)Instantiate(floors[Random.Range(0, floors.Length)], curPosition, Quaternion.identity);
                f.name = "Floor" + numBuildings + "_" + i;
                f.parent = c_parent;
                c_parent = f;
            }
        }

        curPosition.y += 4;

        if (roofs.Length > 0)
        {
            Transform r = (Transform)Instantiate(roofs[Random.Range(0, roofs.Length)], curPosition, Quaternion.identity);
            r.name = "Roof" + numBuildings;
            r.parent = c_parent;
        }
    }

    void OnGUI()
    {
        //public int blockXNum = 10;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Block X Num: ");
        blockXNum = System.Int32.Parse(GUILayout.TextField(blockXNum.ToString(), GUILayout.Width(50)));
        GUILayout.EndHorizontal();

        //public int blockZNum = 10;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Block Z Num: ");
        blockZNum = System.Int32.Parse(GUILayout.TextField(blockZNum.ToString(), GUILayout.Width(50)));
        GUILayout.EndHorizontal();

        //public float blockSpan = 10f;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Block Span: ");
        blockSpan = (float)System.Double.Parse(GUILayout.TextField(blockSpan.ToString(), GUILayout.Width(50)));
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        //public int buildingXNum = 4;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Building X Num: ");
        buildingXNum = System.Int32.Parse(GUILayout.TextField(buildingXNum.ToString(), GUILayout.Width(50)));
        GUILayout.EndHorizontal();

        //public int buildingZNum = 2;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Building Z Num: ");
        buildingZNum = System.Int32.Parse(GUILayout.TextField(buildingZNum.ToString(), GUILayout.Width(50)));
        GUILayout.EndHorizontal();

        //public float buildingSpan = 10f;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Building Span: ");
        buildingSpan = (float)System.Double.Parse(GUILayout.TextField(buildingSpan.ToString(), GUILayout.Width(50)));
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        if (GUILayout.Button("Generate"))
            Generate();

    }
}
