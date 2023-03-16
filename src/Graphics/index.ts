import { errorHandler } from './src/common/error';
import express from 'express';
import { loggerMiddleware } from './src/logging';
import actuator from 'express-actuator';
import * as chart from './src/chart';
import * as common from './src/common';
import * as image from './src/images';

const app = express();
app.use(express.json({ limit: '100mb' }))
app.use(loggerMiddleware);
app.use(actuator({}));
app.use(common.requestsCounter);
app.use(common.durationCounter);

app.post('/chart', common.validate(chart.validators), (req, res, next) => new common.RequestProcessing(req, res, next).execute(chart.onRequest));
app.get('/stats', common.statsEndpoint);
app.post('/image/without-accident', common.validate(image.withoutAccident.validators), (req, res, next) => new common.RequestProcessing(req, res, next).execute(image.withoutAccident.onRequest));
app.post('/image/points', common.validate(image.points.validators), (req, res, next) => new common.RequestProcessing(req, res, next).execute(image.points.onRequest));
app.post('/image/peepo/:method', common.validate(image.peepo.validators), (req, res, next) => new common.RequestProcessing(req, res, next).execute(image.peepo.onRequest));
app.use(errorHandler);

app.listen(3000, () => console.log('App started on port 3000'));
