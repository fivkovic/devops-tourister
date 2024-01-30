FROM node:slim

WORKDIR /src

COPY . .

RUN npm i

EXPOSE 3000

ENTRYPOINT [ "npm", "run", "dev"]