using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using static Extended_Hanoi.HanoiUtil.Hanoi;

namespace Extended_Hanoi.HanoiUtil.Tests
{
    [TestClass()]
    public class HanoiTests
    {
        [TestMethod()]
        public void SolveHanoiTest()
        {
            List<Move> moves = new List<Move>();

            List<Move> test1 = new List<Move>
            {
                new Move(Peg.P1, Peg.P3)
            };
            SolveHanoi(Peg.P1, Peg.P2, Peg.P3, 1, moves);
            Assert.IsTrue(Enumerable.SequenceEqual(test1, moves));

            moves.Clear();
            List<Move> test2 = new List<Move>
            {
                new Move(Peg.P1, Peg.P2),
                new Move(Peg.P1, Peg.P3),
                new Move(Peg.P2, Peg.P3)
            };
            SolveHanoi(Peg.P1, Peg.P2, Peg.P3, 2, moves);
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
            SolveHanoi(Peg.P1, Peg.P2, Peg.P3, 3, moves);
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
            SolveHanoi(Peg.P2, Peg.P3, Peg.P1, 3, moves);
            Assert.IsTrue(Enumerable.SequenceEqual(test3_1, moves));

            moves.Clear();
            _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => SolveHanoi(Peg.P1, Peg.P2, Peg.P3, 0, moves));
            _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => SolveHanoi(Peg.P1, Peg.P2, Peg.P3, -1, moves));
        }

        [TestMethod()]
        public void SolveExHanoiTest()
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
            SolveExHanoi(Peg.P1, Peg.P2, Peg.P3, 1, moves);
            Assert.IsTrue(Enumerable.SequenceEqual(test1, moves));

            moves.Clear();
            _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => SolveExHanoi(Peg.P1, Peg.P2, Peg.P3, 0, moves));
            _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => SolveExHanoi(Peg.P1, Peg.P2, Peg.P3, -1, moves));
        }
    }
}