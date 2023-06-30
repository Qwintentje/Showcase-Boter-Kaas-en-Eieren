const connection = new signalR.HubConnectionBuilder()
    .withUrl("/game")
    .build();

const cells = document.querySelectorAll('.cell');
board = Array(9).fill(null);
const messageDiv = document.getElementById("result-message");
messageDiv.classList.add("hide");

const replayButon = document.getElementById("replay-button");
replayButon.style.display = "none";
replayButon.addEventListener('click', () => {
    connection.invoke("ReplayGame")
});

const leaveButton = document.getElementById("leave-button");
leaveButton.addEventListener('click', () => {
    const token = localStorage.getItem("Player");
    connection.invoke("LeaveGame", token);
});

cells.forEach(cell => {
    cell.addEventListener('click', () => {
        const index = Array.from(cells).indexOf(cell);
        const token = localStorage.getItem("Player");
        connection.invoke("SendMove", index, token);
    });
});

// Client methods
connection.on("moveReceived", (index, playerName, serverBoard) => {
    if (!isWin() && !isTie()) {
        board = serverBoard;
        cells[index].innerText = board[index];
        sessionStorage.setItem("lastMove", playerName);
    }
    if (isWin()) {
        showResult(`${playerName} wint!`);

    }
    else if (isTie()) {
        showResult("Gelijkspel!");
    }
});

connection.on("WaitForPlayer", () => {
    alert("Wacht totdat de 2e speler gejoined is");
});

connection.on("ResetGame", () => {
    board = Array(9).fill(null);
    cells.forEach(cell => {
        cell.innerText = '';
    });
    sessionStorage.clear();
    window.location.href = "/home/rooms";
    name = "";
});

connection.on("ReplayGame", () => {
    board = Array(9).fill(null);
    cells.forEach(cell => {
        cell.innerText = '';
    });
    replayButon.style.display = "none";
    messageDiv.classList.add("hide");
    messageDiv.classList.remove("show");
});

connection.on("GameStateReceived", (serverBoard) => {
    board = serverBoard
    cells.forEach((cell, index) => {
        cell.innerText = board[index];
    });
    if (isWin()) {
        replayButon.style.display = "block";
        name = sessionStorage.getItem("lastMove");
        showResult(`${name} wint!`);
    }
    if (isTie()) {
        replayButon.style.display = "block";
        showResult("Gelijkspel!");
    }
});

connection.on("InvalidGame", () => {
    window.location.href = "/home/rooms"
});

connection.on("SaveGame", () => {
    connection.invoke("StoreGame");
});

function showResult(message) {
    messageDiv.classList.remove("hide");
    messageDiv.classList.add("show");
    messageDiv.innerText = message;
    replayButon.style.display = "block";
}

function loadGameState(serverBoard) {
    board = serverBoard
    cells.forEach((cell, index) => {
        cell.innerText = board[index];
    });
    if (isWin()) {
        replayButon.style.display = "block";
        name = sessionStorage.getItem("lastMove");
        showResult(`${name} wint!`);
    }
    if (isTie()) {
        replayButon.style.display = "block";
        showResult("Gelijkspel!");
    }
}

connection.start().then(() => {
    console.log("SignalR connection started.");
    connection.invoke("GetGameState");
}).catch((err) => {
    console.error(err.toString());
});

//Game Logic
function isWin() {
    const winningCombos = [
        [0, 1, 2],
        [3, 4, 5],
        [6, 7, 8],
        [0, 3, 6],
        [1, 4, 7],
        [2, 5, 8],
        [0, 4, 8],
        [2, 4, 6]
    ];
    for (let i = 0; i < winningCombos.length; i++) {
        const [a, b, c] = winningCombos[i];
        if (board[a] && board[a] === board[b] && board[a] === board[c]) {
            return true;
        }
    }
}
function isTie() {
    if (!board.includes(null)) {
        return true;
    }
}
