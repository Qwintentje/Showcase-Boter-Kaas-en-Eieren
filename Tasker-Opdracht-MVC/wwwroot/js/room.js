const connection = new signalR.HubConnectionBuilder()
    .withUrl("/game")
    .build();

let isEventListenerRegistered = false;

document.getElementById("join-form").addEventListener("submit", function (event) {
    event.preventDefault();

    const playerName = document.getElementById("player-name").value;

    connection.invoke("JoinRoom", playerName).catch(function (err) {
        console.error(err);
    });
});

connection.on("GameIsFull", () => {
    tempMsg("Game is vol")
});

connection.on("InvalidName", () => {
    tempMsg("Vul een geldige gebruikersnaam in")

});

connection.on("PlayerJoined", (token, gameId) => {
    window.location.href = "/home/game?Id=" + gameId;
    localStorage.setItem("Player", token);
});

if (!isEventListenerRegistered) {
    isEventListenerRegistered = true;
}

function tempMsg(msg) {
    const tempMsg = document.getElementById("temp-message")
    tempMsg.textContent = msg;
}

connection.start().then(() => {
    console.log("SignalR connection started.");
}).catch((err) => {
    console.error(err.toString());
});