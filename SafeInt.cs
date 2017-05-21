using UnityEngine;
using System.Collections;
[System.Serializable]
public class SafeInt {
	[SerializeField,SafeField("CalculateHash")]
	private int _safeInt;
	[SerializeField]
	private int _hash;
	[SerializeField]
	private static int _key;
	static SafeInt()
	{
		_key = Random.Range (1000000,8000000);
	}
	public static SafeInt zero()
	{
		return new SafeInt(0);
	}
	public SafeInt(int i)
	{
		_safeInt = i;
		_hash = 0;
		CalculateHash ();
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
	public void CalculateHash()
	{
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
