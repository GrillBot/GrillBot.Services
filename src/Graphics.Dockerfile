ARG NODE_VERSION="22-alpine"

# Build phase
FROM node:${NODE_VERSION} AS build
WORKDIR /usr

RUN apk add build-base g++ cairo-dev jpeg-dev pango-dev giflib-dev
COPY Graphics/package*.json ./
RUN npm ci
COPY Graphics/ .
RUN npm run build

# Production phase
FROM node:${NODE_VERSION} AS production

# Variables
WORKDIR /usr/src/app
EXPOSE 3000
ENV NODE_ENV=production
LABEL org.opencontainers.image.source=https://github.com/grillbot/grillbot.services

# Dependencies
RUN apk add build-base g++ cairo-dev jpeg-dev pango-dev giflib-dev
RUN apk add terminus-font ttf-inconsolata ttf-dejavu font-noto font-noto-cjk ttf-font-awesome font-noto-extra

# Final build
COPY Graphics/package*.json ./
RUN npm ci --omit=dev
COPY --from=build /usr/dist .

CMD ["node", "index.js"]
