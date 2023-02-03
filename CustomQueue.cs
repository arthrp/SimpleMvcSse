namespace SimpleMvcSse;

public class CustomQueue<T>
{
    private readonly Queue<T> _queue = new();
    private readonly object _queueLock = new();

    public void Add(T item)
    {
        lock (_queueLock)
        {
            _queue.Enqueue(item);
        }
    }

    public T Dequeue()
    {
        lock (_queueLock)
        {
            return _queue.Dequeue();
        }
    }

    public int Count()
    {
        lock (_queueLock)
        {
            return _queue.Count;
        }
    }
}