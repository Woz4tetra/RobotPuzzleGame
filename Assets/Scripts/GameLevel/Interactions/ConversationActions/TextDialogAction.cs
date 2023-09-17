using System;
using UnityEngine;

[Serializable]
public class TextDialogAction : ConversationAction
{
    [SerializeField] private string text;
    private bool shouldShow = false;
    public TextDialogAction(string text)
    {
        this.text = text;
    }

    override public void Render()
    {
        Debug.Log(text);
        shouldShow = true;
    }
    override public void Despawn()
    {
        shouldShow = false;
    }

    void OnGUI()
    {
        if (!shouldShow)
        {
            return;
        }
        GUIStyle labelStyle = new GUIStyle
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };
        float labelBorderSize = 5;
        Vector2 size = labelStyle.CalcSize(new GUIContent(text));

        Rect boxRect = new Rect(0, Screen.height - size.y - 2 * labelBorderSize, size.x + 2 * labelBorderSize, size.y + 2 * labelBorderSize);
        Rect labelRect = new Rect(boxRect.x + labelBorderSize, boxRect.y + labelBorderSize, size.x, size.y);
        GUI.Box(boxRect, GUIContent.none);
        GUI.Label(labelRect, text, labelStyle);
    }
}
