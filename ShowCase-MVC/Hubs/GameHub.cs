using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShowCase_MVC.Data;
using ShowCase_MVC.Models;
using System.Collections.Concurrent;

namespace ShowCase_MVC.Hubs
{
    public class GameHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> Users = new();
        private static readonly ConcurrentQueue<string> PlayerQueue = new(new[] { "Player 1", "Player 2" });
        private static string? CurrentTurnPlayerId;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GameHub> _logger;

        public GameHub(ApplicationDbContext context, ILogger<GameHub> logger)
        {
            _context = context;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            if (Users.Count >= 2)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "System", "Game room is full.");
                Context.Abort();
            }
            else
            {
                await base.OnConnectedAsync();
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Users.TryRemove(Context.ConnectionId, out var player))
            {
                PlayerQueue.Enqueue(player);
                await Clients.All.SendAsync("ReceiveMessage", "System", $"{player} has left the game.");
                await Clients.All.SendAsync("UpdatePlayers", Users.Values);
            }

            if (Context.ConnectionId == CurrentTurnPlayerId)
            {
                CurrentTurnPlayerId = Users.Keys.FirstOrDefault();
                await Clients.All.SendAsync("UpdateCurrentTurn", CurrentTurnPlayerId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task RegisterPlayer(string userName)
        {
            if (Users.Count >= 2)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "System", "Game room is full.");
                Context.Abort();
                return;
            }

            if (PlayerQueue.TryDequeue(out var player))
            {
                Users[Context.ConnectionId] = userName;
                await Clients.All.SendAsync("ReceiveMessage", "System", $"Welcome {userName}!");
                await Clients.All.SendAsync("UpdatePlayers", Users.Values);
                await Clients.All.SendAsync("PlayerJoined", Context.ConnectionId, Users.Count);

                if (Users.Count == 1)
                    CurrentTurnPlayerId = Context.ConnectionId;
            }
        }

        public async Task StartGame()
        {
            var userId = Context.UserIdentifier;
            var gameState = _context.GameStates.FirstOrDefault(g => g.PlayerId == userId);
            var opponentGameState = _context.GameStates.FirstOrDefault(g => g.PlayerId != userId);

            if (gameState == null || opponentGameState == null)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "System", "Both players must be in game to start.");
                return;
            }

            var random = new Random();
            var startingPlayerId = random.Next(2) == 0 ? userId : opponentGameState.PlayerId;

            gameState.CurrentTurnPlayerId = startingPlayerId;
            opponentGameState.CurrentTurnPlayerId = startingPlayerId;
            await _context.SaveChangesAsync();

            await Clients.All.SendAsync("UpdateCurrentTurn", startingPlayerId);
            await Clients.All.SendAsync("ReceiveMessage", "System", "The game has started!");
        }

        public async Task MakeMove(string user, int x, int y)
        {
            var gameState = await _context.GameStates.FirstOrDefaultAsync(g => g.PlayerId == user);
            if (gameState == null)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "System", "Game state not found.");
                return;
            }

            if (gameState.CurrentTurnPlayerId != user)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "System", "It's not your turn.");
                return;
            }

            var opponentGameState = await _context.GameStates.FirstOrDefaultAsync(g => g.PlayerId != user);
            if (opponentGameState == null)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "System", "Opponent not found.");
                return;
            }

            var opponentBoard = JsonConvert.DeserializeObject<CellState[,]>(opponentGameState.PlayerBoard!);
            bool hit = false;

            if (opponentBoard![x, y] == CellState.Ship)
            {
                opponentBoard[x, y] = CellState.Hit;
                hit = true;
            }
            else if (opponentBoard[x, y] == CellState.Empty)
            {
                opponentBoard[x, y] = CellState.Miss;
            }

            opponentGameState.PlayerBoard = JsonConvert.SerializeObject(opponentBoard);

            bool gameOver = !opponentBoard.Cast<CellState>().Any(c => c == CellState.Ship);

            if (!gameOver)
            {
                var nextPlayer = opponentGameState.PlayerId;
                gameState.CurrentTurnPlayerId = nextPlayer;
                opponentGameState.CurrentTurnPlayerId = nextPlayer;
                CurrentTurnPlayerId = nextPlayer;
            }

            await _context.SaveChangesAsync();

            await Clients.All.SendAsync("ReceiveMove", user, x, y, hit);
            await Clients.All.SendAsync("UpdateCurrentTurn", CurrentTurnPlayerId);

            if (gameOver)
                await Clients.All.SendAsync("GameOver", user);
        }

        public async Task ResetGame()
        {
            var allStates = _context.GameStates.ToList();
            _context.GameStates.RemoveRange(allStates);
            await _context.SaveChangesAsync();

            Users.Clear();
            while (PlayerQueue.TryDequeue(out _)) { }
            PlayerQueue.Enqueue("Player 1");
            PlayerQueue.Enqueue("Player 2");
            CurrentTurnPlayerId = null;

            await Clients.All.SendAsync("GameReset");
        }

        public async Task SendMove(string userName, int x, int y)
        {
            await MakeMove(userName, x, y);
        }

        public async Task SendMessage(string userName, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", userName, message);
        }
    }
}
