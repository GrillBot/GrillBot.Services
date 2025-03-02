import { errorHandler } from './src/common/error';
import express from 'express';
import { loggerMiddleware } from './src/logging';
import actuator from 'express-actuator';
import * as chart from './src/chart';
import * as common from './src/common';
import { collectDefaultMetrics, register } from 'prom-client';

collectDefaultMetrics({});

const app = express();
app.use((_, res, next) => {
    res.removeHeader('server');
    res.removeHeader('X-Powered-By')
    res.setHeader('X-Frame-Options', 'SAMEORIGIN');
    res.setHeader('X-Xss-Protection', '1; mode=block');
    res.setHeader('X-Content-Type-Options', 'nosniff');

    next();
});
app.use(express.json({ limit: '100mb' }))
app.use(loggerMiddleware);
app.use(actuator({}));
app.use(common.requestsCounter);
app.use(common.durationCounter);

app.post('/chart', common.validate(chart.validators), (req, res, next) => new common.RequestProcessing(req, res, next).execute(chart.onRequest));
app.get('/stats', common.statsEndpoint);
app.get('/api/diag/uptime', common.uptimeEndpoint);
app.get('/prom_metrics', async (_, res) => {
    const metrics = await register.metrics();
    res.status(200).contentType(register.contentType).end(metrics);
});
app.use(errorHandler);

app.listen(3000, () => console.log('App started on port 3000'));
