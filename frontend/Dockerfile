# Этап 1: сборка приложения
FROM node:18-alpine AS build

# Устанавливаем рабочую директорию
WORKDIR /app

# Копируем файл package.json и yarn.lock в контейнер
COPY package.json yarn.lock ./

# Устанавливаем зависимости
RUN yarn install

# Копируем весь проект в контейнер
COPY . .

# Собираем релизную версию приложения
RUN yarn build:release

# Этап 2: запуск приложения
FROM node:18-alpine

# Устанавливаем serve для раздачи статических файлов
RUN yarn global add serve

# Копируем собранные файлы в рабочую директорию
COPY --from=build /app/dist /app/dist

# Экспонируем порт 3000
EXPOSE 3000

# Запуск приложения
CMD ["serve", "-s", "/app/dist", "-l", "tcp://0.0.0.0:3000"]
