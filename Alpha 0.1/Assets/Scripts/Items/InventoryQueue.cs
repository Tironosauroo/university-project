using System;

public class InventoryQueue<T> : IInventoryQueue<T>
{
    private T[] _array;
    private int _head;
    private int _tail;
    private int _size;
    private int _capacity;

    public InventoryQueue(int capacity = 4)
    {
        _capacity = capacity;
        _array = new T[_capacity];
        _head = 0;
        _tail = 0;
        _size = 0;
    }

    public int Count => _size;

    public void Enqueue(T item)
    {
        if (_size == _capacity)
            Resize();

        _array[_tail] = item;
        _tail = (_tail + 1) % _capacity;
        _size++;
    }

    public T Dequeue()
    {
        if (_size == 0)
            throw new InvalidOperationException("Queue is empty");

        T item = _array[_head];
        _array[_head] = default(T);
        _head = (_head + 1) % _capacity;
        _size--;
        return item;
    }

    public T[] IQtoArray()
    {
        T[] result = new T[_size];
        for (int i = 0; i < _size; i++)
        {
            result[i] = _array[(_head + i) % _capacity];
        }
        return result;
    }

    private void Resize()
    {
        int newCapacity = _capacity * 2;
        T[] newArray = new T[newCapacity];

        for (int i = 0; i < _size; i++)
        {
            newArray[i] = _array[(_head + i) % _capacity];
        }

        // cleaning null pointers instead of garb. collector
        for (int i = 0; i < _capacity; i++)
        {
            _array[i] = default(T);
        }

        _array = newArray;
        _head = 0;
        _tail = _size;
        _capacity = newCapacity;
    }
}
