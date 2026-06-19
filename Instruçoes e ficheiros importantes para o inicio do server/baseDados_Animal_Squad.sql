-- MySQL dump 10.13  Distrib 8.0.44, for Win64 (x86_64)
--
-- Host: localhost    Database: animal_squad
-- ------------------------------------------------------
-- Server version	8.0.44

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `animals`
--

DROP TABLE IF EXISTS `animals`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `animals` (
  `id_animal` int NOT NULL AUTO_INCREMENT,
  `name` varchar(50) NOT NULL,
  `description` text,
  `ability1` varchar(100) DEFAULT NULL,
  `ability2` varchar(100) DEFAULT NULL,
  `speed` int DEFAULT NULL,
  `captured` tinyint(1) DEFAULT '0',
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `price_coins` int NOT NULL DEFAULT '250',
  PRIMARY KEY (`id_animal`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `animals`
--

LOCK TABLES `animals` WRITE;
/*!40000 ALTER TABLE `animals` DISABLE KEYS */;
INSERT INTO `animals` VALUES (1,'Pássaro','O Pássaro consegue voar e transportar pequenos objetos. É útil para alcançar zonas altas, levar itens para locais inacessíveis e ajudar a resolver puzzles à distância.',NULL,NULL,NULL,0,'2026-06-13 16:51:46',250),(2,'Rato','O Rato consegue entrar em espaços pequenos e roer objetos frágeis, como fios, madeira fina ou mecanismos simples. É ideal para infiltração e sabotagem.',NULL,NULL,NULL,0,'2026-06-13 16:51:46',250),(3,'Castor','O Castor consegue nadar e destruir madeira. É útil para atravessar rios, partir barragens, abrir caminhos e resolver puzzles ligados à água.',NULL,NULL,NULL,0,'2026-06-13 16:51:46',250),(4,'Urso-malaio','O Urso-malaio consegue empurrar objetos pesados e escalar pequenas estruturas. É útil para mover obstáculos, ativar mecanismos e alcançar zonas mais elevadas.',NULL,NULL,NULL,0,'2026-06-13 16:51:46',250);
/*!40000 ALTER TABLE `animals` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `maps`
--

DROP TABLE IF EXISTS `maps`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `maps` (
  `id_map` int NOT NULL AUTO_INCREMENT,
  `map_name` varchar(50) NOT NULL,
  `difficulty` varchar(20) DEFAULT NULL,
  `description` text,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_map`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `maps`
--

LOCK TABLES `maps` WRITE;
/*!40000 ALTER TABLE `maps` DISABLE KEYS */;
/*!40000 ALTER TABLE `maps` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `match_players`
--

DROP TABLE IF EXISTS `match_players`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `match_players` (
  `id` int NOT NULL AUTO_INCREMENT,
  `id_match` int NOT NULL,
  `id_user` int NOT NULL,
  `id_animal` int NOT NULL,
  `detected` tinyint(1) DEFAULT '0',
  `times_detected` int DEFAULT '0',
  `joined_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `left_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `id_match` (`id_match`),
  KEY `id_user` (`id_user`),
  KEY `id_animal` (`id_animal`),
  CONSTRAINT `match_players_ibfk_1` FOREIGN KEY (`id_match`) REFERENCES `matches` (`id_match`) ON DELETE CASCADE,
  CONSTRAINT `match_players_ibfk_2` FOREIGN KEY (`id_user`) REFERENCES `users` (`id_user`) ON DELETE CASCADE,
  CONSTRAINT `match_players_ibfk_3` FOREIGN KEY (`id_animal`) REFERENCES `animals` (`id_animal`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `match_players`
--

LOCK TABLES `match_players` WRITE;
/*!40000 ALTER TABLE `match_players` DISABLE KEYS */;
/*!40000 ALTER TABLE `match_players` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `matches`
--

DROP TABLE IF EXISTS `matches`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `matches` (
  `id_match` int NOT NULL AUTO_INCREMENT,
  `id_map` int NOT NULL,
  `start_time` datetime DEFAULT NULL,
  `end_time` datetime DEFAULT NULL,
  `status` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`id_match`),
  KEY `id_map` (`id_map`),
  CONSTRAINT `matches_ibfk_1` FOREIGN KEY (`id_map`) REFERENCES `maps` (`id_map`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `matches`
--

LOCK TABLES `matches` WRITE;
/*!40000 ALTER TABLE `matches` DISABLE KEYS */;
/*!40000 ALTER TABLE `matches` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `missions`
--

DROP TABLE IF EXISTS `missions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `missions` (
  `id_mission` int NOT NULL AUTO_INCREMENT,
  `id_map` int NOT NULL,
  `mission_name` varchar(100) NOT NULL,
  `description` text,
  `reward_coins` int DEFAULT '0',
  `order_index` int DEFAULT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_mission`),
  KEY `id_map` (`id_map`),
  CONSTRAINT `missions_ibfk_1` FOREIGN KEY (`id_map`) REFERENCES `maps` (`id_map`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `missions`
--

LOCK TABLES `missions` WRITE;
/*!40000 ALTER TABLE `missions` DISABLE KEYS */;
/*!40000 ALTER TABLE `missions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_animals`
--

DROP TABLE IF EXISTS `user_animals`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_animals` (
  `id_user_animal` int NOT NULL AUTO_INCREMENT,
  `id_user` int NOT NULL,
  `id_animal` int NOT NULL,
  `purchased_at` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_user_animal`),
  UNIQUE KEY `unique_user_animal` (`id_user`,`id_animal`),
  KEY `id_animal` (`id_animal`),
  CONSTRAINT `user_animals_ibfk_1` FOREIGN KEY (`id_user`) REFERENCES `users` (`id_user`),
  CONSTRAINT `user_animals_ibfk_2` FOREIGN KEY (`id_animal`) REFERENCES `animals` (`id_animal`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_animals`
--

LOCK TABLES `user_animals` WRITE;
/*!40000 ALTER TABLE `user_animals` DISABLE KEYS */;
INSERT INTO `user_animals` VALUES (1,1,3,'2026-06-13 19:43:51'),(2,1,4,'2026-06-13 19:44:02'),(3,3,3,'2026-06-13 22:48:02'),(4,2,2,'2026-06-14 00:00:07'),(5,2,1,'2026-06-14 00:00:10');
/*!40000 ALTER TABLE `user_animals` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_progress`
--

DROP TABLE IF EXISTS `user_progress`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_progress` (
  `id_progress` int NOT NULL AUTO_INCREMENT,
  `id_user` int NOT NULL,
  `id_mission` int NOT NULL,
  `completed` tinyint(1) DEFAULT '0',
  `completion_date` datetime DEFAULT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_progress`),
  KEY `id_user` (`id_user`),
  KEY `id_mission` (`id_mission`),
  CONSTRAINT `user_progress_ibfk_1` FOREIGN KEY (`id_user`) REFERENCES `users` (`id_user`) ON DELETE CASCADE,
  CONSTRAINT `user_progress_ibfk_2` FOREIGN KEY (`id_mission`) REFERENCES `missions` (`id_mission`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_progress`
--

LOCK TABLES `user_progress` WRITE;
/*!40000 ALTER TABLE `user_progress` DISABLE KEYS */;
/*!40000 ALTER TABLE `user_progress` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `id_user` int NOT NULL AUTO_INCREMENT,
  `username` varchar(50) NOT NULL,
  `password` varchar(255) NOT NULL,
  `coins` int NOT NULL DEFAULT '250',
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_user`),
  UNIQUE KEY `username` (`username`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'alex','1234',60,'2026-05-19 23:16:35'),(2,'GG','4321',220,'2026-05-20 00:11:34'),(3,'server','server',250,'2026-06-13 16:20:47'),(4,'Faisca','1234',250,'2026-06-19 23:50:34');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-06-20  0:07:45
