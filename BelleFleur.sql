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
        stock_flowers Int NOT NULL,
        start_month  Int NOT NULL,
        end_month  Int NOT NULL
	,CONSTRAINT Flowers_PK PRIMARY KEY (id_flowers)
)ENGINE=InnoDB;


#------------------------------------------------------------
# Table: Shop
#------------------------------------------------------------

CREATE TABLE Shops(
        id_shops     Int NOT NULL AUTO_INCREMENT,
        city_shops   Varchar (50) NOT NULL
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
        id_orders        Int NOT NULL AUTO_INCREMENT ,
        message          Text NOT NULL ,
        order_date       Date NOT NULL ,
        delivery_date    Date NOT NULL ,
        status           Varchar (50) NOT NULL ,
        id_clients       Int NOT NULL ,
        id_addresses     Int NOT NULL ,
        id_shops         Int NOT NULL,
        id_standard 	 Int,
        id_personalized  Int
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
        accessories_personalized  Varchar (400) NOT NULL 
	,CONSTRAINT personalized_PK PRIMARY KEY (id_personalized)
)ENGINE=InnoDB;


#------------------------------------------------------------
# Table: standard
#------------------------------------------------------------

CREATE TABLE Standard(
        id_standard          Int NOT NULL AUTO_INCREMENT,
        name_bouquet         Varchar (50) NOT NULL ,
        description_standard Varchar (500) NOT NULL ,
        price_standard       Int NOT NULL ,
        category             Varchar(50)
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
	ADD CONSTRAINT Orders_Standard3_FK
	FOREIGN KEY (id_standard)
	REFERENCES Standard(id_standard);
ALTER TABLE Orders
	ADD CONSTRAINT Orders_Personalized4_FK
	FOREIGN KEY (id_personalized)
	REFERENCES Personalized(id_personalized);
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
    
INSERT INTO Addresses (id_addresses, first_name_addresses, last_name_addresses, phone_addresses, city, zip_code, street_number, street_name)
VALUE(1, "a", "a", "a", "a", 1, 1, "a");
INSERT INTO Clients (id_clients, first_name, last_name, phone, email, password, loyalty, admin, id_addresses)
VALUE(1, "a", "a", "a" , "a", "a", "a", true, 1);
INSERT INTO Standard (id_standard, name_bouquet, description_standard, price_standard, category)
VALUES 
(1,"Gros Merci", "Arrangement floral avec marguerites et verdure",45,"Toute ocassion"),
(2,"L’amoureux","Arrangement floral avec roses blanches et roses rouges",65,"St_Valentin"),
(3,"L'Exotique","Arrangement floral avec ginger, oiseaux du paradis,roses et genet", 40, "Toute ocassion"),
(4,"Maman", "Arrangement floral avec gerbera, roses blanches, lys et alstroméria",80,"Fête des mères"),
(5,"Vive la mariée","Arrangement floral avec lys et orchidées", 120, "Mariage");
INSERT INTO Shops (id_shops, city_shops)
VALUES
(1, "Castres"),
(2, "Toulouse"),
(3, "Albi"),
(4, "Montpellier"),
(5, "Paris"),
(6, "Lyon"),
(7, "Marseille"),
(8, "Bordeaux"),
(9, "Nantes"),
(10, "Lille");
INSERT INTO Flowers (name_flowers,price_flowers,stock_flowers,start_month,end_month) 
VALUES
("Gerbera", 5, 100,1,12),
("Ginger",4, 100,1,12),
("Glaïeul",1 , 100,5,11),
("Marguerite", 2.25 , 100,1,12),
("Rose rouge", 2.50 ,100,1,12),
("Orchidée",8 ,100,1,12), 
("Tulipe", 1.50 , 100,3,5),
("Lys" , 3.50 ,100,1,12),
("Jacinthe",1.75,100,12,3),
("Œillet",2, 100,1,12),
("Pivoine",6 , 100,5,7),
("Iris",2.75,100,3,6),
("Souci", 1.25,100,6,9),
("Œillet d'Inde",1.50,100, 6,10),
("Primevère", 1.50 , 100,2,4);
INSERT INTO Accessories( name_accessories, price_accessories,stock_accessories)
VALUES
('Vase en céramique', 15.99, 50),
('Ruban décoratif', 2.99, 100),
('Pince à fleurs', 4.99, 75),
('Cache-pot en rotin', 9.99, 40),
("Sachet d'engrais pour fleurs", 6.99, 200);

CREATE USER 'bozo'@'localhost' IDENTIFIED BY 'bozo';
GRANT SELECT ON fleurs.* TO 'bozo'@'localhost';