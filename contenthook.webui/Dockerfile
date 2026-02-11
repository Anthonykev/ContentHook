FROM node:20-alpine

WORKDIR /app

# Dependencies zuerst für Docker-Cache
COPY package*.json ./
RUN npm install

# App-Code
COPY . .

# Vite Standard-Port
EXPOSE 5173

# Wichtig: --host, sonst ist es nur im Container erreichbar
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0", "--port", "5173"]
