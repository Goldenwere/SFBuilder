using UnityEngine;

namespace Goldenwere.Unity
{
    /// <summary>
    /// Extensions to the Unity API
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Find the child with a specific name of a gameobject
        /// </summary>
        /// <param name="parent">The parent gameobject to search from</param>
        /// <param name="name">The name that the child must match</param>
        /// <returns>The found GameObject or null</returns>
        public static GameObject FindChild(this GameObject parent, string name)
        {
            if (parent == null)
                throw new System.ArgumentNullException();

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                GameObject child = parent.transform.GetChild(i).gameObject;
                if (child.name == name)
                    return child;
            }

            return null;
        }

        /// <summary>
        /// Find the child with a specific name of a gameobject, recursively searching through all gameobjects from topmost level down
        /// </summary>
        /// <param name="parent">The parent gameobject to search from</param>
        /// <param name="name">The name that the child must match</param>
        /// <returns>The found GameObject or null</returns>
        public static GameObject FindChildRecursively(this GameObject parent, string name)
        {
            if (parent == null)
                throw new System.ArgumentNullException();

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                GameObject child = parent.transform.GetChild(i).gameObject;
                if (child.name == name)
                    return child;

                else if (child.transform.childCount > 0)
                {
                    GameObject foundTest = child.FindChildRecursively(name);
                    if (foundTest != null)
                        return foundTest;
                }
            }

            return null;
        }

        /// <summary>
        /// Find the child with a specific tag of a gameobject
        /// </summary>
        /// <param name="parent">The parent gameobject to search from</param>
        /// <param name="tag">The tag that the child must match</param>
        /// <returns>The found GameObject or null</returns>
        public static GameObject FindChildWithTag(this GameObject parent, string tag)
        {
            if (parent == null)
                throw new System.ArgumentNullException();

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                GameObject child = parent.transform.GetChild(i).gameObject;
                if (child.tag == tag)
                    return child;
            }

            return null;
        }

        /// <summary>
        /// Find the child with a specific tag of a gameobject
        /// </summary>
        /// <param name="parent">The parent gameobject to search from</param>
        /// <param name="tag">The tag that the child must match</param>
        /// <returns>The found GameObject or null</returns>
        public static GameObject FindChildWithTagRecursively(this GameObject parent, string tag)
        {
            if (parent == null)
                throw new System.ArgumentNullException();

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                GameObject child = parent.transform.GetChild(i).gameObject;
                if (child.tag == tag)
                    return child;

                else if (child.transform.childCount > 0)
                {
                    GameObject foundTest = child.FindChildWithTagRecursively(tag);
                    if (foundTest != null)
                        return foundTest;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds a matching component 
        /// </summary>
        /// <typeparam name="T">The Component being searched for</typeparam>
        /// <param name="other">The GameObject to search from</param>
        /// <returns>The found component reference if not null</returns>
        public static T GetComponentInParents<T>(this GameObject other) where T : Component
        {
            if (other == null)
                throw new System.ArgumentNullException();

            Transform parent = other.transform.parent;
            while (parent != null)
            {
                T pComp = parent.GetComponent<T>();
                if (pComp != null)
                    return pComp;
                else
                    parent = parent.transform.parent;
            }
            return null;
        }

        /// <summary>
        /// Clamps a quaternion's vertical rotation
        /// </summary>
        /// <param name="parent">The quaternion to clamp</param>
        /// <param name="min">The lowest possible vertical rotation in degrees</param>
        /// <param name="max">The highest possible vertical rotation in degrees</param>
        /// <returns>The clamped quaternion</returns>
        public static Quaternion VerticalClampEuler(this Quaternion parent, float min, float max)
        {
            parent.x /= parent.w;
            parent.y /= parent.w;
            parent.z /= parent.w;
            parent.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(parent.x);

            angleX = Mathf.Clamp(angleX, min, max);

            parent.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return parent;
        }

        /// <summary>
        /// Rotates a point around another point
        /// </summary>
        /// <param name="self">The original point to rotate</param>
        /// <param name="pivot">The point to rotate around</param>
        /// <param name="eulerAngles">The angle at which the point is being rotated</param>
        /// <returns>The original point after rotation</returns>
        public static Vector3 RotateSelfAroundPoint(this Vector3 self, Vector3 pivot, Vector3 eulerAngles)
		{
            return Quaternion.Euler(eulerAngles) * (self - pivot) + pivot;
		}

        /// <summary>
        /// Rounds a Vector3's values to the nearest precision
        /// </summary>
        /// <param name="self">The Vector3 being rounded</param>
        /// <param name="precision">The precision</param>
        /// <returns></returns>
        public static Vector3 ToPrecision(this Vector3 self, float precision)
        {
            return new Vector3(
                Mathf.Round(self.x / precision) * precision,
                Mathf.Round(self.y / precision) * precision,
                Mathf.Round(self.z / precision) * precision);
        }

        /// <summary>
        /// Unity serialization escapes double-slashes, which breaks any sort of desired escaping in serialized string fields
        /// </summary>
        /// <param name="other">The string that needs fixed</param>
        /// <returns>The string after fixing</returns>
        public static string RepairSerializedEscaping(this string other)
        {
            return other
                .Replace("\\n", "\n")
                .Replace("\\t", "\t")
                .Replace("\\'", "\'")
                .Replace("\\\"", "\"")
                .Replace("\\\\", "\\")
                .Replace("\\b", "\b")
                .Replace("\\r", "\r");
        }
    }
}