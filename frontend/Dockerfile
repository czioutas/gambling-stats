FROM node:20-alpine AS base

# Install dependencies only when needed
FROM base AS deps
WORKDIR /app

# Copy package files from root
COPY frontend/package.json frontend/package-lock.json* ./

# Install dependencies
RUN npm ci

# Rebuild the source code only when needed
FROM base AS builder
WORKDIR /app

# Pass the environment variable to the builder stage
ARG PUBLIC_API_URL
ENV PUBLIC_API_URL=${PUBLIC_API_URL}

# Copy package files and dependencies from deps stage
COPY --from=deps /app/package.json ./
COPY --from=deps /app/package-lock.json* ./
COPY --from=deps /app/node_modules ./node_modules

# Copy source code and config files
COPY frontend/src/ ./src/
COPY frontend/public/ ./public/
COPY frontend/astro.config.mjs ./
COPY frontend/tailwind.config.mjs ./
COPY frontend/tsconfig.json ./

# Build the project
RUN npm run build

# Production image, copy all the files and run the app
FROM base AS runner
WORKDIR /app

# Install serve globally before switching to non-root user
RUN npm install -g serve

# Don't run production as root
RUN addgroup --system --gid 1001 nodejs
RUN adduser --system --uid 1001 astro

# Copy only necessary files
COPY --from=builder /app/dist ./dist

# Switch to non-root user
USER astro

# Expose the port the app will run on
EXPOSE 4321

# Start the application
CMD ["serve", "dist", "-p", "4321"]