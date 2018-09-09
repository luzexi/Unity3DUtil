using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CQuickSort
{
	public delegate int CompareFunction<T>(T a, T b );

	// base on sort sequence find a object by binary sort
	public static int Search<T>(List<T> A,T v, CompareFunction<T> _compareFunction)
	{
	  int x,y,m;
	  x = 0;
	  y = A.Count;
	  while(x < y)
	  {
	    m = x+(y-x)/2;
	    if(_compareFunction(A[m],v) == 0) return m;
	    else if(_compareFunction(A[m], v) > 0) y = m;
	    else x = m+1;
	  }
	  return -1;
	}

	// quick one, make it in 3 parts , left one is smaller , middle one is equal , right one is bigger.
	public static void Sort<T>(List<T> arr, int low, int high, CompareFunction<T> _compareFunction )
	{
		if(low >= high)
			return;

		int first = low ;
		int last = high ;
		T key = arr[low];

		while(first < last)
		{
			while(first < last && _compareFunction(key, arr[last]) <= 0)
				last--;
			arr[first] = arr[last];
			while(first < last && _compareFunction(key, arr[first]) >= 0)
				first++;
			arr[last] = arr[first];
		}

		arr[first] = key;

		Sort(arr, low, first-1, _compareFunction);
		Sort(arr, first+1, high, _compareFunction);
	}

	// normal one
	public static void SortNormal<T>(List<T> arr, int left, int right, CompareFunction<T> _compareFunction )
	{
		if(_compareFunction  == null) return;

	    int i = left, j = right;
	    int mid = (i+j)/2;

	    T midValue = arr[mid];

	    while( i < j && i <= mid && mid <= j)
	    {
	        while( _compareFunction(arr[i], midValue) < 0 && mid > i ) i++;
	        while( _compareFunction(arr[j], midValue) > 0 && mid < j ) j--;
	        if(i < j && _compareFunction(arr[i],arr[j]) > 0 )
	        {
	            T tmp;
	            tmp = arr[i]; arr[i] = arr[j]; arr[j] = tmp;

	            if(mid == i)
	            {
	            	mid = j;
	            }
	            if(mid == j)
	            {
	            	mid = i;
	            }
	        }
	        else
	        {
	        	break;
	        }
	    }
	    if(mid+1 < right) Sort(arr, mid+1, right, _compareFunction);
	    if(left < mid-1) Sort(arr,left, mid-1, _compareFunction);
	}

	////// test //////////////////////////

	public static void test_find()
	{
		List<int> arr = new List<int>(10);
		arr.Add(2);
		arr.Add(9);
		arr.Add(4);
		arr.Add(5);
		arr.Add(3);
		arr.Add(2);
		arr.Add(8);
		arr.Add(1);
		arr.Add(7);

		CQuickSort.Sort<int>(arr,0,arr.Count-1,test_compare_function);

		string out1 = "";
		for(int i = 0 ; i<arr.Count ; i++)
		{
			out1 += " " + arr[i].ToString();
		}
		Debug.LogError(out1);

		int _index = -1;
		_index = CQuickSort.Search<int>(arr, 9, test_compare_function);

		Debug.LogError("_index " + _index);
	}

	public static void test_sort()
	{
		List<int> arr = new List<int>(10);
		arr.Add(2);
		arr.Add(9);
		arr.Add(4);
		arr.Add(5);
		arr.Add(3);
		arr.Add(2);
		arr.Add(8);
		arr.Add(1);
		arr.Add(7);

		// List<int> arr = new List<int>(10);
		// arr.Add(2);
		// arr.Add(1);

		// List<int> arr = new List<int>(10);
		// arr.Add(2);
		// arr.Add(1);
		// arr.Add(4);

		// List<int> arr = new List<int>(10);
		// arr.Add(1);
		// arr.Add(2);
		// arr.Add(3);
		// arr.Add(4);
		// arr.Add(5);
		// arr.Add(6);
		// arr.Add(7);
		// arr.Add(8);

		// List<int> arr = new List<int>(10);
		// arr.Add(2);
		// arr.Add(2);
		// arr.Add(2);
		// arr.Add(2);
		// arr.Add(2);
		// arr.Add(2);
		// arr.Add(2);
		// arr.Add(2);

		string out1 = "";

		out1 = "";
		for(int i = 0 ; i<arr.Count ; i++)
		{
			out1 += " " + arr[i].ToString();
		}
		Debug.LogError(out1);

		CQuickSort.Sort<int>(arr,0,arr.Count-1,test_compare_function);

		out1 = "";
		for(int i = 0 ; i<arr.Count ; i++)
		{
			out1 += " " + arr[i].ToString();
		}
		Debug.LogError(out1);
	}

	static int test_compare_function(int a, int b)
	{
		return a-b;
	}
}
