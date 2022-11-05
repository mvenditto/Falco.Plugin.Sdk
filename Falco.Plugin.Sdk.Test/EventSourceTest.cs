﻿using Falco.Plugin.Sdk.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Falco.Plugin.Sdk.Test
{
    public unsafe class EventSourceTest
    {
        private static ReadOnlySpan<byte> RandomBytes(int dataSize)
        {
            var bytes = new byte[dataSize];
            Random.Shared.NextBytes(bytes);
            return bytes;
        }

        [Fact]
        public void TestEventPoolSize()
        {
            using var eventPool = new EventBatch(size: 1, dataSize: 1);

            Assert.Equal(1, eventPool.Length);
        }

        [Fact]
        public void ReadWriteArbitraryData()
        {
            var evt = (PluginEvent*)Marshal.AllocHGlobal(sizeof(PluginEvent));

            try
            {
                Assert.NotEqual(IntPtr.Zero, (IntPtr)evt);

                const int dataSize = 64;

                var ew = new EventWriter(evt, dataSize);

                var data = RandomBytes(dataSize);

                ew.Write(data);

                Assert.Equal((uint)dataSize, evt->DataLen);

                var evtData = (byte*)evt->Data;

                for (var i = 0; i < data.Length; i++)
                {
                    Assert.Equal(data[i], evtData[i]);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((IntPtr) evt);
            }
        }

        [Fact]
        public void EventReaderTest()
        {
            var evt = (PluginEvent*)Marshal.AllocHGlobal(sizeof(PluginEvent));

            const string data = "This is a test!";
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var nanos = DateTimeOffset.Now.ToUnixTimeMilliseconds() * 1000000;
            var dataBuff = Marshal.AllocHGlobal(dataBytes.Length);
            dataBytes.CopyTo(new Span<byte>((void*) dataBuff, dataBytes.Length));

            try
            {
                evt->EventNum = 42;
                evt->Timestamp = (ulong) nanos;
                evt->Data = dataBuff;
                evt->DataLen = (uint) dataBytes.Length;

                var r = new EventReader(evt);

                Assert.Equal(42u, r.EventNum);
                Assert.Equal((ulong) nanos, r.Timestamp);
                Assert.Equal(data, Encoding.UTF8.GetString(r.Data));
            }
            finally
            {
                Marshal.FreeHGlobal(evt->Data);
                Marshal.FreeHGlobal((IntPtr)evt);
            }
        }
    }
}