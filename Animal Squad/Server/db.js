const mysql = require("mysql2");

const db = mysql.createConnection({
    host: "localhost",
    user: "root",
    password: "",
    database: "animal_squad"
});

db.connect((err) => {
    if (err) {
        console.log("Erro ao ligar à base de dados:");
        console.log(err);
    } else {
        console.log("MySQL ligado!");
    }
});

module.exports = db;