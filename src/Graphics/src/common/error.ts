import express from 'express';

export const errorHandler = (err: any, request: express.Request, response: express.Response, next: express.NextFunction) => {
    const problemDetails = createProblemDetails(err);

    console.error(err);
    response.status(getStatusCode(err)).json(problemDetails);
};

const createProblemDetails = (err: any) => {
    return {
        title: createTitle(err),
        detail: createDetail(err)
    };
}

const createTitle = (err: any) => {
    if (err instanceof Error) {
        return err.name;
    } else if (typeof err === 'string') {
        return err;
    } else {
        return 'An error occured';
    }
};

const createDetail = (err: any) => err instanceof Error ? err.message : null;
const getStatusCode = (err: any) => err.statusCode ? err.statusCode : 500;
