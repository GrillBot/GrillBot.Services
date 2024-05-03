import express from 'express';
import { PeepoImageBase } from './api/peepo-base';
import { PeepoAngry } from './api/peepoangry';

export const onRequest = async (request: express.Request, response: express.Response) => {
    let renderer: PeepoImageBase | null = null;
    switch (request.params.method) {
        case 'angry':
            renderer = new PeepoAngry(request.body);
            break;
    }

    if (!renderer) {
        response.status(400).json({
            errors: [{
                msg: 'Unsupported method',
                param: 'method',
                location: 'path'
            }]
        });
        return;
    }

    const result = await renderer.render();
    const frames = result.map(o => o.toString('base64'));
    response.status(200).json(frames);
};
