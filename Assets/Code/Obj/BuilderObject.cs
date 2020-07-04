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
        [SerializeField] private MeshRenderer           objectBody;
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
                    objectBody.material = materialNormal;
                    grounder.enabled = false;
                    ranger.SetPlaced(true);
                    ranger.enabled = false;
                    objectPlaced?.Invoke(this);
                }
                else
                {
                    objectBody.material = materialPlacing;
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
                    objectBody.material.SetVector("_FirstOutlineColor", new Vector4(0.04f, 1, 0.08f, 0.5f));
                else
                    objectBody.material.SetVector("_FirstOutlineColor", new Vector4(0.57f, 0, 0, 0.5f));
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
                case ObjectType.PrototypeA: return "Power Source";
                case ObjectType.PrototypeB: return "Residence";
                case ObjectType.PrototypeC: return "Community Garden";
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
                case ObjectType.PrototypeA:
                    happiness = 0;
                    power = 3;
                    sustenance = 0;
                    break;
                case ObjectType.PrototypeB:
                    happiness = 0;
                    power = -5;
                    sustenance = -5;
                    break;
                case ObjectType.PrototypeC:
                    happiness = 0;
                    power = 0;
                    sustenance = 5;
                    break;
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
                case ObjectType.PrototypeA:
                    switch (existing)
                    {
                        case ObjectType.PrototypeA:
                            happiness = 0;
                            power = 3;
                            sustenance = 0;
                            break;
                        case ObjectType.PrototypeB:
                            happiness = -5;
                            power = 0;
                            sustenance = 0;
                            break;
                        case ObjectType.PrototypeC:
                            happiness = 0;
                            power = 0;
                            sustenance = 0;
                            break;
                    }
                    break;
                case ObjectType.PrototypeB:
                    switch (existing)
                    {
                        case ObjectType.PrototypeA:
                            happiness = 0;
                            power = 0;
                            sustenance = 0;
                            break;
                        case ObjectType.PrototypeB:
                            happiness = 0;
                            power = 0;
                            sustenance = 0;
                            break;
                        case ObjectType.PrototypeC:
                            happiness = 0;
                            power = 0;
                            sustenance = 0;
                            break;
                    }
                    break;
                case ObjectType.PrototypeC:
                    switch (existing)
                    {
                        case ObjectType.PrototypeA:
                            happiness = 0;
                            power = 0;
                            sustenance = 0;
                            break;
                        case ObjectType.PrototypeB:
                            happiness = 3;
                            power = 0;
                            sustenance = 3;
                            break;
                        case ObjectType.PrototypeC:
                            happiness = 0;
                            power = 0;
                            sustenance = 0;
                            break;
                    }
                    break;
            }
        }
        #endregion
    }
}