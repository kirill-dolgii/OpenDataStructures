using System.Collections;

namespace DataStructures.Collections.BinarySearchTree;

/// <summary>
/// Represents a mutable unbalanced BinarySearchTree that stores elements in sorted order.
/// The main operations in binary tree are: search, insert and delete.
/// Worst time complexity of the main operations is O(n) in case of not balanced tree.
/// In case of balanced tree performs the main operations in O(log(n)).
/// </summary>
/// <typeparam name="T"></typeparam>
public class BinarySearchTree<T> : ICollection<T>
{
    /// <summary>
    /// Represents a BinarySearchTree node - the base element of a tree.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class TreeNode : IBinaryNode<TreeNode, T>
    {
        public T Value { get; set; }

        public TreeNode? Parent { get; set; }
        public TreeNode? Left { get; set; }
        public TreeNode? Right { get; set; }

        public TreeNode(T elem, TreeNode? left, TreeNode? right)
        {
            Value = elem;
            Left = left;
            Right = right;
        }

        public TreeNode() : this(default!, null, null) { }

        public TreeNode(T elem) : this(elem, null, null) { }

    }

    private readonly BinarySearchTreeSortOrder _order = BinarySearchTreeSortOrder.Ascdending;
    internal TreeNode? _root;
    private int _size;

    public readonly IComparer<T> Comparer = Comparer<T>.Default;
    public BinarySearchTree()
    {
        _root = null;
        _size = 0;
    }

    public BinarySearchTree(IEnumerable<T> data,
                             BinarySearchTreeSortOrder order,
                             IComparer<T>? comparer)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (!data.Any()) throw new ArgumentException(nameof(data));

        _root = new TreeNode(data.First());
        _size = 1;

        _order = order;
        Comparer = comparer ?? Comparer<T>.Default;

        foreach (T elem in data.Skip(1)) Add(elem);
    }

    public BinarySearchTree(IEnumerable<T> data) : this(data, BinarySearchTreeSortOrder.Ascdending, null) { }

    public BinarySearchTree(IComparer<T> comparer) : this() => Comparer = comparer;

    public BinarySearchTree(BinarySearchTreeSortOrder order) : this() => _order = order;

    public BinarySearchTree(IEnumerable<T> data,
                            IComparer<T> comparer) : this(data, BinarySearchTreeSortOrder.Ascdending, comparer) { }

    public BinarySearchTree(IEnumerable<T> data,
                            BinarySearchTreeSortOrder order) : this(data, order, null) { }

    public BinarySearchTree(BinarySearchTreeSortOrder order,
                            IComparer<T> comparer) : this()
    {
        _order = order;
        Comparer = comparer;
    }

    /// <summary>
    /// Adds an element to the Binary Search Tree in O(n) worse case and O(log(n)) average case.
    /// If the given element already exists in the tree adds a duplicate.
    /// </summary>
    /// <param name="item"></param>
    public void Add(T item)
    {
        if (_root == null)
        {
            _root = new TreeNode(item);
            return;
        }
        Add(_root, ref item);
        _size++;
    }

    // returns the subtree of the given TreeNode that contains new item.
    private TreeNode? Add(TreeNode? addRoot, ref T item)
    {
        if (addRoot == null) { addRoot = new TreeNode(item); }
        else
        {
            var cmp = Compare(item, addRoot.Value);
            if (cmp <= 0) addRoot.Left = Add(addRoot.Left, ref item);
            if (cmp > 0) addRoot.Right = Add(addRoot.Right, ref item);
        }

        return addRoot;
    }

    private int Compare(T item1, T item2)
    {
        int compVal = Comparer.Compare(item1, item2);
        return compVal * (int)_order;
    }

    /// <summary>
    /// Clears the BinarySearchTree. Assigns null to the root element.
    /// </summary>
    public void Clear()
    {
        _root = null;
        _size = 0;
    }

    /// <summary>
    /// If BinarySearchTree contains the specified element returns true and false otherwise.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item)
    {
        var node = _root;
        while (node != null)
        {
            int cmp = Compare(item, node.Value);
            if (cmp < 0) node = node.Left;
            else if (cmp > 0) node = node.Right;
            else return true;
        }

        return false;
    }

    /// <summary>
    /// Copies elements of the BinarySearchTree to the specified array in sorted order.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyTo(T[] array, int arrayIndex)
    {
        using var enumerator = GetEnumerator(TreeTraversal.InOrder);
        int i = arrayIndex;
        for (int j = 0; j < i; j++) enumerator.MoveNext();
        while (enumerator.MoveNext()) array[i++] = enumerator.Current;
    }

    /// <summary>
    /// Extracts the minimal element of the BinarySearchTree in O(1) time.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public T Min()
    {
        if (_root == null) throw new NullReferenceException(nameof(_root));
        return Min(_root).Value;
    }

    private TreeNode Min(TreeNode minRoot)
    {
        while (minRoot.Left != null) minRoot = minRoot.Left;
        return minRoot;
    }

    /// <summary>
    /// Extracts the biggest element of the BinarySearchTree in O(n) worse case and O(log(n)) average case.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public T Max()
    {
        if (_root == null) throw new NullReferenceException(nameof(_root));
        return Max(_root).Value;
    }

    private TreeNode Max(TreeNode maxRoot)
    {
        while (maxRoot.Right != null) maxRoot = maxRoot.Right;
        return maxRoot;
    }

    /// <summary>
    /// Performs removal of specified item from the BinarySearchTree in O(n) worse case and O(log(n)) average case.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(T item)
    {
        if (!Contains(item)) return false;

        _root = Remove(_root, item);
        _size--;
        return true;
    }

    private TreeNode? Remove(TreeNode? removeRoot, T item)
    {
        if (removeRoot == null) return null;

        var cmp = Compare(item, removeRoot.Value);

        if (cmp == 1) removeRoot.Right = Remove(removeRoot.Right, item);
        else if (cmp == -1) removeRoot.Left = Remove(removeRoot.Left, item);
        else
        {
            if (removeRoot.Left == null && removeRoot.Right == null) return null;
            //there are 2 leafs
            if (removeRoot.Right != null && removeRoot.Left != null)
            {
                //find min, replace root with the min and remove min
                var rightMin = Min(removeRoot.Right);

                rightMin.Right = Remove(removeRoot.Right, rightMin.Value);
                rightMin.Left = removeRoot.Left;
                return rightMin;
            }

            return removeRoot.Left ?? removeRoot.Right;
        }

        return removeRoot;
    }

    public int Count => _size;
    public bool IsReadOnly => false;

    public IEnumerator<T> GetEnumerator() => GetEnumerator(TreeTraversal.InOrder);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator(TreeTraversal.InOrder);

    public IEnumerator<T> GetEnumerator(TreeTraversal treeTraversal)
    {
        switch (treeTraversal)
        {
            case TreeTraversal.InOrder: return new InOrderIterator(this);
            case TreeTraversal.PreOrder: return new PreOrderIterator(this);
            case TreeTraversal.LevelOrder: return new LevelOrderIterator(this);
        }

        return new InOrderIterator(this);
    }

    private class InOrderIterator : IEnumerator<T>
    {

        private readonly BinarySearchTree<T> _bst;

        private readonly Stack<TreeNode> _stack = new();

        private TreeNode _trav;
        private TreeNode _current;

        public InOrderIterator(BinarySearchTree<T> bst)
        {
            _bst = bst ?? throw new NullReferenceException(nameof(bst));
            _trav = bst._root ?? throw new NullReferenceException(nameof(bst._root));
            _stack.Push(bst._root);
            _current = bst._root;
        }

        public bool MoveNext()
        {
            if (_stack.Count == 0) return false;

            while (_trav.Left != null)
            {
                _stack.Push(_trav.Left);
                _trav = _trav.Left;
            }

            _current = _stack.Pop();

            if (_current.Right != null)
            {
                _stack.Push(_current.Right);
                _trav = _current.Right;
            }

            return true;
        }

        public void Reset()
        {
            if (_bst._root != null)
            {
                _trav = _bst._root;
                _stack.Push(_bst._root);
            }
        }

        public T Current => _current.Value;

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            /*throw new NotImplementedException();*/
        }
    }

    private class PreOrderIterator : IEnumerator<T>
    {
        private readonly BinarySearchTree<T> _bst;

        private readonly Stack<TreeNode> _stack = new();

        private TreeNode _current;

        public PreOrderIterator(BinarySearchTree<T> bst)
        {
            _bst = bst ?? throw new NullReferenceException(nameof(bst));
            _current = bst._root ?? throw new NullReferenceException(nameof(bst._root));
            _stack.Push(bst._root);
        }
        public T Current => _current.Value;

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            if (_stack.Count == 0) return false;
            _current = _stack.Pop();
            if (_current.Right != null) _stack.Push(_current.Right);
            if (_current.Left != null) _stack.Push(_current.Left);
            return true;
        }

        public void Reset()
        {
            if (_bst._root != null)
            {
                _stack.Clear();
                _stack.Push(_bst._root);
            }
        }
    }

    private class LevelOrderIterator : IEnumerator<T>
    {
        private readonly BinarySearchTree<T> _bst;

        private readonly Queue<TreeNode> _queue = new();

        private TreeNode _current;

        public LevelOrderIterator(BinarySearchTree<T> bst)
        {
            _bst = bst ?? throw new ArgumentNullException(nameof(bst));
            if (bst._root == null) throw new NullReferenceException($"{nameof(bst._root)} is null.");
            _current = new TreeNode(bst._root.Value, bst._root.Left, bst._root.Right);
            _queue.Enqueue(bst._root);
        }

        public bool MoveNext()
        {
            if (_queue.Count == 0) return false;

            _current = _queue.Dequeue();

            if (_current.Left != null) _queue.Enqueue(_current.Left);
            if (_current.Right != null) _queue.Enqueue(_current.Right);

            return true;
        }

        public void Reset()
        {
            if (_bst._root != null)
            {
                _queue.Clear();
                _queue.Enqueue(_bst._root);
            }
        }

        public T Current => _current.Value;

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }

}