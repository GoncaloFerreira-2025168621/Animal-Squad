/*
    Animal Squad - Base de Dados
    Ficheiro: test_triggers.sql
    Objetivo: testar triggers da base de dados.

    Ordem aconselhada:
    1) create.sql
    2) logic.sql
    3) populate.sql
    4) test_triggers.sql
*/

USE animal_squad;

/* =========================
   TESTE 1: coins negativas no INSERT de users
   Esperado: coins fica 0
   ========================= */
INSERT INTO users (username, password, coins)
VALUES ('trigger_test', '1234', -50)
ON DUPLICATE KEY UPDATE coins = -50;

SELECT
    'TESTE 1 - coins negativas ficam a 0' AS test_name,
    username,
    coins
FROM users
WHERE username = 'trigger_test';

/* =========================
   TESTE 2: coins negativas no UPDATE de users
   Esperado: coins fica 0
   ========================= */
UPDATE users
SET coins = -100
WHERE username = 'trigger_test';

SELECT
    'TESTE 2 - update negativo fica a 0' AS test_name,
    username,
    coins
FROM users
WHERE username = 'trigger_test';

/* =========================
   TESTE 3: preco negativo no UPDATE de animals
   Esperado: price_coins fica 0
   ========================= */
UPDATE animals
SET price_coins = -20
WHERE name = 'Bird';

SELECT
    'TESTE 3 - preco negativo fica a 0' AS test_name,
    name,
    price_coins
FROM animals
WHERE name = 'Bird';

/* Restaurar preco do Bird */
UPDATE animals
SET price_coins = 0
WHERE name = 'Bird';

/* =========================
   TESTE 4: ao inserir uma missao concluida, o jogador recebe moedas
   Esperado: coins aumenta com reward_coins da missao 1
   ========================= */
SET @test_user_id = (SELECT id_user FROM users WHERE username = 'trigger_test' LIMIT 1);

UPDATE users
SET coins = 0
WHERE id_user = @test_user_id;

DELETE FROM user_progress
WHERE id_user = @test_user_id
  AND id_mission = 1;

INSERT INTO user_progress (id_user, id_mission, completed)
VALUES (@test_user_id, 1, 1);

SELECT
    'TESTE 4 - reward no INSERT de progresso completo' AS test_name,
    u.username,
    u.coins AS current_coins,
    ms.reward_coins AS expected_reward
FROM users u
INNER JOIN user_progress up ON u.id_user = up.id_user
INNER JOIN missions ms ON up.id_mission = ms.id_mission
WHERE u.id_user = @test_user_id
  AND up.id_mission = 1;

/* =========================
   TESTE 5: ao atualizar progresso de 0 para 1, o jogador recebe moedas
   Esperado: coins aumenta com reward_coins da missao 2
   ========================= */
UPDATE users
SET coins = 0
WHERE id_user = @test_user_id;

DELETE FROM user_progress
WHERE id_user = @test_user_id
  AND id_mission = 2;

INSERT INTO user_progress (id_user, id_mission, completed)
VALUES (@test_user_id, 2, 0);

UPDATE user_progress
SET completed = 1
WHERE id_user = @test_user_id
  AND id_mission = 2;

SELECT
    'TESTE 5 - reward no UPDATE de progresso 0 para 1' AS test_name,
    u.username,
    u.coins AS current_coins,
    ms.reward_coins AS expected_reward,
    up.completed,
    up.completion_date
FROM users u
INNER JOIN user_progress up ON u.id_user = up.id_user
INNER JOIN missions ms ON up.id_mission = ms.id_mission
WHERE u.id_user = @test_user_id
  AND up.id_mission = 2;

/* =========================
   TESTE 6: times_detected maior que 0 ativa detected automaticamente
   Esperado: detected = 1
   ========================= */
CALL sp_start_match(1, @trigger_match_id);
CALL sp_add_player_to_match(@trigger_match_id, @test_user_id, 1);

UPDATE match_players
SET times_detected = 3
WHERE id_match = @trigger_match_id
  AND id_user = @test_user_id;

SELECT
    'TESTE 6 - detected automatico' AS test_name,
    id_match,
    id_user,
    detected,
    times_detected
FROM match_players
WHERE id_match = @trigger_match_id
  AND id_user = @test_user_id;
