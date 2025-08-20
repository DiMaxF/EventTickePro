using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorFigureView : EditorView
{
    [SerializeField] Image color;
    public Sprite Form => color.sprite;
    public override void UpdateColor(Color newColor)
    {
        base.UpdateColor(newColor);
        color.color = newColor;
    }

    public void UpdateForm(Sprite form) 
    {
        color.sprite = form; 
        color.type = Image.Type.Sliced;

    }
}
