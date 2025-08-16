using UnityEngine;

public struct DisplayManager
{
    private static Vector2 BordersPositive => Camera.main.ViewportToWorldPoint(Vector2.one);
    private static Vector2 BordersNegative => Camera.main.ViewportToWorldPoint(Vector2.zero);
    
    public static float LeftX => BordersNegative.x;
    public static float BottomY => BordersNegative.y;

    public static float RightX => BordersPositive.x;
    public static float TopY => BordersPositive.y;

    public static float Height => BordersPositive.y - BordersNegative.y;
    public static float Width => BordersPositive.x - BordersNegative.x;
    public static Vector2 Center => new Vector2((LeftX + RightX) / 2, (BottomY + TopY) / 2);
}