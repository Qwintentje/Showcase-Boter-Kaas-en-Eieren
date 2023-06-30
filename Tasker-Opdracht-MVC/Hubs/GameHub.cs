using Microsoft.AspNetCore.SignalR;
using Tasker_Opdracht_MVC.Data.Entities;
using Tasker_Opdracht_MVC.Models;

namespace Tasker_Opdracht_MVC.Hubs;

public class GameHub : Hub
{
    public static GameModel? _gameModel;

    public async Task SendMove(int index, string token)
    {
        //Check if user1 or 2 is trying to make a move or a random person
        if (_gameModel == null || (_gameModel.User1Id != token && _gameModel.User2Id != token))
        {
            await Clients.Caller.SendAsync("InvalidGame");
            return;
        }
        if (_gameModel.Status == GameStatus.WaitingForPlayer)
        {
            await Clients.Caller.SendAsync("WaitForPlayer");
        }
        //Check if it is player 1's turn and if token is correct
        if (_gameModel.Status == GameStatus.Player1Turn && _gameModel.User1Id == token)
        {
            if (_gameModel.Board[index] == null)
            {
                _gameModel.Board[index] = _gameModel.Symbol;
                if (IsWin(_gameModel.Board))
                {
                    await StoreGame(_gameModel.User1);
                }
                SwitchTurns();
            }
            else return;
            await Clients.All.SendAsync("MoveReceived", index, _gameModel.User1, _gameModel.Board);
        }
        //Check if it is player 2's turn and if token is correct
        else if (_gameModel.Status == GameStatus.Player2Turn && _gameModel.User2Id == token)
        {
            if (_gameModel.Board[index] == null)
            {
                _gameModel.Board[index] = _gameModel.Symbol;
                if (IsWin(_gameModel.Board))
                {
                    await StoreGame(_gameModel.User2);
                }
                SwitchTurns();
            }
            else return;
            await Clients.All.SendAsync("MoveReceived", index, _gameModel.User2, _gameModel.Board);
        }
    }

    public void SwitchTurns()
    {
        if (_gameModel != null)
        {
            _gameModel.Symbol = _gameModel.Symbol == "X" ? "O" : "X";
            _gameModel.Status = _gameModel.Status == GameStatus.Player1Turn ? GameStatus.Player2Turn : GameStatus.Player1Turn;
        }
    }

    public async Task ReplayGame()
    {
        if (_gameModel != null)
        {
            _gameModel.IsGameFull = false;
            _gameModel.Board = new string[9];
            _gameModel.Symbol = "X";
            _gameModel.Status = GameStatus.Player1Turn;
            await Clients.All.SendAsync("ReplayGame");
        }
    }

    public async Task LeaveGame(string token)
    {
        //Check if user1 or 2 is trying to leave or a random person
        if (_gameModel?.User1Id == token || _gameModel?.User2Id == token)
        {
            _gameModel = null;
            await Clients.All.SendAsync("ResetGame");
        }
        else
        {
            await Clients.Caller.SendAsync("InvalidGame");
        }
    }


    public async Task GetGameState()
    {
        if (_gameModel != null)
        {
            await Clients.Caller.SendAsync("GameStateReceived", _gameModel.Board);
        }
    }


    public async Task StoreGame(string playerWon)
    {
        try
        {
            var model = new Game
            {
                User1 = _gameModel.User1,
                User2 = _gameModel.User2 == null ? "No opponent" : _gameModel.User2,
                Board = string.Join(",", _gameModel.Board),
                TimeFinished = DateTime.Now,
                PlayerWon = playerWon,
                GameId = _gameModel.GameId
            };
            if (Context.User.Identity.IsAuthenticated)
            {
                model.PlayerWonEmail = model.PlayerWon == _gameModel.User1 ? _gameModel.User1Email : _gameModel.User2Email;
            }
            else
            {
                model.PlayerWonEmail = "";
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7009");

                var response = await client.PostAsJsonAsync("api/GameApi", model);
                var content = response.Content.ReadAsStringAsync(); //Tijdelijk voor checks
                if (!response.IsSuccessStatusCode)
                {
                    //Succes
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public async Task JoinRoom(string playerName)
    {
        if (playerName == "" || playerName.Length > 255)
        {
            await Clients.Caller.SendAsync("InvalidName");
            return;
        }
        else if (_gameModel == null)
        {
            _gameModel = new GameModel()
            {
                GameId = Guid.NewGuid().ToString(),
                User1Id = Guid.NewGuid().ToString(),
                User1 = playerName,
                Status = GameStatus.WaitingForPlayer,
                Board = new string[9],
                Symbol = "X"

            };
            if (Context.User.Identity.IsAuthenticated)
            {
                _gameModel.User1Email = Context.User.Identity.Name;
            }

            await Clients.Caller.SendAsync("PlayerJoined", _gameModel.User1Id, _gameModel.GameId);
        }
        else
        {
            if (_gameModel.IsGameFull)
            {
                await Clients.Caller.SendAsync("GameIsFull");
            }
            else
            {
                _gameModel.User2Id = Guid.NewGuid().ToString();
                _gameModel.User2 = playerName;
                _gameModel.IsGameFull = true;
                _gameModel.Status = GameStatus.Player1Turn;
                await Clients.Caller.SendAsync("PlayerJoined", _gameModel.User2Id, _gameModel.GameId);
                if (Context.User.Identity.IsAuthenticated)
                {
                    _gameModel.User2Email = Context.User.Identity.Name;
                }
            }
        }
    }
    public bool IsWin(string[] board)
    {
        int[,] winningCombos = new int[,]
        {
    {0, 1, 2},
    {3, 4, 5},
    {6, 7, 8},
    {0, 3, 6},
    {1, 4, 7},
    {2, 5, 8},
    {0, 4, 8},
    {2, 4, 6}
        };

        for (int i = 0; i < winningCombos.GetLength(0); i++)
        {
            int a = winningCombos[i, 0];
            int b = winningCombos[i, 1];
            int c = winningCombos[i, 2];

            if (board[a] != null && board[a] == board[b] && board[a] == board[c])
            {
                return true;
            }
        }

        return false;
    }
}


