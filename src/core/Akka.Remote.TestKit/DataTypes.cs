﻿//-----------------------------------------------------------------------
// <copyright file="DataTypes.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2024 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2024 .NET Foundation <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Akka.Remote.Transport;
using Akka.Util;
using Address = Akka.Actor.Address;

namespace Akka.Remote.TestKit
{
    public sealed class RoleName : IEquatable<RoleName>
    {
        public RoleName(string name)
        {
            Name = name;
        }

        public bool Equals(RoleName other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        
        public override bool Equals(object obj) => obj is RoleName role && Equals(role);

        
        public override int GetHashCode() => (Name != null ? Name.GetHashCode() : 0);

        /// <summary>
        /// Compares two specified <see cref="RoleName"/> for equality.
        /// </summary>
        /// <param name="left">The first <see cref="RoleName"/> used for comparison</param>
        /// <param name="right">The second <see cref="RoleName"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="RoleName"/> are equal; otherwise <c>false</c></returns>
        public static bool operator ==(RoleName left, RoleName right) => Equals(left, right);

        /// <summary>
        /// Compares two specified <see cref="RoleName"/> for inequality.
        /// </summary>
        /// <param name="left">The first <see cref="RoleName"/> used for comparison</param>
        /// <param name="right">The second <see cref="RoleName"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="RoleName"/> are not equal; otherwise <c>false</c></returns>
        public static bool operator !=(RoleName left, RoleName right) => !Equals(left, right);

        public string Name { get; }

        
        public override string ToString() => $"RoleName({Name})";
    }

    //TODO: This is messy, better way to do this?
    //Marker interface to avoid using reflection to work out if message
    //is derived from generic type
    interface IToClient
    {
        object Msg { get; }
    }

    class ToClient<T> : IToClient where T : IClientOp, INetworkOp
    {
        private readonly T _msg;

        public ToClient(T msg)
        {
            _msg = msg;
        }

        object IToClient.Msg
        {
            get { return _msg; }
        }

        public T Msg
        {
            get { return _msg; }
        }

        
        protected bool Equals(ToClient<T> other)
        {
            return EqualityComparer<T>.Default.Equals(_msg, other._msg);
        }

        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ToClient<T>)obj);
        }

        
        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(_msg);
        }

        /// <summary>
        /// Compares two specified <see cref="ToClient{T}"/> for equality.
        /// </summary>
        /// <param name="left">The first <see cref="ToClient{T}"/> used for comparison</param>
        /// <param name="right">The second <see cref="ToClient{T}"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="ToClient{T}"/> are equal; otherwise <c>false</c></returns>
        public static bool operator ==(ToClient<T> left, ToClient<T> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two specified <see cref="ToClient{T}"/> for inequality.
        /// </summary>
        /// <param name="left">The first <see cref="ToClient{T}"/> used for comparison</param>
        /// <param name="right">The second <see cref="ToClient{T}"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="ToClient{T}"/> are not equal; otherwise <c>false</c></returns>
        public static bool operator !=(ToClient<T> left, ToClient<T> right)
        {
            return !Equals(left, right);
        }
    }

    //TODO: This is messy, better way to do this?
    //Marker interface to avoid using reflection to work out if message
    //is derived from generic type
    interface IToServer
    {
        object Msg { get; }
    }

    internal class ToServer<T> : IToServer where T : IServerOp, INetworkOp
    {
        public ToServer(T msg)
        {
            Msg = msg;
        }

        public T Msg { get; }

        object IToServer.Msg
        {
            get { return Msg; }
        }

        
        protected bool Equals(ToServer<T> other)
        {
            return EqualityComparer<T>.Default.Equals(Msg, other.Msg);
        }

        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ToServer<T>)obj);
        }

        
        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Msg);
        }

        /// <summary>
        /// Compares two specified <see cref="ToServer{T}"/> for equality.
        /// </summary>
        /// <param name="left">The first <see cref="ToServer{T}"/> used for comparison</param>
        /// <param name="right">The second <see cref="ToServer{T}"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="ToServer{T}"/> are equal; otherwise <c>false</c></returns>
        public static bool operator ==(ToServer<T> left, ToServer<T> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two specified <see cref="ToServer{T}"/> for inequality.
        /// </summary>
        /// <param name="left">The first <see cref="ToServer{T}"/> used for comparison</param>
        /// <param name="right">The second <see cref="ToServer{T}"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="ToServer{T}"/> are not equal; otherwise <c>false</c></returns>
        public static bool operator !=(ToServer<T> left, ToServer<T> right)
        {
            return !Equals(left, right);
        }
    }

    /// <summary>
    /// messages sent to from Conductor to Player
    /// </summary>
    internal interface IClientOp { } 

    /// <summary>
    /// messages sent to from Player to Conductor
    /// </summary>
    internal interface IServerOp { }

    /// <summary>
    /// messages sent from TestConductorExt to Conductor
    /// </summary>
    internal interface ICommandOp { }

    /// <summary>
    ///  messages sent over the wire
    /// </summary> 
    internal interface INetworkOp { }

    /// <summary>
    /// unconfirmed messages going to the Player
    /// </summary>
    internal interface IUnconfirmedClientOp : IClientOp { }

    internal interface IConfirmedClientOp : IClientOp { }

    /// <summary>
    /// First message of connection sets names straight.
    /// </summary>
    internal sealed class Hello : INetworkOp
    {
        private bool Equals(Hello other)
        {
            return string.Equals(Name, other.Name) && Equals(Address, other.Address);
        }

        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Hello hello && Equals(hello);
        }

        
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Address != null ? Address.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Compares two specified <see cref="Hello"/> for equality.
        /// </summary>
        /// <param name="left">The first <see cref="Hello"/> used for comparison</param>
        /// <param name="right">The second <see cref="Hello"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="Hello"/> are equal; otherwise <c>false</c></returns>
        public static bool operator ==(Hello left, Hello right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two specified <see cref="Hello"/> for inequality.
        /// </summary>
        /// <param name="left">The first <see cref="Hello"/> used for comparison</param>
        /// <param name="right">The second <see cref="Hello"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="Hello"/> are not equal; otherwise <c>false</c></returns>
        public static bool operator !=(Hello left, Hello right)
        {
            return !Equals(left, right);
        }

        public Hello(string name, Address address)
        {
            Name = name;
            Address = address;
        }

        public string Name { get; }

        public Address Address { get; }

        public override string ToString()
        {
            return $"Hello(Name: {Name}, Address: {Address})";
        }
    }

    internal sealed class EnterBarrier : IServerOp, INetworkOp
    {
        public EnterBarrier(string name, TimeSpan? timeout, RoleName role)
        {
            Name = name;
            Timeout = timeout;
            Role = role;
        }

        public override string ToString()
        {
            return $"EnterBarrier(Name: {Name}, Role: [{Role}], Timeout:{(Timeout.HasValue ? Timeout.Value.ToString() : "null")})";
        }

        private bool Equals(EnterBarrier other)
        {
            return string.Equals(Name, other.Name) && Timeout.Equals(other.Timeout);
        }

        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is EnterBarrier barrier && Equals(barrier);
        }

        
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Timeout.GetHashCode();
            }
        }

        /// <summary>
        /// Compares two specified <see cref="EnterBarrier"/> for equality.
        /// </summary>
        /// <param name="left">The first <see cref="EnterBarrier"/> used for comparison</param>
        /// <param name="right">The second <see cref="EnterBarrier"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="EnterBarrier"/> are equal; otherwise <c>false</c></returns>
        public static bool operator ==(EnterBarrier left, EnterBarrier right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two specified <see cref="EnterBarrier"/> for inequality.
        /// </summary>
        /// <param name="left">The first <see cref="EnterBarrier"/> used for comparison</param>
        /// <param name="right">The second <see cref="EnterBarrier"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="EnterBarrier"/> are not equal; otherwise <c>false</c></returns>
        public static bool operator !=(EnterBarrier left, EnterBarrier right)
        {
            return !Equals(left, right);
        }

        public string Name { get; }

        public TimeSpan? Timeout { get; }
        
        public RoleName Role { get; }
    }

    internal sealed class FailBarrier : IServerOp, INetworkOp
    {
        public FailBarrier(string name, RoleName role)
        {
            Name = name;
            Role = role;
        }

        public string Name { get; }
        
        public RoleName Role { get; }

        private bool Equals(FailBarrier other)
        {
            return string.Equals(Name, other.Name);
        }

        public override string ToString()
        {
            return $"FailBarrier(Name: {Name}, Role: [{Role}])";
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is FailBarrier barrier && Equals(barrier);
        }

        
        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        /// <summary>
        /// Compares two specified <see cref="FailBarrier"/> for equality.
        /// </summary>
        /// <param name="left">The first <see cref="FailBarrier"/> used for comparison</param>
        /// <param name="right">The second <see cref="FailBarrier"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="FailBarrier"/> are equal; otherwise <c>false</c></returns>
        public static bool operator ==(FailBarrier left, FailBarrier right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two specified <see cref="FailBarrier"/> for inequality.
        /// </summary>
        /// <param name="left">The first <see cref="FailBarrier"/> used for comparison</param>
        /// <param name="right">The second <see cref="FailBarrier"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="FailBarrier"/> are not equal; otherwise <c>false</c></returns>
        public static bool operator !=(FailBarrier left, FailBarrier right)
        {
            return !Equals(left, right);
        }
    }

    internal sealed class BarrierResult : IUnconfirmedClientOp, INetworkOp
    {
        readonly string _name;
        readonly bool _success;

        public BarrierResult(string name, bool success)
        {
            _name = name;
            _success = success;
        }

        public string Name
        {
            get { return _name; }
        }

        public bool Success
        {
            get { return _success; }
        }

        bool Equals(BarrierResult other)
        {
            return string.Equals(_name, other._name) && _success.Equals(other._success);
        }

        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is BarrierResult result && Equals(result);
        }

        
        public override int GetHashCode()
        {
            unchecked
            {
                return ((_name != null ? _name.GetHashCode() : 0) * 397) ^ _success.GetHashCode();
            }
        }

        /// <summary>
        /// Compares two specified <see cref="BarrierResult"/> for equality.
        /// </summary>
        /// <param name="left">The first <see cref="BarrierResult"/> used for comparison</param>
        /// <param name="right">The second <see cref="BarrierResult"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="BarrierResult"/> are equal; otherwise <c>false</c></returns>
        public static bool operator ==(BarrierResult left, BarrierResult right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two specified <see cref="BarrierResult"/> for inequality.
        /// </summary>
        /// <param name="left">The first <see cref="BarrierResult"/> used for comparison</param>
        /// <param name="right">The second <see cref="BarrierResult"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="BarrierResult"/> are not equal; otherwise <c>false</c></returns>
        public static bool operator !=(BarrierResult left, BarrierResult right)
        {
            return !Equals(left, right);
        }
    }

    sealed class Throttle : ICommandOp
    {
        readonly RoleName _node;
        readonly RoleName _target;
        readonly ThrottleTransportAdapter.Direction _direction;
        readonly float _rateMBit;

        public Throttle(RoleName node, RoleName target, ThrottleTransportAdapter.Direction direction, float rateMBit)
        {
            _node = node;
            _target = target;
            _direction = direction;
            _rateMBit = rateMBit;
        }

        public RoleName Node
        {
            get { return _node; }
        }

        public RoleName Target
        {
            get { return _target; }
        }

        public ThrottleTransportAdapter.Direction Direction
        {
            get { return _direction; }
        }

        public float RateMBit
        {
            get { return _rateMBit; }
        }

        bool Equals(Throttle other)
        {
            return Equals(_node, other._node) && Equals(_target, other._target) && Equals(_direction, other._direction) && _rateMBit.Equals(other._rateMBit);
        }

        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Throttle throttle && Equals(throttle);
        }

        
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (_node != null ? _node.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_target != null ? _target.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ _direction.GetHashCode();
                hashCode = (hashCode * 397) ^ _rateMBit.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Compares two specified <see cref="Throttle"/> for equality.
        /// </summary>
        /// <param name="left">The first <see cref="Throttle"/> used for comparison</param>
        /// <param name="right">The second <see cref="Throttle"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="Throttle"/> are equal; otherwise <c>false</c></returns>
        public static bool operator ==(Throttle left, Throttle right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two specified <see cref="Throttle"/> for inequality.
        /// </summary>
        /// <param name="left">The first <see cref="Throttle"/> used for comparison</param>
        /// <param name="right">The second <see cref="Throttle"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="Throttle"/> are not equal; otherwise <c>false</c></returns>
        public static bool operator !=(Throttle left, Throttle right)
        {
            return !Equals(left, right);
        }
    }

    sealed class ThrottleMsg : IConfirmedClientOp, INetworkOp
    {
        readonly Address _target;
        readonly ThrottleTransportAdapter.Direction _direction;
        readonly float _rateMBit;

        public ThrottleMsg(Address target, ThrottleTransportAdapter.Direction direction, float rateMBit)
        {
            _target = target;
            _direction = direction;
            _rateMBit = rateMBit;
        }

        public Address Target
        {
            get { return _target; }
        }

        public ThrottleTransportAdapter.Direction Direction
        {
            get { return _direction; }
        }

        public float RateMBit
        {
            get { return _rateMBit; }
        }

        bool Equals(ThrottleMsg other)
        {
            return Equals(_target, other._target) && Equals(_direction, other._direction) && _rateMBit.Equals(other._rateMBit);
        }

        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is ThrottleMsg msg && Equals(msg);
        }

        
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_target != null ? _target.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ _direction.GetHashCode();
                hashCode = (hashCode * 397) ^ _rateMBit.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Compares two specified <see cref="ThrottleMsg"/> for equality.
        /// </summary>
        /// <param name="left">The first <see cref="ThrottleMsg"/> used for comparison</param>
        /// <param name="right">The second <see cref="ThrottleMsg"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="ThrottleMsg"/> are equal; otherwise <c>false</c></returns>
        public static bool operator ==(ThrottleMsg left, ThrottleMsg right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two specified <see cref="ThrottleMsg"/> for inequality.
        /// </summary>
        /// <param name="left">The first <see cref="ThrottleMsg"/> used for comparison</param>
        /// <param name="right">The second <see cref="ThrottleMsg"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="ThrottleMsg"/> are not equal; otherwise <c>false</c></returns>
        public static bool operator !=(ThrottleMsg left, ThrottleMsg right)
        {
            return !Equals(left, right);
        }
    }

    sealed class Disconnect : ICommandOp
    {
        readonly RoleName _node;
        readonly RoleName _target;
        readonly bool _abort;

        public Disconnect(RoleName node, RoleName target, bool abort)
        {
            _node = node;
            _target = target;
            _abort = abort;
        }

        public RoleName Node
        {
            get { return _node; }
        }

        public RoleName Target
        {
            get { return _target; }
        }

        public bool Abort
        {
            get { return _abort; }
        }

        bool Equals(Disconnect other)
        {
            return Equals(_node, other._node) && Equals(_target, other._target) && _abort.Equals(other._abort);
        }

        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Disconnect disconnect && Equals(disconnect);
        }

        
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (_node != null ? _node.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_target != null ? _target.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ _abort.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Compares two specified <see cref="Disconnect"/> for equality.
        /// </summary>
        /// <param name="left">The first <see cref="Disconnect"/> used for comparison</param>
        /// <param name="right">The second <see cref="Disconnect"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="Disconnect"/> are equal; otherwise <c>false</c></returns>
        public static bool operator ==(Disconnect left, Disconnect right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two specified <see cref="Disconnect"/> for inequality.
        /// </summary>
        /// <param name="left">The first <see cref="Disconnect"/> used for comparison</param>
        /// <param name="right">The second <see cref="Disconnect"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="Disconnect"/> are not equal; otherwise <c>false</c></returns>
        public static bool operator !=(Disconnect left, Disconnect right)
        {
            return !Equals(left, right);
        }
    }

    sealed class DisconnectMsg : IConfirmedClientOp, INetworkOp
    {
        readonly Address _target;
        readonly bool _abort;

        public DisconnectMsg(Address target, bool abort)
        {
            _target = target;
            _abort = abort;
        }

        public Address Target
        {
            get { return _target; }
        }

        public bool Abort
        {
            get { return _abort; }
        }

        bool Equals(DisconnectMsg other)
        {
            return Equals(_target, other._target) && _abort.Equals(other._abort);
        }

        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is DisconnectMsg msg && Equals(msg);
        }

        
        public override int GetHashCode()
        {
            unchecked
            {
                return ((_target != null ? _target.GetHashCode() : 0) * 397) ^ _abort.GetHashCode();
            }
        }

        /// <summary>
        /// Compares two specified <see cref="DisconnectMsg"/> for equality.
        /// </summary>
        /// <param name="left">The first <see cref="DisconnectMsg"/> used for comparison</param>
        /// <param name="right">The second <see cref="DisconnectMsg"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="DisconnectMsg"/> are equal; otherwise <c>false</c></returns>
        public static bool operator ==(DisconnectMsg left, DisconnectMsg right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two specified <see cref="DisconnectMsg"/> for inequality.
        /// </summary>
        /// <param name="left">The first <see cref="DisconnectMsg"/> used for comparison</param>
        /// <param name="right">The second <see cref="DisconnectMsg"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="DisconnectMsg"/> are not equal; otherwise <c>false</c></returns>
        public static bool operator !=(DisconnectMsg left, DisconnectMsg right)
        {
            return !Equals(left, right);
        }
    }

    sealed class Terminate : ICommandOp
    {
        readonly RoleName _node;
        readonly Either<bool, int> _shutdownOrExit;

        public Terminate(RoleName node, Either<bool, int> shutdownOrExit)
        {
            _node = node;
            _shutdownOrExit = shutdownOrExit;
        }

        public RoleName Node
        {
            get { return _node; }
        }

        public Either<bool, int> ShutdownOrExit
        {
            get { return _shutdownOrExit; }
        }

        bool Equals(Terminate other)
        {
            return Equals(_node, other._node) && Equals(_shutdownOrExit, other._shutdownOrExit);
        }

        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Terminate terminate && Equals(terminate);
        }

        
        public override int GetHashCode()
        {
            unchecked
            {
                return ((_node != null ? _node.GetHashCode() : 0) * 397) ^ (_shutdownOrExit != null ? _shutdownOrExit.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Compares two specified <see cref="Terminate"/> for equality.
        /// </summary>
        /// <param name="left">The first <see cref="Terminate"/> used for comparison</param>
        /// <param name="right">The second <see cref="Terminate"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="Terminate"/> are equal; otherwise <c>false</c></returns>
        public static bool operator ==(Terminate left, Terminate right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two specified <see cref="Terminate"/> for inequality.
        /// </summary>
        /// <param name="left">The first <see cref="Terminate"/> used for comparison</param>
        /// <param name="right">The second <see cref="Terminate"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="Terminate"/> are not equal; otherwise <c>false</c></returns>
        public static bool operator !=(Terminate left, Terminate right)
        {
            return !Equals(left, right);
        }
    }

    sealed class TerminateMsg : IConfirmedClientOp, INetworkOp
    {
        readonly Either<bool, int> _shutdownOrExit;

        public TerminateMsg(Either<bool, int> shutdownOrExit)
        {
            _shutdownOrExit = shutdownOrExit;
        }

        public Either<bool, int> ShutdownOrExit
        {
            get { return _shutdownOrExit; }
        }

        bool Equals(TerminateMsg other)
        {
            return Equals(_shutdownOrExit, other._shutdownOrExit);
        }

        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is TerminateMsg msg && Equals(msg);
        }

        
        public override int GetHashCode()
        {
            return (_shutdownOrExit != null ? _shutdownOrExit.GetHashCode() : 0);
        }

        /// <summary>
        /// Compares two specified <see cref="TerminateMsg"/> for equality.
        /// </summary>
        /// <param name="left">The first <see cref="TerminateMsg"/> used for comparison</param>
        /// <param name="right">The second <see cref="TerminateMsg"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="TerminateMsg"/> are equal; otherwise <c>false</c></returns>
        public static bool operator ==(TerminateMsg left, TerminateMsg right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two specified <see cref="TerminateMsg"/> for inequality.
        /// </summary>
        /// <param name="left">The first <see cref="TerminateMsg"/> used for comparison</param>
        /// <param name="right">The second <see cref="TerminateMsg"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="TerminateMsg"/> are not equal; otherwise <c>false</c></returns>
        public static bool operator !=(TerminateMsg left, TerminateMsg right)
        {
            return !Equals(left, right);
        }
    }

    sealed class GetAddress : IServerOp, INetworkOp
    {
        readonly RoleName _node;

        public GetAddress(RoleName node)
        {
            _node = node;
        }

        public RoleName Node
        {
            get { return _node; }
        }

        bool Equals(GetAddress other)
        {
            return Equals(_node, other._node);
        }

        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is GetAddress address && Equals(address);
        }

        
        public override int GetHashCode()
        {
            return (_node != null ? _node.GetHashCode() : 0);
        }

        /// <summary>
        /// Compares two specified <see cref="GetAddress"/> for equality.
        /// </summary>
        /// <param name="left">The first <see cref="GetAddress"/> used for comparison</param>
        /// <param name="right">The second <see cref="GetAddress"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="GetAddress"/> are equal; otherwise <c>false</c></returns>
        public static bool operator ==(GetAddress left, GetAddress right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two specified <see cref="GetAddress"/> for inequality.
        /// </summary>
        /// <param name="left">The first <see cref="GetAddress"/> used for comparison</param>
        /// <param name="right">The second <see cref="GetAddress"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="GetAddress"/> are not equal; otherwise <c>false</c></returns>
        public static bool operator !=(GetAddress left, GetAddress right)
        {
            return !Equals(left, right);
        }
    }

    sealed class AddressReply : IUnconfirmedClientOp, INetworkOp
    {
        readonly RoleName _node;
        readonly Address _addr;

        public AddressReply(RoleName node, Address addr)
        {
            _node = node;
            _addr = addr;
        }

        public RoleName Node
        {
            get { return _node; }
        }

        public Address Addr
        {
            get { return _addr; }
        }

        bool Equals(AddressReply other)
        {
            return Equals(_node, other._node) && Equals(_addr, other._addr);
        }

        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is AddressReply reply && Equals(reply);
        }

        
        public override int GetHashCode()
        {
            unchecked
            {
                return ((_node != null ? _node.GetHashCode() : 0) * 397) ^ (_addr != null ? _addr.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Compares two specified <see cref="AddressReply"/> for equality.
        /// </summary>
        /// <param name="left">The first <see cref="AddressReply"/> used for comparison</param>
        /// <param name="right">The second <see cref="AddressReply"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="AddressReply"/> are equal; otherwise <c>false</c></returns>
        public static bool operator ==(AddressReply left, AddressReply right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two specified <see cref="AddressReply"/> for inequality.
        /// </summary>
        /// <param name="left">The first <see cref="AddressReply"/> used for comparison</param>
        /// <param name="right">The second <see cref="AddressReply"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="AddressReply"/> are not equal; otherwise <c>false</c></returns>
        public static bool operator !=(AddressReply left, AddressReply right)
        {
            return !Equals(left, right);
        }
    }

    public class Done : IServerOp, IUnconfirmedClientOp, INetworkOp
    {
        private Done() { }
        public static Done Instance { get; } = new();
    }

    sealed class Remove : ICommandOp
    {
        readonly RoleName _node;

        public Remove(RoleName node)
        {
            _node = node;
        }

        public RoleName Node
        {
            get { return _node; }
        }

        bool Equals(Remove other)
        {
            return Equals(_node, other._node);
        }

        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Remove remove && Equals(remove);
        }

        
        public override int GetHashCode()
        {
            return (_node != null ? _node.GetHashCode() : 0);
        }

        /// <summary>
        /// Compares two specified <see cref="Remove"/> for equality.
        /// </summary>
        /// <param name="left">The first <see cref="Remove"/> used for comparison</param>
        /// <param name="right">The second <see cref="Remove"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="Remove"/> are equal; otherwise <c>false</c></returns>
        public static bool operator ==(Remove left, Remove right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two specified <see cref="Remove"/> for inequality.
        /// </summary>
        /// <param name="left">The first <see cref="Remove"/> used for comparison</param>
        /// <param name="right">The second <see cref="Remove"/> used for comparison</param>
        /// <returns><c>true</c> if both <see cref="Remove"/> are not equal; otherwise <c>false</c></returns>
        public static bool operator !=(Remove left, Remove right)
        {
            return !Equals(left, right);
        }
    }
}

