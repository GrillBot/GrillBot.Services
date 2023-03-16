import express from 'express';

export class RequestProcessing {
    constructor(
        private request: express.Request,
        private response: express.Response,
        private next: express.NextFunction
    ) { }

    async execute(action: (request: express.Request, response: express.Response) => Promise<void>): Promise<void> {
        try {
            await action(this.request, this.response);
        } catch (e) {
            this.next(e);
        }
    }
}
