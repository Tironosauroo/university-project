public interface IInventoryQueue<T>
{
    int Count { get; }
    void Enqueue(T item);
    T Dequeue();
    T[] IQtoArray();
}
