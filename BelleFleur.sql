#------------------------------------------------------------
#        Script MySQL.
#------------------------------------------------------------


#------------------------------------------------------------
# Table: Accessories
#------------------------------------------------------------
DROP DATABASE IF EXISTS Fleurs;
CREATE DATABASE Fleurs;

USE Fleurs;

CREATE TABLE Accessories(
        id_accessories    Int NOT NULL AUTO_INCREMENT,
        name_accessories  Varchar (50) NOT NULL ,
        price_accessories Int NOT NULL ,
        stock_accessories Int NOT NULL
	,CONSTRAINT Accessories_PK PRIMARY KEY (id_accessories)
)ENGINE=InnoDB;


#------------------------------------------------------------
# Table: Flowers
#------------------------------------------------------------

CREATE TABLE Flowers(
        id_flowers    Int NOT NULL AUTO_INCREMENT,
        name_flowers  Varchar (50) NOT NULL ,
        price_flowers Int NOT NULL ,
        stock_flowers Int NOT NULL
	,CONSTRAINT Flowers_PK PRIMARY KEY (id_flowers)
)ENGINE=InnoDB;


#------------------------------------------------------------
# Table: Shop
#------------------------------------------------------------

CREATE TABLE Shops(
        id_shops     Int NOT NULL AUTO_INCREMENT,
        name_shops   Varchar (50) NOT NULL ,
        city_shops   Varchar (50) NOT NULL ,
        address_shops Varchar (50) NOT NULL
	,CONSTRAINT Shops_PK PRIMARY KEY (id_shops)
)ENGINE=InnoDB;


#------------------------------------------------------------
# Table: Stock
#------------------------------------------------------------

CREATE TABLE Stocks(
        id_stocks Int NOT NULL AUTO_INCREMENT,
        quantity Int NOT NULL ,
        id_shops  Int NOT NULL
	,CONSTRAINT Stocks_PK PRIMARY KEY (id_stocks)
)ENGINE=InnoDB;


#------------------------------------------------------------
# Table: Clients
#------------------------------------------------------------

CREATE TABLE Clients(
        id_clients     Int NOT NULL AUTO_INCREMENT,
        first_name    Varchar (50) NOT NULL ,
        last_name     Varchar (50) NOT NULL ,
        phone         Varchar (10) NOT NULL ,
        email         Varchar (100) NOT NULL ,
        password      Varchar (100) NOT NULL ,
        loyalty       Varchar (50) NOT NULL ,
        admin         Bool NOT NULL ,
        id_addresses  Int NOT NULL,
        CONSTRAINT Clients_PK PRIMARY KEY (id_clients)
)ENGINE=InnoDB;


#------------------------------------------------------------
# Table: Orders
#------------------------------------------------------------

CREATE TABLE Orders(
        id_orders       Int NOT NULL AUTO_INCREMENT ,
        shipping_address Text NOT NULL ,
        message         Text NOT NULL ,
        delivery_date   Date NOT NULL ,
        order_date      Date NOT NULL ,
        status          Varchar (50) NOT NULL ,
        id_clients       Int NOT NULL ,
        id_shops         Int NOT NULL ,
        id_addresses       Int NOT NULL
	,CONSTRAINT Orders_PK PRIMARY KEY (id_orders)
)ENGINE=InnoDB;

    
#------------------------------------------------------------
# Table: personalized
#------------------------------------------------------------

CREATE TABLE Personalized(
        id_personalized          Int NOT NULL AUTO_INCREMENT,
        price_personalized       Int NOT NULL ,
        description_personalized Varchar (400) NOT NULL ,
        flowers_personalized     Varchar (400) NOT NULL ,
        accesories_personalized  Varchar (400) NOT NULL ,
        id_orders                Int NOT NULL
	,CONSTRAINT personalized_PK PRIMARY KEY (id_personalized)
)ENGINE=InnoDB;


#------------------------------------------------------------
# Table: standard
#------------------------------------------------------------

CREATE TABLE Standard(
        id_standard          Int NOT NULL AUTO_INCREMENT,
        name_bouquet         Varchar (50) NOT NULL ,
        description_standard Varchar (50) NOT NULL ,
        price_standard       Int NOT NULL ,
        id_orders            Int NOT NULL
	,CONSTRAINT standard_PK PRIMARY KEY (id_standard)
)ENGINE=InnoDB;


#------------------------------------------------------------
# Table: Adress
#------------------------------------------------------------

CREATE TABLE Addresses(
        id_addresses         Int NOT NULL AUTO_INCREMENT ,
        first_name_addresses Varchar (50) NOT NULL ,
        last_name_addresses  Varchar (50) NOT NULL ,
        phone_addresses      Varchar (10) NOT NULL ,
        city              	 Varchar (100) NOT NULL ,
        zip_code             Int NOT NULL ,
        street_number        Int NOT NULL ,
        street_name          Varchar (100) NOT NULL
	,CONSTRAINT Addresses_PK PRIMARY KEY (id_addresses)
)ENGINE=InnoDB;


#------------------------------------------------------------
# Table: Contains
#------------------------------------------------------------

CREATE TABLE Contains(
        id_accessories  Int NOT NULL ,
        id_personalized Int NOT NULL
	,CONSTRAINT Contains_PK PRIMARY KEY (id_accessories,id_personalized)
)ENGINE=InnoDB;


#------------------------------------------------------------
# Table: contain
#------------------------------------------------------------

CREATE TABLE contain(
        id_flowers  Int NOT NULL ,
        id_standard Int NOT NULL
	,CONSTRAINT contain_PK PRIMARY KEY (id_flowers,id_standard)
)ENGINE=InnoDB;

ALTER TABLE Orders
	ADD CONSTRAINT Orders_Clients0_FK
	FOREIGN KEY (id_clients)
	REFERENCES Clients(id_clients);

ALTER TABLE Orders
	ADD CONSTRAINT Orders_Shops1_FK
	FOREIGN KEY (id_shops)
	REFERENCES Shops(id_shops);

ALTER TABLE Orders
	ADD CONSTRAINT Orders_Addresses2_FK
	FOREIGN KEY (id_addresses)
	REFERENCES Addresses(id_addresses);

ALTER TABLE Orders 
	ADD CONSTRAINT Orders_Addresses0_AK
	FOREIGN KEY (id_addresses)
	REFERENCES Addresses(id_addresses);
    
ALTER TABLE Stocks
	ADD CONSTRAINT Stocks_Shops0_FK
	FOREIGN KEY (id_shops)
	REFERENCES Shops(id_shops);

ALTER TABLE Clients
	ADD CONSTRAINT Clients_Addresses0_FK
	FOREIGN KEY (id_addresses)
	REFERENCES Addresses(id_addresses);