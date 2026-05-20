// Importa a ligação à base de dados
const db = require("./db");


//REGISTER
//Responsável por criar novas contas

exports.register = (req, res) => {

    // Dados enviados pela Unity
    const { username, password } = req.body;

    // Verifica se os campos estão vazios
    if (!username || !password) {

        return res.json({
            success: false,
            message: "Preenche todos os campos"
        });
    }

    // Verifica se o username já existe
    db.query(
        "SELECT * FROM users WHERE username = ?",
        [username],

        (err, result) => {

            if (err) {

                return res.json({
                    success: false,
                    message: "Erro na base de dados"
                });
            }

            // Username já existe
            if (result.length > 0) {

                return res.json({
                    success: false,
                    message: "Username já existe"
                });
            }

            // Cria novo utilizador
            db.query(
                "INSERT INTO users (username, password) VALUES (?, ?)",
                [username, password],

                (err2) => {

                    if (err2) {

                        return res.json({
                            success: false,
                            message: "Erro ao criar conta"
                        });
                    }

                    // Conta criada
                    res.json({
                        success: true,
                        message: "Conta criada com sucesso"
                    });
                }
            );
        }
    );
};



//LOGIN 
//Responsável por verificar login

exports.login = (req, res) => {

    // Dados recebidos da Unity
    const { username, password } = req.body;

    // Procura utilizador na base de dados
    db.query(
        "SELECT * FROM users WHERE username = ? AND password = ?",
        [username, password],

        (err, result) => {

            if (err) {

                return res.json({
                    success: false,
                    message: "Erro na base de dados"
                });
            }

            // Login incorreto
            if (result.length === 0) {

                return res.json({
                    success: false,
                    message: "Username ou password incorreta"
                });
            }

            // Utilizador encontrado
            const user = result[0];

            // Login correto
            res.json({
                success: true,
                message: "Login efetuado",
                userID: user.id_user,
                username: user.username
            });
        }
    );
};