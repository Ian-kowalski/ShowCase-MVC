using ShowCase_MVC.Models;

namespace TestProject
{
    [TestFixture]
    public class GameBoardTests
    {
        [Test]
        public void InitialBoard_AllCellsAreEmpty()
        {
            var board = new GameBoard();

            for (int x = 0; x < GameBoard.BoardSize; x++)
                for (int y = 0; y < GameBoard.BoardSize; y++)
                    Assert.That(board.Cells[x, y], Is.EqualTo(CellState.Empty));
        }

        [Test]
        public void BoardSize_IsTenByTen()
        {
            var board = new GameBoard();

            Assert.That(board.Cells.GetLength(0), Is.EqualTo(10));
            Assert.That(board.Cells.GetLength(1), Is.EqualTo(10));
        }

        [Test]
        public void IsHit_WhenCellIsShip_ReturnsTrue()
        {
            var board = new GameBoard();
            board.Cells[3, 4] = CellState.Ship;

            bool result = board.IsHit(3, 4);

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsHit_WhenCellIsShip_SetsCellToHit()
        {
            var board = new GameBoard();
            board.Cells[3, 4] = CellState.Ship;

            board.IsHit(3, 4);

            Assert.That(board.Cells[3, 4], Is.EqualTo(CellState.Hit));
        }

        [Test]
        public void IsHit_WhenCellIsEmpty_ReturnsFalse()
        {
            var board = new GameBoard();

            bool result = board.IsHit(0, 0);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsHit_WhenCellIsEmpty_SetsCellToMiss()
        {
            var board = new GameBoard();

            board.IsHit(0, 0);

            Assert.That(board.Cells[0, 0], Is.EqualTo(CellState.Miss));
        }

        [Test]
        public void IsHit_WhenCellAlreadyHit_ReturnsFalse()
        {
            var board = new GameBoard();
            board.Cells[1, 1] = CellState.Hit;

            bool result = board.IsHit(1, 1);

            Assert.That(result, Is.False);
        }
    }
}
