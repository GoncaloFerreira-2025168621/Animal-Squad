// Importa o express (servidor)
const express = require("express");

// Permite comunicação entre Unity e Node.js
const cors = require("cors");

// Importa as funções do request-handlers.js
const handlers = require("./request-handlers");

// Cria o servidor
const app = express();

// Permite requests externas
app.use(cors());

// Permite receber JSON da Unity
app.use(express.json());



//ROUTES

// Route de registo
app.post("/register", handlers.register);

// Route de login
app.post("/login", handlers.login);

// Rotas do shop
app.get("/shop/:userID", handlers.getShop);
app.post("/shop/buy", handlers.buyAnimal);

// Route para apanhar moedas
app.post("/coins/collect", handlers.collectCoin);


//SERVER 

// Inicia servidor na porta 3000
app.listen(3000, () => {

    console.log("Servidor online na porta 3000");
});