const WebSocket = require('ws');
const wss = new WebSocket.Server({ port: 8080 });

let clients = [];

function log(msg) {
    const now = new Date().toLocaleTimeString();
    console.log(`[${now}] ${msg}`);
}

wss.on('connection', function connection(ws, req) {
    clients.push(ws);
    const ip = req?.socket?.remoteAddress || "okänd";
    log(`Client connected (${ip === '::1' ? 'localhost' : ip}). Totalt anslutna: ${clients.length}`);

    ws.on('message', function incoming(message) {
        log(`Meddelande från klient: ${message}`);
        // Broadcast till andra spelare
        clients.forEach(client => {
            if (client !== ws && client.readyState === WebSocket.OPEN) {
                client.send(message);
            }
        });
    });

    ws.on('close', function () {
        clients = clients.filter(c => c !== ws);
        log(`Client disconnected. Totalt anslutna: ${clients.length}`);
    });
});

log("WebSocket server is running on ws://localhost:8080");
