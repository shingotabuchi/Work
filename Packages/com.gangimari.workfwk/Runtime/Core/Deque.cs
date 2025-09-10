using System;
using System.Collections;
using System.Collections.Generic;

namespace Fwk
{
    public class Deque<T> : IEnumerable<T>
    {
        private readonly LinkedList<T> _list = new();

        public int Count => _list.Count;

        public void AddToFront(T item)
        {
            _list.AddFirst(item);
        }

        public void AddToBack(T item)
        {
            _list.AddLast(item);
        }

        public T RemoveFromFront()
        {
            if (_list.Count == 0) throw new InvalidOperationException("Deque is empty.");
            var value = _list.First.Value;
            _list.RemoveFirst();
            return value;
        }

        public T RemoveFromBack()
        {
            if (_list.Count == 0) throw new InvalidOperationException("Deque is empty.");
            var value = _list.Last.Value;
            _list.RemoveLast();
            return value;
        }

        public T PeekFront()
        {
            if (_list.Count == 0) throw new InvalidOperationException("Deque is empty.");
            return _list.First.Value;
        }

        public T PeekBack()
        {
            if (_list.Count == 0) throw new InvalidOperationException("Deque is empty.");
            return _list.Last.Value;
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}