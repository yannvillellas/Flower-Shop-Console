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
        id_accessories    Int NOT NULL ,
        name_accessories  Varchar (50) NOT NULL ,
        price_accessories Int NOT NULL ,
        stock_accessories Int NOT NULL
	,CONSTRAINT Accessories_PK PRIMARY KEY (id_accessories)
)ENGINE=InnoDB;


#------------------------------------------------------------
# Table: Flowers
#------------------------------------------------------------

CREATE TABLE Flowers(
        id_flowers    Int NOT NULL ,
        name_flowers  Varchar (50) NOT NULL ,
        price_flowers Int NOT NULL ,
        stock_flowers Int NOT NULL
	,CONSTRAINT Flowers_PK PRIMARY KEY (id_flowers)
)ENGINE=InnoDB;


#------------------------------------------------------------
# Table: Shop
#------------------------------------------------------------

CREATE TABLE Shops(
        id_shop     Int NOT NULL ,
        name_shop   Varchar (50) NOT NULL ,
        city_shop   Varchar (50) NOT NULL ,
        address_shop Varchar (50) NOT NULL
	,CONSTRAINT Shops_PK PRIMARY KEY (id_shop)
)ENGINE=InnoDB;


#------------------------------------------------------------
# Table: Stock
#------------------------------------------------------------

CREATE TABLE Stocks(
        id_stock Int NOT NULL ,
        quantity Int NOT NULL ,
        id_shop  Int NOT NULL
	,CONSTRAINT Stocks_PK PRIMARY KEY (id_stock)
)ENGINE=InnoDB;


#------------------------------------------------------------
# Table: Clients
#------------------------------------------------------------

CREATE TABLE Clients(
        id_client     Int NOT NULL AUTO_INCREMENT,
        first_name    Varchar (50) NOT NULL ,
        last_name     Varchar (50) NOT NULL ,
        phone         Varchar (10) NOT NULL ,
        email         Varchar (100) NOT NULL ,
        password      Varchar (100) NOT NULL ,
        loyalty       Varchar (50) NOT NULL ,
        admin         Bool NOT NULL ,
        CONSTRAINT Clients_PK PRIMARY KEY (id_client)
)ENGINE=InnoDB;


#------------------------------------------------------------
# Table: Order
#------------------------------------------------------------

CREATE TABLE Orders(
        id_orders       Int NOT NULL AUTO_INCREMENT ,
        shipping_address Text NOT NULL ,
        message         Text NOT NULL ,
        delivery_date   Date NOT NULL ,
        order_date      Date NOT NULL ,
        status          Varchar (50) NOT NULL ,
        id_client       Int NOT NULL ,
        id_shop         Int NOT NULL ,
        id_address       Int NOT NULL
	,CONSTRAINT Orders_PK PRIMARY KEY (id_orders)
)ENGINE=InnoDB;


#------------------------------------------------------------
# Table: personalized
#------------------------------------------------------------

CREATE TABLE Personalizeds(
        id_personalized          Int NOT NULL AUTO_INCREMENT,
        price_personalized       Int NOT NULL ,
        description_personalized Varchar (400) NOT NULL ,
        flowers_personalized     Varchar (400) NOT NULL ,
        accesories_personalized  Varchar (400) NOT NULL ,
        id_orders                Int NOT NULL
	,CONSTRAINT personalizeds_PK PRIMARY KEY (id_personalized)
)ENGINE=InnoDB;


#------------------------------------------------------------
# Table: standard
#------------------------------------------------------------

CREATE TABLE Standards(
        id_standard          Int NOT NULL AUTO_INCREMENT,
        name_bouquet         Varchar (50) NOT NULL ,
        description_standard Varchar (50) NOT NULL ,
        price_standard       Int NOT NULL ,
        id_orders            Int NOT NULL
	,CONSTRAINT standards_PK PRIMARY KEY (id_standard)
)ENGINE=InnoDB;


#------------------------------------------------------------
# Table: Adress
#------------------------------------------------------------

CREATE TABLE Addresses(
        id_address         Int NOT NULL AUTO_INCREMENT ,
        first_name_address Varchar (50) NOT NULL ,
        last_name_address  Varchar (50) NOT NULL ,
        phone_address      Varchar (10) NOT NULL ,
        name_street       Varchar (100) NOT NULL ,
        city              Varchar (100) NOT NULL ,
        zipcode           Int NOT NULL ,
        number_street     Int NOT NULL ,
        id_orders         Int NOT NULL
	,CONSTRAINT Adresses_PK PRIMARY KEY (id_adress)
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




ALTER TABLE Stocks
	ADD CONSTRAINT Stocks_Shops0_FK
	FOREIGN KEY (id_shop)
	REFERENCES Shops(id_shop);

ALTER TABLE Clients
	ADD CONSTRAINT Clients_Addresses0_FK
	FOREIGN KEY (id_address)
	REFERENCES Adresses(id_address);

ALTER TABLE Orders
	ADD CONSTRAINT Orders_Clients0_FK
	FOREIGN KEY (id_client)
	REFERENCES Clients(id_client);

ALTER TABLE Orders
	ADD CONSTRAINT Orders_Shops1_FK
	FOREIGN KEY (id_shop)
	REFERENCES Shops(id_shop);

ALTER TABLE Orders
	ADD CONSTRAINT Orders_Addresses2_FK
	FOREIGN KEY (id_address)
	REFERENCES Adresses(id_address);

ALTER TABLE Orders 
	ADD CONSTRAINT Orders_Addresses0_AK
	FOREIGN KEY (id_address)
	REFERENCES Adresses(id_address);


