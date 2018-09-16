using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//dictionary for string key, faster then dictionary in big data.
public class DictionaryStr<T> : DictionarySort<string, T>
{
	//compare function
	protected override int CompareFunction(string _a, string _b)
	{
		return _a.CompareTo(_b);
	}

	public static void test()
	{
		DictionaryStr<string> dic_str = new DictionaryStr<string>();
		dic_str.Add("1","ok1");
		dic_str.Add("10","ok10");
		dic_str.Add("100","ok100");

		string v = dic_str["1"];
		Debug.LogError("v " + v);

		v = dic_str["10"];
		Debug.LogError("v " + v);

		v = dic_str["100"];
		Debug.LogError("v " + v);

		bool isexist = dic_str.ContainsKey("1");
		Debug.LogError("isexist " + isexist);

		isexist = dic_str.ContainsKey("2");
		Debug.LogError("isexist " + isexist);

		isexist = dic_str.ContainsKey("100");
		Debug.LogError("isexist " + isexist);

		bool is_remove = dic_str.Remove("10");
		Debug.LogError("is remove " + is_remove);

		isexist = dic_str.ContainsKey("10");
		Debug.LogError("isexist " + isexist);

		dic_str["99"] = "ok99";
		Debug.LogError(dic_str["99"]);
	}

	// private List<string> mListKey;
	// private List<T> mListValue;

	// public DictionaryStr()
	// {
	// 	mListKey = new List<string>();
	// 	mListValue = new List<T>();
	// }

	// public DictionaryStr(int size)
	// {
	// 	mListKey = new List<string>(size);
	// 	mListValue = new List<T>(size);
	// }

	// //whether contain the key
	// public bool ContainsKey(string _key)
	// {
	// 	int index = CQuickSort.Search<string>(mListKey, _key, CompareInt);
	// 	if(index >= 0) return true;
	// 	return false;
	// }

	// //try get value by key
	// public bool TryGet(string _key, out T _value)
	// {
	// 	int index = CQuickSort.Search<string>(mListKey, _key, CompareInt);
	// 	if(index >= 0)
	// 	{
	// 		_value = mListValue[index];
	// 		return true;
	// 	}

	// 	_value = default(T);
	// 	return false;
	// }

	// public T this[string index] 
	// {
	// 	get
	// 	{
	// 		T obj;
	// 		if(TryGet(index, out obj))
	// 		{
	// 			return obj;
	// 		}

	// 		return default(T);
	// 	}
	// 	set
	// 	{
	// 		Add(index, value);
	// 	}
	// }

	// //add by key and value
	// public bool Add(string _key, T _value)
	// {
	// 	int index = CQuickSort.Search<string>(mListKey, _key, CompareInt);
	// 	if(index >= 0)
	// 	{
	// 		throw new Exception("already have key " +_key);
	// 		return false;
	// 	}

	// 	index = CQuickSort.SearchInsert<string>(mListKey, _key, CompareInt);
	// 	if(index >= mListKey.Count)
	// 	{
	// 		mListKey.Insert(index, _key);
	// 		mListValue.Insert(index, _value);
	// 	}
	// 	else
	// 	{
	// 		mListKey.Add(_key);
	// 		mListValue.Add(_value);
	// 	}

	// 	return true;
	// }

	// //remove by key
	// public bool Remove(string _key)
	// {
	// 	int index = CQuickSort.Search<string>(mListKey, _key, CompareInt);
	// 	if(index < 0)
	// 	{
	// 		throw new Exception("no key to remove, key:" + _key);
	// 		return false;
	// 	}

	// 	mListKey.RemoveAt(index);
	// 	mListValue.RemoveAt(index);

	// 	return true;
	// }



	// //compare function
	// private int CompareInt(string _a, string _b)
	// {
	// 	return _a.CompareTo(_b);
	// }

}

