using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//dictionary for binary sort, faster then dictionary in big data.
public class DictionarySort<K,T>
{
	private List<K> mListKey;
	private List<T> mListValue;

	public DictionarySort()
	{
		mListKey = new List<K>();
		mListValue = new List<T>();
	}

	public DictionarySort(int size)
	{
		mListKey = new List<K>(size);
		mListValue = new List<T>(size);
	}

	//whether contain the key
	public bool ContainsKey(K _key)
	{
		int index = CQuickSort.Search<K>(mListKey, _key, CompareFunction);
		if(index >= 0) return true;
		return false;
	}

	//try get value by key
	public bool TryGet(K _key, out T _value)
	{
		int index = CQuickSort.Search<K>(mListKey, _key, CompareFunction);
		if(index >= 0)
		{
			_value = mListValue[index];
			return true;
		}
		_value = default(T);
		return false;
	}

	public T this[K _key] 
	{
		get
		{
			T obj;
			if(TryGet(_key, out obj))
			{
				return obj;
			}

			return default(T);
		}
		set
		{
			int index = CQuickSort.Search<K>(mListKey, _key, CompareFunction);
			if(index < 0)
			{
				Add(_key,value);
				throw new Exception("Key not exist, key " +_key);
			}
			else
			{
				mListValue[index] = value;
			}
		}
	}

	//add by key and value
	public bool Add(K _key, T _value)
	{
		int index = CQuickSort.Search<K>(mListKey, _key, CompareFunction);
		if(index >= 0)
		{
			throw new Exception("already have key " +_key);
			return false;
		}

		index = CQuickSort.SearchInsert<K>(mListKey, _key, CompareFunction);
		if(index >= mListKey.Count)
		{
			mListKey.Add(_key);
			mListValue.Add(_value);
		}
		else
		{
			mListKey.Insert(index, _key);
			mListValue.Insert(index, _value);
		}

		return true;
	}

	//remove by key
	public bool Remove(K _key)
	{
		int index = CQuickSort.Search<K>(mListKey, _key, CompareFunction);
		if(index < 0)
		{
			throw new Exception("no key to remove, key:" + _key);
			return false;
		}

		mListKey.RemoveAt(index);
		mListValue.RemoveAt(index);

		return true;
	}

	//compare function
	protected virtual int CompareFunction(K _a, K _b)
	{
		return 0;
	}

}

