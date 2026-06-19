/*
    Animal Squad - Base de Dados
    Ficheiro: create.sql
    Objetivo: criar a base de dados e todas as tabelas.

    ATENCAO: este script apaga a base de dados animal_squad caso ela ja exista.
    Usar apenas quando se pretende recriar tudo do zero.
*/

DROP DATABASE IF EXISTS animal_squad;
CREATE DATABASE animal_squad
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE animal_squad;

/* =========================
   Tabela de utilizadores
   ========================= */
CREATE TABLE users (
    id_user INT AUTO_INCREMENT,
    username VARCHAR(50) NOT NULL,
    password VARCHAR(255) NOT NULL,
    coins INT NOT NULL DEFAULT 0,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT pk_users PRIMARY KEY (id_user),
    CONSTRAINT uq_users_username UNIQUE (username),
    CONSTRAINT ck_users_coins CHECK (coins >= 0)
) ENGINE = InnoDB;

/* =========================
   Tabela de animais jogaveis
   ========================= */
CREATE TABLE animals (
    id_animal INT AUTO_INCREMENT,
    name VARCHAR(50) NOT NULL,
    description TEXT,
    ability1 VARCHAR(100),
    ability2 VARCHAR(100),
    speed INT NOT NULL DEFAULT 0,
    captured TINYINT(1) NOT NULL DEFAULT 0,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    price_coins INT NOT NULL DEFAULT 0,

    CONSTRAINT pk_animals PRIMARY KEY (id_animal),
    CONSTRAINT uq_animals_name UNIQUE (name),
    CONSTRAINT ck_animals_speed CHECK (speed >= 0),
    CONSTRAINT ck_animals_captured CHECK (captured IN (0, 1)),
    CONSTRAINT ck_animals_price CHECK (price_coins >= 0)
) ENGINE = InnoDB;

/* =========================
   Tabela de mapas
   ========================= */
CREATE TABLE maps (
    id_map INT AUTO_INCREMENT,
    map_name VARCHAR(50) NOT NULL,
    difficulty VARCHAR(20) NOT NULL,
    description TEXT,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT pk_maps PRIMARY KEY (id_map),
    CONSTRAINT uq_maps_name UNIQUE (map_name)
) ENGINE = InnoDB;

/* =========================
   Tabela de missoes
   Cada missao pertence a um mapa
   ========================= */
CREATE TABLE missions (
    id_mission INT AUTO_INCREMENT,
    id_map INT NOT NULL,
    mission_name VARCHAR(100) NOT NULL,
    description TEXT,
    reward_coins INT NOT NULL DEFAULT 0,
    order_index INT NOT NULL DEFAULT 1,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT pk_missions PRIMARY KEY (id_mission),
    CONSTRAINT fk_missions_maps
        FOREIGN KEY (id_map) REFERENCES maps(id_map)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,
    CONSTRAINT ck_missions_reward CHECK (reward_coins >= 0),
    CONSTRAINT ck_missions_order CHECK (order_index >= 1),
    CONSTRAINT uq_missions_map_order UNIQUE (id_map, order_index),
    CONSTRAINT uq_missions_map_name UNIQUE (id_map, mission_name)
) ENGINE = InnoDB;

/* =========================
   Tabela de partidas
   Cada partida acontece num mapa
   ========================= */
CREATE TABLE matches (
    id_match INT AUTO_INCREMENT,
    id_map INT NOT NULL,
    start_time DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    end_time DATETIME NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'running',

    CONSTRAINT pk_matches PRIMARY KEY (id_match),
    CONSTRAINT fk_matches_maps
        FOREIGN KEY (id_map) REFERENCES maps(id_map)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,
    CONSTRAINT ck_matches_status CHECK (status IN ('running', 'completed', 'failed', 'cancelled'))
) ENGINE = InnoDB;

/* =========================
   Tabela de jogadores por partida
   Guarda analytics: deteccoes, animal usado, entrada e saida
   ========================= */
CREATE TABLE match_players (
    id INT AUTO_INCREMENT,
    id_match INT NOT NULL,
    id_user INT NOT NULL,
    id_animal INT NOT NULL,
    detected TINYINT(1) NOT NULL DEFAULT 0,
    times_detected INT NOT NULL DEFAULT 0,
    joined_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    left_at DATETIME NULL,

    CONSTRAINT pk_match_players PRIMARY KEY (id),
    CONSTRAINT fk_match_players_matches
        FOREIGN KEY (id_match) REFERENCES matches(id_match)
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT fk_match_players_users
        FOREIGN KEY (id_user) REFERENCES users(id_user)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,
    CONSTRAINT fk_match_players_animals
        FOREIGN KEY (id_animal) REFERENCES animals(id_animal)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,
    CONSTRAINT ck_match_players_detected CHECK (detected IN (0, 1)),
    CONSTRAINT ck_match_players_times_detected CHECK (times_detected >= 0),
    CONSTRAINT uq_match_players_user_match UNIQUE (id_match, id_user)
) ENGINE = InnoDB;

/* =========================
   Tabela de animais comprados por utilizador
   ========================= */
CREATE TABLE user_animals (
    id_user_animal INT AUTO_INCREMENT,
    id_user INT NOT NULL,
    id_animal INT NOT NULL,
    purchased_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT pk_user_animals PRIMARY KEY (id_user_animal),
    CONSTRAINT fk_user_animals_users
        FOREIGN KEY (id_user) REFERENCES users(id_user)
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT fk_user_animals_animals
        FOREIGN KEY (id_animal) REFERENCES animals(id_animal)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,
    CONSTRAINT uq_user_animals_user_animal UNIQUE (id_user, id_animal)
) ENGINE = InnoDB;

/* =========================
   Tabela de progresso dos jogadores
   Cada registo indica se um utilizador completou uma missao
   ========================= */
CREATE TABLE user_progress (
    id_progress INT AUTO_INCREMENT,
    id_user INT NOT NULL,
    id_mission INT NOT NULL,
    completed TINYINT(1) NOT NULL DEFAULT 0,
    completion_date DATETIME NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT pk_user_progress PRIMARY KEY (id_progress),
    CONSTRAINT fk_user_progress_users
        FOREIGN KEY (id_user) REFERENCES users(id_user)
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT fk_user_progress_missions
        FOREIGN KEY (id_mission) REFERENCES missions(id_mission)
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT ck_user_progress_completed CHECK (completed IN (0, 1)),
    CONSTRAINT uq_user_progress_user_mission UNIQUE (id_user, id_mission)
) ENGINE = InnoDB;

/* =========================
   Indices adicionais para melhorar pesquisas
   ========================= */
CREATE INDEX idx_missions_id_map ON missions(id_map);
CREATE INDEX idx_matches_id_map ON matches(id_map);
CREATE INDEX idx_match_players_id_match ON match_players(id_match);
CREATE INDEX idx_match_players_id_user ON match_players(id_user);
CREATE INDEX idx_user_animals_id_user ON user_animals(id_user);
CREATE INDEX idx_user_progress_id_user ON user_progress(id_user);
CREATE INDEX idx_user_progress_id_mission ON user_progress(id_mission);
