-- MySQL Script generated by MySQL Workbench
-- Sat May 11 10:25:27 2024
-- Model: New Model    Version: 1.0
-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `mydb` DEFAULT CHARACTER SET utf8 ;
USE `mydb` ;

-- -----------------------------------------------------
-- Table `mydb`.`Cliets`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Cliets` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(128) NOT NULL,
  `phoneNomber` VARCHAR(13) NOT NULL,
  `email` VARCHAR(128) NOT NULL,
  `unp` VARCHAR(45) NULL,
  `urAdress` VARCHAR(45) NULL,
  `bankAccount` VARCHAR(45) NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`VacuumInstallations`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`VacuumInstallations` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(45) NOT NULL,
  `description` TEXT(255) NOT NULL,
  `photoPath` VARCHAR(128) NULL,
  `price` DOUBLE(10,2) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`Orders`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Orders` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `Cliets_id` INT NOT NULL,
  `VacuumInstallations_id` INT NOT NULL,
  `createDate` DATE NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_Orders_Cliets_idx` (`Cliets_id` ASC) VISIBLE,
  INDEX `fk_Orders_VacuumInstallations1_idx` (`VacuumInstallations_id` ASC) VISIBLE,
  CONSTRAINT `fk_Orders_Cliets`
    FOREIGN KEY (`Cliets_id`)
    REFERENCES `mydb`.`Cliets` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Orders_VacuumInstallations1`
    FOREIGN KEY (`VacuumInstallations_id`)
    REFERENCES `mydb`.`VacuumInstallations` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`Agreements`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Agreements` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `Orders_id` INT NOT NULL,
  `conclusionDate` DATE NOT NULL,
  `price` DOUBLE(10,2) NOT NULL,
  `conditions` TEXT(255) NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_Agreements_Orders1_idx` (`Orders_id` ASC) VISIBLE,
  CONSTRAINT `fk_Agreements_Orders1`
    FOREIGN KEY (`Orders_id`)
    REFERENCES `mydb`.`Orders` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`Roles`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Roles` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`Users`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Users` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(45) NOT NULL,
  `Roles_id` INT NOT NULL,
  `pass` VARCHAR(255) NOT NULL,
  `Cliets_id` INT NOT NULL,
  PRIMARY KEY (`id`, `Cliets_id`),
  INDEX `fk_Users_Roles1_idx` (`Roles_id` ASC) VISIBLE,
  INDEX `fk_Users_Cliets1_idx` (`Cliets_id` ASC) VISIBLE,
  CONSTRAINT `fk_Users_Roles1`
    FOREIGN KEY (`Roles_id`)
    REFERENCES `mydb`.`Roles` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Users_Cliets1`
    FOREIGN KEY (`Cliets_id`)
    REFERENCES `mydb`.`Cliets` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
