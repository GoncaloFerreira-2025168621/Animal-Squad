/*
    Animal Squad - Base de Dados
    Ficheiro: populate.sql
    Objetivo: inserir dados iniciais para demonstrar e testar o jogo.
*/

USE animal_squad;

/* =========================
   Utilizadores de teste
   password = texto de teste. No jogo real deve ser guardada com hash.
   ========================= */
INSERT INTO users (username, password, coins)
VALUES
    ('Goncalo', '1234', 500),
    ('Afonso', '1234', 350),
    ('TestePlayer', '1234', 200)
ON DUPLICATE KEY UPDATE
    password = VALUES(password),
    coins = VALUES(coins);

/* =========================
   Animais
   ========================= */
INSERT INTO animals (name, description, ability1, ability2, speed, captured, price_coins)
VALUES
    ('Bird', 'Animal capaz de voar e transportar pequenos objetos.', 'Fly', 'Carry objects', 8, 0, 0),
    ('Beaver', 'Animal capaz de nadar e destruir madeira, como barragens.', 'Swim', 'Break wood', 5, 0, 100),
    ('Bear', 'Animal forte capaz de empurrar objetos pesados e escalar pequenas estruturas.', 'Climb', 'Push objects', 4, 0, 150),
    ('Mouse', 'Animal pequeno capaz de entrar em buracos e roer fios.', 'Small spaces', 'Chew wires', 7, 0, 100)
ON DUPLICATE KEY UPDATE
    description = VALUES(description),
    ability1 = VALUES(ability1),
    ability2 = VALUES(ability2),
    speed = VALUES(speed),
    captured = VALUES(captured),
    price_coins = VALUES(price_coins);

/* =========================
   Mapas
   ========================= */
INSERT INTO maps (map_name, difficulty, description)
VALUES
    ('Forest', 'Easy', 'Primeiro mapa. Floresta de Borneo invadida por lenhadores.'),
    ('House', 'Medium', 'Zona da casa humana com cameras e vigilancia.'),
    ('Factory', 'Hard', 'Zona industrial com maquinas e puzzles mais complexos.'),
    ('Farm', 'Medium', 'Zona rural com obstaculos e estruturas humanas.')
ON DUPLICATE KEY UPDATE
    difficulty = VALUES(difficulty),
    description = VALUES(description);

/* =========================
   Missoes do mapa Forest
   ========================= */
INSERT INTO missions (id_map, mission_name, description, reward_coins, order_index)
VALUES
    (1, 'Stop the Fire', 'Apagar o incendio usando a agua da barragem ou outras solucoes dos animais.', 50, 1),
    (1, 'Sabotage the Lumber Camp', 'Desativar o centro dos lenhadores destruindo fios, gerador e deposito.', 75, 2),
    (1, 'Free the Animals', 'Libertar animais presos em jaulas espalhadas pelo mapa.', 100, 3),
    (1, 'Close the Forest Entrance', 'Bloquear a entrada dos lenhadores para a floresta.', 125, 4),
    (2, 'Escape the House', 'Completar puzzles dentro da casa sem ser visto.', 150, 1),
    (3, 'Factory Shutdown', 'Desativar a fabrica principal dos humanos.', 200, 1),
    (4, 'Farm Rescue', 'Ajudar animais presos na zona da quinta.', 150, 1)
ON DUPLICATE KEY UPDATE
    description = VALUES(description),
    reward_coins = VALUES(reward_coins);

/* =========================
   Animais iniciais dos utilizadores
   ========================= */
INSERT IGNORE INTO user_animals (id_user, id_animal)
VALUES
    (1, 1), -- Goncalo tem Bird
    (1, 2), -- Goncalo tem Beaver
    (2, 1), -- Afonso tem Bird
    (2, 4), -- Afonso tem Mouse
    (3, 1); -- TestePlayer tem Bird

/* =========================
   Progresso inicial
   completed = 0 nao da reward.
   completed = 1 da reward automaticamente atraves do trigger.
   ========================= */
INSERT INTO user_progress (id_user, id_mission, completed)
VALUES
    (1, 1, 1),
    (1, 2, 0),
    (2, 1, 1),
    (3, 1, 0)
ON DUPLICATE KEY UPDATE
    completed = VALUES(completed);

/* =========================
   Partida de exemplo
   ========================= */
INSERT INTO matches (id_map, start_time, end_time, status)
VALUES
    (1, NOW(), DATE_ADD(NOW(), INTERVAL 15 MINUTE), 'completed');

SET @example_match_id = LAST_INSERT_ID();

INSERT INTO match_players (id_match, id_user, id_animal, detected, times_detected, joined_at, left_at)
VALUES
    (@example_match_id, 1, 2, 0, 0, NOW(), DATE_ADD(NOW(), INTERVAL 15 MINUTE)),
    (@example_match_id, 2, 4, 1, 2, NOW(), DATE_ADD(NOW(), INTERVAL 15 MINUTE));
