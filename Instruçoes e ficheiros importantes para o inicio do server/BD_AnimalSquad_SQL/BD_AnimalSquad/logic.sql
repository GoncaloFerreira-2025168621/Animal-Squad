/*
    Animal Squad - Base de Dados
    Ficheiro: logic.sql
    Objetivo: criar views, functions, procedures e triggers.
*/

USE animal_squad;

/* =========================================================
   LIMPAR LOGICA ANTIGA, CASO O SCRIPT SEJA CORRIDO DE NOVO
   ========================================================= */
DROP VIEW IF EXISTS vw_user_animals;
DROP VIEW IF EXISTS vw_user_progress;
DROP VIEW IF EXISTS vw_match_details;
DROP VIEW IF EXISTS vw_user_statistics;

DROP FUNCTION IF EXISTS fn_user_has_animal;
DROP FUNCTION IF EXISTS fn_user_completed_missions;

DROP PROCEDURE IF EXISTS sp_buy_animal;
DROP PROCEDURE IF EXISTS sp_complete_mission;
DROP PROCEDURE IF EXISTS sp_start_match;
DROP PROCEDURE IF EXISTS sp_add_player_to_match;
DROP PROCEDURE IF EXISTS sp_finish_match;

DROP TRIGGER IF EXISTS trg_users_no_negative_coins_insert;
DROP TRIGGER IF EXISTS trg_users_no_negative_coins_update;
DROP TRIGGER IF EXISTS trg_animals_no_negative_price_insert;
DROP TRIGGER IF EXISTS trg_animals_no_negative_price_update;
DROP TRIGGER IF EXISTS trg_user_progress_set_completion_date_insert;
DROP TRIGGER IF EXISTS trg_user_progress_set_completion_date_update;
DROP TRIGGER IF EXISTS trg_user_progress_reward_insert;
DROP TRIGGER IF EXISTS trg_user_progress_reward_update;
DROP TRIGGER IF EXISTS trg_match_players_detected_insert;
DROP TRIGGER IF EXISTS trg_match_players_detected_update;

/* =========================
   VIEWS
   ========================= */

CREATE VIEW vw_user_animals AS
SELECT
    ua.id_user_animal,
    u.id_user,
    u.username,
    a.id_animal,
    a.name AS animal_name,
    a.ability1,
    a.ability2,
    a.speed,
    a.price_coins,
    ua.purchased_at
FROM user_animals ua
INNER JOIN users u ON ua.id_user = u.id_user
INNER JOIN animals a ON ua.id_animal = a.id_animal;

CREATE VIEW vw_user_progress AS
SELECT
    up.id_progress,
    u.id_user,
    u.username,
    mp.id_map,
    mp.map_name,
    ms.id_mission,
    ms.mission_name,
    up.completed,
    up.completion_date,
    ms.reward_coins
FROM user_progress up
INNER JOIN users u ON up.id_user = u.id_user
INNER JOIN missions ms ON up.id_mission = ms.id_mission
INNER JOIN maps mp ON ms.id_map = mp.id_map;

CREATE VIEW vw_match_details AS
SELECT
    mt.id_match,
    mt.status,
    mt.start_time,
    mt.end_time,
    mp.map_name,
    u.username,
    a.name AS animal_name,
    mpl.detected,
    mpl.times_detected,
    mpl.joined_at,
    mpl.left_at
FROM matches mt
INNER JOIN maps mp ON mt.id_map = mp.id_map
LEFT JOIN match_players mpl ON mt.id_match = mpl.id_match
LEFT JOIN users u ON mpl.id_user = u.id_user
LEFT JOIN animals a ON mpl.id_animal = a.id_animal;

CREATE VIEW vw_user_statistics AS
SELECT
    u.id_user,
    u.username,
    u.coins,
    COUNT(DISTINCT ua.id_animal) AS animals_owned,
    COUNT(DISTINCT CASE WHEN up.completed = 1 THEN up.id_mission END) AS completed_missions,
    COUNT(DISTINCT mpl.id_match) AS matches_played,
    COALESCE(SUM(mpl.times_detected), 0) AS total_times_detected
FROM users u
LEFT JOIN user_animals ua ON u.id_user = ua.id_user
LEFT JOIN user_progress up ON u.id_user = up.id_user
LEFT JOIN match_players mpl ON u.id_user = mpl.id_user
GROUP BY u.id_user, u.username, u.coins;

/* =========================
   FUNCTIONS
   ========================= */

DELIMITER $$

CREATE FUNCTION fn_user_has_animal(p_id_user INT, p_id_animal INT)
RETURNS TINYINT
DETERMINISTIC
READS SQL DATA
BEGIN
    DECLARE v_total INT;

    SELECT COUNT(*)
    INTO v_total
    FROM user_animals
    WHERE id_user = p_id_user
      AND id_animal = p_id_animal;

    IF v_total > 0 THEN
        RETURN 1;
    END IF;

    RETURN 0;
END$$

CREATE FUNCTION fn_user_completed_missions(p_id_user INT)
RETURNS INT
DETERMINISTIC
READS SQL DATA
BEGIN
    DECLARE v_total INT;

    SELECT COUNT(*)
    INTO v_total
    FROM user_progress
    WHERE id_user = p_id_user
      AND completed = 1;

    RETURN v_total;
END$$

/* =========================
   PROCEDURES
   ========================= */

CREATE PROCEDURE sp_buy_animal(
    IN p_id_user INT,
    IN p_id_animal INT
)
BEGIN
    DECLARE v_price INT;
    DECLARE v_user_coins INT;
    DECLARE v_already_owned INT;

    SELECT price_coins
    INTO v_price
    FROM animals
    WHERE id_animal = p_id_animal;

    SELECT coins
    INTO v_user_coins
    FROM users
    WHERE id_user = p_id_user;

    SELECT COUNT(*)
    INTO v_already_owned
    FROM user_animals
    WHERE id_user = p_id_user
      AND id_animal = p_id_animal;

    IF v_already_owned > 0 THEN
        SELECT 'O utilizador ja tem este animal.' AS message;
    ELSEIF v_user_coins < v_price THEN
        SELECT 'Moedas insuficientes para comprar este animal.' AS message;
    ELSE
        UPDATE users
        SET coins = coins - v_price
        WHERE id_user = p_id_user;

        INSERT INTO user_animals (id_user, id_animal)
        VALUES (p_id_user, p_id_animal);

        SELECT 'Animal comprado com sucesso.' AS message;
    END IF;
END$$

CREATE PROCEDURE sp_complete_mission(
    IN p_id_user INT,
    IN p_id_mission INT
)
BEGIN
    INSERT INTO user_progress (id_user, id_mission, completed, completion_date)
    VALUES (p_id_user, p_id_mission, 1, NOW())
    ON DUPLICATE KEY UPDATE
        completed = 1,
        completion_date = IF(completion_date IS NULL, NOW(), completion_date);

    SELECT 'Missao marcada como concluida.' AS message;
END$$

CREATE PROCEDURE sp_start_match(
    IN p_id_map INT,
    OUT p_id_match INT
)
BEGIN
    INSERT INTO matches (id_map, start_time, status)
    VALUES (p_id_map, NOW(), 'running');

    SET p_id_match = LAST_INSERT_ID();

    SELECT p_id_match AS new_match_id;
END$$

CREATE PROCEDURE sp_add_player_to_match(
    IN p_id_match INT,
    IN p_id_user INT,
    IN p_id_animal INT
)
BEGIN
    INSERT INTO match_players (id_match, id_user, id_animal, detected, times_detected, joined_at)
    VALUES (p_id_match, p_id_user, p_id_animal, 0, 0, NOW())
    ON DUPLICATE KEY UPDATE
        id_animal = VALUES(id_animal),
        left_at = NULL;

    SELECT 'Jogador adicionado a partida.' AS message;
END$$

CREATE PROCEDURE sp_finish_match(
    IN p_id_match INT,
    IN p_status VARCHAR(20)
)
BEGIN
    UPDATE matches
    SET status = p_status,
        end_time = NOW()
    WHERE id_match = p_id_match;

    UPDATE match_players
    SET left_at = IF(left_at IS NULL, NOW(), left_at)
    WHERE id_match = p_id_match;

    SELECT 'Partida terminada.' AS message;
END$$

/* =========================
   TRIGGERS
   ========================= */

CREATE TRIGGER trg_users_no_negative_coins_insert
BEFORE INSERT ON users
FOR EACH ROW
BEGIN
    IF NEW.coins < 0 THEN
        SET NEW.coins = 0;
    END IF;
END$$

CREATE TRIGGER trg_users_no_negative_coins_update
BEFORE UPDATE ON users
FOR EACH ROW
BEGIN
    IF NEW.coins < 0 THEN
        SET NEW.coins = 0;
    END IF;
END$$

CREATE TRIGGER trg_animals_no_negative_price_insert
BEFORE INSERT ON animals
FOR EACH ROW
BEGIN
    IF NEW.price_coins < 0 THEN
        SET NEW.price_coins = 0;
    END IF;
END$$

CREATE TRIGGER trg_animals_no_negative_price_update
BEFORE UPDATE ON animals
FOR EACH ROW
BEGIN
    IF NEW.price_coins < 0 THEN
        SET NEW.price_coins = 0;
    END IF;
END$$

CREATE TRIGGER trg_user_progress_set_completion_date_insert
BEFORE INSERT ON user_progress
FOR EACH ROW
BEGIN
    IF NEW.completed = 1 AND NEW.completion_date IS NULL THEN
        SET NEW.completion_date = NOW();
    END IF;
END$$

CREATE TRIGGER trg_user_progress_set_completion_date_update
BEFORE UPDATE ON user_progress
FOR EACH ROW
BEGIN
    IF OLD.completed = 0 AND NEW.completed = 1 AND NEW.completion_date IS NULL THEN
        SET NEW.completion_date = NOW();
    END IF;
END$$

CREATE TRIGGER trg_user_progress_reward_insert
AFTER INSERT ON user_progress
FOR EACH ROW
BEGIN
    DECLARE v_reward INT;

    IF NEW.completed = 1 THEN
        SELECT reward_coins
        INTO v_reward
        FROM missions
        WHERE id_mission = NEW.id_mission;

        UPDATE users
        SET coins = coins + v_reward
        WHERE id_user = NEW.id_user;
    END IF;
END$$

CREATE TRIGGER trg_user_progress_reward_update
AFTER UPDATE ON user_progress
FOR EACH ROW
BEGIN
    DECLARE v_reward INT;

    IF OLD.completed = 0 AND NEW.completed = 1 THEN
        SELECT reward_coins
        INTO v_reward
        FROM missions
        WHERE id_mission = NEW.id_mission;

        UPDATE users
        SET coins = coins + v_reward
        WHERE id_user = NEW.id_user;
    END IF;
END$$

CREATE TRIGGER trg_match_players_detected_insert
BEFORE INSERT ON match_players
FOR EACH ROW
BEGIN
    IF NEW.times_detected > 0 THEN
        SET NEW.detected = 1;
    END IF;
END$$

CREATE TRIGGER trg_match_players_detected_update
BEFORE UPDATE ON match_players
FOR EACH ROW
BEGIN
    IF NEW.times_detected > 0 THEN
        SET NEW.detected = 1;
    ELSE
        SET NEW.detected = 0;
    END IF;
END$$

DELIMITER ;
