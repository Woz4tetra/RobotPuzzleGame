using System;
using UnityEngine;

[Serializable]
public class TextDialogAction : ConversationAction
{
    [SerializeField] private string text;
    public TextDialogAction(string text)
    {
        this.text = text;
    }

    override public void Render()
    {
        Debug.Log(text);
    }
    override public void Despawn()
    {

    }

    // void OnGUI()
    // {
    //     if (!isDialogActive)
    //     {
    //         return;
    //     }
    //     string text = currentText;
    //     GUIStyle labelStyle = new GUIStyle
    //     {
    //         fontSize = 20,
    //         fontStyle = FontStyle.Bold,
    //         normal = { textColor = Color.white }
    //     };
    //     Vector2 size = labelStyle.CalcSize(new GUIContent(text));

    //     Rect boxRect = new Rect(0, Screen.height - size.y - 2 * labelBorderSize, size.x + 2 * labelBorderSize, size.y + 2 * labelBorderSize);
    //     Rect labelRect = new Rect(boxRect.x + labelBorderSize, boxRect.y + labelBorderSize, size.x, size.y);
    //     GUI.Box(boxRect, GUIContent.none);
    //     GUI.Label(labelRect, text, labelStyle);
    // }
}
