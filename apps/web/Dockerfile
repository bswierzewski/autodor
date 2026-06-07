# syntax=docker/dockerfile:1.7

# Stage 1: Build the frontend assets.
FROM node:26-alpine AS builder

ARG GIT_SHA
ARG VITE_CLERK_PUBLISHABLE_KEY

WORKDIR /app

# Install dependencies first to improve layer caching.
COPY apps/web/package.json apps/web/package-lock.json ./
RUN npm ci

COPY apps/web/ ./

# Expose build-time values through Vite's expected environment names.
ENV VITE_GIT_SHA=${GIT_SHA}
ENV VITE_CLERK_PUBLISHABLE_KEY=${VITE_CLERK_PUBLISHABLE_KEY}

RUN npm run build

# Stage 2: Serve the built app with nginx.
FROM nginx:1.29-alpine AS production

COPY apps/web/nginx.conf.template /etc/nginx/templates/default.conf.template
COPY --from=builder /app/dist /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
