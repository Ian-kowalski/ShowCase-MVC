using ShowCase_MVC.Models;

namespace TestProject
{
    [TestFixture]
    public class ShipGeneratorTests
    {
        // Fleet: 5+4+3+3+2 = 17 ship cells
        private const int ExpectedShipCells = 17;

        [Test]
        public void GenerateShips_PlacesCorrectTotalShipCells()
        {
            var board = new GameBoard();

            ShipGenerator.GenerateShips(board);

            int shipCells = board.Cells.Cast<CellState>().Count(c => c == CellState.Ship);
            Assert.That(shipCells, Is.EqualTo(ExpectedShipCells));
        }

        [Test]
        public void GenerateShips_AllShipsWithinBounds()
        {
            var board = new GameBoard();

            ShipGenerator.GenerateShips(board);

            // If GenerateShips throws or places out of bounds, this test fails
            Assert.That(() => ShipGenerator.GenerateShips(new GameBoard()), Throws.Nothing);
        }

        [Test]
        public void GenerateShips_NoNonShipCellsBecome_Ship_AfterGeneration()
        {
            var board = new GameBoard();

            ShipGenerator.GenerateShips(board);

            // Every cell is either Empty or Ship — no Hit or Miss should appear
            var invalidCells = board.Cells.Cast<CellState>()
                .Where(c => c != CellState.Empty && c != CellState.Ship)
                .ToList();

            Assert.That(invalidCells, Is.Empty);
        }

        [Test]
        public void GenerateShips_CanBeCalledMultipleTimes_WithoutException()
        {
            for (int i = 0; i < 20; i++)
            {
                var board = new GameBoard();
                Assert.That(() => ShipGenerator.GenerateShips(board), Throws.Nothing);
            }
        }

        [Test]
        public void GenerateShips_ProducesDifferentLayouts()
        {
            // Run enough times to be statistically certain randomness is working
            var layouts = new HashSet<string>();
            for (int i = 0; i < 10; i++)
            {
                var board = new GameBoard();
                ShipGenerator.GenerateShips(board);
                string key = string.Join("", board.Cells.Cast<CellState>().Select(c => (int)c));
                layouts.Add(key);
            }

            Assert.That(layouts.Count, Is.GreaterThan(1));
        }
    }
}
