using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Extended_Hanoi.HanoiUtil.Hanoi;

namespace Extended_Hanoi.HanoiUtil.Tests
{
    [TestClass()]
    public class HanoiTests
    {
        private static CancellationToken ctn = CancellationToken.None;

        [TestMethod()]
        public async Task SolveHanoiTest()
        {
            List<Move> moves = new List<Move>();

            List<Move> test1 = new List<Move>
            {
                new Move(Peg.P1, Peg.P3)
            };
            await SolveHanoi(Peg.P1, Peg.P2, Peg.P3, 1, moves, ctn);
            Assert.IsTrue(Enumerable.SequenceEqual(test1, moves));

            moves.Clear();
            List<Move> test2 = new List<Move>
            {
                new Move(Peg.P1, Peg.P2),
                new Move(Peg.P1, Peg.P3),
                new Move(Peg.P2, Peg.P3)
            };
            await SolveHanoi(Peg.P1, Peg.P2, Peg.P3, 2, moves, ctn);
            Assert.IsTrue(Enumerable.SequenceEqual(test2, moves));

            moves.Clear();
            List<Move> test3 = new List<Move>
            {
                new Move(Peg.P1, Peg.P3),
                new Move(Peg.P1, Peg.P2),
                new Move(Peg.P3, Peg.P2),
                new Move(Peg.P1, Peg.P3),
                new Move(Peg.P2, Peg.P1),
                new Move(Peg.P2, Peg.P3),
                new Move(Peg.P1, Peg.P3)
            };
            await SolveHanoi(Peg.P1, Peg.P2, Peg.P3, 3, moves, ctn);
            Assert.IsTrue(Enumerable.SequenceEqual(test3, moves));

            moves.Clear();
            List<Move> test3_1 = new List<Move>
            {
                new Move(Peg.P2, Peg.P1),
                new Move(Peg.P2, Peg.P3),
                new Move(Peg.P1, Peg.P3),
                new Move(Peg.P2, Peg.P1),
                new Move(Peg.P3, Peg.P2),
                new Move(Peg.P3, Peg.P1),
                new Move(Peg.P2, Peg.P1)
            };
            await SolveHanoi(Peg.P2, Peg.P3, Peg.P1, 3, moves, ctn);
            Assert.IsTrue(Enumerable.SequenceEqual(test3_1, moves));

            moves.Clear();
            _ = Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => SolveHanoi(Peg.P1, Peg.P2, Peg.P3, 0, moves, ctn));
            _ = Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => SolveHanoi(Peg.P1, Peg.P2, Peg.P3, -1, moves, ctn));

            CancellationTokenSource cts = new CancellationTokenSource(10);
            _ = Assert.ThrowsExceptionAsync<OperationCanceledException>(() => SolveHanoi(Peg.P1, Peg.P2, Peg.P3, 100, moves, cts.Token));
        }

        [TestMethod()]
        public async Task SolveExHanoiTest()
        {
            List<Move> moves = new List<Move>();

            List<Move> test1 = new List<Move>
            {
                new Move(Peg.P3, Peg.P2),
                new Move(Peg.P1, Peg.P3),
                new Move(Peg.P2, Peg.P1),
                new Move(Peg.P2, Peg.P3),
                new Move(Peg.P1, Peg.P3)
            };
            await SolveExHanoi(Peg.P1, Peg.P2, Peg.P3, 1, moves, ctn);
            Assert.IsTrue(Enumerable.SequenceEqual(test1, moves));

            moves.Clear();
            _ = Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => SolveExHanoi(Peg.P1, Peg.P2, Peg.P3, 0, moves, ctn));
            _ = Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => SolveExHanoi(Peg.P1, Peg.P2, Peg.P3, -1, moves, ctn));

            CancellationTokenSource cts = new CancellationTokenSource(10);
            _ = Assert.ThrowsExceptionAsync<OperationCanceledException>(() => SolveExHanoi(Peg.P1, Peg.P2, Peg.P3, 100, moves, cts.Token));
        }
    }
}