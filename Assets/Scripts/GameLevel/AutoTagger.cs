using UnityEngine;

public class AutoTagger : MonoBehaviour
{
    [SerializeField] private string tagToApply = "Untagged";
    void Start()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.tag = tagToApply;
        }
    }
}