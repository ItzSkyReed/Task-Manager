# Сборка приложения
FROM node:24-alpine AS build
WORKDIR /app

# Сначала копируем только файлы зависимостей для кэширования слоя npm ci
COPY package.json package-lock.json ./
RUN npm ci

# Копируем исходный код и собираем production
COPY . .
RUN npm run build

# Раздача и проксирование API через Nginx
FROM nginx:alpine AS final

# Удаляем дефолтный конфиг и даем свой
RUN rm /etc/nginx/conf.d/default.conf
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Копируем собранный фронтенд
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
