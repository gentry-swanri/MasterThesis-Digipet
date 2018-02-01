-- phpMyAdmin SQL Dump
-- version 4.7.4
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Feb 01, 2018 at 09:34 AM
-- Server version: 10.1.26-MariaDB
-- PHP Version: 7.1.9

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `digipetdb`
--

-- --------------------------------------------------------

--
-- Table structure for table `game_data`
--

CREATE TABLE `game_data` (
  `id` int(11) NOT NULL,
  `pet_name` varchar(255) CHARACTER SET utf8 NOT NULL,
  `rest` int(11) NOT NULL,
  `energy` int(11) NOT NULL,
  `agility` int(11) NOT NULL,
  `stress` int(11) NOT NULL,
  `heart` int(11) NOT NULL,
  `money` int(11) NOT NULL,
  `xp` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `last_modified` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `game_data`
--

INSERT INTO `game_data` (`id`, `pet_name`, `rest`, `energy`, `agility`, `stress`, `heart`, `money`, `xp`, `user_id`, `last_modified`, `created_at`) VALUES
(1, 'pet1', 0, 0, 0, 0, 2, 100, 0, 1, '2018-01-09 07:54:13', '2017-12-24 08:31:21'),
(2, 'pet2', 100, 100, 100, 100, 6, 100, 0, 2, '2017-12-30 10:51:32', '2017-12-24 10:36:17'),
(3, 'pet3', 85, 85, 100, 25, 6, 121, 249, 3, '2017-12-31 23:57:12', '2017-12-24 10:39:05'),
(4, 'doggy', 100, 100, 100, 100, 6, 100, 0, 4, '2017-12-27 09:03:35', '2017-12-27 09:03:35'),
(5, 'my pet', 100, 100, 100, 100, 6, 100, 0, 5, '2018-01-01 04:13:50', '2018-01-01 04:13:50'),
(6, 'namahewan', 100, 100, 100, 100, 6, 100, 0, 6, '2018-01-01 07:03:39', '2018-01-01 07:03:39'),
(7, 'namahewan', 100, 100, 100, 100, 6, 100, 0, 7, '2018-01-01 07:07:47', '2018-01-01 07:07:47'),
(11, 'buddy', 100, 100, 100, 100, 6, 100, 0, 11, '2018-01-09 11:34:54', '2018-01-08 22:57:51');

-- --------------------------------------------------------

--
-- Table structure for table `user`
--

CREATE TABLE `user` (
  `id` int(11) NOT NULL,
  `username` varchar(255) CHARACTER SET utf8 NOT NULL,
  `password` varchar(255) CHARACTER SET utf8 NOT NULL,
  `is_active` tinyint(1) NOT NULL,
  `first_name` varchar(255) CHARACTER SET utf8 NOT NULL,
  `last_name` varchar(255) CHARACTER SET utf8 NOT NULL,
  `email` varchar(255) CHARACTER SET utf8 NOT NULL,
  `last_modified` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `user`
--

INSERT INTO `user` (`id`, `username`, `password`, `is_active`, `first_name`, `last_name`, `email`, `last_modified`, `created_at`) VALUES
(1, 'user1', '54321', 0, 'first', 'user', 'gentryswanri@gmail.com', '2018-01-09 07:54:12', '2017-12-24 08:31:21'),
(2, 'user2', '54321', 1, 'second', 'user', 'gentryswanri@gmail.com', '2017-12-30 10:51:37', '2017-12-24 10:36:17'),
(3, 'user3', '12345', 1, 'Third', 'user', 'gentryswanri@gmail.com', '2018-01-01 12:47:18', '2017-12-24 10:39:05'),
(4, 'kay', '12345', 0, 'user', '4', 'yoni.azhar@gmail.com', '2017-12-27 09:03:35', '2017-12-27 09:03:35'),
(5, 'gentrys', '12345', 1, 'gentry', 'swanri', 'gentryswanri@yahoo.com', '2018-01-01 04:14:07', '2018-01-01 04:13:50'),
(6, 'nama1', '12345', 1, 'nama1', 'nama2', 'gentryswanri@yahoo.com', '2018-01-01 07:03:52', '2018-01-01 07:03:39'),
(7, 'nama2', '12345', 0, 'namadepan', 'namabelakang', 'gentryswanri@yahoo.com', '2018-01-01 07:13:00', '2018-01-01 07:07:47'),
(11, 'gentry', '12345', 0, 'gentry', 'swanri', 'gentryswanri@gmail.com', '2018-01-09 11:34:54', '2018-01-08 22:57:51');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `game_data`
--
ALTER TABLE `game_data`
  ADD PRIMARY KEY (`id`),
  ADD KEY `user_id` (`user_id`);

--
-- Indexes for table `user`
--
ALTER TABLE `user`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `game_data`
--
ALTER TABLE `game_data`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT for table `user`
--
ALTER TABLE `user`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `game_data`
--
ALTER TABLE `game_data`
  ADD CONSTRAINT `game_data_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `user` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
