﻿//-----------------------------------------------------------------------
// <copyright file="DeviceManager.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2024 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2024 .NET Foundation <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;

namespace Tutorials.Tutorial3
{
    #region device-manager-full
    public static partial class MainDeviceGroup
    {
        #region device-manager-msgs
        public sealed class RequestTrackDevice
        {
            public RequestTrackDevice(string groupId, string deviceId)
            {
                GroupId = groupId;
                DeviceId = deviceId;
            }

            public string GroupId { get; }
            public string DeviceId { get; }
        }

        public sealed class DeviceRegistered
        {
            public static DeviceRegistered Instance { get; } = new();
            private DeviceRegistered() { }
        }
        #endregion

        public class DeviceManager : UntypedActor
        {
            private Dictionary<string, IActorRef> groupIdToActor = new();
            private Dictionary<IActorRef, string> actorToGroupId = new();

            protected override void PreStart() => Log.Info("DeviceManager started");
            protected override void PostStop() => Log.Info("DeviceManager stopped");

            protected ILoggingAdapter Log { get; } = Context.GetLogger();

            protected override void OnReceive(object message)
            {
                switch (message)
                {
                    case RequestTrackDevice trackMsg:
                        if (groupIdToActor.TryGetValue(trackMsg.GroupId, out var actorRef))
                        {
                            actorRef.Forward(trackMsg);
                        }
                        else
                        {
                            Log.Info($"Creating device group actor for {trackMsg.GroupId}");
                            var groupActor = Context.ActorOf(DeviceGroup.Props(trackMsg.GroupId), $"group-{trackMsg.GroupId}");
                            Context.Watch(groupActor);
                            groupActor.Forward(trackMsg);
                            groupIdToActor.Add(trackMsg.GroupId, groupActor);
                            actorToGroupId.Add(groupActor, trackMsg.GroupId);
                        }
                        break;
                    case Terminated t:
                        var groupId = actorToGroupId[t.ActorRef];
                        Log.Info($"Device group actor for {groupId} has been terminated");
                        actorToGroupId.Remove(t.ActorRef);
                        groupIdToActor.Remove(groupId);
                        break;
                }
            }

            public static Props Props() => Akka.Actor.Props.Create<DeviceManager>();
        }
    }
    #endregion
}
