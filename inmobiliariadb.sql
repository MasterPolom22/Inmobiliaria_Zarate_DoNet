-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 21-09-2025 a las 02:11:42
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.0.30

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
(2, 7, 4, '2025-09-09', '2026-09-09', 2500.00, NULL, NULL, 'VIGENTE', 1, NULL, '2025-09-09 13:34:28');

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
(2, 2, 2, 'RESIDENCIAL', 'Calle Falsa 202', 2, -33, -66, 150000.00, 1, 0, '2025-09-02 20:49:35'),
(3, 3, 3, 'COMERCIAL', 'Av. Central 303', 5, -33, -66, 500000.00, 1, 0, '2025-09-02 20:49:35'),
(4, 4, 1, 'RESIDENCIAL', 'Calle Libertad 404', 4, -33, -66, 250000.00, 1, 0, '2025-09-02 20:49:35'),
(5, 5, 2, 'RESIDENCIAL', 'Av. Mitre 505', 3, -33, -66, 180000.00, 1, 0, '2025-09-02 20:49:35'),
(6, 6, 3, 'COMERCIAL', 'Calle San Juan 606', 6, -33, -66, 600000.00, 1, 0, '2025-09-02 20:49:35'),
(7, 7, 1, 'RESIDENCIAL', 'Av. Belgrano 707', 2, -33, -66, 120000.00, 1, 0, '2025-09-02 20:49:35'),
(8, 8, 2, 'RESIDENCIAL', 'Calle Rivadavia 808', 3, -33, -66, 160000.00, 1, 0, '2025-09-02 20:49:35'),
(9, 9, 3, 'COMERCIAL', 'Av. Córdoba 909', 4, -33, -66, 450000.00, 1, 0, '2025-09-02 20:49:35'),
(10, 10, 1, 'RESIDENCIAL', 'Calle San Martín 1001', 5, -33, -66, 300000.00, 1, 0, '2025-09-02 20:49:35'),
(11, 10, 1, 'RESIDENCIAL', 'Av. Siempreviva 742', 1, NULL, NULL, 2500000.00, 1, 0, '2025-09-04 14:42:02'),
(12, 6, 1, 'RESIDENCIAL', 'sdsa', 1, NULL, NULL, 0.00, 1, 0, '2025-09-04 14:42:42');

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
(1, '30000001', 'Díaz', 'Martín', '2664110001', 'martin1@correo.com', '2025-09-02 20:49:15'),
(2, '30000002', 'García', 'Valentina', '2664110002', 'valentina2@correo.com', '2025-09-02 20:49:15'),
(3, '30000003', 'Rojas', 'Federico', '2664110003', 'federico3@correo.com', '2025-09-02 20:49:15'),
(4, '30000004', 'Alvarez', 'Camila', '2664110004', 'camila4@correo.com', '2025-09-02 20:49:15'),
(5, '30000005', 'Romero', 'Santiago', '2664110005', 'santiago5@correo.com', '2025-09-02 20:49:15'),
(6, '30000006', 'Vega', 'Julieta', '2664110006', 'julieta6@correo.com', '2025-09-02 20:49:15'),
(7, '30000007', 'Molina', 'Diego', '2664110007', 'diego7@correo.com', '2025-09-02 20:49:15'),
(8, '30000008', 'Gutiérrez', 'Luciana', '2664110008', 'luciana8@correo.com', '2025-09-02 20:49:15'),
(9, '30000009', 'Herrera', 'Tomás', '2664110009', 'tomas9@correo.com', '2025-09-02 20:49:15'),
(10, '30000010', 'Castro', 'Paula', '2664110010', 'paula10@correo.com', '2025-09-02 20:49:15');

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
  `creado_en` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

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
(1, '20000001', 'Pérez', 'Ana', '2664000001', 'ana1@correo.com', 1, '2025-09-02 20:49:15'),
(2, '20000002', 'Gómez', 'Juan', '2664000002', 'juan2@correo.com', 1, '2025-09-02 20:49:15'),
(3, '20000003', 'López', 'María', '2664000003', 'maria3@correo.com', 1, '2025-09-02 20:49:15'),
(4, '20000004', 'Martínez', 'Carlos', '2664000004', 'carlos4@correo.com', 1, '2025-09-02 20:49:15'),
(5, '20000005', 'Rodríguez', 'Laura', '2664000005', 'laura5@correo.com', 1, '2025-09-02 20:49:15'),
(6, '20000006', 'Fernández', 'Miguel', '2664000006', 'miguel6@correo.com', 1, '2025-09-02 20:49:15'),
(7, '20000007', 'Sánchez', 'Lucía', '2664000007', 'lucia7@correo.com', 1, '2025-09-02 20:49:15'),
(8, '20000008', 'Torres', 'Diego', '2664000008', 'diego8@correo.com', 1, '2025-09-02 20:49:15'),
(9, '20000009', 'Ramírez', 'Sofía', '2664000009', 'sofia9@correo.com', 1, '2025-09-02 20:49:15'),
(10, '20000010', 'Flores', 'Javier', '2664000010', 'javier10@correo.com', 1, '2025-09-02 20:49:15');

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
(3, 'Local', 1, '2025-09-02 20:49:15');

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
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT de la tabla `pago`
--
ALTER TABLE `pago`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `propietario`
--
ALTER TABLE `propietario`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT de la tabla `tipo_inmueble`
--
ALTER TABLE `tipo_inmueble`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

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
