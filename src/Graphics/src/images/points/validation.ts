import { body } from 'express-validator';

export const validators = [
    body().exists(),
    body('points').exists().isInt().customSanitizer(val => parseInt(val, 10)).isInt(),
    body('position').exists().isInt().customSanitizer(val => parseInt(val, 10)).isInt(),
    body('nickname').exists().isString(),
    body('profilePicture').exists().isBase64(),
    body('backgroundColor').exists().isHexColor(),
    body('textBackground').exists().isHexColor()
];
