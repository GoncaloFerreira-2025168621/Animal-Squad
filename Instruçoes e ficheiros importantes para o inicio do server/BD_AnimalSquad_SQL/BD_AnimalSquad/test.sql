/*
    Animal Squad - Base de Dados
    Ficheiro: test.sql
    Objetivo: teste global da base de dados.

    Ordem aconselhada para demonstrar tudo:
    1) create.sql
    2) logic.sql
    3) populate.sql
    4) queries.sql
    5) test_triggers.sql
    6) test.sql
*/

USE animal_squad;

/* =========================
   TESTE GLOBAL 1: confirmar tabelas principais
   ========================= */
SELECT 'UTILIZADORES' AS section;
SELECT * FROM users;

SELECT 'ANIMAIS' AS section;
SELECT * FROM animals;

SELECT 'MAPAS' AS section;
SELECT * FROM maps;

SELECT 'MISSOES' AS section;
SELECT * FROM missions;

/* =========================
   TESTE GLOBAL 2: testar function para saber se user tem animal
   ========================= */
SELECT
    u.username,
    a.name AS animal_name,
    fn_user_has_animal(u.id_user, a.id_animal) AS has_animal
FROM users u
CROSS JOIN animals a
WHERE u.username = 'Goncalo'
ORDER BY a.id_animal;

/* =========================
   TESTE GLOBAL 3: comprar animal com procedure
   O Goncalo tenta comprar o Bear.
   Se ja tiver comprado, a procedure devolve mensagem.
   ========================= */
SET @goncalo_id = (SELECT id_user FROM users WHERE username = 'Goncalo' LIMIT 1);
SET @bear_id = (SELECT id_animal FROM animals WHERE name = 'Bear' LIMIT 1);

SELECT 'ANTES DA COMPRA' AS section;
SELECT username, coins FROM users WHERE id_user = @goncalo_id;
SELECT * FROM vw_user_animals WHERE id_user = @goncalo_id;

CALL sp_buy_animal(@goncalo_id, @bear_id);

SELECT 'DEPOIS DA COMPRA' AS section;
SELECT username, coins FROM users WHERE id_user = @goncalo_id;
SELECT * FROM vw_user_animals WHERE id_user = @goncalo_id;

/* =========================
   TESTE GLOBAL 4: completar uma missao com procedure
   A reward e dada automaticamente pelos triggers.
   ========================= */
SET @mission_id = (SELECT id_mission FROM missions WHERE mission_name = 'Free the Animals' LIMIT 1);

SELECT 'ANTES DE COMPLETAR MISSAO' AS section;
SELECT username, coins FROM users WHERE id_user = @goncalo_id;

CALL sp_complete_mission(@goncalo_id, @mission_id);

SELECT 'DEPOIS DE COMPLETAR MISSAO' AS section;
SELECT username, coins FROM users WHERE id_user = @goncalo_id;
SELECT * FROM vw_user_progress WHERE id_user = @goncalo_id;

/* =========================
   TESTE GLOBAL 5: criar partida, adicionar jogadores e terminar partida
   ========================= */
CALL sp_start_match(1, @new_match_id);

SET @afonso_id = (SELECT id_user FROM users WHERE username = 'Afonso' LIMIT 1);
SET @beaver_id = (SELECT id_animal FROM animals WHERE name = 'Beaver' LIMIT 1);
SET @mouse_id = (SELECT id_animal FROM animals WHERE name = 'Mouse' LIMIT 1);

CALL sp_add_player_to_match(@new_match_id, @goncalo_id, @beaver_id);
CALL sp_add_player_to_match(@new_match_id, @afonso_id, @mouse_id);

UPDATE match_players
SET times_detected = 1
WHERE id_match = @new_match_id
  AND id_user = @afonso_id;

CALL sp_finish_match(@new_match_id, 'completed');

SELECT 'PARTIDA CRIADA NO TESTE GLOBAL' AS section;
SELECT * FROM vw_match_details WHERE id_match = @new_match_id;

/* =========================
   TESTE GLOBAL 6: estatisticas finais
   ========================= */
SELECT 'ESTATISTICAS FINAIS' AS section;
SELECT * FROM vw_user_statistics;
