# Use an official Node.js runtime as a parent image
FROM node:23-alpine AS build

# Set the working directory
WORKDIR /app

# Copy package.json and package-lock.json
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of the application code
COPY . .

# Build the Angular application
RUN npm run build --prod

# Use an Nginx image to serve the Angular app
FROM nginx:alpine

# Copy the built Angular app to the Nginx HTML directory
COPY --from=build /app/dist/tsa-submissions-coding-ui/browser /usr/share/nginx/html

# Expose port 80
EXPOSE 80

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]
