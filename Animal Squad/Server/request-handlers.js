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

//Shop
//Responsavel pela loja

exports.getShop = (req, res) => {
    const userID = req.params.userID;

    const userQuery = `
        SELECT coins
        FROM users
        WHERE id_user = ?
    `;

    db.query(userQuery, [userID], (err, userResult) => {
        if (err) {
            console.log(err);
            return res.json({
                success: false,
                message: "Erro ao buscar moedas do utilizador"
            });
        }

        if (userResult.length === 0) {
            return res.json({
                success: false,
                message: "Utilizador não encontrado"
            });
        }

        const coins = userResult[0].coins;

        const animalsQuery = `
            SELECT 
                a.id_animal,
                a.name,
                a.description,
                a.ability1,
                a.ability2,
                a.speed,
                a.price_coins,
                CASE 
                    WHEN ua.id_user IS NULL THEN 0 
                    ELSE 1 
                END AS owned
            FROM animals a
            LEFT JOIN user_animals ua
                ON ua.id_animal = a.id_animal
                AND ua.id_user = ?
            ORDER BY a.id_animal
        `;

        db.query(animalsQuery, [userID], (err, animalsResult) => {
            if (err) {
                console.log(err);
                return res.json({
                    success: false,
                    message: "Erro ao buscar animais"
                });
            }

            res.json({
                success: true,
                message: "Shop carregado com sucesso",
                userID: parseInt(userID),
                coins: coins,
                animals: animalsResult
            });
        });
    });
};