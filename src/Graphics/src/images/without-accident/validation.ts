import { body } from 'express-validator';

export const validators = [
    body().exists(),
    body('days').exists().isInt().customSanitizer(val => parseInt(val, 10)).isInt(),
    body('profilePicture').exists().isString().isBase64()
];
