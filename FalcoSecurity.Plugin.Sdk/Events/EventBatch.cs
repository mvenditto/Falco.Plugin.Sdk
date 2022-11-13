﻿using System.Runtime.InteropServices;

namespace FalcoSecurity.Plugin.Sdk.Events
{
    public unsafe class EventBatch : IEventBatch
    {
        private int _size;

        private readonly int _dataSize;

        private readonly IntPtr _eventsPtr;

        private bool _disposed = false;

        public EventBatch(int size, int dataSize)
        {
            _size = size;

            _dataSize = dataSize;

            _eventsPtr = Marshal.AllocHGlobal(sizeof(PluginEvent) * size);
        }

        public int Length => _size;

        public IntPtr UnderlyingArray => _eventsPtr;

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            for (var i = 0; i < _size; i++)
            {
                Marshal.FreeHGlobal(((PluginEvent*)_eventsPtr)[i].Data);
            }

            Marshal.FreeHGlobal(_eventsPtr);

            _size = 0;

            _disposed = true;
        }

        public IEventWriter Get(int eventIndex)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(EventBatch));
            }

            if (eventIndex >= _size)
            {
                throw new IndexOutOfRangeException($"{eventIndex} is greater or equal than {_size}");
            }

            var evt = &((PluginEvent*)_eventsPtr)[eventIndex];

            return new EventWriter(evt, (uint) _dataSize);
        }
    }
}
