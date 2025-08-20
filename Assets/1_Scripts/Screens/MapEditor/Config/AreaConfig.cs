using UnityEngine;

[CreateAssetMenu(fileName = "AreaConfig", menuName = "Scriptable Objects/AreaConfig")]
public class AreaConfig : ScriptableObject
{
    public Vector2 gridSize = new Vector2(50f, 50f);
    public float cellSize = 1f;
    public Color gridLineColor = Color.gray;
    public float lineWidth = 0.05f; 
    public float gridZ = 5f;
    public Shader shaderGrid;
}
