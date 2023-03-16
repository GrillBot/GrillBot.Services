import { CanvasRenderingContext2D, Image, createCanvas, loadImage } from 'canvas';
import express from 'express';

export class ResponseHelper {
    static writePngResponse(response: express.Response, data: Buffer): void {
        this.writeResponse(response, data, 200, 'image/png');
    }

    static writeResponse(response: express.Response, data: Buffer, statusCode: number, contentType: string): void {
        response.writeHead(statusCode, {
            'Content-Type': contentType,
            'Content-Length': data.length
        }).end(data);
    }
}

export class FormatHelper {
    static formatNumber(value: number): string {
        return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ' ');
    }
}

export class CanvasHelper {
    static async createCircleImage(image: Image): Promise<Image> {
        const canvas = createCanvas(image.width, image.height);
        const context = canvas.getContext('2d');

        this.setAntialias(context);
        context.save();
        context.beginPath();
        context.arc(image.width / 2, image.height / 2, image.width / 2, 0, Math.PI * 2, false);
        context.clip();
        context.drawImage(image, 0, 0);
        context.restore();

        return await loadImage(canvas.toBuffer());
    }

    static setAntialias(context: CanvasRenderingContext2D): void {
        context.globalCompositeOperation = 'source-over';
        context.antialias = 'subpixel';
        context.imageSmoothingEnabled = true;
    }
}
