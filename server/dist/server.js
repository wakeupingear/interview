"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const ws_1 = require("ws");
const dotenv_1 = __importDefault(require("dotenv"));
const openai_1 = __importDefault(require("openai"));
dotenv_1.default.config();
const openai = new openai_1.default({});
const wss = new ws_1.WebSocketServer({ port: 8080 });
var ToServerMessageType;
(function (ToServerMessageType) {
    ToServerMessageType[ToServerMessageType["Initialize"] = 0] = "Initialize";
})(ToServerMessageType || (ToServerMessageType = {}));
var ToClientMessageType;
(function (ToClientMessageType) {
    ToClientMessageType[ToClientMessageType["Initialize"] = 0] = "Initialize";
})(ToClientMessageType || (ToClientMessageType = {}));
wss.on('connection', function connection(ws) {
    console.log('New connection');
    const user = {
        uid: '',
    };
    ws.on('error', console.error);
    ws.on('message', function message(rawData) {
        const rawString = rawData.toString().trim();
        const jsonData = JSON.parse(rawString);
        const { type, uid, message } = jsonData;
        user.uid = uid;
        switch (type) {
            case ToServerMessageType.Initialize:
                console.log(`Initialize '${message}'`);
                break;
            default:
                console.error('Unknown message type', type);
                break;
        }
    });
});
