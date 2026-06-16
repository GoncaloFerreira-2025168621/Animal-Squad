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
    // Vai buscar o userID que vem no URL
    // Exemplo: /shop/3
    const userID = req.params.userID;

    // Primeiro vamos buscar as moedas do utilizador
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

        // Se não encontrar nenhum utilizador com esse id
        if (userResult.length === 0) {
            return res.json({
                success: false,
                message: "Utilizador não encontrado"
            });
        }

        const coins = userResult[0].coins;

        // Agora vamos buscar todos os animais.
        // O LEFT JOIN serve para saber se o user já comprou cada animal ou não.
        const animalsQuery = `
            SELECT 
                a.id_animal,
                a.name,
                a.description,
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

            // Resposta que vai para o Unity
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

//Responsavel pela compra na loja
exports.buyAnimal = (req, res) => {
    // O Unity vai enviar isto:
    // {
    //   "userID": 1,
    //   "animalID": 2
    // }

    const { userID, animalID } = req.body;

    if (!userID || !animalID) {
        return res.json({
            success: false,
            message: "Dados inválidos"
        });
    }

    // Usamos transaction porque a compra faz 2 coisas:
    // 1. Retira moedas ao jogador
    // 2. Guarda o animal comprado
    // Se uma falhar, a outra também é cancelada.
    db.beginTransaction((err) => {
        if (err) {
            console.log(err);
            return res.json({
                success: false,
                message: "Erro ao iniciar compra"
            });
        }

        // Verifica se o user já tem este animal
        const checkOwnedQuery = `
            SELECT id_user_animal
            FROM user_animals
            WHERE id_user = ? AND id_animal = ?
        `;

        db.query(checkOwnedQuery, [userID, animalID], (err, ownedResult) => {
            if (err) {
                return db.rollback(() => {
                    console.log(err);
                    res.json({
                        success: false,
                        message: "Erro ao verificar animal comprado"
                    });
                });
            }

            if (ownedResult.length > 0) {
                return db.rollback(() => {
                    res.json({
                        success: false,
                        message: "Já compraste este animal"
                    });
                });
            }

            // Buscar moedas do user e preço do animal
            const dataQuery = `
                SELECT 
                    u.coins,
                    a.price_coins
                FROM users u
                JOIN animals a
                WHERE u.id_user = ?
                AND a.id_animal = ?
            `;

            db.query(dataQuery, [userID, animalID], (err, dataResult) => {
                if (err) {
                    return db.rollback(() => {
                        console.log(err);
                        res.json({
                            success: false,
                            message: "Erro ao buscar dados da compra"
                        });
                    });
                }

                if (dataResult.length === 0) {
                    return db.rollback(() => {
                        res.json({
                            success: false,
                            message: "Utilizador ou animal inválido"
                        });
                    });
                }

                const coins = dataResult[0].coins;
                const price = dataResult[0].price_coins;

                // Verifica se tem moedas suficientes
                if (coins < price) {
                    return db.rollback(() => {
                        res.json({
                            success: false,
                            message: "Moedas insuficientes"
                        });
                    });
                }

                // Retirar moedas ao user
                const updateCoinsQuery = `
                    UPDATE users
                    SET coins = coins - ?
                    WHERE id_user = ?
                `;

                db.query(updateCoinsQuery, [price, userID], (err) => {
                    if (err) {
                        return db.rollback(() => {
                            console.log(err);
                            res.json({
                                success: false,
                                message: "Erro ao retirar moedas"
                            });
                        });
                    }

                    // Guardar animal comprado
                    const insertAnimalQuery = `
                        INSERT INTO user_animals (id_user, id_animal)
                        VALUES (?, ?)
                    `;

                    db.query(insertAnimalQuery, [userID, animalID], (err) => {
                        if (err) {
                            return db.rollback(() => {
                                console.log(err);
                                res.json({
                                    success: false,
                                    message: "Erro ao guardar animal comprado"
                                });
                            });
                        }

                        // Se tudo correu bem, confirma a compra
                        db.commit((err) => {
                            if (err) {
                                return db.rollback(() => {
                                    console.log(err);
                                    res.json({
                                        success: false,
                                        message: "Erro ao finalizar compra"
                                    });
                                });
                            }

                            res.json({
                                success: true,
                                message: "Animal comprado com sucesso",
                                newCoins: coins - price
                            });
                        });
                    });
                });
            });
        });
    });
};