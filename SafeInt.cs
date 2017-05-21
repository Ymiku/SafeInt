using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class SafeInt {
	[System.Serializable]
	public class SafeHash
	{
		public int hash = 0;
	}
	[SerializeField,SafeField("CalculateHash")]
	private int _safeInt;
	[SerializeField]
	private SafeHash _hash;
	[SerializeField]
	private static int _key;
	private static Stack<SafeHash> hashStack = new Stack<SafeHash>();
	static SafeInt()
	{
		_key = Random.Range (1000000,8000000);
		#if UNITY_EDITOR
			_key = 0;
		#endif
	}
	public static SafeInt zero()
	{
		return new SafeInt(0);
	}
	private static SafeHash GetHash()
	{
		if (hashStack.Count > 0)
			return hashStack.Pop ();
		return new SafeHash ();
	}
	public SafeInt(int i)
	{
		_safeInt = i;
		CalculateHash ();
	}
	public bool IsCheat()
	{
		if (_hash.hash == Noise(_safeInt))
			return false;
		Debug.Log ("cheat!You cant init safeInt value in the inspector,init this value in the \"Start\" func instead");
		return true;
	}
	public int Noise(int x)
	{
		x = (x << 13) ^ x;
		return (int)((1f - ((x * (x * x * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824f)*10000000f)^_key;
	}
	public void CalculateHash()
	{
		SafeHash sh = GetHash ();
		sh.hash = Noise (_safeInt);
		if(_hash !=null)
		hashStack.Push (_hash);
		_hash = sh;
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
		CalculateHash ();
	}
	public void Decrease(int i)
	{
		if (IsCheat ())
			Application.Quit ();
		_safeInt -= i;
		CalculateHash ();
	}
	public void Multiply(int i)
	{
		if (IsCheat ())
			Application.Quit ();
		_safeInt *= i;
		CalculateHash ();
	}
	public void Divided(int i)
	{
		if (IsCheat ())
			Application.Quit ();
		_safeInt /= i;
		CalculateHash ();
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
