using UnityEngine;
using System.Collections.Generic;

namespace SFBuilder.Obj
{
    public delegate void ProtoObjectDelegate(BuilderObject obj);

    /// <summary>
    /// A BuilderObject represents the structures the player places in the game
    /// </summary>
    public class BuilderObject : MonoBehaviour
    {
        #region Fields
#pragma warning disable 0649
        [SerializeField] private BuilderObjectGrounder  grounder;
        [SerializeField] private Material               materialNormal;
        [SerializeField] private Material               materialPlacing;
        [SerializeField] private MeshRenderer[]         objectBody;
        [SerializeField] private BuilderObjectRanger    ranger;
        [SerializeField] private ObjectType             type;
#pragma warning restore 0649
        /**************/ private List<GameObject>       collidedObjects;
        /**************/ private bool                   isPlaced;
        #endregion
        #region Properties
        /// <summary>
        /// Whether or not something is colliding with this object
        /// </summary>
        public bool IsCollided  { get; private set; }

        /// <summary>
        /// Whether or not the object is grounded
        /// </summary>
        public bool IsGrounded  { get { return grounder.IsGrounded; } }

        /// <summary>
        /// Whether or not the object is placed; used for setting the state of the object and listeners
        /// </summary>
        public bool IsPlaced
        {
            get { return isPlaced; }
            set
            {
                isPlaced = value;
                if (isPlaced)
                {
                    foreach (MeshRenderer child in objectBody)
                        child.material = materialNormal;
                    grounder.enabled = false;
                    ranger.SetPlaced(true);
                    ranger.enabled = false;
                    objectPlaced?.Invoke(this);
                }
                else
                {
                    foreach (MeshRenderer child in objectBody)
                        child.material = materialPlacing;
                    grounder.enabled = true;
                    ranger.SetPlaced(false);
                    ranger.enabled = true;
                    objectRecalled?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Whether or not the object's placement is valid (a combination of ground-state and collision-state)
        /// </summary>
        public bool IsValid     { get { return IsGrounded && !IsCollided; } }

        /// <summary>
        /// The type of the object, set in inspector at prefab level
        /// </summary>
        public ObjectType Type  { get { return type; } }
        #endregion
        #region Events
        /// <summary>
        /// Invoked when the object is placed, sends reference to the related BuilderObject instance
        /// </summary>
        public ProtoObjectDelegate objectPlaced;

        /// <summary>
        /// Invoked when the object is no longer placed (on Undo), sends reference to the related BuilderObject instance
        /// </summary>
        public ProtoObjectDelegate objectRecalled;
        #endregion
        #region Instance Methods
        /// <summary>
        /// Instantiate list on Start
        /// </summary>
        private void Start()
        {
            collidedObjects = new List<GameObject>(16);
        }

        /// <summary>
        /// Set the material outline of the object when in placing state to indicate whether the object's placement will be valid
        /// </summary>
        private void Update()
        {
            if (!IsPlaced)
            {
                if (IsValid)
                    foreach (MeshRenderer child in objectBody)
                        child.material.SetVector("_FirstOutlineColor", new Vector4(0.04f, 1, 0.08f, 0.5f));
                else
                    foreach (MeshRenderer child in objectBody)
                        child.material.SetVector("_FirstOutlineColor", new Vector4(0.57f, 0, 0, 0.5f));
            }
        }

        /// <summary>
        /// When the object is destroyed, ensure other BuilderObjects no longer hold reference to it
        /// </summary>
        private void OnDestroy()
        {
            foreach (GameObject g in collidedObjects)
            {
                BuilderObject other = g.GetComponent<BuilderObject>();
                if (other != null)
                    other.OtherObjectWasDestroyed(gameObject);
            }
        }

        /// <summary>
        /// Track other objects when they enter the collider
        /// </summary>
        /// <param name="other">The other collider that entered</param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.name != "Ranger")
            {
                IsCollided = true;
                collidedObjects.Add(other.gameObject);
            }
        }

        /// <summary>
        /// Untrack other objects when they exit the collider
        /// </summary>
        /// <param name="other">The other collider that exited</param>
        private void OnTriggerExit(Collider other)
        {
            if (other.name != "Ranger")
            {
                collidedObjects.Remove(other.gameObject);
                if (collidedObjects.Count == 0)
                    IsCollided = false;
            }
        }

        /// <summary>
        /// Called by OnDestroy of another BuilderObject in order to stop tracking a collided object
        /// </summary>
        /// <param name="g">The other object that is about to be destroyed</param>
        private void OtherObjectWasDestroyed(GameObject g)
        {
            collidedObjects.Remove(g);
            if (collidedObjects.Count == 0)
                IsCollided = false;
        }
        #endregion
        #region Static Methods
        /// <summary>
        /// Converts ObjectType to a UI-friendly string
        /// </summary>
        /// <param name="type">The type being converted</param>
        /// <returns>What the type represents</returns>
        public static string NameOfType(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Pro_A: return "Power Source";
                case ObjectType.Pro_B: return "Residence";
                case ObjectType.Pro_C: return "Community Garden";
                default: return "Unknown";
            }
        }

        /// <summary>
        /// Determine the base happiness, power, and sustenance of an object
        /// </summary>
        /// <param name="toBePlaced">The type of the object currently being placed</param>
        /// <param name="happiness">Resulting happiness</param>
        /// <param name="power">Resulting power</param>
        /// <param name="sustenance">Resulting sustenance</param>
        public static void ScoreOfSingleType(ObjectType type, out int happiness, out int power, out int sustenance)
        {
            happiness = 0;
            power = 0;
            sustenance = 0;

            switch (type)
            {
                case ObjectType.Pro_A: happiness = +00; power = +03; sustenance = +00; break;
                case ObjectType.Pro_B: happiness = +00; power = -05; sustenance = -05; break;
                case ObjectType.Pro_C: happiness = +00; power = +00; sustenance = +05; break;
                case ObjectType.Env_A: happiness = +00; power = +00; sustenance = +05; break;
                case ObjectType.Env_B: happiness = +00; power = +00; sustenance = +08; break;
                case ObjectType.Env_C: happiness = +00; power = +00; sustenance = +10; break;
                case ObjectType.Env_D: happiness = +00; power = -03; sustenance = +20; break;
                case ObjectType.Res_A: happiness = +00; power = -06; sustenance = -05; break;
                case ObjectType.Res_B: happiness = +00; power = -06; sustenance = -08; break;
                case ObjectType.Res_C: happiness = +00; power = -10; sustenance = -10; break;
                case ObjectType.Res_D: happiness = +00; power = -15; sustenance = -15; break;
                case ObjectType.Pow_A: happiness = +00; power = +03; sustenance = +00; break;
                case ObjectType.Pow_B: happiness = +00; power = +05; sustenance = +00; break;
                case ObjectType.Pow_C: happiness = +00; power = +10; sustenance = +00; break;
                case ObjectType.Pow_D: happiness = +00; power = +06; sustenance = +00; break;
                case ObjectType.Pow_E: happiness = +00; power = +12; sustenance = +00; break;
                case ObjectType.Pow_F: happiness = +00; power = -02; sustenance = +00; break;
            }
        }

        /// <summary>
        /// Determine the net happiness, power, and sustenance between an object being placed and a placed object
        /// </summary>
        /// <param name="toBePlaced">The type of the object currently being placed</param>
        /// <param name="existing">The type of the object that is already placed</param>
        /// <param name="happiness">Resulting happiness</param>
        /// <param name="power">Resulting power</param>
        /// <param name="sustenance">Resulting sustenance</param>
        public static void ScoreOfTwoTypes(ObjectType toBePlaced, ObjectType existing, out int happiness, out int power, out int sustenance)
        {
            happiness = 0;
            power = 0;
            sustenance = 0;

            switch (toBePlaced)
            {
                #region Old
                case ObjectType.Pro_A:
                    switch (existing)
                    {
                        case ObjectType.Pro_A: happiness = +00; power = +03; sustenance = +00; break;
                        case ObjectType.Pro_B: happiness = -05; power = +00; sustenance = +00; break;
                    }
                    break;
                case ObjectType.Pro_B:
                    switch (existing)
                    {
                        case ObjectType.Pro_A: happiness = -05; power = +00; sustenance = +00; break;
                        case ObjectType.Pro_C: happiness = +03; power = +00; sustenance = +03; break;
                    }
                    break;
                case ObjectType.Pro_C:
                    switch (existing)
                    {
                        case ObjectType.Pro_B: happiness = +03; power = +00; sustenance = +03; break;
                    }
                    break;
                #endregion
                #region Power
                case ObjectType.Pow_A:
                case ObjectType.Pow_B:
                    switch(existing)
                    {
                        case ObjectType.Res_A:
                        case ObjectType.Res_B:
                        case ObjectType.Res_C:
                        case ObjectType.Res_D: happiness = -01; power = +00; sustenance = +00; break;
                        case ObjectType.Pow_C: happiness = +00; power = +01; sustenance = +00; break;
                    }
                    break;
                case ObjectType.Pow_C:
                    switch (existing)
                    {
                        case ObjectType.Res_A:
                        case ObjectType.Res_B:
                        case ObjectType.Res_C:
                        case ObjectType.Res_D: happiness = -03; power = +00; sustenance = +00; break;
                        case ObjectType.Pow_A:
                        case ObjectType.Pow_B: happiness = +00; power = +01; sustenance = +00; break;
                    }
                    break;
                #endregion
                #region Residential
                case ObjectType.Res_A:
                case ObjectType.Res_B:
                    switch (existing)
                    {
                        case ObjectType.Pow_A:
                        case ObjectType.Pow_B: happiness = -01; power = +00; sustenance = +00; break;
                        case ObjectType.Pow_C:
                        case ObjectType.Pow_D: happiness = -03; power = +00; sustenance = +00; break;
                        case ObjectType.Pow_E: happiness = -05; power = +00; sustenance = +00; break;
                        case ObjectType.Pow_F: happiness = -08; power = +00; sustenance = +00; break;
                        case ObjectType.Res_A:
                        case ObjectType.Res_B: happiness = +00; power = +01; sustenance = +01; break;
                    }
                    break;
                case ObjectType.Res_C:
                    switch(existing)
                    {
                        case ObjectType.Pow_A:
                        case ObjectType.Pow_B: happiness = -01; power = +00; sustenance = +00; break;
                        case ObjectType.Pow_C:
                        case ObjectType.Pow_D: happiness = -03; power = +00; sustenance = +00; break;
                        case ObjectType.Pow_E: happiness = -05; power = +00; sustenance = +00; break;
                        case ObjectType.Pow_F: happiness = -08; power = +00; sustenance = +00; break;
                        case ObjectType.Res_C:
                        case ObjectType.Res_D: happiness = -05; power = +00; sustenance = +00; break;
                    }
                    break;
                case ObjectType.Res_D:
                    switch (existing)
                    {
                        case ObjectType.Pow_A:
                        case ObjectType.Pow_B: happiness = -01; power = +00; sustenance = +00; break;
                        case ObjectType.Pow_C:
                        case ObjectType.Pow_D: happiness = -03; power = +00; sustenance = +00; break;
                        case ObjectType.Pow_E: happiness = -05; power = +00; sustenance = +00; break;
                        case ObjectType.Pow_F: happiness = -08; power = +00; sustenance = +00; break;
                        case ObjectType.Res_C:
                        case ObjectType.Res_D: happiness = -08; power = +00; sustenance = +00; break;
                    }
                    break;
                #endregion
                #region Environment
                case ObjectType.Env_A:
                    switch (existing)
                    {
                        case ObjectType.Nat_A: happiness = +00; power = +00; sustenance = +02; break;
                        case ObjectType.Env_A: happiness = +00; power = +00; sustenance = +01; break;
                        case ObjectType.Res_A:
                        case ObjectType.Res_B: happiness = +03; power = +00; sustenance = +00; break;
                        case ObjectType.Res_C:
                        case ObjectType.Res_D: happiness = +05; power = +00; sustenance = +00; break;
                    }
                    break;
                #endregion
            }
        }
        #endregion
    }
}