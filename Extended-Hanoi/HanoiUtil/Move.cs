using System;
using System.Collections.Generic;
using static Extended_Hanoi.HanoiUtil.Hanoi;

namespace Extended_Hanoi.HanoiUtil
{
    /// <summary>
    /// Represents a disk movement in Tower of Hanoi.
    /// </summary>
    public class Move : IEquatable<Move>
    {
        /// <summary>
        /// The initial place of disk.
        /// </summary>
        public Peg Source { get; }

        /// <summary>
        /// The final place of disk.
        /// </summary>
        public Peg Destination { get; }

        /// <summary>
        /// Initializes a new instance of the <c>Move</c> class.
        /// </summary>
        /// <param name="source">The initial place of disk.</param>
        /// <param name="destination">The final place of disk.</param>
        public Move(Peg source, Peg destination)
        {
            Source = source;
            Destination = destination;
        }

        /// <summary>
        /// Returns a string representation of this move.
        /// </summary>
        /// <returns>A string that shows the source and destination of this move.</returns>
        public override string ToString()
        {
            return Source + " -> " + Destination;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Move);
        }

        public bool Equals(Move other)
        {
            return other != null &&
                   Source == other.Source &&
                   Destination == other.Destination;
        }

        public override int GetHashCode()
        {
            int hashCode = 1937123878;
            hashCode = (hashCode * -1521134295) + Source.GetHashCode();
            hashCode = (hashCode * -1521134295) + Destination.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Move left, Move right)
        {
            return EqualityComparer<Move>.Default.Equals(left, right);
        }

        public static bool operator !=(Move left, Move right)
        {
            return !(left == right);
        }
    }
}