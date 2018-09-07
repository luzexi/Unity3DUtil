using System;
using System.Collections.Generic;

public class Heap<T> where T : IComparable<T>
{
    public enum HeapType
    {
        Max,
        Min
    }

    internal delegate bool IndexComparor(int childIx, int parentIx);
    internal IndexComparor IsHigherPriorityThanParent;

    #region Constructors

    /// <summary>
    /// Constructs a new max heap.
    /// </summary>
    public Heap() : this(HeapType.Min) {}

    /// <summary>
    /// Constructs a new heap with the given type, allowing you to specify if this heap will be a min or a max heap.
    /// </summary>
    /// <param name="type"></param>
    public Heap(HeapType type)
    {
        this.List = new List<T>();
        this.Init(type);
    }
    
    /// <summary>
    /// Constructs a new heap with the given type and initial capacity.
    /// </summary>
    /// <param name="type">The type of heap that determines if it will be a min or max heap.</param>
    /// <param name="capacity">The initial capacity of the heap.</param>
    public Heap(HeapType type, int capacity)
    {
        this.List = new List<T>(capacity);
        this.Init(type);
    }

    /// <summary>
    /// Initializes some properties of the tree
    /// </summary>
    /// <param name="type"></param>
    private void Init(HeapType type)
    {
        this.Type = type;

        if (type == HeapType.Max)
            this.IsHigherPriorityThanParent = this.IsLargerThan;
        else
            this.IsHigherPriorityThanParent = this.IsSmallerThanParent;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Identifies the heap as either being a max heap or a min heap
    /// </summary>
    public HeapType Type { get; internal set; }

    internal List<T> List { get; set; }

    /// <summary>
    /// Returns the number of items in the heap
    /// </summary>
    public int Count { get { return this.List.Count; } }

    #endregion

    #region Indexers

    /// <summary>
    /// Given an index of a node, this method returns the index of that node's parent
    /// </summary>
    internal static int ParentIndexOf(int ix)
    {
        if (ix == 0)
            throw new Exception("Resources.Errors.ParentOfRootDoesntExist");
        else
        {
            return (int)Math.Floor((double)(ix - 1) / 2);
        }
    }

    /// <summary>
    /// Given an index of a node, this method returns the index of that node's left child
    /// </summary>
    internal static int LeftChildIndexOf(int ix)
    {
        return (2 * ix) + 1;
    }

    /// <summary>
    /// Given an index of a node, this method returns the index of that node's right child
    /// </summary>
    internal static int RightChildIndexOf(int ix)
    {
        return (2 * ix) + 2;
    }

    #endregion

    #region Heap Compare Functions

    /// <summary>
    /// Returns true if the value of the node at the first index is larger than the value of the node at the second index.
    /// </summary>
    /// <param name="first">The index of the first value</param>
    /// <param name="second">The index of the second value</param>
    internal bool IsLargerThan(int first, int second)
    {
        return this.List[first].CompareTo(this.List[second]) > 0;
    }

    /// <summary>
    /// Returns true if the value of the node at the first index is smaller than the value of the node at the second index.
    /// </summary>
    /// <param name="first">The index of the first value</param>
    /// <param name="second">The index of the second value</param>
    internal bool IsSmallerThanParent(int first, int second)
    {
        return this.List[first].CompareTo(this.List[second]) < 0;
    }

    #endregion

    #region Swap

    /// <summary>
    /// Swaps the value at index ix1 with the value at index ix2
    /// </summary>
    internal void Swap(int ix1, int ix2)
    {
        T temp = this.List[ix1];
        this.List[ix1] = this.List[ix2];
        this.List[ix2] = temp;
    }

    #endregion

    #region Insert

    /// <summary>
    /// Inserts the given value into the heap, maintaining the heap property as defined by Heap.HeapType
    /// </summary>
    public void Insert(T value)
    {
        int insertedAtIndex = this.List.Count;
        this.List.Add(value);
        this.HeapifyUp(insertedAtIndex);
    }

    /// <summary>
    /// Helper function that takes the node inserted at insertedIx and makes sure it is bubbled up to the correct place in the heap.
    /// </summary>
    internal void HeapifyUp(int insertedIx)
    {
        int currentIx = insertedIx;

        while (currentIx > 0)
        {
            int parentIx = ParentIndexOf(currentIx);
            if (this.IsHigherPriorityThanParent(currentIx, parentIx))
                this.Swap(currentIx, parentIx);
            else
                break;

            currentIx = parentIx;
        }
    }

    #endregion

    #region Remove

    /// <summary>
    /// Removes the node at the top of the heap and returns its value.
    /// </summary>
    /// <returns>The largest node in the heap if the HeapType = Max, the smallest node in the heap if the HeapType = Min</returns>
    public T Remove()
    {
        int lastIx = this.List.Count - 1;
        T result = default(T);
        if (lastIx < 0)
            throw new Exception("Resources.Errors.CannotRemoveFromEmptyHeap");
        else
        {
            result = this.List[0];

            if (lastIx > 0)
                this.List[0] = this.List[lastIx];

            this.List.RemoveAt(lastIx);
            this.HeapifyDown();
        }

        return result;
    }

    /// <summary>
    /// Starting at the top of the heap, examines all nodes underneath it ensuring that the heap property is maintained at all nodes.
    /// </summary>
    internal void HeapifyDown()
    {
        int parentIx = 0;
        while (parentIx < this.Count)
        {
            int leftIx = LeftChildIndexOf(parentIx);
            int rightIx = RightChildIndexOf(parentIx);
            int largest = parentIx;

            if (leftIx < this.Count && this.IsHigherPriorityThanParent(leftIx, largest))
                largest = leftIx;

            if (rightIx < this.Count && this.IsHigherPriorityThanParent(rightIx, largest))
                largest = rightIx;

            if (largest != parentIx)
            {
                this.Swap(largest, parentIx);
                parentIx = largest;
            }
            else
                break;
        }
    }

    #endregion

    #region Peek

    /// <summary>
    /// Returns the value of the node at the top of the heap without removing it.
    /// </summary>
    public T Peek()
    {
        if (this.List.Count > 0)
            return this.List[0];
        else
            throw new Exception("Resources.Errors.PeekEmptyTree");
    }

    #endregion


    #region test
    ////// test //////////////////////////

    class Testobj : IComparable<Testobj>
    {
        public int v;

        public Testobj(int _v)
        {
            v = _v;
        }

        public int CompareTo(Testobj b)
        {
            return this.v - b.v;
        }
    }

    public static void test_push()
    {

        Heap<Testobj> test_heap = new Heap<Testobj>();
        test_heap.Insert(new Testobj(9));
        test_heap.Insert(new Testobj(2));
        test_heap.Insert(new Testobj(6));
        test_heap.Insert(new Testobj(7));
        test_heap.Insert(new Testobj(3));
        test_heap.Insert(new Testobj(4));

        Testobj t1 = test_heap.Remove();
        Debug.LogError("" + t1.v);

        t1 = test_heap.Remove();
        Debug.LogError("" + t1.v);

        t1 = test_heap.Remove();
        Debug.LogError("" + t1.v);

        t1 = test_heap.Remove();
        Debug.LogError("" + t1.v);

        t1 = test_heap.Remove();
        Debug.LogError("" + t1.v);

    }

    #endregion
}