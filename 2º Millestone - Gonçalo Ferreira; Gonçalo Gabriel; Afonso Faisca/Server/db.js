// Importa a biblioteca mysql2
// Esta biblioteca permite ao Node.js comunicar com o MySQL
const mysql = require("mysql2");


// Cria a ligação à base de dados
const db = mysql.createConnection({

    // Servidor onde o MySQL está a correr
    // localhost = o teu próprio PC
    host: "localhost",

    // Utilizador do MySQL
    user: "root",

    // Password do utilizador
    // Neste caso está vazia
    password: "123456789",

    // Nome da base de dados criada no MySQL Workbench
    database: "animal_squad"
});


// Tenta ligar à base de dados
db.connect((err) => {

    // Se existir erro na ligação
    if (err) {

        console.log("Erro ao ligar à base de dados:");
        console.log(err);

    } else {

        // Ligação feita com sucesso
        console.log("MySQL ligado!");
    }
});


// Exporta a ligação db
// Assim outros ficheiros podem usar esta ligação
module.exports = db;