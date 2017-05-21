using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;

[CustomPropertyDrawer(typeof(SafeFieldAttribute))]
public class SafeFieldAttributeDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginChangeCheck ();
		EditorGUI.PropertyField(position, property, label);

		SafeFieldAttribute setProperty = attribute as SafeFieldAttribute;
		if (EditorGUI.EndChangeCheck())
		{
			setProperty.IsDirty = true;
		} 
		else if (setProperty.IsDirty||setProperty.IsFirst)
		{
			setProperty.IsFirst = false;
			object parent = GetParentObjectOfProperty(property.propertyPath, property.serializedObject.targetObject);
			Type type = parent.GetType();
			MethodInfo mi = type.GetMethod(setProperty.Name);
			if (mi == null)
			{
				Debug.LogError("Invalid property name: " + setProperty.Name + "\nCheck your [SetProperty] attribute");
			}
			else
			{
				mi.Invoke (parent,null);
			}
			setProperty.IsDirty = false;
		}

	}
	
	private object GetParentObjectOfProperty(string path, object obj)
	{
		string[] fields = path.Split('.');
		if (fields.Length == 1)
		{
			return obj;
		}
			
		FieldInfo fi = obj.GetType().GetField(fields[0], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		obj = fi.GetValue(obj);

		return GetParentObjectOfProperty(string.Join(".", fields, 1, fields.Length - 1), obj);
	}
}
