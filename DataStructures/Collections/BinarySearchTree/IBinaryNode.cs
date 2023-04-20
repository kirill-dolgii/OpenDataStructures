namespace DataStructures.Collections.BinarySearchTree;
public interface IBinaryNode<T, TVal> where T : IBinaryNode<T, TVal>
{
    public T? Parent { get; set; }
    public T? Left { get; set; }
    public T? Right { get; set; }
    public TVal Value { get; }

}
