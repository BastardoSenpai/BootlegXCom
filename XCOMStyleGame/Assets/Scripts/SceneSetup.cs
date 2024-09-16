using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    public GridSystem gridSystem;
    public Material gridMaterial;
    public Material halfCoverMaterial;
    public Material fullCoverMaterial;

    public GameObject soldierPrefab;
    public GameObject sniperPrefab;
    public GameObject heavyPrefab;

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
                Vector3 position = new Vector3(x * gridSystem.cellSize, 0, y * gridSystem.cellSize);
                GameObject cellObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cellObject.transform.position = position;
                cellObject.transform.localScale = new Vector3(gridSystem.cellSize, 0.1f, gridSystem.cellSize);
                cellObject.GetComponent<Renderer>().material = gridMaterial;
                cellObject.transform.SetParent(gridParent.transform);
            }
        }
    }

    public void CreateCoverVisual(Vector3 position, CoverType coverType)
    {
        GameObject coverObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        coverObject.transform.position = position + Vector3.up * 0.5f;
        coverObject.transform.localScale = new Vector3(0.8f, 1f, 0.8f);

        switch (coverType)
        {
            case CoverType.Half:
                coverObject.GetComponent<Renderer>().material = halfCoverMaterial;
                break;
            case CoverType.Full:
                coverObject.GetComponent<Renderer>().material = fullCoverMaterial;
                coverObject.transform.localScale = new Vector3(0.8f, 1.5f, 0.8f);
                break;
        }
    }

    public GameObject CreateUnitVisual(UnitType unitType, Vector3 position)
    {
        GameObject unitObject;

        switch (unitType)
        {
            case UnitType.Sniper:
                unitObject = Instantiate(sniperPrefab, position, Quaternion.identity);
                break;
            case UnitType.Heavy:
                unitObject = Instantiate(heavyPrefab, position, Quaternion.identity);
                break;
            default:
                unitObject = Instantiate(soldierPrefab, position, Quaternion.identity);
                break;
        }

        return unitObject;
    }
}