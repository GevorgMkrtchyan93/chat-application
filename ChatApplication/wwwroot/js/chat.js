﻿class RedisCacheDataModel {
    constructor(username, text, when) {
        this.userName = username;
        this.text = text;
        this.when = when;
    }
}

let usersOnline = 0;
const username = document.getElementById('current-user').value;
const textInput = document.getElementById('messageText');
const chat = document.getElementById('chat');
const messagesQueue = [];

document.getElementById('submitButton').addEventListener('click', () => {
    let when = document.createElement('span');
    var currentdate = new Date();
    when.innerHTML =
        (currentdate.getMonth() + 1) + "/"
        + currentdate.getDate() + "/"
        + currentdate.getFullYear() + ""
        + currentdate.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', second: 'numeric' });
});

function clearInputField() {
    messagesQueue.push(textInput.value);
    textInput.value = "";
}

function sendMessage() {
    let text = messagesQueue.shift() || "";
    if (text.trim() === "") return;
    let message = new RedisCacheDataModel(username, text);
    sendMessageToHub(message);
}

function addMessageToChat(message) {
    let isCurrentUserMessage = message.userName === username;
    let container = document.createElement('div');
    container.className = isCurrentUserMessage ? "container darker time-left" : "container time-right";

    let timePosition, textAligne, offset;
    if (!isCurrentUserMessage) {
        containerClass = "container";
        timePosition = "time-left";
        textAligne = "text-left";
        offset = "";
    }
    else {
        containerClass = "connection darker";
        timePosition = "time-right text-right";
        textAligne = "text-right";
        offset = "col-md-6 offset-md-6";
    }

    const messageHTML = `<div class="row">
        <div class="${offset}">
                <p class="sender ${textAligne}">${message.userName}</p>
                <p class="${textAligne}">${message.text}</p>
                <p class="${timePosition}">${message.when}</p>
        </div>
    </div>`

    chat.appendChild(htmlToElement(messageHTML));
}

function htmlToElement(html) {
    var template = document.createElement('template');
    html = html.trim();
    template.innerHTML = html;
    return template.content.firstChild;
}

const onlineUsersBlock = document.getElementById('onlineUsersCounter');

function incrementOnlineUsersCounter(initialValue) {
    if (initialValue) {
        usersOnline = initialValue;
    }
    else {
        usersOnline++;
    }
    onlineUsersBlock.innerHTML = `Users online : ${usersOnline}`;
}

function disconnectedUsersCounter(finishValue) {
    if (finishValue) {
        usersOnline = finishValue;
    }
    else {
        usersOnline--;
    }
    onlineUsersBlock.innerHTML = `Users online : ${usersOnline}`;
}
