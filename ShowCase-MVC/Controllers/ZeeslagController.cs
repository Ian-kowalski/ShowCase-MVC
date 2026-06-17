using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShowCase_MVC.Data;
using ShowCase_MVC.Hubs;
using ShowCase_MVC.Models;

namespace ShowCase_MVC.Controllers
{
    [Authorize]
    public class ZeeslagController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHubContext<GameHub> _hubContext;

        public ZeeslagController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHubContext<GameHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        public IActionResult Start()
        {
            return View();
        }

        public IActionResult LobbySelect()
        {
            return View();
        }

        public IActionResult Lobby()
        {
            return View();
        }

        public async Task<IActionResult> Game()
        {
            var userName = User.Identity?.Name;
            var gameState = _context.GameStates.FirstOrDefault(g => g.PlayerId == userName);

            GameBoard playerBoard;
            GameBoard trackingBoard;
            string? currentTurnPlayerId;

            if (gameState == null)
            {
                var allGameStates = _context.GameStates.ToList();
                if (allGameStates.Count >= 2)
                    return BadRequest("The game is full. Only two players are allowed.");

                playerBoard = new GameBoard();
                trackingBoard = new GameBoard();
                ShipGenerator.GenerateShips(playerBoard);

                gameState = new GameState
                {
                    PlayerId = userName,
                    PlayerBoard = JsonConvert.SerializeObject(playerBoard.Cells),
                    TrackingBoard = JsonConvert.SerializeObject(trackingBoard.Cells),
                    CurrentTurnPlayerId = allGameStates.Count == 0 ? userName : allGameStates[0].PlayerId
                };

                _context.GameStates.Add(gameState);
                await _context.SaveChangesAsync();

                await _hubContext.Clients.All.SendAsync("PlayerJoined", userName, allGameStates.Count + 1);
            }
            else
            {
                playerBoard = new GameBoard
                {
                    Cells = JsonConvert.DeserializeObject<CellState[,]>(gameState.PlayerBoard!)!
                };
                trackingBoard = new GameBoard
                {
                    Cells = JsonConvert.DeserializeObject<CellState[,]>(gameState.TrackingBoard!)!
                };
            }

            currentTurnPlayerId = gameState.CurrentTurnPlayerId;
            var viewModel = new GameBoardViewModel(playerBoard, trackingBoard, currentTurnPlayerId);
            ViewBag.UserName = userName;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> StartGame()
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName)) return Unauthorized();

            var gameState = await _context.GameStates.FirstOrDefaultAsync(g => g.PlayerId == userName);
            var opponentGameState = await _context.GameStates.FirstOrDefaultAsync(g => g.PlayerId != userName);

            if (gameState == null) return BadRequest("Game state not found.");
            if (opponentGameState == null) return BadRequest("Opponent not found.");

            var startingPlayerId = new Random().Next(2) == 0 ? userName : opponentGameState.PlayerId;
            gameState.CurrentTurnPlayerId = startingPlayerId;
            opponentGameState.CurrentTurnPlayerId = startingPlayerId;
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("UpdateCurrentTurn", startingPlayerId);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> MakeMove(int x, int y)
        {
            var userName = User.Identity?.Name;
            var gameState = _context.GameStates.FirstOrDefault(g => g.PlayerId == userName);
            if (gameState == null) return BadRequest("Game state not found.");
            if (gameState.CurrentTurnPlayerId != userName) return BadRequest("It's not your turn.");

            var opponentGameState = _context.GameStates.FirstOrDefault(g => g.PlayerId != userName);
            if (opponentGameState == null) return BadRequest("Opponent not found.");

            var opponentBoard = new GameBoard
            {
                Cells = JsonConvert.DeserializeObject<CellState[,]>(opponentGameState.PlayerBoard!)!
            };
            var trackingBoard = new GameBoard
            {
                Cells = JsonConvert.DeserializeObject<CellState[,]>(gameState.TrackingBoard!)!
            };

            bool hit = opponentBoard.Cells[x, y] == CellState.Ship;
            if (hit) opponentBoard.Cells[x, y] = CellState.Hit;
            else if (opponentBoard.Cells[x, y] == CellState.Empty) opponentBoard.Cells[x, y] = CellState.Miss;

            opponentGameState.PlayerBoard = JsonConvert.SerializeObject(opponentBoard.Cells);
            trackingBoard.Cells[x, y] = hit ? CellState.Hit : CellState.Miss;
            gameState.TrackingBoard = JsonConvert.SerializeObject(trackingBoard.Cells);

            bool gameOver = !opponentBoard.Cells.Cast<CellState>().Any(c => c == CellState.Ship);
            if (!gameOver) gameState.CurrentTurnPlayerId = opponentGameState.PlayerId;

            _context.SaveChanges();

            await _hubContext.Clients.All.SendAsync("ReceiveMove", userName, x, y, hit);
            if (gameOver) await _hubContext.Clients.All.SendAsync("GameOver", userName);

            return Json(new { hit, gameOver });
        }

        [HttpPost]
        public async Task<IActionResult> ResetGame()
        {
            var allStates = _context.GameStates.ToList();
            _context.GameStates.RemoveRange(allStates);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("GameReset");
            return Ok();
        }

        // kept for legacy Zeeslag chat/placement view
        public IActionResult Zeeslag()
        {
            return View();
        }
    }
}
