using UnityEngine;
public static class Helpers
{
    public static bool InTagInTree(GameObject obj, string tag)
    {
        if (obj.tag.Equals(tag))
        {
            return true;
        }
        Transform tf = obj.transform;
        while (true)
        {
            if (tf == null)
            {
                return false;
            }
            if (tf.gameObject.tag.Equals(tag))
            {
                return true;
            }
            tf = tf.parent;
        }
    }
}