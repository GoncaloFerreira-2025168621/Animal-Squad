CREATE DATABASE db_animalsquad;
USE db_animalsquad;

CREATE TABLE users (
    id_user INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    coins INT DEFAULT 0,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE animals (
    id_animal INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    description TEXT,
    ability1 VARCHAR(100),
    ability2 VARCHAR(100),
    speed INT,
    captured BOOLEAN DEFAULT FALSE,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE maps (
    id_map INT AUTO_INCREMENT PRIMARY KEY,
    map_name VARCHAR(50) NOT NULL,
    difficulty VARCHAR(20),
    description TEXT,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE missions (
    id_mission INT AUTO_INCREMENT PRIMARY KEY,
    id_map INT NOT NULL,
    mission_name VARCHAR(100) NOT NULL,
    description TEXT,
    reward_coins INT DEFAULT 0,
    order_index INT,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (id_map)
    REFERENCES maps(id_map)
    ON DELETE CASCADE
);

CREATE TABLE user_progress (
    id_progress INT AUTO_INCREMENT PRIMARY KEY,
    id_user INT NOT NULL,
    id_mission INT NOT NULL,
    completed BOOLEAN DEFAULT FALSE,
    completion_date DATETIME,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (id_user)
    REFERENCES users(id_user)
    ON DELETE CASCADE,

    FOREIGN KEY (id_mission)
    REFERENCES missions(id_mission)
    ON DELETE CASCADE
);

CREATE TABLE matches (
    id_match INT AUTO_INCREMENT PRIMARY KEY,
    id_map INT NOT NULL,
    start_time DATETIME,
    end_time DATETIME,
    status VARCHAR(20),

    FOREIGN KEY (id_map)
    REFERENCES maps(id_map)
    ON DELETE CASCADE
);

CREATE TABLE match_players (
    id INT AUTO_INCREMENT PRIMARY KEY,
    id_match INT NOT NULL,
    id_user INT NOT NULL,
    id_animal INT NOT NULL,
    detected BOOLEAN DEFAULT FALSE,
    times_detected INT DEFAULT 0,
    joined_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    left_at DATETIME,

    FOREIGN KEY (id_match)
    REFERENCES matches(id_match)
    ON DELETE CASCADE,

    FOREIGN KEY (id_user)
    REFERENCES users(id_user)
    ON DELETE CASCADE,

    FOREIGN KEY (id_animal)
    REFERENCES animals(id_animal)
    ON DELETE CASCADE
);