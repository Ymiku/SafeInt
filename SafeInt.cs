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
	[SerializeField]
	private SafeValue _safeIntValue;
	[SafeField("EncryptInEditor")]
	public int safeInt;
	//在editor里测试时更改safeInt的值即可，不要更改_safeIntValue和 _hash，真机运行时一定要在使用前重新生成实例，不能直接使用editor里的设置
	//例如，在editor里设置了safeInt为10，真机时，一定要在start或awake中重新赋值，safeInt=10，否则会闪退
	private int _safeInt{
		get{
			return (_safeIntValue.sv+5)^_key;
		}
		set{
			SafeValue sh = GetSafeValue ();
			sh.sv = (value^_key)-5;
			if(_safeIntValue!=null)
				safeValueStack.Push (_safeIntValue);
			_safeIntValue = sh;
		}
	}
	[SerializeField]
	private int _hash;
	private static int _key;
	private static Stack<SafeValue> safeValueStack = new Stack<SafeValue>();

	private static int zeroCipher;
	private static int zeroHash;

	static SafeInt()
	{
		_key = Random.Range (76005,5313000);
		#if UNITY_EDITOR
			_key = 15731;
		#endif
		zeroCipher = (0^_key)-5;
		int x = 0;
		x = (x << 13) ^ x;
		zeroHash = (int)((1f - ((x * (x * x * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824f)*10000000f)^_key; 
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
	/// <summary>
	/// Check if the int is zero.
	/// </summary>
	public bool IsZero()
	{
		if (_safeIntValue.sv == zeroCipher && _hash == zeroHash)
			return true;
		return false;
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
