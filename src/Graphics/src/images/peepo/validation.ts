import { body } from "express-validator";

export const validators = [body().exists().isArray()];
