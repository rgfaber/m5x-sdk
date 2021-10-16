﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace M5x.DEC.Core
{
    public class CircularBuffer<T> : IEnumerable<T>
    {
        private readonly T[] _buffer;
        private int _end;
        private int _start;

        public CircularBuffer(int capacity)
        {
            if (capacity <= 0) throw new ArgumentException(nameof(capacity));

            _buffer = new T[capacity + 1];
            _start = 0;
            _end = 0;
        }

        public CircularBuffer(
            int capacity,
            params T[] items)
            : this(capacity)
        {
            if (items.Length > capacity) throw new ArgumentException(nameof(capacity));

            foreach (var item in items) Put(item);
        }

        public int Capacity => _buffer.Length - 1;

        public IEnumerator<T> GetEnumerator()
        {
            var i = _start;
            while (i != _end)
            {
                yield return _buffer[i];
                i = (i + 1) % _buffer.Length;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Put(T item)
        {
            _buffer[_end] = item;
            _end = (_end + 1) % _buffer.Length;
            if (_end == _start) _start = (_start + 1) % _buffer.Length;
        }
    }
}