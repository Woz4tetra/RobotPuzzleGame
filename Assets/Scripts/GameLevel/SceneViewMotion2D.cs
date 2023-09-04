using UnityEditor;
using UnityEngine;
/// <summary>
/// Allows to change rotation axis of the scene view to Z.
/// Based on unity implementation <see href="https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/SceneView/SceneViewMotion.cs"/>
/// </summary>
[InitializeOnLoad]
public static class SceneViewMotion2D
{
    // there is the main difference from unity implementation.
    // rotation around Y axis applies with Vector3.right and Vector3.up in next 2 lines respectively
    private static readonly Vector3 pitchDir = Vector3.right;
    private static readonly Vector3 yawDir = Vector3.back;

    private static bool isOrbit;
    private static bool useZAxis = true;

    [System.Obsolete]
    static SceneViewMotion2D()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        DrawToggle();

        if (!useZAxis)
        {
            return;
        }

        if (sceneView.isRotationLocked)
        {
            return;
        }

        // prevents orbit without Alt after Alt+tab from unity.
        if (EditorWindow.mouseOverWindow == null)
        {
            isOrbit = false;
        }

        Event e = Event.current;

        switch (e.type)
        {
            case EventType.MouseDrag:
                if (e.button == 1)
                {
                    FPSCameraBehaviour(sceneView, e);
                    e.Use();
                }
                else if (isOrbit && e.button == 0)
                {
                    OrbitCameraBehavior(sceneView, e);
                    e.Use();
                }

                break;
            case EventType.KeyDown:
                if (e.keyCode == KeyCode.LeftAlt)
                {
                    isOrbit = true;
                }

                break;
            case EventType.KeyUp:
                if (e.keyCode == KeyCode.LeftAlt)
                {
                    isOrbit = false;
                }

                break;
        }
    }

    private static void DrawToggle()
    {
        Handles.BeginGUI();
        GUI.color = new Color(1, 1, 1, 0.1f);
        useZAxis = GUI.Toggle(new Rect(10, 10, 100, 50), useZAxis, "");
        Handles.EndGUI();
    }

    private static void FPSCameraBehaviour(SceneView sceneView, Event e)
    {
        Vector3 camPos = sceneView.pivot - sceneView.rotation * Vector3.forward * sceneView.cameraDistance;

        Quaternion rotation = sceneView.rotation;
        rotation = Quaternion.AngleAxis(e.delta.y * .003f * Mathf.Rad2Deg, rotation * pitchDir) * rotation;
        rotation = Quaternion.AngleAxis(e.delta.x * .003f * Mathf.Rad2Deg, yawDir) * rotation;
        sceneView.rotation = rotation;

        sceneView.pivot = camPos + rotation * Vector3.forward * sceneView.cameraDistance;
    }

    private static void OrbitCameraBehavior(SceneView sceneView, Event e)
    {
        sceneView.FixNegativeSize();
        Quaternion rotation = sceneView.rotation;
        rotation = Quaternion.AngleAxis(e.delta.y * .003f * Mathf.Rad2Deg, rotation * pitchDir) * rotation;
        rotation = Quaternion.AngleAxis(e.delta.x * .003f * Mathf.Rad2Deg, yawDir) * rotation;
        if (sceneView.size < 0)
        {
            sceneView.pivot = sceneView.camera.transform.position;
            sceneView.size = 0;
        }

        sceneView.rotation = rotation;
    }
}