using UnityEngine;

public class GridGenerator 
{
    private AreaConfig areaConfig;

    public GridGenerator(AreaConfig areaConfig) 
    {
        this.areaConfig = areaConfig;   
    }
    private GameObject gridContainer;

    public void GenerateGrid()
    {
        gridContainer = new GameObject("GridLayout");
        gridContainer.transform.position = new Vector3(0, 0, areaConfig.gridZ);

        int horizontalLines = Mathf.CeilToInt(areaConfig.gridSize.y / areaConfig.cellSize) + 1;
        int verticalLines = Mathf.CeilToInt(areaConfig.gridSize.x / areaConfig.cellSize) + 1;

        for (int i = 0; i < horizontalLines; i++)
        {
            float y = -areaConfig.gridSize.y / 2 + i * areaConfig.cellSize;
            CreateLine(
                new Vector3(-areaConfig.gridSize.x / 2, y, areaConfig.gridZ), 
                new Vector3(areaConfig.gridSize.x / 2, y, areaConfig.gridZ), 
                $"horizontal_{i}");
        }

        for (int i = 0; i < verticalLines; i++)
        {
            float x = -areaConfig.gridSize.x / 2 + i * areaConfig.cellSize;
            CreateLine(
                new Vector3(x, -areaConfig.gridSize.y / 2, areaConfig.gridZ), 
                new Vector3(x, areaConfig.gridSize.y / 2, areaConfig.gridZ),
                $"vertical_{i}");
        }
    }

    private void CreateLine(Vector3 start, Vector3 end, string name)
    {

        GameObject lineObj = new GameObject(name);
        lineObj.transform.SetParent(gridContainer.transform);
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();

        lineRenderer.material = new Material(areaConfig.shaderGrid);

        lineRenderer.material.color = areaConfig.gridLineColor;
        lineRenderer.startColor = areaConfig.gridLineColor;
        lineRenderer.endColor = areaConfig.gridLineColor;
        lineRenderer.startWidth = areaConfig.lineWidth;
        lineRenderer.endWidth = areaConfig.lineWidth;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.useWorldSpace = true;
    }


}
