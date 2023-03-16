import express from 'express';
import { ValidationChain, validationResult } from 'express-validator';

export const validate = (validations: ValidationChain[]) => {
    return async (request: express.Request, response: express.Response, next: express.NextFunction) => {
        await Promise.all(validations.map(o => o.run(request)));

        const errors = validationResult(request);
        if (errors.isEmpty()) {
            return next();
        }

        return response.status(400).json({ errors: errors.array() });
    };
};
