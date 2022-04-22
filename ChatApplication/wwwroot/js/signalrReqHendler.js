connection = new signalR.HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Debug)
    .withUrl("/chatHub")
    .build();

connection.on('ReceiveMessage', addMessageToChat);
connection.on('NewConnection', incrementOnlineUsersCounter);
connection.on('GetOnlineUsersCount', incrementOnlineUsersCounter);
connection.on('GetDisconnectionUsersCount', disconnectedUsersCounter);

connection.start()
    .catch(error => {
        console.error(error(message));
    });

function sendMessageToHub(message) {
    connection.invoke('sendMessage', message)
}

function sendNewConnectionToHub() {
    connection.invoke('newConnection')
}

function fetchOnlineUsersCount() {
    connection.invoke('getOnlineUsersCount');
}

function fetchDisconnectionOnlineUsersCount() {
    alert("getDisconnectionUsersCount")
    connection.invoke('getDisconnectionUsersCount')
}
setTimeout(() => {
    fetchOnlineUsersCount();
    sendNewConnectionToHub();
    fetchDisconnectionOnlineUsersCount();
}, 3000);