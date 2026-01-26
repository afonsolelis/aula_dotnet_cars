-- Script de inicializacao do banco de dados Volkswagen Dashboard
-- Este script cria as tabelas e popula com dados de teste

-- Tabela de usuarios
CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabela de carros
CREATE TABLE IF NOT EXISTS volksdatatable (
    id SERIAL PRIMARY KEY,
    carname VARCHAR(100) NOT NULL,
    car_date_release DATE
);

-- Tabela de whitelist de emails permitidos para registro
CREATE TABLE IF NOT EXISTS email_whitelist (
    id SERIAL PRIMARY KEY,
    email VARCHAR(255) NOT NULL UNIQUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- =============================================
-- DADOS DE TESTE
-- =============================================

-- Whitelist de emails permitidos para registro
INSERT INTO email_whitelist (email) VALUES
    ('admin@vw.com'),
    ('teste@vw.com'),
    ('aluno@inteli.edu.br'),
    ('professor@inteli.edu.br'),
    ('dev@volkswagen.com.br')
ON CONFLICT (email) DO NOTHING;

-- Usuario padrao de teste (senha: admin123 em MD5)
-- MD5 de 'admin123' = 0192023a7bbd73250516f069df18b500
INSERT INTO users (username, email, password) VALUES
    ('Administrador', 'admin@vw.com', '0192023a7bbd73250516f069df18b500')
ON CONFLICT (email) DO NOTHING;

-- Carros de teste
INSERT INTO volksdatatable (carname, car_date_release) VALUES
    ('Gol', '1980-05-15'),
    ('Polo', '2002-03-10'),
    ('Golf', '1974-05-01'),
    ('Jetta', '1979-06-01'),
    ('Tiguan', '2007-09-01'),
    ('T-Cross', '2019-04-01'),
    ('Nivus', '2020-06-01'),
    ('Amarok', '2010-01-01'),
    ('Virtus', '2018-02-01'),
    ('Saveiro', '1982-08-01')
ON CONFLICT DO NOTHING;

-- =============================================
-- INFORMACOES DE ACESSO
-- =============================================
-- Usuario: admin@vw.com
-- Senha: admin123
-- =============================================
