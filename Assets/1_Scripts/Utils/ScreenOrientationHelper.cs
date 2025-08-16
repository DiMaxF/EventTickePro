using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenOrientationHelper : MonoBehaviour
{
    public enum ScreenOrientationType
    {
        Portrait,
        Landscape,
        Auto
    }

    [SerializeField] private ScreenOrientationType screenOrientation;

    private static ScreenOrientationHelper instance;

    public static ScreenOrientationHelper Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void UpdateOrientation(ScreenOrientationType orientation)
    {
        screenOrientation = orientation;
        ApplyChanges();

    }

    void Start()
    {
        ApplyChanges();
    }

    private void ApplyChanges()
    {
        switch (screenOrientation)
        {
            case ScreenOrientationType.Portrait:
                Screen.orientation = ScreenOrientation.Portrait;
                break;
            case ScreenOrientationType.Landscape:
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                break;
            case ScreenOrientationType.Auto:
                Screen.orientation = ScreenOrientation.AutoRotation;
                break;
        }
    }
}