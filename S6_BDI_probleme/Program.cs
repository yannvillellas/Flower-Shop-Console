﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using MySql.Data.MySqlClient;
using S6_BDI_probleme;

string connectionStringAdmin = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=root;PASSWORD=root;";
string connectionStringUser = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=bozo;PASSWORD=bozo;";
string connectionString = connectionStringUser;
Console.WriteLine("Do you want to connect as admin or bozo?");
string user = Console.ReadLine();
if (user == "admin")
{
    connectionString = connectionStringAdmin;
    Console.WriteLine("You are connected as admin");
}
else
{
    Console.WriteLine("You are connected as bozo");
}
MySqlConnection sqlConnection = new(connectionString);
using (MySqlConnection connection = sqlConnection)
{
    connection.Open();
    string email = "";
    authentication();
    //TODO: Ajouter bool admin dans la table clients et vérifier si l'utilisateur est admin pour stats
    menu();

    void authentication()
    {
        Console.Write("Enter email: ");
        email = Console.ReadLine();

        // Check if user already exists
        int count;
        string selectQuery = "SELECT COUNT(*) FROM clients WHERE email = @email";
        using (MySqlCommand command = new(selectQuery, connection))
        {
            command.Parameters.AddWithValue("@email", email);
            count = Convert.ToInt32(command.ExecuteScalar());
        }

        if (count == 1)
        {
            login();
        }
        else
        {
            Console.Write("User does not exist. Create account? (Y/N) ");
            string answer = Console.ReadLine().ToLower();
            
            if (answer == "y")
            {
                Client newClient = new Client(email);
                createClient(newClient);
            }
            else
            {
                Console.WriteLine("Try another email. Press any key to continue.");
                Console.ReadKey();
                Console.Clear();
                authentication();
            }
        }
    }
    void login()
    {
        int attempts = 3;
        bool loggedIn = false;
        while (attempts > 0 && !loggedIn)
        {
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            // Check if password matches
            string selectPasswordQuery = "SELECT COUNT(*) FROM clients WHERE email = @email AND password = @password";
            int passwordCount;
            using (MySqlCommand command = new(selectPasswordQuery, connection))
            {
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", password);
                passwordCount = Convert.ToInt32(command.ExecuteScalar());
            }

            if (passwordCount == 1)
            {
                Console.WriteLine("Login successful.");
                loggedIn = true;
            }
            else
            {
                Console.WriteLine("Incorrect password. " + --attempts + " attempt(s) left.");
                Console.WriteLine("Do you want to enter another email? (Y/N) ");
                string answer = Console.ReadLine().ToLower();
                if (answer == "y")
                {
                    Console.Clear();
                    authentication();
                }
            }
        }
        if (!loggedIn)
        {
            Console.WriteLine("Max attempts exceeded. Exiting program.");
            Environment.Exit(0);
        }
    }
    void createClient(Client newClient)
    {
        Console.Clear();
        Console.Write("Enter first name: ");
        newClient.FirstName = Console.ReadLine();
        Console.Write("Enter last name: ");
        newClient.LastName = Console.ReadLine();
        Console.Write("Enter phone number: ");
        newClient.PhoneNumber = Console.ReadLine();
        Console.Write("Enter password: ");
        newClient.Password = Console.ReadLine();
        Console.Write("Enter address. Press any key to continue.");
        Console.ReadKey();
        enterAddress(newClient);
        string selectQuery = "SELECT id_addresses FROM addresses ORDER BY id_addresses DESC LIMIT 1";
        using (MySqlCommand command = new(selectQuery, connection))
        {
            newClient.BillingAddressID = Convert.ToInt32(command.ExecuteScalar());
        }

        string insertQuery = "INSERT INTO clients (first_name, last_name, phone, email, password, loyalty, admin, id_addresses)" +
            "\r\nVALUES (@firstName, @lastName, @phoneNumber, @email, @password, @loyaltyStatus, @adminStatus, @billingAddressID);";
        using (MySqlCommand command = new(insertQuery, connection))
        {
            command.Parameters.AddWithValue("@firstName", newClient.FirstName);
            command.Parameters.AddWithValue("@lastName", newClient.LastName);
            command.Parameters.AddWithValue("@phoneNumber", newClient.PhoneNumber);
            command.Parameters.AddWithValue("@email", newClient.Email);
            command.Parameters.AddWithValue("@password", newClient.Password);
            command.Parameters.AddWithValue("@loyaltyStatus", newClient.Loyalty);
            command.Parameters.AddWithValue("@adminStatus", newClient.Admin);
            command.Parameters.AddWithValue("@billingAddressID", newClient.BillingAddressID);

            
            debugRowsAffected(command);
        }
        Console.WriteLine("Account created. Press any key to continue.");
        Console.ReadKey();
        Console.Clear();
        authentication();
    }
    void enterAddress(Client newClient)
    {
        Console.Clear();
        Console.Write("Enter city: ");
        string city = Console.ReadLine();
        Console.Write("Enter zip code: ");
        string zipCode = Console.ReadLine();
        Console.Write("Enter street number: ");
        string streetNumber = Console.ReadLine();
        Console.Write("Enter street name: ");
        string streetName = Console.ReadLine();

        string insertQuery = "INSERT INTO addresses (first_name_addresses, last_name_addresses, phone_addresses, city, zip_code, street_name, street_number)" +
            "\r\nVALUES (@firstName, @lastName, @phone, @city, @zipCode, @streetName, @streetNumber);";
        using (MySqlCommand command = new(insertQuery, connection))
        {
            command.Parameters.AddWithValue("@firstName", newClient.FirstName);
            command.Parameters.AddWithValue("@lastName", newClient.LastName);
            command.Parameters.AddWithValue("@phone", newClient.PhoneNumber);
            command.Parameters.AddWithValue("@city", city);
            command.Parameters.AddWithValue("@zipCode", zipCode);
            command.Parameters.AddWithValue("@streetName", streetName);
            command.Parameters.AddWithValue("@streetNumber", streetNumber);
            debugRowsAffected(command);
        }
        Console.WriteLine("Address created. Press any key to continue.");
        Console.ReadKey();
        Console.Clear();
    }
    void debugRowsAffected(MySqlCommand command)
    {
        int numRowsAffected = command.ExecuteNonQuery();
        Console.WriteLine($"{numRowsAffected} row(s) affected.");
    }
    void menu()
    {
        Console.Clear();
        Console.WriteLine("1. Display existing bouquets of flowers");
        Console.WriteLine("2. Personnalize your bouquet");
        Console.WriteLine("3. Display your order history");
        Console.WriteLine("4. Display your loyalty status");
        Console.WriteLine("5. Exit");
        Console.Write("Enter your choice: ");
        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                displayBouquets();
                break;
            case "2":
                personalizeBouquet();
                break;
            case "3":
                displayOrderHistory();
                break;
            case "4":
                displayLoyaltyStatus();
                break;
            case "5":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Invalid choice. Press any key to continue.");
                Console.ReadKey();
                Console.Clear();
                menu();
                break;
        }
    }
    void displayBouquets()
    {

    }
    void personalizeBouquet()
    {
        Console.Clear();
        Bouquet personalizedBouquet = new();
        Console.WriteLine("1. Choose flower(s)");
        Console.WriteLine("2. Choose accessorie(s)");
        Console.WriteLine("3. Enter your budget");
        Console.WriteLine("4. Enter a special request");
        Console.WriteLine("5. Validate your bouquet - Be sure to have chosen at least one flower and one accessorie and to have entered a budget. You will not be able to modify your bouquet after validation.");
        Console.WriteLine("6. Exit");
        Console.Write("Enter your choice: ");
        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                choosestrings();
                break;
            case "2":
                chooseAccessories();
                break;
            case "3":
                enterBudget(personalizedBouquet);
                break;
            case "4":
                enterSpecialRequest();
                break;
            case "5":
                validateBouquet(personalizedBouquet);
                break;
            case "6":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Invalid choice. Press any key to continue.");
                Console.ReadKey();
                Console.Clear();
                personalizeBouquet();
                break;
        }
    }
    void displayOrderHistory()
    {
        
    }
    void displayLoyaltyStatus()
    {
        
    }
    void choosestrings()
    {
        
    }
    void chooseAccessories()
    {

    }
    void enterBudget(Bouquet personalizedBouquet)
    {
        Console.Clear();
        Console.Write("Enter your budget: ");
        string budget = Console.ReadLine();
        double budgetDouble=0.0;
        try
        {
            budgetDouble = Convert.ToDouble(budget);
        }
        catch (FormatException)
        {
            Console.WriteLine("Invalid budget. You must enter a number. If you want decimals, use a comma instead of a dot. Press any key to continue.");
            Console.ReadKey();
            enterBudget(personalizedBouquet);
        }
        personalizedBouquet.Price = budgetDouble;
        Console.WriteLine("Budget entered. Press any key to continue.");
        Console.ReadKey();
        personalizeBouquet();
    }
    void enterSpecialRequest()
    {
        Console.Write("Enter your special request: ");
        string specialRequest = Console.ReadLine();
    }
    void validateBouquet(Bouquet personalizedBouquet)
    {
        if (personalizedBouquet.strings.Count == 0 || personalizedBouquet.Accessories.Count == 0 || personalizedBouquet.Price == 0.0)
        {
            Console.WriteLine("You must choose at least one flower and one accessorie and enter a budget. Press any key to continue.");
            Console.ReadKey();
            personalizeBouquet();
        }
        else
        {
            Console.WriteLine("Bouquet validated. Press any key to continue.");
            Console.ReadKey();
            menu();
        }
    }
}
Console.WriteLine("Press any key to exit.");
Console.ReadKey();