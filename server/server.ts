import { WebSocketServer } from 'ws';
import dotenv from 'dotenv';
import OpenAI from 'openai';
import { randomUUID } from 'crypto';

dotenv.config();

const openai = new OpenAI({});

const wss = new WebSocketServer({ port: 8080 });

type User = {
    uid: string;
    resumeContext?: string;
};

enum ToServerMessageType {
    Initialize = 0,
}

enum ToClientMessageType {
    Initialize = 0,
}

type Message = {
    type: number;
    uid: string;
    message: string;
};

wss.on('connection', function connection(ws) {
    console.log('New connection');

    const user: User = {
        uid: '',
    };

    ws.on('error', console.error);

    ws.on('message', function message(rawData) {
        const rawString = rawData.toString().trim();
        const jsonData: Message = JSON.parse(rawString) as Message;
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
