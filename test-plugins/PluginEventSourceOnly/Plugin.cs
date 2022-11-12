﻿using FalcoSecurity.Plugin.Sdk.Events;
using FalcoSecurity.Plugin.Sdk.Fields;

namespace FalcoSecurity.Plugin.Sdk.Test
{
    [FalcoPlugin(
        Id = 999,
        Name = "test.event_source_only",
        Description = "test description!",
        Contacts = "<test test@test.com>",
        RequiredApiVersion = "2.0.0",
        Version = "1.2.3")]
    public class TestPlugin : PluginBase, IEventSource		
    {
        public string EventSourceName => "test_eventsource";

        public IEnumerable<OpenParam> OpenParameters
            => Enumerable.Empty<OpenParam>();

        public IEventSourceInstance Open(IEnumerable<OpenParam>? openParams)
        {
            throw new NotImplementedException();
        }

        public void Close(IEventSourceInstance instance)
        {
            instance.Dispose();
        }
    }
}
