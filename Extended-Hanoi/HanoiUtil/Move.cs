using System;
using static Extended_Hanoi.HanoiUtil.Hanoi;

namespace Extended_Hanoi.HanoiUtil
{
    /// <summary>
    /// Represents a disk movement in Tower of Hanoi.
    /// </summary>
    public struct Move : IEquatable<Move>
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

        /// <summary>
        /// Reverses a move.
        /// </summary>
        /// <param name="move">the <c>Move</c> object to reverse.</param>
        /// <returns>
        /// A new <c>Move</c> which its <c>Destination</c> is <c>Source</c> of <c>move</c> and vice versa.
        /// </returns>
        public static Move Reverse(Move move)
        {
            return new Move(move.Destination, move.Source);
        }

        public override bool Equals(object obj)
        {
            return obj is Move move && Equals(move);
        }

        public bool Equals(Move other)
        {
            return Source == other.Source &&
                   Destination == other.Destination;
        }

        public override int GetHashCode()
        {
            int hashCode = 1918477335;
            hashCode = hashCode * -1521134295 + Source.GetHashCode();
            hashCode = hashCode * -1521134295 + Destination.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Move left, Move right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Move left, Move right)
        {
            return !(left == right);
        }
    }
}