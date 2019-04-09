using System.Collections;
using System.Collections.Generic;


public class CQuickSort
{
	public delegate int CompareFunction<T>(T a, T b, params object[] args);

	// base on sort sequence find a object by binary sort
	public static int Search<T>(List<T> A,T v, CompareFunction<T> _compareFunction, params object[] args)
	{
	  int x,y,m;
	  x = 0;
	  y = A.Count;
	  while(x < y)
	  {
	    m = x+(y-x)/2;
	    if(_compareFunction(A[m], v, args) == 0) return m;
	    else if(_compareFunction(A[m], v, args) > 0) y = m;
	    else x = m+1;
	  }
	  return -1;
	}

	// base on sort sequence find a object by binary sort
	// if v is the biggest one, will return the count of list
	public static int SearchInsert<T>(List<T> A,T v, CompareFunction<T> _compareFunction, params object[] args)
	{
	  int x,y,m;
	  x = 0;
	  y = A.Count;
	  while(x < y)
	  {
	    m = x+(y-x)/2;
	    if(_compareFunction(A[m], v, args) == 0) return m;
	    else if(_compareFunction(A[m], v, args) > 0) y = m;
	    else x = m+1;
	  }
	  return y;
	}

	// quick one, make it in 3 parts , left one is smaller , middle one is equal , right one is bigger.
	public static void Sort<T>(List<T> arr, int low, int high, CompareFunction<T> _compareFunction, params object[] args)
	{
		if(low >= high)
			return;

		int first = low ;
		int last = high ;

		if (high - low + 1 <= 8)
	    {
	        InsertSort(arr, low, high, _compareFunction, args);
	        return;
	    }

		T key = arr[low];

		while(first < last)
		{
			while(first < last && _compareFunction(key, arr[last], args) <= 0)
				last--;
			arr[first] = arr[last];
			while(first < last && _compareFunction(key, arr[first], args) >= 0)
				first++;
			arr[last] = arr[first];
		}

		arr[first] = key;

		Sort(arr, low, first-1, _compareFunction, args);
		Sort(arr, first+1, high, _compareFunction, args);
	}

	// //三数中值+聚集相等元素
	// public static void Sort(List<T> arr, int low,int high, CompareFunction<T> _compareFunction, params object[] args)
	// {
	//     int first = low;
	//     int last = high;

	//     int left = low;
	//     int right = high;

	//     int leftLen = 0;
	//     int rightLen = 0;

	//     if (high - low + 1 < 3)
	//     {
	//         InsertSort(arr, low, high, _compareFunction, args);
	//         return;
	//     }
	    
	//     //一次分割
	//     T key = SelectMedian(arr,low,high);//使用三数取中法选择枢轴
	        
	//     while(low < high)
	//     {
	//     	int diff = _compareFunction(arr[high], key, args);
	//         while(high > low &&  diff >= 0)
	//         {
	//             if (diff == 0)//处理相等元素
	//             {
	//                 swap(arr, right, high);
	//                 right--;
	//                 rightLen++;
	//             }
	//             high--;
	//         }
	//         arr[low] = arr[high];
	//         diff = _compareFunction(arr[low] , key, args);
	//         while(high > low && diff <= 0)
	//         {
	//             if (diff == 0)
	//             {
	//                 swap(arr, left, low);
	//                 left++;
	//                 leftLen++;
	//             }
	//             low++;
	//         }
	//         arr[high] = arr[low];
	//     }
	//     arr[low] = key;

	//     //一次快排结束
	//     //把与枢轴key相同的元素移到枢轴最终位置周围
	//     int i = low - 1;
	//     int j = first;
	//     // diff = _compareFunction(arr[i] , key, args)
	//     while(j < left && _compareFunction(arr[i] , key, args) != 0)
	//     {
	//         swap(arr, i, j);
	//         i--;
	//         j++;
	//     }
	//     i = low + 1;
	//     j = last;
	//     while(j > right && _compareFunction(arr[i] , key, args) != 0)
	//     {
	//         swap(arr, i, j);
	//         i++;
	//         j--;
	//     }
	//     Sort(arr, first, low - 1 - leftLen);
	//     Sort(arr, low + 1 + rightLen,last);
	// }

	static void InsertSort<T>(List<T> arr, int low,int high, CompareFunction<T> _compareFunction, params object[] args)
	{
	    for (int i = low + 1; i <= high; i++)
	    {
	        int j = i;
	        T target = arr[i];
	 
	        while (j > low && _compareFunction(target, arr[j-1]) < 0)
	        {
	            arr[j] = arr[j - 1];
	            j--;
	        }
	 
	        arr[i] = target;
	    }
	}

	// static void swap(List<T> arr, int index1, int index2)
	// {
	// 	T tmp = arr[index1];
	// 	arr[index1] = arr[index2];
	// 	arr[index2] = tmp;
	// }

	// static T SelectMedian(List<T> a, int left, int right, CompareFunction<T> _compareFunction, params object[] args)
	// {
	//     int midIndex = (left + right)>>1;

	//     int diff = _compareFunction(a[left], a[midIndex], args);
	//     if (diff > 0)
	//     {
	//         swap(a, left, midIndex);
	//     }

	//     diff = _compareFunction(a[left], a[right], args);
	//     if (diff > 0)
	//     {
	//         swap(a, left, right);
	//     }

	//     diff = _compareFunction(a[midIndex], a[right], args);
	//     if (diff > 0)
	//     {
	//         swap(a, midIndex, right);
	//     }

	//     swap(a, midIndex, right);

	//     return a[right-1]; //返回中轴
	// }

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
		//Debug.LogError(out1);

		int _index = -1;
		_index = CQuickSort.Search<int>(arr, 9, test_compare_function);

		//Debug.LogError("_index " + _index);
	}

	public static void test_search_insert()
	{
		List<int> arr = new List<int>(10);
		arr.Add(1);
		arr.Add(2);
		arr.Add(5);
		arr.Add(6);
		arr.Add(7);
		arr.Add(8);
		arr.Add(9);

		string out1 = "";
		for(int i = 0 ; i<arr.Count ; i++)
		{
			out1 += " " + arr[i].ToString();
		}
		//Debug.LogError(out1);

		int _index = -1;

		int search_insert_num = 0;
		//Debug.LogError("SearchInsert " + search_insert_num);
		_index = CQuickSort.SearchInsert<int>(arr, search_insert_num, test_compare_function);
		//Debug.LogError("_index " + _index);

		search_insert_num = 3;
		//Debug.LogError("SearchInsert " + search_insert_num);
		_index = CQuickSort.SearchInsert<int>(arr, search_insert_num, test_compare_function);
		//Debug.LogError("_index " + _index);

		search_insert_num = 10;
		//Debug.LogError("SearchInsert " + search_insert_num);
		_index = CQuickSort.SearchInsert<int>(arr, search_insert_num, test_compare_function);
		//Debug.LogError("_index " + _index);

		search_insert_num = 9;
		//Debug.LogError("SearchInsert " + search_insert_num);
		_index = CQuickSort.SearchInsert<int>(arr, search_insert_num, test_compare_function);
		//Debug.LogError("_index " + _index);

		search_insert_num = 7;
		//Debug.LogError("SearchInsert " + search_insert_num);
		_index = CQuickSort.SearchInsert<int>(arr, search_insert_num, test_compare_function);
		//Debug.LogError("_index " + _index);
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
		//Debug.LogError(out1);

		CQuickSort.Sort<int>(arr,0,arr.Count-1,test_compare_function);

		out1 = "";
		for(int i = 0 ; i<arr.Count ; i++)
		{
			out1 += " " + arr[i].ToString();
		}
		//Debug.LogError(out1);
	}

	static int test_compare_function(int a, int b, params object[] args)
	{
		return a-b;
	}
}
