 
function setMessage(messageText, userName, messageTime,idMessage) {
    document.getElementById("message").value = "";
    var nickname = document.getElementById("nicknameVal").value;
    var messageTextDiv = document.createElement("div");
    messageTextDiv.classList.add("messageTextBlock");
    messageTextDiv.innerHTML = messageText;
    messageTextDiv.setAttribute("id", "message" + idMessage);
    var chat = document.getElementById("chat");
    var messageUserDiv = document.createElement("div");
    messageUserDiv.classList.add("messageUserBlock");
    messageUserDiv.innerHTML =userName;
    var messageTimeDiv = document.createElement("div");
    messageTimeDiv.classList.add("messageTimeBlock");
    var messageTimeLabel = document.createElement("label");
    messageTimeLabel.innerHTML = messageTime;
    messageTimeDiv.appendChild(messageTimeLabel);
    var messageDiv = document.createElement("div");
    messageDiv.setAttribute("id", "id" + idMessage);
    if (userName===nickname) {
        messageDiv.classList.add("myMessageBlock");
        var messageMenu = document.createElement("div");
        messageMenu.classList.add("messageMenu");
        messageMenu.setAttribute("id", "messageMenu");
        var btnEdit = document.createElement("button");
        btnEdit.textContent = "Edit";
        btnEdit.addEventListener("click", function (event) {
            messageMenu.remove();
            editMessage(idMessage, messageText);
        });
        messageMenu.appendChild(btnEdit);
        var btnDelete = document.createElement("button");
        btnDelete.textContent = "Delete";
        btnDelete.addEventListener("click", function (event) {
            messageMenu.remove();
            connection.invoke("DeleteMessage", idMessage).catch(function (err) {
                return console.error(err.toString());
            });
        });
        messageMenu.appendChild(btnDelete);
        chat.appendChild(messageMenu);
    }
    else {
        messageDiv.classList.add("messageBlock");
    }
    var messageInfoBlock = document.createElement("div");
    messageInfoBlock.classList.add("messageInfoBlock");
    messageInfoBlock.appendChild(messageUserDiv);
    messageInfoBlock.appendChild(messageTimeDiv);
    messageDiv.appendChild(messageInfoBlock);
    messageDiv.appendChild(messageTextDiv);
    chat.appendChild(messageDiv);
    messageDiv.addEventListener("contextmenu", function (event) {
        event.preventDefault();
        messageMenu.style.display = "block";
        messageMenu.style.left = event.pageX + 'px';
        messageMenu.style.top = event.pageY + 'px';
    });
}

function editMessage(idMessage,textMessage) {
    var editBlock = document.getElementById("editMessage");
    document.getElementById("editText").innerHTML = textMessage;
    document.getElementById("editText").value = idMessage;
    editBlock.style.visibility = "visible";
    sendEdit();
}

function closeEdit() {
    editSend();
    document.getElementById("editMessage").style.visibility="hidden";
}

function setChat(name, description) {
    document.getElementById("ChatName").innerHTML = name;
    document.getElementById("ChatDesc").innerHTML = description;
}


function sendEdit() {
    sendBtn = document.getElementById("sendBtn");
    editBtn = document.getElementById("editBtn");
    sendBtn.classList.remove("chatSend");
    sendBtn.classList.add("chatEdit");
    editBtn.classList.remove("chatEdit");
    editBtn.classList.add("chatSend");
}

function editSend() {
    sendBtn = document.getElementById("sendBtn");
    editBtn = document.getElementById("editBtn");
    editBtn.classList.remove("chatSend");
    editBtn.classList.add("chatEdit");
    sendBtn.classList.remove("chatEdit");
    sendBtn.classList.add("chatSend");
}


function deleteMessage(idMessage) {
    var message = document.getElementById("id" + idMessage);
    message.remove();
}

function deleteChat() {
    var chatId = document.getElementById("idChat").value;
    connection.invoke("DeleteChat", chatId).catch(function (err) {
        return console.error(err.toString());
    });
}
function edit(text, idMessage) {
    document.getElementById("message" + idMessage).innerHTML = text;
    document.getElementById("message").value = "";
}


const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chat")
    .build();
document.getElementById("sendBtn").disabled = true;

connection.on("ReceiveMessage", function (message, nickname, time, chat,idMessage) {
    if (chat === document.querySelector(".chatsActualChatBlock").getAttribute("data-chat-name")) {
        setMessage(message, nickname, time,idMessage);
    }
});

connection.on("DeleteMessage", function (idChat) {
});

connection.start().then(function () {
    document.getElementById("sendBtn").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendBtn").addEventListener("click", function (event) {
    var message = document.getElementById("message").value;
    var nickname = document.getElementById("nicknameField").value;
    var chat = document.querySelector(".chatsActualChatBlock").getAttribute("data-chat-id");
    connection.invoke("SendMessage", message,nickname,chat).catch(function (err) {
        return console.error(err.toString());
    });
});

document.getElementById("editBtn").addEventListener("click", function (event) {
    var message = document.getElementById("message").value;
    var idChat = document.getElementById("editText").value;
    editSend();
    document.getElementById("editMessage").style.visibility = "hidden";
    connection.invoke("EditMessage", message,idChat).catch(function (err) {
        return console.error(err.toString());
    });
});


function changeChat(chatId) {
    var newChat = document.getElementById("chat-" + chatId);
    var currentActiveChat = document.querySelector(".chatsActualChatBlock");
    let oldId = currentActiveChat.getAttribute("data-chat-id");
    if (currentActiveChat) {
        currentActiveChat.classList.remove("chatsActualChatBlock");
        currentActiveChat.classList.add("chatsChatBlock");
    }
    newChat.classList.remove("chatsChatBlock");
    newChat.classList.add("chatsActualChatBlock");
    var chat = document.getElementById("chat");
    chat.innerHTML = "";
    connection.invoke("LeaveChat", oldId).catch(function (err) {
        return console.error(err.toString());
    });
    connection.invoke("JoinChat", chatId).catch(function (err) {
        return console.error(err.toString());
    });
    connection.invoke("ChangeChat", chatId).catch(function (err) {
        return console.error(err.toString());
    });
    connection.invoke("GetChatInfo", chatId).catch(function (err) {
        return console.error(err.toString());
    });
    connection.invoke("GetChatUsers", chatId).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("idChat").value = chatId.toString();
}

connection.on("GetChat", function (text, name, time,idMessage) {
    setMessage(text, name, time,idMessage);
});

connection.on("EditMessage", function (text, idMessage) {
    edit(text, idMessage);
});

connection.on("GetChatInfo",function(name,description){
    setChat(name, description);
});

connection.on("GetChatUsers", function (name, description) {
    setMessage(description, name, '12:08', 9);
});

connection.on("DeleteMessage", function (idMessage) {
    deleteMessage(idMessage);
});

var contextMenu = document.getElementById("messageMenu");

document.addEventListener("click", function (event) {
    if (!event.target.closest("#contextMenu")) {
        contextMenu.style.display = "none";
    }
});

document.querySelectorAll(".messageBlock").forEach(function (message) {
    message.addEventListener("contextmenu", function (event) {
        event.preventDefault();
        contextMenu.style.display = "block";
        contextMenu.style.left = event.pageX + 'px';
        contextMenu.style.top = event.pageY + 'px';
    });
});


document.querySelectorAll(".messageBlock").forEach(function (message) {
    message.addEventListener('click', function (event) {
        connection.invoke("DeleteMessage", chatId).catch(function (err) {
            return console.error(err.toString());
        });
    });
});

