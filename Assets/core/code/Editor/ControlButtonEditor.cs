using UnityEditor;

namespace SFBuilder.UI
{
    /// <summary>
    /// Editor code for the ControlButton
    /// </summary>
    [CustomEditor(typeof(ControlButton))]
    public class ControlButtonEditor : UnityEditor.UI.ButtonEditor
    {
        /// <summary>
        /// Overrides and calls OnInspectorGUI for Button before displaying additional fields created by the ControlButton
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Separator();
            SerializedProperty control = serializedObject.FindProperty("associatedControl");
            EditorGUILayout.PropertyField(control);
            SerializedProperty input = serializedObject.FindProperty("expectedInput");
            EditorGUILayout.PropertyField(input);
            serializedObject.ApplyModifiedProperties();
        }
    }
}