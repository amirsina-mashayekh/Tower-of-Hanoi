using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tower_of_Hanoi.HanoiUtil
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
        /// <param name="count">The height of tower.</param>
        /// <param name="moves">The list to put results in it. Recommended to be empty.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When <c>height</c> is less than 1.
        /// </exception>
        /// <exception cref="OperationCanceledException"></exception>
        public static async Task SolveHanoi(Peg src, Peg aux, Peg dst, int height, List<Move> moves, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (height < 1)
            {
                throw new ArgumentOutOfRangeException("height", "height should be at least 1.");
            }

            if (height == 1)
            {
                moves.Add(new Move(src, dst));
            }
            else
            {
                await SolveHanoi(src, dst, aux, height - 1, moves, cancellationToken);
                moves.Add(new Move(src, dst));
                await SolveHanoi(aux, src, dst, height - 1, moves, cancellationToken);
            }
        }

        /// <summary>
        /// Recursive method to solve Extended Tower of Hanoi.
        /// </summary>
        /// <param name="src">The source peg which is the initial place of disks.</param>
        /// <param name="aux">The auxilary peg.</param>
        /// <param name="dst">The destination peg which is the final place of disks.</param>
        /// <param name="count">The height tower.</param>
        /// <param name="moves">The list to put results in it. Recommended to be empty.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When <c>height</c> is less than 1.
        /// </exception>
        /// <exception cref="OperationCanceledException"></exception>
        public static async Task SolveExHanoi(Peg src, Peg aux, Peg dst, int height, List<Move> moves, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (height < 1)
            {
                throw new ArgumentOutOfRangeException("height", "height should be at least 1.");
            }

            if (height == 1)
            {
                moves.Add(new Move(dst, aux));
                moves.Add(new Move(src, dst));
                moves.Add(new Move(aux, src));
                moves.Add(new Move(aux, dst));
                moves.Add(new Move(src, dst));
            }
            else
            {
                await SolveExHanoi(src, aux, dst, height - 1, moves, cancellationToken);
                await SolveHanoi(dst, src, aux, (3 * height) - 2, moves, cancellationToken);
                moves.Add(new Move(src, dst));
                await SolveHanoi(aux, src, dst, (3 * height) - 1, moves, cancellationToken);
            }
        }
    }
}