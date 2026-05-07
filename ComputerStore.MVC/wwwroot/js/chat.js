"use strict";

// 1. Kết nối tới trạm phát sóng ChatHub
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

// Tạm khóa nút gửi khi chưa kết nối xong
document.getElementById("sendButton").disabled = true;

// 2. Lắng nghe tin nhắn từ Server gửi về (Nhận tin)
connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    li.className = "list-group-item";
    li.innerHTML = `<strong>${user}</strong>: ${message}`;
    document.getElementById("messagesList").appendChild(li);

    // Tự động cuộn xuống tin nhắn mới nhất
    var messageListElement = document.getElementById("messagesList");
    messageListElement.scrollTop = messageListElement.scrollHeight;
});

// 3. Khởi động kết nối
connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
    console.log("Đã kết nối SignalR thành công!");
}).catch(function (err) {
    return console.error(err.toString());
});

// 4. Sự kiện khi bấm nút "Gửi"
document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value || "Ẩn danh";
    var message = document.getElementById("messageInput").value;

    if (message.trim() === "") return;

    // Gửi tin nhắn lên Server
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });

    // Xóa ô nhập sau khi gửi và nháy chuột lại vào ô
    document.getElementById("messageInput").value = "";
    document.getElementById("messageInput").focus();
    event.preventDefault();
});

// 5. Tính năng bấm Enter để gửi tin nhắn
document.getElementById("messageInput").addEventListener("keypress", function (event) {
    if (event.key === "Enter") {
        event.preventDefault();
        document.getElementById("sendButton").click();
    }
});