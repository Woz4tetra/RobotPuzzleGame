using System;
using UnityEngine;

[Serializable]
public class TextDialogAction : ConversationAction
{
    [SerializeField] private string text;
    private bool shouldShow = false;
    private GUIStyle labelStyle;
    void Start()
    {
        labelStyle = new GUIStyle
        {
            fontSize = 36,
            fontStyle = FontStyle.Normal,
            normal = { textColor = Color.white },
        };
        labelStyle.normal.background = MakeTexture(2, 2, new Color(0.0f, 0.0f, 0.0f, 1.0f));
    }
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

        Vector2 size = labelStyle.CalcSize(new GUIContent(text));
        float box_height = size.y * 2.0f;

        Rect boxRect = new Rect(0, Screen.height - box_height, Screen.width, box_height);
        Rect labelRect = new Rect(boxRect.x, boxRect.y + size.y / 2.0f, size.x, size.y);
        GUI.Box(boxRect, GUIContent.none, labelStyle);
        GUI.Label(labelRect, text, labelStyle);
    }
    private Texture2D MakeTexture(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
