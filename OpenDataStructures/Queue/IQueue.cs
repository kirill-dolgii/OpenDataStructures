namespace OpenDataStructures.Queue;

public interface IQueue<T>
{
    public void Enqueue(T item);

    public T Dequeue();

	public T Peek();

	public bool IsEmpty();

    public int Count();
}
