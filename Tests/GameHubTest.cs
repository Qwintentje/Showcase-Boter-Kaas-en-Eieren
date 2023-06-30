using Microsoft.AspNetCore.SignalR;
using Moq;
using System.Reflection;
using System.Security.Claims;
using Tasker_Opdracht_MVC.Hubs;
using Tasker_Opdracht_MVC.Models;

namespace Tests;

public class GameHubTest
{
    private readonly GameModel _gameModel;
    private readonly Mock<GameHub> _mockGameHub;
    private readonly Mock<IClientProxy> _mockClientsAll;

    private readonly string User1Token = "TokenUser1";
    private readonly string User2Token = "TokenUser2";

    private GameHub _hub;
    private Mock<IHubCallerClients> _mockClients;
    private Mock<HubCallerContext> _mockCallerContext;
    private Mock<IClientProxy> _mockClientProxy;

    public GameHubTest()
    {
        // Init
        _mockGameHub = new Mock<GameHub>();
        _mockClientsAll = new Mock<IClientProxy>();
        _gameModel = new GameModel()
        {
            GameId = Guid.NewGuid().ToString(),
            User1Id = User1Token,
            User2Id = User2Token,
            User1 = "A",
            User2 = "B",
            Status = GameStatus.WaitingForPlayer,
            Board = new string[9],
            Symbol = "X",
        };

        // Seed
        FieldInfo? gameModelField = typeof(GameHub).GetField("_gameModel", BindingFlags.Public | BindingFlags.Static);
        if (gameModelField is null)
        {
            Assert.Fail("No such property");
        }

        gameModelField.SetValue(null, _gameModel);

        // Setup Hub and Clients
        _hub = new GameHub();
        _mockClients = new Mock<IHubCallerClients>();
        _mockCallerContext = new Mock<HubCallerContext>();
        _mockClientProxy = new Mock<IClientProxy>();

        _mockClients.Setup(clients => clients.Caller).Returns(_mockClientProxy.Object);
        _mockClients.Setup(clients => clients.All).Returns(_mockClientProxy.Object);
        _mockCallerContext.Setup(context => context.ConnectionId).Returns("connectionId");

        var mockIdentity = new Mock<ClaimsIdentity>();
        mockIdentity.SetupGet(identity => identity.Name).Returns("TestUser");
        mockIdentity.SetupGet(identity => identity.IsAuthenticated).Returns(true);
        mockIdentity.Setup(identity => identity.AuthenticationType).Returns("TestAuthentication");

        var mockPrincipal = new Mock<ClaimsPrincipal>();
        mockPrincipal.Setup(principal => principal.Identity).Returns(mockIdentity.Object);

        _mockCallerContext.Setup(context => context.User).Returns(mockPrincipal.Object);

        _hub.Clients = _mockClients.Object;
        _hub.Context = _mockCallerContext.Object;
    }

    [Theory]
    [InlineData("O", "X")]
    [InlineData("X", "O")]
    public void CanSwitchTurns(string input, string expected)
    {
        // Arrange
        _gameModel.Symbol = input;

        // Act
        _mockGameHub.Object.SwitchTurns();

        // Assert
        Assert.Equal(expected, _gameModel.Symbol);
    }

    [Theory]
    [InlineData("X", "X", "X", "X", "X", "X", "X", "X", "X", true)]
    [InlineData("O", "O", "O", "O", "O", "O", "O", "O", "O", true)]
    [InlineData("X", "O", "X", "O", "X", "O", null, "O", null, false)]
    public void CanWin(string cell1, string cell2, string cell3, string cell4, string cell5, string cell6, string cell7, string cell8, string cell9, bool expected)
    {
        // Arrange
        _gameModel.Board = new string[9]
        {
            cell1, cell2, cell3,
            cell4, cell5, cell6,
            cell7, cell8, cell9,
        };

        // Act
        bool actual = _mockGameHub.Object.IsWin(_gameModel.Board);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    //Invalid Token
    [InlineData("invalidToken", "InvalidGame", new object[] { }, GameStatus.WaitingForPlayer)]
    //User 1 valid move
    [InlineData("TokenUser1", "MoveReceived", new object[] { 0, "A", new object[] { "X", null, null, null, null, null, null, null, null } }, GameStatus.Player1Turn)]
    //User 2 valid move
    [InlineData("TokenUser2", "MoveReceived", new object[] { 0, "B", new object[] { "X", null, null, null, null, null, null, null, null } }, GameStatus.Player2Turn)]
    //Move while status = waiting for player
    [InlineData("TokenUser2", "WaitForPlayer", new object[] { }, GameStatus.WaitingForPlayer)]
    public async Task CanSendMove(
        string token,
        string expectedMethod,
        object[] expectedArguments, GameStatus status)
    {
        // Arrange
        string methodName = null;
        object[] arguments = null;
        _mockClientProxy
            .Setup(proxy => proxy.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default(CancellationToken)))
            .Callback<string, object[], CancellationToken>((name, args, ct) =>
            {
                methodName = name;
                arguments = args;
            })
            .Returns(Task.CompletedTask);

        _gameModel.Status = status;

        // Act
        await _hub.SendMove(0, token);

        // Assert
        Assert.Equal(expectedMethod, methodName);
        Assert.Equal(expectedArguments, arguments);
    }

    [Theory]
    //Invalid name
    [InlineData("", "InvalidName", new object[] { GameStatus.WaitingForPlayer }, false)]
    //Valid name
    [InlineData("Test Naam", "PlayerJoined", new object[] { GameStatus.WaitingForPlayer }, false)]
    //Valid name second player
    [InlineData("Test Naam", "PlayerJoined", new object[] { GameStatus.Player1Turn }, true)]
    public async Task CanJoinRoom(
        string name,
        string expectedMethod,
        object[] expectedArguments, bool isSecondPlayer)
    {
        // Arrange
        var mockIdentity = new Mock<ClaimsIdentity>();
        mockIdentity.SetupGet(identity => identity.IsAuthenticated).Returns(false);

        var mockPrincipal = new Mock<ClaimsPrincipal>();
        mockPrincipal.Setup(principal => principal.Identity).Returns(mockIdentity.Object);

        _mockCallerContext.Setup(context => context.User).Returns(mockPrincipal.Object);

        string methodName = null;
        object[] arguments = null;
        _mockClientProxy
            .Setup(proxy => proxy.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default(CancellationToken)))
            .Callback<string, object[], CancellationToken>((name, args, ct) =>
            {
                methodName = name;
                arguments = args;
            })
            .Returns(Task.CompletedTask);

        // Seed
        FieldInfo? gameModelField = typeof(GameHub).GetField("_gameModel", BindingFlags.Public | BindingFlags.Static);
        if (gameModelField is null)
        {
            Assert.Fail("No such property");
        }

        gameModelField.SetValue(null, null);
        // Act
        if (isSecondPlayer)
        {
            await _hub.JoinRoom("Player1");
        }
        await _hub.JoinRoom(name);
        gameModelField = typeof(GameHub).GetField("_gameModel", BindingFlags.Public | BindingFlags.Static);

        var _gameModel = (GameModel)gameModelField.GetValue(null) ?? new GameModel();

        // Assert
        Assert.Equal(expectedMethod, methodName);
        Assert.Equal(expectedArguments, new object[] { _gameModel.Status });
    }
}
