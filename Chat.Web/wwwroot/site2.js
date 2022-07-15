"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
///document.getElementById("sendButton").disabled = true;

//connection.on("ReceiveMessage", function (user, message) {
//    var li = document.createElement("li");
//    document.getElementById("messagesList").appendChild(li);
//    // We can assign user-supplied strings to an element's textContent because it
//    // is not interpreted as markup. If you're assigning in any other way, you 
//    // should be aware of possible script injection concerns.
//    li.textContent = `${user} says ${message}`;
//});

connection.start().then(function () {
    document.getElementById("messengerButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

//document.getElementById("sendButton").addEventListener("click", function (event) {
//    var user = document.getElementById("userInput").value;
//    var message = document.getElementById("messageInput").value;
//    connection.invoke("SendMessage", user, message).catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//});

document.getElementById("messengerButton").addEventListener("click", function (event) {
    var message = document.getElementById("messengerInput").value;
    var receiverId = document.getElementById("receiverId").value;
    var time = document.getElementById("time").value;
    if (message!="") {
        connection.invoke("SendPrivateMessage", receiverId, message, time).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
        const p = document.createElement("p");
        var i = document.createElement("p");
        p.textContent = `${message }`;
        i.textContent = `${time}`;
        i.classList.add("sender2");
        p.classList.add("sender");
        
        i.appendChild(p)
        document.getElementsByClassName("messageCustom")[0].appendChild(i);

        document.getElementById("messengerInput").value = ""
    }
});

connection.on("ReceiveMessage", function (senderName, message, time) {
    var p = document.createElement("p");
    var i = document.createElement("p");
    p.textContent = `${message}`;
    i.textContent = `${time}`;
    i.classList.add("receiver2");
    p.classList.add("receiver");

    i.appendChild(p)
    document.getElementsByClassName("messageCustom")[0].appendChild(i);
});

document.getElementById("messengerInput").addEventListener("focus", function (event) {
    var receiverId = document.getElementById("receiverId").value;

    connection.invoke("ShowTyping", receiverId).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

connection.on("ShowTyping", function () {
    var p = document.createElement("p");
    p.textContent = `Typing...`;
    p.classList.add("showtyping")
    document.getElementsByClassName("messageCustom")[0].appendChild(p);
});

document.getElementById("messengerInput").addEventListener("blur", function (event) {
    var receiverId = document.getElementById("receiverId").value;

    connection.invoke("HideTyping", receiverId).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

connection.on("HideTyping", function () {
    document.getElementsByClassName("showtyping")[0].remove();
});

$(function () {
    $('ul#users-list').on('click', 'li', function () {
        var username = $("input[type=hidden].username", $(this)).val();
        var input = $('#chat-message');

        var text = input.val();
        if (text.startsWith("/")) {
            text = text.split(")")[1];
        }

        text = "/private(" + username + ") " + text.trim();
        input.val(text);
        input.change();
        input.focus();
    });

    $('#emojis-container').on('click', 'a', function () {
        var value = $("input", $(this)).val();
        var input = $('#messengerInput');
        input.val(input.val() + value);
        input.focus();
        input.change();
    });

    $("#emojibtn").click(function () {
        $("#emojis-container").toggleClass("d-none");
    });

    $("#chat-message, #btn-send-message").click(function () {
        $("#emojis-container").addClass("d-none")
    });

    $('.modal').on('hidden.bs.modal', function () {
        $(".modal-body input").val("");
    });

    $(".alert .close").on('click', function () {
        $(this).parent().hide();
    });
});
