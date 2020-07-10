using UnityEngine;

namespace SFBuilder.Obj
{
    /// <summary>
    /// Structure for placement system to associate types to prefabs
    /// </summary>
    [System.Serializable]
    public struct BuilderObjectTypeToPrefab
    {
        public ObjectType type;
        public GameObject prefab;
    }
}