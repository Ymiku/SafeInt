using UnityEngine;
using System.Collections;

public class SafeFieldAttribute : PropertyAttribute
{
	public bool IsDirty { get; set; }
	public bool IsFirst{ get; set; }
	public string Name{ get; set;}
	public SafeFieldAttribute(string name)
	{
		Name = name;
		IsFirst = true;
	}
}
