﻿//-----------------------------------------------------------------------
// <copyright file="MemoryJournalShared.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2024 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2024 .NET Foundation <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using System;
using Akka.Actor;

namespace Akka.Cluster.Sharding.Tests
{
    public class MemoryJournalShared : AsyncWriteProxyEx
    {
        public override TimeSpan Timeout { get; }

        public MemoryJournalShared()
        {
            Timeout = Context.System.Settings.Config.GetTimeSpan("akka.persistence.journal.memory-journal-shared.timeout", null);
        }

        public static void SetStore(IActorRef store, ActorSystem system)
        {
            Persistence.Persistence.Instance.Get(system).JournalFor(null).Tell(new SetStore(store));
        }
    }
}
