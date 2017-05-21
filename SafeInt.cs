using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class SafeInt {
	[System.Serializable]
	public class SafeValue
	{
		public int sv = 0;
	}
	[SerializeField,HideInInspector]
	private SafeValue _safeIntValue;
	[SafeField("EncryptInEditor")]
	public int safeInt;
	private int _safeInt{
		get{
			return _safeIntValue.sv^_key + 5;
		}
		set{
			SafeValue sh = GetSafeValue ();
			sh.sv = (value - 5)^_key;
			if(_safeIntValue!=null)
				safeValueStack.Push (_safeIntValue);
			_safeIntValue = sh;
		}
	}
	[SerializeField,HideInInspector]
	private int _hash;
	private static int _key;
	private static Stack<SafeValue> safeValueStack = new Stack<SafeValue>();
	static SafeInt()
	{
		_key = Random.Range (76005,5313000);
		#if UNITY_EDITOR
			_key = 15731;
		#endif
	}
	public static SafeInt zero()
	{
		return new SafeInt(0);
	}
	private static SafeValue GetSafeValue()
	{
		if (safeValueStack.Count > 0)
			return safeValueStack.Pop ();
		return new SafeValue ();
	}
	public SafeInt(int i)
	{
		safeInt = Random.Range (76005,5313000);
		_safeInt = i;
		Encrypt ();
	}
	public bool IsCheat()
	{
		if (_hash == Noise(_safeInt))
			return false;
		Debug.Log ("cheat!You cant init safeInt value in the inspector,init this value in the \"Start\" func instead");
		return true;
	}
	public int Noise(int x)
	{
		x = (x << 13) ^ x;
		return (int)((1f - ((x * (x * x * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824f)*10000000f)^_key;
	}
	public void Encrypt()
	{
		#if UNITY_EDITOR 
		 safeInt = _safeInt;
		#endif
		_hash = Noise (_safeInt);
	}
	public void EncryptInEditor()
	{
		#if UNITY_EDITOR 
			_safeInt = safeInt;
		#endif
		_hash = Noise (_safeInt);
	}
	public int Get()
	{
		if (IsCheat ())
			Application.Quit ();
		return _safeInt;
	}
	public void Add(int i)
	{
		if (IsCheat ())
			Application.Quit ();
		_safeInt += i;
		Encrypt ();
	}
	public void Decrease(int i)
	{
		if (IsCheat ())
			Application.Quit ();
		_safeInt -= i;
		Encrypt ();
	}
	public void Multiply(int i)
	{
		if (IsCheat ())
			Application.Quit ();
		_safeInt *= i;
		Encrypt ();
	}
	public void Divided(int i)
	{
		if (IsCheat ())
			Application.Quit ();
		_safeInt /= i;
		Encrypt ();
	}
	public static SafeInt operator +(SafeInt lhs,int rhs)
	{
		lhs.Add (rhs);
		return lhs;
	}
	public static SafeInt operator -(SafeInt lhs,int rhs)
	{
		lhs.Decrease (rhs);
		return lhs;
	}
	public static SafeInt operator *(SafeInt lhs,int rhs)
	{
		lhs.Multiply (rhs);
		return lhs;
	}
	public static SafeInt operator /(SafeInt lhs,int rhs)
	{
		lhs.Divided (rhs);
		return lhs;
	}
	public static SafeInt operator ++(SafeInt i)
	{
		i.Add (1);
		return i;
	}
	public static SafeInt operator --(SafeInt i)
	{
		i.Decrease (1);
		return i;
	}
	public static implicit operator int(SafeInt i)
	{
		return i.Get();
	}
	public static implicit operator SafeInt(int i)
	{
		return new SafeInt (i);
	}
}
