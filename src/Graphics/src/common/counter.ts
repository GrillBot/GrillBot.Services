import express from 'express';

interface RequestStatistics {
    endpoint: string;
    count: number;
    lastRequestAt: string;
    totalTime: number;
    avgTime: number;
    lastTime: number;
}

interface Stats {
    requestsCount: number;
    measuredFrom: string | null;
    endpoints: RequestStatistics[];
    cpuTime: number;
    database: null
}

const stats: Stats = {
    requestsCount: 0,
    measuredFrom: null,
    endpoints: [],
    cpuTime: 0,
    database: null
};

export const requestsCounter = (request: express.Request, response: express.Response, next: express.NextFunction): void => {
    stats.requestsCount++;

    if (!stats.measuredFrom) {
        stats.measuredFrom = new Date().toISOString();
    }

    const url = `${request.method} ${request.url}`;
    const endpoint = stats.endpoints.find(o => o.endpoint === url);
    if (!endpoint) {
        stats.endpoints.push({
            count: 1,
            lastRequestAt: new Date().toISOString(),
            endpoint: url,
            totalTime: 0,
            avgTime: 0,
            lastTime: 0
        });
    } else {
        endpoint.count++;
        endpoint.lastRequestAt = new Date().toISOString();
    }

    const cpuUsage = process.cpuUsage();
    stats.cpuTime = Math.round((cpuUsage.system + cpuUsage.user) / 1000);

    next();
};

export const statsEndpoint = (request: express.Request, response: express.Response): void => {
    response.status(200).json(stats);
};

export const durationCounter = (req: express.Request, res: express.Response, next: express.NextFunction) => {
    const start = process.hrtime();

    res.on('finish', () => {
        const end = process.hrtime(start);
        const duration = Math.round((end[0] * 1e9 + end[1]) / 1e6);

        const endpoint = stats.endpoints.find(o => o.endpoint === `${req.method} ${req.url}`);
        if (!endpoint) { return; }
        endpoint.totalTime += duration;
        endpoint.lastTime = duration;
        endpoint.avgTime = Math.round(endpoint.totalTime / endpoint.count);
    });

    next();
};
