using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    public GridSystem gridSystem;
    public Material gridMaterial;
    public Material unitMaterial;

    void Start()
    {
        if (gridSystem == null) gridSystem = GetComponent<GridSystem>();
        CreateGridVisual();
    }

    void CreateGridVisual()
    {
        GameObject gridParent = new GameObject("Grid Visual");

        for (int x = 0; x < gridSystem.width; x++)
        {
            for (int y = 0; y < gridSystem.height; y++)
            {
                for (int z = 0; z < gridSystem.depth; z++)
                {
                    Vector3 position = new Vector3(x * gridSystem.cellSize, z * gridSystem.cellSize, y * gridSystem.cellSize);
                    GameObject cellObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cellObject.transform.position = position;
                    cellObject.transform.localScale = Vector3.one * gridSystem.cellSize * 0.9f;
                    cellObject.GetComponent<Renderer>().material = gridMaterial;
                    cellObject.transform.SetParent(gridParent.transform);
                }
            }
        }
    }

    public GameObject CreateUnitVisual(Vector3 position)
    {
        GameObject unitObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        unitObject.transform.position = position + Vector3.up * gridSystem.cellSize * 0.5f;
        unitObject.transform.localScale = Vector3.one * gridSystem.cellSize * 0.5f;
        unitObject.GetComponent<Renderer>().material = unitMaterial;
        return unitObject;
    }
}