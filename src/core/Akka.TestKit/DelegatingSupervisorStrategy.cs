﻿//-----------------------------------------------------------------------
// <copyright file="DelegatingSupervisorStrategy.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2024 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2024 .NET Foundation <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Util;

namespace Akka.TestKit
{
    public class DelegatingSupervisorStrategy : SupervisorStrategy
    {
        private Dictionary<IActorRef, SupervisorStrategy> Delegates { get; } = new();

        public override IDecider Decider { get; } = DefaultDecider;
        
        protected override Directive Handle(IActorRef child, Exception exception)
        {
            var childDelegate = Delegates[child];
            var handleMethod = typeof(SupervisorStrategy).GetMethod(
                name: "Handle", 
                bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic, 
                binder: Type.DefaultBinder,
                types: new[] {typeof(IActorRef), typeof(Exception)}, 
                modifiers: null);
            var result = (Directive) handleMethod.Invoke(childDelegate, new object[]{ child, exception });
            return result;
        }
        
        public override void ProcessFailure(IActorContext context, bool restart, IActorRef child, Exception cause, ChildRestartStats stats,
            IReadOnlyCollection<ChildRestartStats> children)
        {
            Delegates[child].ProcessFailure(context, restart, child, cause, stats, children);
        }

        public void Update(IActorRef child, SupervisorStrategy supervisorStrategy)
        {
            Delegates[child] = supervisorStrategy;
        }

        public override void HandleChildTerminated(IActorContext actorContext, IActorRef child, IEnumerable<IInternalActorRef> children)
        {
            Delegates.Remove(child);
        }

        public override ISurrogate ToSurrogate(ActorSystem system)
        {
            throw new NotImplementedException();
        }

        private SupervisorStrategy Delegate(IActorRef child)
        {
            return Delegates.TryGetValue(child, out var strategy) ? strategy : StoppingStrategy;
        }
    }
}
