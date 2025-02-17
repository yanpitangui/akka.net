﻿//-----------------------------------------------------------------------
// <copyright file="OutputStreamSubscriber.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2024 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2024 .NET Foundation <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Akka.IO;
using Akka.Streams.Actors;
using Akka.Streams.IO;

namespace Akka.Streams.Implementation.IO
{
    /// <summary>
    /// INTERNAL API
    /// </summary>
    internal sealed class OutputStreamSubscriber : ActorSubscriber
    {
        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="os">TBD</param>
        /// <param name="completionPromise">TBD</param>
        /// <param name="bufferSize">TBD</param>
        /// <param name="autoFlush">TBD</param>
        /// <exception cref="ArgumentException">TBD</exception>
        /// <returns>TBD</returns>
        public static Props Props(Stream os, TaskCompletionSource<IOResult> completionPromise, int bufferSize, bool autoFlush)
        {
            if (bufferSize <= 0)
                throw new ArgumentException("Buffer size must be > 0");

            return
                Actor.Props.Create<OutputStreamSubscriber>(os, completionPromise, bufferSize, autoFlush)
                    .WithDeploy(Deploy.Local);
        }

        private readonly Stream _outputStream;
        private readonly TaskCompletionSource<IOResult> _completionPromise;
        private readonly bool _autoFlush;
        private long _bytesWritten;
        private readonly ILoggingAdapter _log;

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="outputStream">TBD</param>
        /// <param name="completionPromise">TBD</param>
        /// <param name="bufferSize">TBD</param>
        /// <param name="autoFlush">TBD</param>
        /// If this gets changed you must change <see cref="OutputStreamSubscriber.Props"/> as well!
        public OutputStreamSubscriber(Stream outputStream, TaskCompletionSource<IOResult> completionPromise, int bufferSize, bool autoFlush)
        {
            _outputStream = outputStream;
            _completionPromise = completionPromise;
            _autoFlush = autoFlush;
            RequestStrategy = new WatermarkRequestStrategy(highWatermark: bufferSize);
            _log = Context.GetLogger();
        }

        /// <summary>
        /// TBD
        /// </summary>
        public override IRequestStrategy RequestStrategy { get; }

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="message">TBD</param>
        /// <returns>TBD</returns>
        protected override bool Receive(object message)
        {
            switch (message)
            {
                case OnNext next:
                    try
                    {
                        var bytes = (ByteString)next.Element;
                        //blocking write
                        _outputStream.Write(bytes.ToArray(), 0, bytes.Count);
                        _bytesWritten += bytes.Count;
                        if (_autoFlush)
                            _outputStream.Flush();
                    }
                    catch (Exception ex)
                    {
                        _completionPromise.TrySetResult(IOResult.Failed(_bytesWritten, ex));
                        Cancel();
                    }
                    return true;
                case OnError error:
                    _log.Error(error.Cause, "Tearing down OutputStreamSink due to upstream error, wrote bytes: {0}", _bytesWritten);
                    _completionPromise.TrySetException(new AbruptIOTerminationException(IOResult.Success(_bytesWritten), error.Cause));
                    Context.Stop(Self);
                    return true;
                case OnComplete _:
                    Context.Stop(Self);
                    _outputStream.Flush();
                    return true;
            }

            return false;
        }

        /// <summary>
        /// TBD
        /// </summary>
        protected override void PostStop()
        {
            try
            {
                _outputStream?.Dispose();
            }
            catch (Exception ex)
            {
                _completionPromise.TrySetResult(IOResult.Failed(_bytesWritten, ex));
            }

            _completionPromise.TrySetResult(IOResult.Success(_bytesWritten));
            base.PostStop();
        }
    }
}
