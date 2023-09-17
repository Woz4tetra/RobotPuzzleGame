using System;
using UnityEngine;

[Serializable]
public abstract class ConversationAction : MonoBehaviour
{
    public abstract void Render();
    public abstract void Despawn();
}
