# Build the production-ready SPA bundle.
FROM node:22-alpine AS build

ARG VITE_CLERK_PUBLISHABLE_KEY
ARG VITE_APP_GIT_SHA=local

WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci

COPY . ./

ENV VITE_CLERK_PUBLISHABLE_KEY=${VITE_CLERK_PUBLISHABLE_KEY}
ENV VITE_APP_GIT_SHA=${VITE_APP_GIT_SHA}

RUN npm run build

# Serve the built SPA with nginx.
FROM nginx:alpine AS final

COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
