import { createCanvas, loadImage, Image, CanvasRenderingContext2D } from 'canvas';
import express from 'express';
import { RequestData } from './request';
import { CanvasHelper, FormatHelper, ResponseHelper } from '../../common/helper';
import Color from 'color'
import * as fs from 'fs';
import * as path from 'path';

const size = { width: 1000, height: 340 };
const pfpSize = { width: 250, height: 250 };
const border: number = 25;
const radius: number = 20;
const trophy = fs.readFileSync(path.join(path.join(__dirname, 'templates'), 'trophy.png'));

export const onRequest = async (request: express.Request, response: express.Response) => {
    const body: RequestData = request.body;
    const canvas = createCanvas(size.width, size.height);
    const context = canvas.getContext('2d');
    const profilePicture = await loadImage(Buffer.from(body.profilePicture, 'base64'));
    const trophyImg = body.position === 1 ? await loadImage(trophy) : null;

    CanvasHelper.setAntialias(context);
    drawBackground(context, body.backgroundColor, false);
    drawBackground(context, body.textBackground, true);
    drawNickname(context, body.nickname);
    drawPoints(context, body.points, body.position, trophyImg);
    await drawProfilePicture(context, profilePicture);

    const image = canvas.toBuffer();
    ResponseHelper.writePngResponse(response, image);
};

const drawBackground = (context: CanvasRenderingContext2D, background: string, isInner: boolean): void => {
    const pos = isInner ? border : 0;
    const width = size.width - (isInner ? border * 2 : 0);
    const height = size.height - (isInner ? border * 2 : 0);

    context.fillStyle = Color(background).hexa();
    context.beginPath();
    context.roundRect(pos, pos, width, height, radius);
    context.fill();
}

const drawNickname = (context: CanvasRenderingContext2D, nickname: string): void => {
    context.font = '80px Arial';
    context.fillStyle = 'white';

    const textSize = context.measureText(nickname);
    const xPos = (border * 3) + pfpSize.width;
    const yPos = textSize.actualBoundingBoxAscent + (border * 2.5);
    const maxWidth = size.width - (border * 5) - pfpSize.width;

    context.fillText(cutToWidth(nickname, maxWidth, context), xPos, yPos);
}

const cutToWidth = (text: string, maxWidth: number, context: CanvasRenderingContext2D): string => {
    if (context.measureText(text).width <= maxWidth) { return text; }
    let result = '';

    for (let i = 0; i < text.length; i++) {
        if (context.measureText(result + text[i]).width >= maxWidth) break;
        result += text[i];
    }

    return result;
}

const drawPoints = (context: CanvasRenderingContext2D, points: number, position: number, trophy: Image | null): void => {
    context.font = '60px Arial';
    context.fillStyle = 'white';

    const text = `${position}. místo\n${formatPoints(points)}`;
    let xPos = (border * 3) + pfpSize.width;

    if (trophy) {
        context.drawImage(trophy, xPos, 160);
        xPos += trophy.width + 10;
    }

    context.fillText(text, xPos, 205);
};

const formatPoints = (points: number): string => {
    const pts = FormatHelper.formatNumber(points);

    if (points === 1) return '1 bod';
    else if (points > 1 && points < 5) { return `${pts} body`; }
    else return `${pts} bodů`;
}

const drawProfilePicture = async (context: CanvasRenderingContext2D, profilePicture: Image): Promise<void> => {
    const circleAvatar = await CanvasHelper.createCircleImage(profilePicture);
    context.drawImage(circleAvatar, border * 2, border * 2, pfpSize.width, pfpSize.width);
}
