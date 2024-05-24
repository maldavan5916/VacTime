-- -----------------------------------------------------
-- Schema VacTimeDB
-- -----------------------------------------------------

CREATE DATABASE VacTimeDB;
GO

USE VacTimeDB;
GO

-- -----------------------------------------------------
-- Table `VacTimeDB`.`Clients`
-- -----------------------------------------------------
CREATE TABLE Clients (
  id INT NOT NULL IDENTITY(1,1),
  name NVARCHAR(128) NOT NULL,
  phoneNomber NVARCHAR(13) NOT NULL,
  email NVARCHAR(128) NOT NULL,
  unp NVARCHAR(45) NULL,
  urAdress NVARCHAR(45) NULL,
  bankAccount NVARCHAR(45) NULL,
  PRIMARY KEY (id)
);


-- -----------------------------------------------------
-- Table `VacTimeDB`.`VacuumInstallations`
-- -----------------------------------------------------
CREATE TABLE VacuumInstallations (
  id INT NOT NULL IDENTITY(1,1),
  name NVARCHAR(45) NOT NULL,
  description NVARCHAR(MAX) NOT NULL,
  photoPath NVARCHAR(128) NULL,
  price DECIMAL(10,2) NOT NULL,
  visibility BIT NOT NULL DEFAULT 1,
  PRIMARY KEY (id)
);


-- -----------------------------------------------------
-- Table `VacTimeDB`.`Orders`
-- -----------------------------------------------------
CREATE TABLE Orders (
  id INT NOT NULL IDENTITY(1,1),
  Clients_id INT NOT NULL,
  VacuumInstallations_id INT NOT NULL,
  createDate DATE NOT NULL,
  pathOrder NVARCHAR(128),
  PRIMARY KEY (id),
  FOREIGN KEY (Clients_id)
    REFERENCES Clients (id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  FOREIGN KEY (VacuumInstallations_id)
    REFERENCES VacuumInstallations (id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);


-- -----------------------------------------------------
-- Table `VacTimeDB`.`Agreements`
-- -----------------------------------------------------
CREATE TABLE Agreements (
  id INT NOT NULL IDENTITY(1,1),
  Orders_id INT NOT NULL,
  conclusionDate DATE NOT NULL,
  price DECIMAL(10,2) NOT NULL,
  conditions NVARCHAR(255) NOT NULL,
  PRIMARY KEY (id),
  FOREIGN KEY (Orders_id)
    REFERENCES Orders (id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);


-- -----------------------------------------------------
-- Table `VacTimeDB`.`Roles`
-- -----------------------------------------------------
CREATE TABLE Roles (
  id INT NOT NULL IDENTITY(1,1),
  name NVARCHAR(45) NOT NULL,
  PRIMARY KEY (id)
);


-- -----------------------------------------------------
-- Table `VacTimeDB`.`Users`
-- -----------------------------------------------------
CREATE TABLE Users (
  id INT NOT NULL IDENTITY(1,1),
  name NVARCHAR(45) NOT NULL,
  Roles_id INT NOT NULL,
  pass NVARCHAR(255) NOT NULL,
  Clients_id INT NOT NULL,
  PRIMARY KEY (id, Clients_id),
  FOREIGN KEY (Roles_id)
    REFERENCES Roles (id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  FOREIGN KEY (Clients_id)
    REFERENCES Clients (id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);

-- Заполнение таблиц данными

-- Заполнение таблицы Clients
INSERT INTO Clients (name, phoneNomber, email, unp, urAdress, bankAccount)
VALUES 
('John Doe', '+123456789', 'john.doe@example.com', '1234567890', '123 Main St', '12345678901234567890'),
('Jane Smith', '+987654321', 'jane.smith@example.com', '0987654321', '456 Oak Ave', '09876543210987654321'),
('Michael Johnson', '+1122334455', 'michael.johnson@example.com', '1122334455', '789 Elm St', '11223344551122334455'),
('Emily Davis', '+9988776655', 'emily.davis@example.com', '9988776655', '321 Maple Ave', '99887766559988776655'),
('William Brown', '+5544332211', 'william.brown@example.com', '5544332211', '456 Pine St', '55443322115544332211');

-- Заполнение таблицы VacuumInstallations
INSERT INTO VacuumInstallations (name, description, photoPath, price)
VALUES 
('Model A', 'High-powered vacuum cleaner for industrial use', 'C:\VacuumPhoto\1.jpg', 999.99),
('Model B', 'Compact vacuum cleaner for household use', 'C:\VacuumPhoto\2.jpg', 299.99),
('Model C', 'Portable vacuum cleaner for car interiors', 'C:/VacuumPhoto/3.jpg', 149.99),
('Model D', 'Robotic vacuum cleaner with AI capabilities', 'C:\VacuumPhoto\4.jpg', 799.99),
('Model E', 'Industrial-grade vacuum cleaner for large spaces', '\photos\model_e.jpg', 1999.99);

-- Заполнение таблицы Orders
-- INSERT INTO Orders (Clients_id, VacuumInstallations_id, createDate)
-- VALUES 
-- (1, 1, '2024-05-11'),
-- (2, 2, '2024-05-10'),
-- (3, 3, '2024-05-09'),
-- (4, 4, '2024-05-08'),
-- (5, 5, '2024-05-07');

-- Заполнение таблицы Agreements
INSERT INTO Agreements (Orders_id, conclusionDate, price, conditions)
VALUES 
(1, '2024-05-11', 999.99, 'Standard warranty applies'), 
(2, '2024-05-10', 299.99, 'Extended warranty included'),
(3, '2024-05-09', 149.99, 'Limited warranty'),
(4, '2024-05-08', 799.99, 'Extended warranty included'),
(5, '2024-05-07', 1999.99, 'Full warranty coverage');

-- Заполнение таблицы Roles
INSERT INTO Roles (name)
VALUES 
('admin'),
('manager'),
('client');

-- Заполнение таблицы Users
-- INSERT INTO Users (name, Roles_id, pass, Clients_id)
-- VALUES 
-- ('admin', 1, 'admin', 1);

-- Вывод содержимого таблиц
SELECT * FROM Clients;
SELECT * FROM VacuumInstallations;
SELECT * FROM Orders;
SELECT * FROM Agreements;
SELECT * FROM Roles;
SELECT * FROM Users;