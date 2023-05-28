# Build phase
FROM node:18-alpine as build
WORKDIR /usr

RUN apk add build-base g++ cairo-dev jpeg-dev pango-dev giflib-dev
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

# Production phase
FROM node:18-alpine as production

# Variables
WORKDIR /usr/src/app
EXPOSE 3000
ENV NODE_ENV production
LABEL org.opencontainers.image.source https://github.com/grillbot/grillbot.services

# Dependencies
RUN apk add build-base g++ cairo-dev jpeg-dev pango-dev giflib-dev
RUN apk add terminus-font ttf-inconsolata ttf-dejavu font-noto font-noto-cjk ttf-font-awesome font-noto-extra

# Final build
RUN npm install pm2 -g
COPY package*.json ./
RUN npm ci --omit=dev
COPY --from=build /usr/dist .

CMD ["pm2-runtime", "index.js"]
