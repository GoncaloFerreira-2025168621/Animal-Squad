/*
    Animal Squad - Base de Dados
    Ficheiro: queries.sql
    Objetivo: consultas de demonstracao da base de dados.
*/

USE animal_squad;

/* 1. Ver todos os utilizadores */
SELECT *
FROM users;

/* 2. Ver todos os animais disponiveis na loja */
SELECT
    id_animal,
    name,
    ability1,
    ability2,
    speed,
    price_coins
FROM animals
ORDER BY price_coins ASC;

/* 3. Ver animais comprados por cada utilizador */
SELECT
    username,
    animal_name,
    ability1,
    ability2,
    purchased_at
FROM vw_user_animals
ORDER BY username, animal_name;

/* 4. Ver mapas e respetivas missoes */
SELECT
    mp.map_name,
    mp.difficulty,
    ms.order_index,
    ms.mission_name,
    ms.reward_coins
FROM maps mp
INNER JOIN missions ms ON mp.id_map = ms.id_map
ORDER BY mp.id_map, ms.order_index;

/* 5. Ver progresso dos utilizadores nas missoes */
SELECT
    username,
    map_name,
    mission_name,
    completed,
    completion_date,
    reward_coins
FROM vw_user_progress
ORDER BY username, map_name, mission_name;

/* 6. Ranking de jogadores por moedas */
SELECT
    username,
    coins
FROM users
ORDER BY coins DESC;

/* 7. Numero de missoes completas por utilizador */
SELECT
    username,
    fn_user_completed_missions(id_user) AS completed_missions
FROM users
ORDER BY completed_missions DESC;

/* 8. Estatisticas gerais dos utilizadores */
SELECT *
FROM vw_user_statistics
ORDER BY completed_missions DESC, coins DESC;

/* 9. Historico de partidas com jogadores e animais usados */
SELECT
    id_match,
    status,
    map_name,
    username,
    animal_name,
    times_detected,
    start_time,
    end_time
FROM vw_match_details
ORDER BY id_match DESC, username;

/* 10. Animais mais usados em partidas */
SELECT
    a.name AS animal_name,
    COUNT(mp.id) AS times_used
FROM animals a
LEFT JOIN match_players mp ON a.id_animal = mp.id_animal
GROUP BY a.id_animal, a.name
ORDER BY times_used DESC;

/* 11. Analytics: total de vezes que os jogadores foram detetados */
SELECT
    u.username,
    COALESCE(SUM(mp.times_detected), 0) AS total_times_detected
FROM users u
LEFT JOIN match_players mp ON u.id_user = mp.id_user
GROUP BY u.id_user, u.username
ORDER BY total_times_detected DESC;

/* 12. Missoes completadas por mapa */
SELECT
    mp.map_name,
    COUNT(up.id_progress) AS completed_times
FROM maps mp
INNER JOIN missions ms ON mp.id_map = ms.id_map
LEFT JOIN user_progress up ON ms.id_mission = up.id_mission AND up.completed = 1
GROUP BY mp.id_map, mp.map_name
ORDER BY completed_times DESC;
