﻿//-----------------------------------------------------------------------
// <copyright file="SubstreamCancelStrategy.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2024 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2024 .NET Foundation <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

namespace Akka.Streams
{
    /// <summary>
    /// Represents a strategy that decides how to deal with substream events.
    /// </summary>
    public enum SubstreamCancelStrategy
    {
        /// <summary>
        /// Cancel the stream of streams if any substream is cancelled.
        /// </summary>
        Propagate,

        /// <summary>
        /// Drain substream on cancellation in order to prevent stalling of the stream of streams.
        /// </summary>
        Drain
    }
}
