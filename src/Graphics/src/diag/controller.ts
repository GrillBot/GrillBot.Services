import express from 'express';
import { stats } from '../common';

export const onRequest = (request: express.Request, response: express.Response) => {
    response.status(200).json({
        usedMemory: process.memoryUsage().rss,
        uptime: process.uptime(),
        databaseStatistics: null,
        operations: [],
        ...stats
    });
}
