﻿using System.Collections.Generic;
using UnityEngine;

namespace MH_AIFramework
{
	/// <summary>
	/// Behaviour Tree 에서 쓰는 데이터를 저장하는 저장소..
	/// 한 마디로 AI에서 사용하는 데이터들을 모아둔 곳..
	/// </summary>
	public sealed class Blackboard
	{
		private Dictionary<string, string> _stringContainer = new Dictionary<string, string>();
		private Dictionary<string, float> _floatContainer = new Dictionary<string, float>();
		private Dictionary<string, int> _intContainer = new Dictionary<string, int>();
		private Dictionary<string, bool> _boolContainer = new Dictionary<string, bool>();
		private Dictionary<string, Vector3> _vec3Container = new Dictionary<string, Vector3>();
		private Dictionary<string, Quaternion> _quatContainer = new Dictionary<string, Quaternion>();
		private Dictionary<string, object> _objectContainer = new Dictionary<string, object>();

		/// <summary>
		/// 모든 Container들을 클리어 시키는 함수..
		/// </summary>
		public void Clear()
		{
			_stringContainer.Clear();
			_floatContainer.Clear();
			_intContainer.Clear();
			_boolContainer.Clear();
			_vec3Container.Clear();
			_quatContainer.Clear();
			_objectContainer.Clear();
		}

		// ===============================================================================================================================
		// --- Data Getter..
		// ===============================================================================================================================
//#if UNITY_EDITOR
//		public string GetStringValue(string key)
//		{
//			if (!_stringContainer.ContainsKey(key))
//				Debug.Assert(false, key + " invalid key");
//			return _stringContainer[key];
//		}
//		public float GetFloatValue(string key)
//		{
//			if (!_floatContainer.ContainsKey(key))
//				Debug.Assert(false, key + " invalid key");
//			return _floatContainer[key];
//		}
//		public int GetIntValue(string key)
//		{
//			if (!_intContainer.ContainsKey(key))
//				Debug.Assert(false, key + " invalid key");
//			return _intContainer[key];
//		}
//		public bool GetBoolValue(string key)
//		{
//			if (!_boolContainer.ContainsKey(key))
//				Debug.Assert(false, key + " invalid key");
//			return _boolContainer[key];
//		}
//		public void GetVectorValue(string key, out Vector3 get)
//		{
//			if (!_vec3Container.ContainsKey(key))
//				Debug.Assert(false, key + " invalid key");
//			get = _vec3Container[key];
//		}
//		public void GetRotatorValue(string key, out Quaternion get)
//		{
//			if (!_quatContainer.ContainsKey(key))
//				Debug.Assert(false, key + " invalid key");
//			get = _quatContainer[key];
//		}
//		public object GetObjectValue(string key)
//		{
//			if (!_objectContainer.ContainsKey(key))
//				Debug.Assert(false, key + "invalid key");
//			return _objectContainer[key];
//		}
//#else
		public string GetStringValue( string key ) => _stringContainer[key];
		public float GetFloatValue( string key ) => _floatContainer[key];
		public int GetIntValue( string key ) => _intContainer[key];
		public bool GetBoolValue( string key ) => _boolContainer[key];
		public void GetVectorValue(string key, out Vector3 get) => get = _vec3Container[key];
		public void GetRotatorValue(string key, out Quaternion get) => get = _quatContainer[key];
		public object GetObjectValue( string key ) => _objectContainer[key];
//#endif

		// ===============================================================================================================================
		// --- Data Setter..
		// ===============================================================================================================================
		public void SetStringValue(string key, string value) => _stringContainer[key] = value;
		public void SetFloatValue(string key, float value) => _floatContainer[key] = value;
		public void SetIntValue(string key, int value) => _intContainer[key] = value;
		public void SetBoolValue(string key, bool value) => _boolContainer[key] = value;
		public void SetVectorValue(string key, in Vector3 value) => _vec3Container[key] = value;
		public void SetRotatorValue(string key, in Quaternion value) => _quatContainer[key] = value;
		public void SetObjectValue(string key, object value) => _objectContainer[key] = value;
	}
}