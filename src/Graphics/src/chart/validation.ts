import { body } from 'express-validator';

export const validators = [
    body('options').exists().isObject(),
    body('options.width').exists().isInt().customSanitizer(val => parseInt(val, 10)),
    body('options.height').exists().isInt().customSanitizer(val => parseInt(val, 10)),
    body('options.backgroundColor').default('black').isString(),
    body('options.type').isString().exists().contains('line'),
    body('options.legendPosition').optional().isString(),
    body('options.pointRadius').optional().isInt().customSanitizer(val => parseInt(val, 10)),

    body('data.topLabel').optional().isObject(),
    body('data.topLabel.text').optional().isString(),
    body('data.topLabel.align').optional().isString(),
    body('data.topLabel.size').optional().isInt().customSanitizer(val => parseInt(val, 10)),
    body('data.topLabel.color').optional().isString(),
    body('data.topLabel.weight').optional().isString(),

    body('data.datasets').exists().isArray(),
    body('data.datasets.*.label').exists().isString(),
    body('data.datasets.*.data').exists().isArray(),
    body('data.datasets.*.data.*.label').exists().isString(),
    body('data.datasets.*.data.*.value').optional().customSanitizer(val => val != null ? parseInt(val, 10) : null),
    body('data.datasets.*.color').optional().isString(),
    body('data.datasets.*.width').optional().isInt().customSanitizer(val => parseInt(val, 10))
];
