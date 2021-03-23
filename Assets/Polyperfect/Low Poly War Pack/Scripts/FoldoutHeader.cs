
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;

// This class defines the ColorSpacer attribute, so that
// it can be used in your regular MonoBehaviour scripts:

public class FoldoutHeader : PropertyAttribute
{
    public string header;
    public string lastProperty;
    public bool folded;
    public FoldoutHeader(string header, string lastProperty)
    {
        this.header = header;
        this.lastProperty = lastProperty;
        // unfortunately we can't pass a color through as a Color object
        // so we pass as 3 floats and make the object here
    }
}


// This defines how the ColorSpacer should be drawn
// in the inspector, when inspecting a GameObject with
// a MonoBehaviour which uses the ColorSpacer attribute

[CustomPropertyDrawer(typeof(FoldoutHeader))]
public class FoldoutHeaderDrawer : PropertyDrawer
{
    FoldoutHeader foldoutHeader
    {
        get { return ((FoldoutHeader)attribute); }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUIStyle style = EditorStyles.foldout;
        FontStyle previousStyle = style.fontStyle;
        style.fontStyle = FontStyle.Bold;
       
        // calculate the rect values for where to draw the line in the inspector
        foldoutHeader.folded = EditorGUI.Foldout(position, foldoutHeader.folded, foldoutHeader.header,true,style);
        style.fontStyle = previousStyle;
        if (foldoutHeader.folded)
        {
            
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(property,false);

            while ( property.Next(false))
            {
                Debug.Log(property.name+" "+property.isArray+" "+property.hasChildren);
                    EditorGUILayout.PropertyField(property.serializedObject.FindProperty(property.name), true);
                if (property.name == foldoutHeader.lastProperty) { break; }
            }
            EditorGUI.indentLevel--;
        }
    }
    public void DrawProperties(SerializedProperty p,bool childer)
    {
       
        foreach(SerializedProperty property in p)
        {
                EditorGUILayout.PropertyField(property, true);

        }
    }
}
#endif