import express from 'express';
import { renderLineChart } from './line-chart';
import { RequestData } from './request';

export const onRequest = async (request: express.Request, response: express.Response) => {
    const body: RequestData = request.body;

    switch (body.options.type) {
        case 'line':
            const image = await renderLineChart(body);
            response.writeHead(200, {
                'Content-Type': 'image/png',
                'Content-Length': image.length
            });
            response.end(image);
            break;
        default:
            throw new Error(`Unsupported chart type (${body.options.type})`);
    }
};
