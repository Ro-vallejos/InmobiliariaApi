-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Nov 10, 2025 at 04:15 AM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `inmobiliaria`
--

-- --------------------------------------------------------

--
-- Table structure for table `auditoria`
--

CREATE TABLE `auditoria` (
  `id_auditoria` int(11) NOT NULL,
  `tipo` enum('Contrato','Pago','Inmueble','Usuario') NOT NULL,
  `id_registro_afectado` int(11) NOT NULL,
  `accion` enum('Crear','Actualizar','Finalizar','Anular','Recibir') NOT NULL,
  `usuario` varchar(100) NOT NULL,
  `fecha_hora` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `auditoria`
--

INSERT INTO `auditoria` (`id_auditoria`, `tipo`, `id_registro_afectado`, `accion`, `usuario`, `fecha_hora`) VALUES
(1, 'Contrato', 1, 'Crear', 'admin@gmail.com', '2025-09-26 17:46:29'),
(2, 'Contrato', 2, 'Crear', 'admin@gmail.com', '2025-09-26 17:47:05'),
(3, 'Pago', 1, 'Recibir', 'admin@gmail.com', '2025-09-26 17:47:14');

-- --------------------------------------------------------

--
-- Table structure for table `contrato`
--

CREATE TABLE `contrato` (
  `id` int(11) NOT NULL,
  `id_inquilino` int(11) NOT NULL,
  `id_inmueble` int(11) NOT NULL,
  `fecha_inicio` date NOT NULL,
  `fecha_fin` date NOT NULL,
  `estado` int(11) DEFAULT 1,
  `fecha_terminacion_anticipada` date DEFAULT NULL,
  `multa` decimal(10,2) DEFAULT NULL,
  `monto_mensual` decimal(10,2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `contrato`
--

INSERT INTO `contrato` (`id`, `id_inquilino`, `id_inmueble`, `fecha_inicio`, `fecha_fin`, `estado`, `fecha_terminacion_anticipada`, `multa`, `monto_mensual`) VALUES
(1, 2, 5, '2025-09-26', '2025-12-26', 1, NULL, NULL, 678123.00),
(2, 1, 7, '2025-10-31', '2026-03-31', 1, NULL, NULL, 560000.00);

--
-- Triggers `contrato`
--
DELIMITER $$
CREATE TRIGGER `generar_pagos_contrato` AFTER INSERT ON `contrato` FOR EACH ROW BEGIN
    DECLARE fecha_iteracion DATE;
    DECLARE contador INT DEFAULT 1;
    DECLARE total_meses INT;

    SET total_meses = PERIOD_DIFF(DATE_FORMAT(NEW.fecha_fin, '%Y%m'), DATE_FORMAT(NEW.fecha_inicio, '%Y%m'));

    IF DAY(NEW.fecha_fin) < DAY(NEW.fecha_inicio) THEN
        SET total_meses = total_meses - 1;
    END IF;

    IF total_meses < 1 THEN
         SET total_meses = 1; 
    END IF;

    SET fecha_iteracion = NEW.fecha_inicio;


    WHILE contador <= total_meses DO
        INSERT INTO pago (id_contrato, nro_pago, estado, concepto)
        VALUES (
            NEW.id, 
            contador, 
            'pendiente',

            CONCAT('Pago ', contador, ' | Mes de ', DATE_FORMAT(fecha_iteracion, '%M %Y'))
        );

        SET fecha_iteracion = DATE_ADD(fecha_iteracion, INTERVAL 1 MONTH);
        SET contador = contador + 1;
    END WHILE;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `inmueble`
--

CREATE TABLE `inmueble` (
  `id` int(11) NOT NULL,
  `id_propietario` int(11) NOT NULL,
  `tipo` varchar(30) NOT NULL,
  `direccion` varchar(250) NOT NULL,
  `uso` varchar(40) NOT NULL,
  `ambientes` int(11) DEFAULT NULL,
  `superficie` decimal(10,0) DEFAULT NULL,
  `latitud` decimal(10,0) DEFAULT NULL,
  `precio` decimal(12,2) NOT NULL,
  `estado` int(11) NOT NULL,
  `imagen` varchar(500) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `inmueble`
--

INSERT INTO `inmueble` (`id`, `id_propietario`, `tipo`, `direccion`, `uso`, `ambientes`, `superficie`, `latitud`, `precio`, `estado`, `imagen`) VALUES
(3, 2, 'Casa', 'Colón 800', 'Comercial', 3, 1, 2, 80000.00, 2, '/Uploads/Inmuebles/b17ea5c8-fd9c-4e0c-849b-c05f658b67b4.png'),
(4, 5, 'Casa', '2 de Abril', 'Comercial', 3, 0, 0, 80000.00, 1, '/Uploads/Inmuebles/a2870676-070b-40f6-b886-220b65251247.png'),
(5, 5, 'Casa', 'Calle acá', 'Comercial', 1, 0, 0, 9.00, 3, '/Uploads/Inmuebles/0464d96a-9970-46db-9f94-8dd48d71634e.png'),
(6, 5, 'Casa', 'aca', 'Residencial', 2, 1, 3, 2.00, 2, '/Uploads/Inmuebles/f1fd2fab-9c58-45ea-b962-3afc5c3776c7.png'),
(7, 5, 'Cas', 'san martin', 'Re', 2, 3, 3, 2.00, 3, '/Uploads/Inmuebles/64703b3c-3897-4161-b8f7-eec1779d166e.jpg'),
(8, 5, 'D', 'nueva', 'Re', 2, 3, 3, 3.00, 1, '/Uploads/Inmuebles/673abd7d-08b0-4ef9-a336-d168345c1d7d.jpg'),
(9, 5, 'Casa', 'Bolivar 790', 'Residencial', 1, 0, 0, 78000.00, 1, '/Uploads/Inmuebles/c01e24cf-ebc1-4d2b-93bd-99d9a40a95aa.jpg');

-- --------------------------------------------------------

--
-- Table structure for table `inquilino`
--

CREATE TABLE `inquilino` (
  `id` int(11) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `dni` varchar(20) NOT NULL,
  `email` varchar(100) DEFAULT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `estado` int(11) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `inquilino`
--

INSERT INTO `inquilino` (`id`, `nombre`, `apellido`, `dni`, `email`, `telefono`, `estado`) VALUES
(1, 'FAVER', 'CASTELL', '00000003', 'fc@gmail.com', '266400003', 1),
(2, 'CANDELA', 'ESCUDERO', '00000004', 'candela@gmail.com', '2664000005', 1);

-- --------------------------------------------------------

--
-- Table structure for table `pago`
--

CREATE TABLE `pago` (
  `id` int(11) NOT NULL,
  `id_contrato` int(11) NOT NULL,
  `nro_pago` int(11) NOT NULL,
  `fecha_pago` date DEFAULT NULL,
  `estado` enum('pendiente','recibido','anulado') NOT NULL DEFAULT 'pendiente',
  `concepto` varchar(255) NOT NULL,
  `importe` decimal(15,0) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `pago`
--

INSERT INTO `pago` (`id`, `id_contrato`, `nro_pago`, `fecha_pago`, `estado`, `concepto`, `importe`) VALUES
(1, 1, 1, '2025-09-26', 'recibido', 'Mes de Septiembre 2025', 6),
(2, 1, 2, '2025-10-16', 'recibido', 'Pago 2 | Mes de October 2025', 6),
(3, 1, 3, NULL, 'pendiente', 'Pago 3 | Mes de November 2025', NULL),
(4, 2, 1, '2025-11-07', 'pendiente', 'Mes de Octubre 2025', 7000),
(5, 2, 2, NULL, 'pendiente', 'Pago 2 | Mes de November 2025', NULL),
(6, 2, 3, NULL, 'pendiente', 'Pago 3 | Mes de December 2025', NULL),
(7, 2, 4, NULL, 'pendiente', 'Pago 4 | Mes de January 2026', NULL),
(8, 2, 5, NULL, 'pendiente', 'Pago 5 | Mes de February 2026', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `propietario`
--

CREATE TABLE `propietario` (
  `id` int(11) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `dni` varchar(20) NOT NULL,
  `email` varchar(100) DEFAULT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `estado` int(11) DEFAULT 1,
  `clave` varchar(512) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `propietario`
--

INSERT INTO `propietario` (`id`, `nombre`, `apellido`, `dni`, `email`, `telefono`, `estado`, `clave`) VALUES
(2, 'ROBERTA', 'VALLEJOS', '43690464', 'roberta.vallejos@gmail.com', '2664970148', 1, 'QbnGKZzJFTrDjJE0YzMxQLsBR8zghfpjFhBIU+5/6bE='),
(5, 'LOLA', 'YOUNG', '39562180', 'lola@gmail.com', '1132487681', 1, 'QbnGKZzJFTrDjJE0YzMxQLsBR8zghfpjFhBIU+5/6bE=');

-- --------------------------------------------------------

--
-- Table structure for table `usuario`
--

CREATE TABLE `usuario` (
  `id` int(11) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `dni` varchar(20) NOT NULL,
  `email` varchar(100) NOT NULL,
  `password` varchar(255) NOT NULL,
  `rol` enum('Admin','Empleado','Propietario') NOT NULL,
  `estado` int(11) NOT NULL DEFAULT 1,
  `avatar` varchar(2550) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `usuario`
--

INSERT INTO `usuario` (`id`, `nombre`, `apellido`, `dni`, `email`, `password`, `rol`, `estado`, `avatar`) VALUES
(1, 'ADMIN', '1', '00000001', 'admin@gmail.com', 'wDXKsKiK3VDizV00g01VpnFZXYEZ8AfOz23lnTm+iR4=', 'Admin', 1, 'https://ui-avatars.com/api/?name=ADMIN%201&background=343a40&color=fff&rounded=true&size=128'),
(2, 'EMPLEADO', '1', '00000002', 'empleado@gmail.com', '123', 'Empleado', 1, 'https://ui-avatars.com/api/?name=EMPLEADO%201&background=343a40&color=fff&rounded=true&size=128');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `auditoria`
--
ALTER TABLE `auditoria`
  ADD PRIMARY KEY (`id_auditoria`);

--
-- Indexes for table `contrato`
--
ALTER TABLE `contrato`
  ADD PRIMARY KEY (`id`),
  ADD KEY `id_inquilino` (`id_inquilino`),
  ADD KEY `id_inmueble` (`id_inmueble`);

--
-- Indexes for table `inmueble`
--
ALTER TABLE `inmueble`
  ADD PRIMARY KEY (`id`),
  ADD KEY `id_propietario` (`id_propietario`);

--
-- Indexes for table `inquilino`
--
ALTER TABLE `inquilino`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `dni` (`dni`),
  ADD UNIQUE KEY `email` (`email`);

--
-- Indexes for table `pago`
--
ALTER TABLE `pago`
  ADD PRIMARY KEY (`id`),
  ADD KEY `id_contrato` (`id_contrato`);

--
-- Indexes for table `propietario`
--
ALTER TABLE `propietario`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `dni` (`dni`),
  ADD UNIQUE KEY `email` (`email`);

--
-- Indexes for table `usuario`
--
ALTER TABLE `usuario`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `dni` (`dni`),
  ADD UNIQUE KEY `email` (`email`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `auditoria`
--
ALTER TABLE `auditoria`
  MODIFY `id_auditoria` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `contrato`
--
ALTER TABLE `contrato`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `inmueble`
--
ALTER TABLE `inmueble`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- AUTO_INCREMENT for table `inquilino`
--
ALTER TABLE `inquilino`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `pago`
--
ALTER TABLE `pago`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT for table `propietario`
--
ALTER TABLE `propietario`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `usuario`
--
ALTER TABLE `usuario`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `contrato`
--
ALTER TABLE `contrato`
  ADD CONSTRAINT `contrato_ibfk_1` FOREIGN KEY (`id_inquilino`) REFERENCES `inquilino` (`id`),
  ADD CONSTRAINT `contrato_ibfk_2` FOREIGN KEY (`id_inmueble`) REFERENCES `inmueble` (`id`);

--
-- Constraints for table `inmueble`
--
ALTER TABLE `inmueble`
  ADD CONSTRAINT `inmueble_ibfk_1` FOREIGN KEY (`id_propietario`) REFERENCES `propietario` (`id`);

--
-- Constraints for table `pago`
--
ALTER TABLE `pago`
  ADD CONSTRAINT `pago_ibfk_1` FOREIGN KEY (`id_contrato`) REFERENCES `contrato` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
