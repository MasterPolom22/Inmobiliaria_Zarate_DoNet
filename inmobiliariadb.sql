-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 25-09-2025 a las 18:37:58
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmobiliariadb`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contrato`
--

CREATE TABLE `contrato` (
  `id` int(11) NOT NULL,
  `inmueble_id` int(11) NOT NULL,
  `inquilino_id` int(11) NOT NULL,
  `fecha_inicio` date NOT NULL,
  `fecha_fin_original` date NOT NULL,
  `monto_mensual` decimal(12,2) NOT NULL CHECK (`monto_mensual` > 0),
  `fecha_fin_anticipada` date DEFAULT NULL,
  `contrato_origen_id` int(11) DEFAULT NULL,
  `estado` enum('VIGENTE','FINALIZADO','RESCINDIDO') NOT NULL DEFAULT 'VIGENTE',
  `creado_por` int(11) NOT NULL,
  `terminado_por` int(11) DEFAULT NULL,
  `creado_en` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contrato`
--

INSERT INTO `contrato` (`id`, `inmueble_id`, `inquilino_id`, `fecha_inicio`, `fecha_fin_original`, `monto_mensual`, `fecha_fin_anticipada`, `contrato_origen_id`, `estado`, `creado_por`, `terminado_por`, `creado_en`) VALUES
(7, 18, 13, '2025-01-01', '2025-12-31', 15000.00, NULL, NULL, 'VIGENTE', 9, NULL, '2025-09-24 14:29:31'),
(8, 17, 15, '2025-03-24', '2026-02-24', 100000.00, NULL, NULL, 'VIGENTE', 9, NULL, '2025-09-24 14:35:06'),
(9, 16, 14, '2024-01-24', '2024-12-24', 200000.00, '2024-12-21', NULL, 'FINALIZADO', 9, 7, '2025-09-24 14:35:55'),
(10, 15, 16, '2025-07-24', '2026-09-24', 240000.00, '2026-08-24', NULL, 'VIGENTE', 9, NULL, '2025-09-24 14:36:31');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmueble`
--

CREATE TABLE `inmueble` (
  `id` int(11) NOT NULL,
  `propietario_id` int(11) NOT NULL,
  `tipo_id` int(11) NOT NULL,
  `uso` enum('RESIDENCIAL','COMERCIAL') NOT NULL,
  `direccion` varchar(200) NOT NULL,
  `ambientes` int(11) NOT NULL CHECK (`ambientes` >= 1),
  `latitud` decimal(10,0) DEFAULT NULL,
  `longitud` decimal(10,0) DEFAULT NULL,
  `precio_base` decimal(12,2) NOT NULL CHECK (`precio_base` >= 0),
  `disponible` tinyint(1) NOT NULL DEFAULT 1,
  `suspendido` tinyint(1) NOT NULL DEFAULT 0,
  `creado_en` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmueble`
--

INSERT INTO `inmueble` (`id`, `propietario_id`, `tipo_id`, `uso`, `direccion`, `ambientes`, `latitud`, `longitud`, `precio_base`, `disponible`, `suspendido`, `creado_en`) VALUES
(14, 17, 1, 'RESIDENCIAL', 'Av. Siempreviva 742', 2, -33, -66, 120000.00, 1, 0, '2025-09-24 13:55:50'),
(15, 16, 1, 'RESIDENCIAL', 'Calle San Martín 1001', 3, -99, -66, 200000.00, 1, 0, '2025-09-24 14:19:11'),
(16, 18, 3, 'COMERCIAL', 'Calle San Juan 606', 1, -66, -77, 240000.00, 1, 0, '2025-09-24 14:19:43'),
(17, 17, 1, 'RESIDENCIAL', 'Calle San Patricio 606', 4, -55, -88, 350000.00, 0, 1, '2025-09-24 14:20:16'),
(18, 16, 1, 'RESIDENCIAL', 'Av. Siem 888', 2, -88, -77, 35000.00, 1, 0, '2025-09-24 14:21:17');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilino`
--

CREATE TABLE `inquilino` (
  `id` int(11) NOT NULL,
  `dni` varchar(20) NOT NULL,
  `apellido` varchar(60) NOT NULL,
  `nombre` varchar(120) NOT NULL,
  `telefono` varchar(30) DEFAULT NULL,
  `email` varchar(120) DEFAULT NULL,
  `creado_en` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilino`
--

INSERT INTO `inquilino` (`id`, `dni`, `apellido`, `nombre`, `telefono`, `email`, `creado_en`) VALUES
(13, '39990021', 'Ibarra', 'Mauro', '2664600001', 'mauro.ibarra@demo.com', '2025-09-24 13:52:55'),
(14, '39990022', 'Quiroga', 'Nadia', '2664600002', 'nadia.quiroga@demo.com', '2025-09-24 13:53:20'),
(15, '39990023', 'Suarez', 'Pedro', '2664600003', 'pedro.suarez@demo.com', '2025-09-24 13:53:44'),
(16, '39990024', 'Tevez', 'Rocío', '2664600004', 'rocio.tevez@demo.com', '2025-09-24 13:54:08'),
(17, '39990025', 'Ulloa', 'Sergio', '2664600005', 'sergio.ulloa@demo.com', '2025-09-24 13:54:34');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pago`
--

CREATE TABLE `pago` (
  `id` int(11) NOT NULL,
  `contrato_id` int(11) NOT NULL,
  `numero` int(11) NOT NULL,
  `fecha` date NOT NULL,
  `detalle` varchar(200) NOT NULL,
  `importe` decimal(12,2) NOT NULL CHECK (`importe` >= 0),
  `anulado` tinyint(1) NOT NULL DEFAULT 0,
  `creado_por` int(11) NOT NULL,
  `anulado_por` int(11) DEFAULT NULL,
  `anulado_en` datetime DEFAULT NULL,
  `creado_en` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `pago`
--

INSERT INTO `pago` (`id`, `contrato_id`, `numero`, `fecha`, `detalle`, `importe`, `anulado`, `creado_por`, `anulado_por`, `anulado_en`, `creado_en`) VALUES
(5, 10, 1, '2025-09-24', 'se pago septiembre', 2500000.00, 0, 7, NULL, NULL, '2025-09-24 16:15:00'),
(6, 10, 2, '2025-10-24', 'pago mes de octubre', 25000.00, 1, 7, 7, '2025-09-25 09:57:34', '2025-09-24 16:28:10'),
(7, 10, 3, '2025-09-24', 'se realiza pago', 250000.00, 0, 7, NULL, NULL, '2025-09-24 16:45:39'),
(8, 9, 1, '2025-09-25', 'pago octubre', 250000.00, 0, 7, NULL, NULL, '2025-09-25 10:01:33');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietario`
--

CREATE TABLE `propietario` (
  `id` int(11) NOT NULL,
  `dni` varchar(20) NOT NULL,
  `apellido` varchar(60) NOT NULL,
  `nombre` varchar(60) NOT NULL,
  `telefono` varchar(30) DEFAULT NULL,
  `email` varchar(120) DEFAULT NULL,
  `activo` tinyint(1) NOT NULL DEFAULT 1,
  `creado_en` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietario`
--

INSERT INTO `propietario` (`id`, `dni`, `apellido`, `nombre`, `telefono`, `email`, `activo`, `creado_en`) VALUES
(15, '29990011', 'Testa', 'Ana', '2664500001', 'ana.testa@demo.com', 1, '2025-09-24 13:49:41'),
(16, '29990012', 'Paz', 'Bruno', '2664500002', 'bruno.paz@demo.com', 1, '2025-09-24 13:50:35'),
(17, '29990013', 'Neri', 'Celia', '2664500003', 'celia.neri@demo.com', 1, '2025-09-24 13:51:08'),
(18, '29990014', 'Soto', 'Diego', '2664500004', 'diego.soto@demo.com', 1, '2025-09-24 13:51:44'),
(19, '29990015', 'Vidal', 'Elsa', '2664500005', 'elsa.vidal@demo.com', 1, '2025-09-24 13:52:20');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tipo_inmueble`
--

CREATE TABLE `tipo_inmueble` (
  `id` int(11) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `activo` tinyint(1) NOT NULL DEFAULT 1,
  `creado_en` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tipo_inmueble`
--

INSERT INTO `tipo_inmueble` (`id`, `nombre`, `activo`, `creado_en`) VALUES
(1, 'Casa', 1, '2025-09-02 20:49:15'),
(2, 'Departamento', 1, '2025-09-02 20:49:15'),
(3, 'Local', 1, '2025-09-02 20:49:15'),
(5, 'Oficina', 1, '2025-09-25 12:58:15'),
(6, 'Galpón', 1, '2025-09-25 12:58:15'),
(7, 'Terreno', 1, '2025-09-25 12:58:15'),
(8, 'Cochera', 1, '2025-09-25 12:58:15'),
(9, 'Quinta', 1, '2025-09-25 12:58:15');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuario`
--

CREATE TABLE `usuario` (
  `id` int(11) NOT NULL,
  `nombre` varchar(60) NOT NULL,
  `apellido` varchar(60) NOT NULL,
  `email` varchar(120) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `rol` enum('ADMIN','EMPLEADO') NOT NULL,
  `activo` tinyint(1) NOT NULL DEFAULT 1,
  `creado_en` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuario`
--

INSERT INTO `usuario` (`id`, `nombre`, `apellido`, `email`, `password_hash`, `rol`, `activo`, `creado_en`) VALUES
(7, 'Lucas', 'Zarate', 'admin@gmail.com', '$2a$12$b6UA.517HZNjfv/iajhI9eOqQ8plMpoJtx8l4l2Rl05DAk7Hm5KLa', 'ADMIN', 1, '2025-09-19 18:16:26'),
(8, 'Hugo', 'Flores', 'admin1@gmail.com', '$2a$12$lcXgPVOj8AeTt4z6nGK4TOHNx4WgLgDazskiwftxdhornMnje6ysG', 'ADMIN', 1, '2025-09-19 18:16:54'),
(9, 'Lucía', 'Flores', 'empleado1@gmail.com', '$2a$12$3UZhappbQyjLhejNg32weOLoR2LPW6oa2z2OVGGZPG7qx8lw/k3Gi', 'EMPLEADO', 1, '2025-09-20 19:53:07'),
(10, 'Daniela', 'Zavala', 'empleado2@gmail.com', '$2a$12$sCvY/BbOR/1fK/QPG5qFlejzhMUFmaJihmYIUeig7GsResx2Xvfi6', 'EMPLEADO', 1, '2025-09-20 21:08:39');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `contrato`
--
ALTER TABLE `contrato`
  ADD PRIMARY KEY (`id`),
  ADD KEY `inmueble_id` (`inmueble_id`),
  ADD KEY `inquilino_id` (`inquilino_id`);

--
-- Indices de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD PRIMARY KEY (`id`),
  ADD KEY `propietario_id` (`propietario_id`),
  ADD KEY `tipo_id` (`tipo_id`);

--
-- Indices de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `dni` (`dni`);

--
-- Indices de la tabla `pago`
--
ALTER TABLE `pago`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `ux_pago_contrato_numero` (`contrato_id`,`numero`),
  ADD KEY `contrato_id` (`contrato_id`);

--
-- Indices de la tabla `propietario`
--
ALTER TABLE `propietario`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `dni` (`dni`);

--
-- Indices de la tabla `tipo_inmueble`
--
ALTER TABLE `tipo_inmueble`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `nombre` (`nombre`);

--
-- Indices de la tabla `usuario`
--
ALTER TABLE `usuario`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `email` (`email`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `contrato`
--
ALTER TABLE `contrato`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=19;

--
-- AUTO_INCREMENT de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=18;

--
-- AUTO_INCREMENT de la tabla `pago`
--
ALTER TABLE `pago`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT de la tabla `propietario`
--
ALTER TABLE `propietario`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=20;

--
-- AUTO_INCREMENT de la tabla `tipo_inmueble`
--
ALTER TABLE `tipo_inmueble`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT de la tabla `usuario`
--
ALTER TABLE `usuario`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contrato`
--
ALTER TABLE `contrato`
  ADD CONSTRAINT `contrato_ibfk_1` FOREIGN KEY (`inmueble_id`) REFERENCES `inmueble` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `contrato_ibfk_2` FOREIGN KEY (`inquilino_id`) REFERENCES `inquilino` (`id`) ON DELETE CASCADE;

--
-- Filtros para la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD CONSTRAINT `inmueble_ibfk_1` FOREIGN KEY (`propietario_id`) REFERENCES `propietario` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `inmueble_ibfk_2` FOREIGN KEY (`tipo_id`) REFERENCES `tipo_inmueble` (`id`) ON DELETE CASCADE;

--
-- Filtros para la tabla `pago`
--
ALTER TABLE `pago`
  ADD CONSTRAINT `pago_ibfk_1` FOREIGN KEY (`contrato_id`) REFERENCES `contrato` (`id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
