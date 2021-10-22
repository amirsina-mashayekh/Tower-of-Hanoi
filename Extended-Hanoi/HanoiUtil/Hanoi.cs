using System;
using System.Collections.Generic;

namespace Extended_Hanoi.HanoiUtil
{
    /// <summary>
    /// Provides methods to solve Standard and Extended Tower of Hanoi.
    /// </summary>
    public static class Hanoi
    {
        /// <summary>
        /// Specifies a peg in Tower of Hanoi.
        /// </summary>
        public enum Peg { P1, P2, P3 }

        /// <summary>
        /// Recursive method to solve Standard Tower of Hanoi.
        /// </summary>
        /// <param name="src">The source peg which is the initial place of disks.</param>
        /// <param name="aux">The auxilary peg.</param>
        /// <param name="dst">The destination peg which is the final place of disks.</param>
        /// <param name="count">The count of disks on the source peg.</param>
        /// <param name="moves">The list to put results in it. Recommended to be empty.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When <c>count</c> is less than 1.
        /// </exception>
        public static void SolveHanoi(Peg src, Peg aux, Peg dst, int count, List<Move> moves)
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException("count", "count should be at least 1.");
            }

            if (count == 1)
            {
                moves.Add(new Move(src, dst));
            }
            else
            {
                SolveHanoi(src, dst, aux, count - 1, moves);
                moves.Add(new Move(src, dst));
                SolveHanoi(aux, src, dst, count - 1, moves);
            }
        }

        /// <summary>
        /// Recursive method to solve Extended Tower of Hanoi.
        /// </summary>
        /// <param name="src">The source peg which is the initial place of disks.</param>
        /// <param name="aux">The auxilary peg.</param>
        /// <param name="dst">The destination peg which is the final place of disks.</param>
        /// <param name="count">The count of disks on the source peg.</param>
        /// <param name="moves">The list to put results in it. Recommended to be empty.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When <c>count</c> is less than 1.
        /// </exception>
        public static void SolveExHanoi(Peg src, Peg aux, Peg dst, int count, List<Move> moves)
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException("count", "count should be at least 1.");
            }

            if (count == 1)
            {
                moves.Add(new Move(dst, aux));
                moves.Add(new Move(src, dst));
                moves.Add(new Move(aux, src));
                moves.Add(new Move(aux, dst));
                moves.Add(new Move(src, dst));
            }
            else
            {
                SolveExHanoi(src, aux, dst, count - 1, moves);
                SolveHanoi(dst, src, aux, (3 * count) - 2, moves);
                moves.Add(new Move(src, dst));
                SolveHanoi(aux, src, dst, (3 * count) - 1, moves);
            }
        }
    }
}