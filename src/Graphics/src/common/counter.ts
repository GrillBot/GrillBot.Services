import express from 'express';

interface RequestStatistics {
    endpoint: string;
    lastRequestAt: string;
    totalTime: number;
    lastTime: number;
    successCount: number;
    failedCount: number;
}

interface Stats {
    requestsCount: number;
    measuredFrom: string | null;
    endpoints: RequestStatistics[];
    cpuTime: number;
}

const stats: Stats = {
    requestsCount: 0,
    measuredFrom: null,
    endpoints: [],
    cpuTime: 0
};

export const requestsCounter = (request: express.Request, response: express.Response, next: express.NextFunction): void => {
    stats.requestsCount++;

    if (!stats.measuredFrom) {
        stats.measuredFrom = new Date().toISOString();
    }

    const url = `${request.method} ${request.url}`;
    if (!stats.endpoints.some(o => o.endpoint === url)) {
        const isSuccess = response.statusCode < 400;

        stats.endpoints.push({
            lastRequestAt: new Date().toISOString(),
            endpoint: url,
            totalTime: 0,
            lastTime: 0,
            failedCount: isSuccess ? 0 : 1,
            successCount: isSuccess ? 1 : 0
        });
    }

    const cpuUsage = process.cpuUsage();
    stats.cpuTime = Math.round((cpuUsage.system + cpuUsage.user) / 1000);

    next();
};

export const statsEndpoint = (_: express.Request, response: express.Response): void => {
    response.status(200).json(stats);
};

export const uptimeEndpoint = (_: express.Request, response: express.Response): void => {
    const uptime = Math.floor(process.uptime() * 1000);
    response.status(200).json(uptime);
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
        endpoint.lastRequestAt = new Date().toISOString();

        if (res.statusCode < 400) {
            endpoint.successCount++;
        } else {
            endpoint.failedCount++;
        }
    });

    next();
};
