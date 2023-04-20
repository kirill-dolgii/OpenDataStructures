using System.Collections;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DataStructures.Tests")]

namespace DataStructures.Collections;



public class RedBlackTree<T> : ICollection<T>
{
    internal class RedBlackNode
    {
        public RedBlackNode(RedBlackNode? parent,
                            T value,
                            RedBlackNode? leftChild,
                            RedBlackNode? rightChild,
                            bool isBlack)
        {
            Parent = parent;
            IsBlack = isBlack;
            Value = value;
            Left = leftChild;
            Right = rightChild;
        }

        public RedBlackNode? Parent { get; set; }
        public RedBlackNode? Left { get; set; }
        public RedBlackNode? Right { get; set; }
        public bool IsBlack { get; set; }
        public T Value { get; }
    }

    private readonly IComparer<T> _comparer;
    internal RedBlackNode? _root;
    private int _count;
    public int Count => _count;
    public bool IsReadOnly => false;

    public RedBlackTree()
    {
        _comparer = Comparer<T>.Default;
        _count = 1;
    }

    #region ICollection members

    public void Add(T value)
    {
        if (value == null) throw new ArgumentNullException();
        if (_root == null)
        {
            _root = new RedBlackNode(null, value, null, null, true);
            _count++;
            return;
        }

        if (Contains(value)) return;

        RedBlackNode prev = FindPrev(value);
        var node = new RedBlackNode(prev, value, null, null, false);

        int cmp = _comparer.Compare(value, prev.Value);
        if (cmp == -1) prev.Left = node;
        else if (cmp == 1) prev.Right = node;
        else throw new Exception("bad find x");

        _count++;
        FixInsertion(node);
    }

    public bool Remove(T value)
    {
        if (value == null) throw new ArgumentNullException();
        if (!Contains(value)) return false;
        _count--;
        return Remove(_root, value);
    }

    public void Clear()
    {
        _root = null;
        _count = 0;
    }

    public bool Contains(T item)
    {
        if (item == null) throw new ArgumentNullException();
        if (_comparer.Compare(item, _root.Value) == 0) return true;

        var prev = FindPrev(item);

        if (prev.Left != null && _comparer.Compare(item, prev.Left.Value) == 0 ||
            prev.Right != null && _comparer.Compare(item, prev.Right.Value) == 0)
            return true;
        return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException();
        if (array.Length - arrayIndex < _count) throw new ArgumentOutOfRangeException();

        int i = arrayIndex;
        using var lvlEnumer = GetEnumerator();
        while (lvlEnumer.MoveNext()) array[i++] = lvlEnumer.Current;
    }


    #endregion

    private void ReplaceChild(RedBlackNode? parent,
                              RedBlackNode? oldChild,
                              RedBlackNode? newChild)
    {
        if (parent == null && newChild == null) throw new AggregateException("root is lost");

        if (parent == null) _root = newChild;
        else if (parent!.Left == oldChild) parent.Left = newChild;
        else if (parent!.Right == oldChild) parent.Right = newChild;

        if (newChild != null) newChild.Parent = parent;
    }

    internal void RotateRight(RedBlackNode node)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));

        RedBlackNode? parent = node.Parent;
        RedBlackNode left = node.Left!;

        if (left != null)
        {
            node.Left = left.Right;
            if (left.Right != null) left.Right.Parent = node;

            left.Parent = parent;

            left.Right = node;
            if (left.Right != null) node.Parent = left;

            ReplaceChild(parent, node, left);
        }
    }

    internal void RotateLeft(RedBlackNode node)
    {
        if (node == null) throw new ArgumentNullException();

        RedBlackNode? parent = node.Parent;
        RedBlackNode right = node.Right!;

        if (right != null)
        {
            node.Right = right.Left;
            if (right.Left != null) right.Left.Parent = node;

            right.Parent = parent;

            right.Left = node;
            if (right.Left != null) node.Parent = right;

            ReplaceChild(parent, node, right);
        }
    }

    private RedBlackNode FindPrev(T value)
    {
        if (value == null) throw new ArgumentNullException();
        RedBlackNode? node = _root;
        RedBlackNode prevNode = _root!;

        while (node != null)
        {
            prevNode = node!;
            int cmp = _comparer.Compare(value, node.Value);
            if (cmp == -1) node = node.Left!;
            if (cmp == 1) node = node.Right!;
            if (cmp == 0) return node.Parent!;
        }

        return prevNode;
    }
    private void Transplaint(RedBlackNode node, RedBlackNode other)
    {
        if (node == _root) _root = other;
        else if (node == node.Parent.Left) node.Parent.Left = other;
        else if (node == node.Parent.Right) node.Parent.Right = other;
        if (other != null) other.Parent = node.Parent;
    }

    private void FixInsertion(RedBlackNode node)
    {
        if (node == null) throw new ArgumentNullException();
        if (node == _root) return;

        while (node != _root && !node.Parent.IsBlack && !node.IsBlack)
        {
            var parent = node.Parent;
            var grandParent = parent.Parent;

            if (parent == grandParent.Right)
            {
                RedBlackNode? uncle = grandParent.Left;

                if (uncle != null && !uncle.IsBlack)
                {
                    if (grandParent != _root) grandParent.IsBlack = false;
                    parent.IsBlack = true;
                    if (uncle != null) uncle.IsBlack = true;
                    node = grandParent;
                    continue;
                }

                if (node == parent.Left)
                {
                    node = parent;
                    RotateRight(node);
                    continue;
                }

                node.Parent.IsBlack = true;
                node.Parent.Parent.IsBlack = false;

                RotateLeft(node.Parent.Parent);
                node = node.Parent;
            }
            else
            {
                var uncle = grandParent.Right;

                if (uncle != null && !uncle.IsBlack)
                {
                    if (grandParent != _root) grandParent.IsBlack = false;
                    parent.IsBlack = true;
                    if (uncle != null) uncle.IsBlack = true;
                    node = grandParent;
                    continue;
                }

                if (node == parent.Right)
                {
                    node = parent;
                    RotateLeft(node);
                    continue;
                }

                parent.IsBlack = true;
                grandParent.IsBlack = false;

                RotateRight(node.Parent.Parent);
                node = node.Parent;
            }
        }
        _root.IsBlack = true;
    }

    private bool Remove(RedBlackNode node, T value)
    {
        if (_root == null) throw new ArgumentNullException();

        RedBlackNode z = _root!;
        RedBlackNode? x = null;
        RedBlackNode y = null;

        while (node != null)
        {
            int cmp = _comparer.Compare(value, node.Value);
            if (cmp == 0)
            {
                z = node;
                break;
            }
            else if (cmp == -1)
                node = node.Left;
            else
                node = node.Right;
        }

        if (node == null) return false;

        y = z;
        bool oldIsBlack = z.IsBlack;

        if (z.Left == null)
        {
            x = z.Right;
            Transplaint(z, z.Right);
        }
        else if (z.Right == null)
        {
            x = z.Left;
            Transplaint(z, z.Left);
        }
        else
        {
            y = z.Right;
            while (y.Left != null) y = y.Left;

            x = y.Right;
            if (y.Parent == z)
            {
                if (x != null) x.Parent = y;
            }
            else
            {
                Transplaint(y, y.Right);
                y.Right = z.Right;
                if (y.Right != null) y.Right.Parent = y;
            }

            Transplaint(z, y);
            y.Left = z.Left;
            y.Left.Parent = y;
            y.IsBlack = oldIsBlack;
        }

        if (oldIsBlack) FixRemoval(x);
        return true;
    }

    private void FixRemoval(RedBlackNode? x)
    {
        RedBlackNode bro;
        while (x != null && x != _root && x.IsBlack)
        {
            if (x == x.Parent.Left)
            {
                bro = x.Parent.Right;
                if (bro != null && !bro.IsBlack)
                {
                    bro.IsBlack = true;
                    x.Parent.IsBlack = false;
                    RotateLeft(x.Parent);
                    bro = x.Parent.Right;
                }

                if (bro?.Right != null && bro.Left != null && bro.Left.IsBlack && bro.Right.IsBlack)
                {
                    bro.IsBlack = false;
                    x = x.Parent;
                }
                else
                {
                    if (bro?.Right != null && bro.Right.IsBlack)
                    {
                        if (bro.Left != null) bro.Left.IsBlack = true;
                        bro.IsBlack = false;
                        RotateRight(bro);
                        bro = x.Parent.Right;
                    }

                    if (bro != null)
                    {
                        bro.IsBlack = x.Parent.IsBlack;
                        x.Parent.IsBlack = true;
                        if (bro.Right != null) bro.Right.IsBlack = true;
                    }

                    RotateLeft(x.Parent);
                    break;
                }
            }
            else
            {
                bro = x.Parent.Left;
                if (bro != null && !bro.IsBlack)
                {
                    bro.IsBlack = true;
                    x.Parent.IsBlack = false;
                    RotateLeft(x.Parent);
                    bro = x.Parent.Left;
                }

                if (bro?.Left != null && bro.Right != null && bro.Right.IsBlack && bro.Left.IsBlack)
                {
                    bro.IsBlack = false;
                    x = x.Parent;
                }
                else
                {
                    if (bro?.Left != null && bro.Left.IsBlack)
                    {
                        if (bro.Right != null) bro.Right.IsBlack = true;
                        bro.IsBlack = false;
                        RotateLeft(bro);
                        bro = x.Parent.Left;
                        continue;
                    }

                    if (bro != null)
                    {
                        bro.IsBlack = x.Parent.IsBlack;
                        x.Parent.IsBlack = true;
                        if (bro.Left != null) bro.Left.IsBlack = true;
                    }

                    RotateRight(x.Parent);
                    break;
                }
            }
        }
        if (x != null)
            x.IsBlack = true;
    }

    #region Enumerators

    private IEnumerable<T> InOrderTraversal(RedBlackNode node)
    {
        if (node == null) yield break;
        foreach (T item in InOrderTraversal(node.Left))
            yield return item;
        yield return node.Value;
        foreach (T item in InOrderTraversal(node.Right))
            yield return item;
    }

    private IEnumerable<T> PreOrderTraversal(RedBlackNode node)
    {
        if (node == null) yield break;
        yield return node.Value;
        foreach (T item in PreOrderTraversal(node.Left))
            yield return item;
        foreach (T item in PreOrderTraversal(node.Right))
            yield return item;
    }

    private IEnumerable<T> PostOrderTraversal(RedBlackNode node)
    {
        if (node == null) yield break;
        foreach (T item in PostOrderTraversal(node.Left))
            yield return item;
        foreach (T item in PostOrderTraversal(node.Right))
            yield return item;
        yield return node.Value;
    }

    private IEnumerable<T> LevelOrderTraversal(RedBlackNode node)
    {
        var queue = new Queue.LinkedListQueue<RedBlackNode>();
        queue.Enqueue(node);

        while (!queue.IsEmpty())
        {
            var current = queue.Dequeue();
            if (current.Left != null) queue.Enqueue(current.Left);
            if (current.Right != null) queue.Enqueue(current.Right);
            yield return current.Value;
        }
    }

    public IEnumerator<T> GetEnumerator(TreeTraversal trav)
    {
        switch (trav)
        {
            case TreeTraversal.LevelOrder:
                return LevelOrderTraversal(_root).GetEnumerator();
            case TreeTraversal.InOrder:
                return InOrderTraversal(_root).GetEnumerator();
            case TreeTraversal.PreOrder:
                return PreOrderTraversal(_root).GetEnumerator();
            case TreeTraversal.PostOrder:
                return PostOrderTraversal(_root).GetEnumerator();
        }
        return GetEnumerator();
    }

    public IEnumerator<T> GetEnumerator() => InOrderTraversal(_root).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion
}

