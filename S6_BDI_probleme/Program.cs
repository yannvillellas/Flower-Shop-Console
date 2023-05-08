using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1;
using S6_BDI_probleme;

string connectionStringAdmin = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=root;PASSWORD=root;";
string connectionStringUser = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=bozo;PASSWORD=bozo;";
string connectionString = connectionStringUser;
string email = "";
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
    authentication();
    //TODO: Ajouter bool admin dans la table clients et vérifier si l'utilisateur est admin pour stats
    menu();


    void authentication()
    {
        Console.Clear();
        Console.WriteLine("AUTHENTICATION");
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
        Console.WriteLine("CREATE CLIENT");
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
            // debugRowsAffected(command);
        }
        Console.WriteLine("Account created. Press any key to continue.");
        Console.ReadKey();
        Console.Clear();
        authentication();
    }
    void enterAddress(Client user, bool isClient = true)
    {
        Console.Clear();
        Console.WriteLine("CREATE ADDRESS");
        if (!isClient)
        {
            Console.Write("Enter the first name of the recipient: ");
            user.FirstName = Console.ReadLine();
            Console.Write("Enter the last name of the recipient: ");
            user.LastName = Console.ReadLine();
            Console.Write("Enter the phone number of the recipient: ");
            user.PhoneNumber = Console.ReadLine();
        }
        Console.Write("Enter city: ");
        string city = Console.ReadLine();
        Console.Write("Enter zip code: ");
        string zipCode = Console.ReadLine();
        Console.Write("Enter street number: ");
        string streetNumber = Console.ReadLine();
        Console.Write("Enter street name: ");
        string streetName = Console.ReadLine();

        string insertQuery = "INSERT INTO addresses (first_name_addresses, last_name_addresses, phone_addresses, city, zip_code, street_number, street_name)" +
            "\r\nVALUES (@firstName, @lastName, @phone, @city, @zipCode, @streetNumber, @streetName);";
        using (MySqlCommand command = new(insertQuery, connection))
        {
            command.Parameters.AddWithValue("@firstName", user.FirstName);
            command.Parameters.AddWithValue("@lastName", user.LastName);
            command.Parameters.AddWithValue("@phone", user.PhoneNumber);
            command.Parameters.AddWithValue("@city", city);
            command.Parameters.AddWithValue("@zipCode", zipCode);
            command.Parameters.AddWithValue("@streetNumber", streetNumber);
            command.Parameters.AddWithValue("@streetName", streetName);
            // debugRowsAffected(command);
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
        Console.WriteLine("MENU OPTIONS");
        Console.WriteLine("1. Order existing bouquet of flowers");
        Console.WriteLine("2. Personnalize your bouquet");
        Console.WriteLine("3. Display your order history");
        Console.WriteLine("4. Display your loyalty status");
        Console.WriteLine("0. Disconnect");
        Console.Write("Enter your choice: ");
        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                orderBouquet();
                break;
            case "2":
                Bouquet personalizedBouquet = new();
                personalizeBouquet(personalizedBouquet);
                break;
            case "3":
                displayOrderHistory();
                break;
            case "4":
                displayLoyaltyStatus();
                break;
            case "0":
                authentication();
                break;
            default:
                Console.WriteLine("Invalid choice. Press any key to continue.");
                Console.ReadKey();
                Console.Clear();
                menu();
                break;
        }
    }
    void orderBouquet()
    {
        Console.Clear();
        Console.WriteLine("ORDER BOUQUET");
        string selectQuery = "SELECT * FROM standard;";
        using (MySqlCommand command = new(selectQuery, connection))
        {
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["id_standard"]}. {reader["name_bouquet"]} - {reader["category"]}: {reader["description_standard"]} - {reader["price_standard"]}$");
                }
            }
            Console.WriteLine("0. Return to menu");
            Console.Write("Enter the id of the bouquet you want to order: ");
            int idStandard = Convert.ToInt32(Console.ReadLine());
            if (idStandard == 0)
            {
                menu();
            }
            else
            {
                createOrder(false, 0, idStandard);
            }
        }
    }
    void personalizeBouquet(Bouquet personalizedBouquet)
    {
        Console.Clear();
        Console.WriteLine("PERSONALIZE BOUQUET");
        Console.WriteLine("1. Choose flower(s)");
        Console.WriteLine("2. Choose accessorie(s)");
        Console.WriteLine("3. Enter your budget");
        Console.WriteLine("4. Enter a special request");
        Console.WriteLine("5. Validate your bouquet - Be sure to have chosen at least one flower and one accessorie and to have entered a budget. You will not be able to modify your bouquet after validation.");
        Console.WriteLine("0. Return to menu");
        Console.Write("Enter your choice: ");
        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                chooseFlowers(personalizedBouquet);
                break;
            case "2":
                chooseAccessories(personalizedBouquet);
                break;
            case "3":
                enterBudget(personalizedBouquet);
                break;
            case "4":
                enterSpecialRequest(personalizedBouquet);
                break;
            case "5":
                validateBouquet(personalizedBouquet);
                break;
            case "0":
                menu();
                break;
            default:
                Console.WriteLine("Invalid choice. Press any key to continue.");
                Console.ReadKey();
                Console.Clear();
                personalizeBouquet(personalizedBouquet);
                break;
        }
    }
    void displayOrderHistory()
    {
        
    }
    void displayLoyaltyStatus()
    {
        
    }
    void chooseFlowers(Bouquet personalizedBouquet)
    {
        Console.Clear();
        Console.WriteLine("CHOOSE FLOWER(S)");
        string selectQuery = "SELECT * FROM flowers WHERE stock_flowers > 0;"; // here change with the date of availability too
        using (MySqlCommand command = new(selectQuery, connection))
        {
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["id_flowers"]}. {reader["name_flowers"]} - {reader["price_flowers"]}$");
                }
            }
            Console.WriteLine("0. Return to personalize options");
            Console.Write("Enter the id of the flower you want to add to your bouquet: ");
            int idFlowers = Convert.ToInt32(Console.ReadLine());
            if (idFlowers == 0)
            {
                personalizeBouquet(personalizedBouquet);
            }
            else
            {
                personalizedBouquet.Flowers.Add(idFlowers);
                Console.WriteLine("Flower added.");
                Console.WriteLine("Do you want to add another flower to your bouquet? (Y/N)");
                string choice = Console.ReadLine().ToLower();
                if (choice == "y")
                {
                    chooseFlowers(personalizedBouquet);
                }
                else
                {
                    personalizeBouquet(personalizedBouquet);
                }
            }
        }
    }
    void chooseAccessories(Bouquet personalizedBouquet)
    {
        Console.Clear();
        Console.WriteLine("CHOOSE ACCESSORIE(S)");
        string selectQuery = "SELECT * FROM accessories WHERE stock_accessories > 0;"; // here change with the date of availability
        using (MySqlCommand command = new(selectQuery, connection))
        {
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["id_accessories"]}. {reader["name_accessories"]} - {reader["price_accessories"]}$");
                }
            }
            Console.WriteLine("0. Return to personalize options");
            Console.Write("Enter the id of the accessorie you want to add to your bouquet: ");
            int idAccessories = Convert.ToInt32(Console.ReadLine());
            if (idAccessories == 0)
            {
                personalizeBouquet(personalizedBouquet);
            }
            else
            {
                personalizedBouquet.Accessories.Add(idAccessories);
                Console.WriteLine("Accessorie added.");
                Console.WriteLine("Do you want to add another accessorie to your bouquet? (Y/N)");
                string choice = Console.ReadLine().ToLower();
                if (choice == "y")
                {
                    chooseAccessories(personalizedBouquet);
                }
                else
                {
                    personalizeBouquet(personalizedBouquet);
                }
            }
        }
    }    
    void enterBudget(Bouquet personalizedBouquet)
    {
        Console.Clear();
        Console.WriteLine("PRICE");
        Console.Write("Enter your budget: ");
        string budget = Console.ReadLine();
        double budgetDouble=0.0;
        try
        {
            budgetDouble = Convert.ToDouble(budget);
            while (budgetDouble < 0)
            {
                Console.WriteLine("Invalid budget. Please enter a positive number.");
                Console.Write("Enter your budget: ");
                budget = Console.ReadLine();
                budgetDouble = Convert.ToDouble(budget);
            }
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
        personalizeBouquet(personalizedBouquet);
    }
    void enterSpecialRequest(Bouquet personalizedBouquet)
    {
        Console.Clear();
        Console.WriteLine("SPECIAL REQUEST");
        Console.Write("Enter your special request: ");
        personalizedBouquet.Description = Console.ReadLine();
        Console.WriteLine("Special request entered. Press any key to continue.");
        Console.ReadKey();
        personalizeBouquet(personalizedBouquet);
    }
    void validateBouquet(Bouquet personalizedBouquet)
    {
        if (personalizedBouquet.Flowers.Count == 0 && personalizedBouquet.Accessories.Count == 0 || personalizedBouquet.Price == 0)
        {
            Console.WriteLine("You must choose at least one flower and one accessorie and enter a budget. Press any key to continue.");
            Console.ReadKey();
            personalizeBouquet(personalizedBouquet);
        }
        else
        {
            string insertQuery = "INSERT INTO Personalized (price_personalized, description_personalized, flowers_personalized, accessories_personalized)" +
                "\r\nVALUES (@price_personalized, @description_personalized, @flowers_personalized, @accessories_personalized);";
            using (MySqlCommand command = new(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@price_personalized", personalizedBouquet.Price);
                command.Parameters.AddWithValue("@description_personalized", personalizedBouquet.Description);
                command.Parameters.AddWithValue("@flowers_personalized", personalizedBouquet.FlowersString);
                command.Parameters.AddWithValue("@accessories_personalized", personalizedBouquet.AccessoriesString);
                // debugRowsAffected(command);
                Console.WriteLine("Bouquet validated. Press any key to continue.");
            }
            Console.ReadKey();
            int idPersonalized;
            string selectQuery = "SELECT id_personalized FROM personalized ORDER BY id_personalized DESC LIMIT 1;";
            using (MySqlCommand command = new(selectQuery, connection))
            {
                idPersonalized = Convert.ToInt32(command.ExecuteScalar());
            }
            createOrder(true, idPersonalized);
        }
    }
    void createOrder(bool isPersonalizedCommand, int idPersonalized = 0, int idStandard=0)
    {
        Console.Clear();
        Client recipient = new();

        Console.WriteLine("CREATE ORDER");
        
        Console.Write("Enter your shipping address. Press any key to continue.");
        Console.ReadKey();
        enterAddress(recipient, false);
        int idAddresses = 0;
        string selectQuery = "SELECT id_addresses FROM addresses ORDER BY id_addresses DESC LIMIT 1";
        using (MySqlCommand command = new(selectQuery, connection))
        {
            idAddresses = Convert.ToInt32(command.ExecuteScalar());
        }
        
        DateTime orderDateDateTime = DateTime.Now;
        
        DateTime deliveryDateDateTime = new();
        enterDateTime(ref deliveryDateDateTime);
        
        Console.WriteLine("Enter a personnalized message for the recipient: ");
        string message = Console.ReadLine();

        string status = "";
        bool inventoryVerified = false;
        TimeSpan difference = deliveryDateDateTime - orderDateDateTime;
        bool delayIsEnough = difference.TotalDays > 3;
        if (isPersonalizedCommand)
        {
            // personnalized command to verify
            status = "CPAV";
        }
        else if(!isPersonalizedCommand)
        {
            if (!delayIsEnough)
            {
                // command to verify
                status = "VINV";
            }
            else
            {
                // completed command
                inventoryVerified = true;
            }
        }
        if (inventoryVerified)
        {
            // completed command
            status = "CC";

        }
        int idClients=0;
        selectQuery = "SELECT id_clients FROM clients WHERE email = @email";
        using (MySqlCommand command = new(selectQuery, connection))
        {
            command.Parameters.AddWithValue("@email",email);
            idClients = Convert.ToInt32(command.ExecuteScalar());
        }
        Console.Clear();
        Console.WriteLine("CHOOSE SHOP");
        selectQuery = "SELECT * FROM shops";
        using (MySqlCommand command = new(selectQuery, connection))
        {
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"{reader["id_shops"]} - {reader["city_shops"]}");
            }
            reader.Close();
        }
        Console.Write("Enter the id of the shop you want to order from: ");
        int idShops = Convert.ToInt32(Console.ReadLine());

        if (isPersonalizedCommand)
        {
            string insertQuery = "INSERT INTO orders (id_addresses, order_date, delivery_date, message, status, id_clients, id_shops, id_personalized)" +
                "VALUES (@id_addresses, @order_date, @delivery_date, @message, @status, @id_clients, @id_shops, @id_personalized)";
            using (MySqlCommand command = new(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@id_addresses", idAddresses);
                command.Parameters.AddWithValue("@order_date", orderDateDateTime);
                command.Parameters.AddWithValue("@delivery_date", deliveryDateDateTime);
                command.Parameters.AddWithValue("@message", message);
                command.Parameters.AddWithValue("@status", status);
                command.Parameters.AddWithValue("@id_clients", idClients);
                command.Parameters.AddWithValue("@id_shops", idShops);
                command.Parameters.AddWithValue("@id_personalized", idPersonalized);
                command.ExecuteNonQuery();
            }
        }
        else
        {
            string insertQuery = "INSERT INTO orders (id_addresses, order_date, delivery_date, message, status, id_clients, id_shops, id_standard)" +
                "VALUES (@id_addresses, @order_date, @delivery_date, @message, @status, @id_clients, @id_shops, @id_standard)";
            using (MySqlCommand command = new(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@id_addresses", idAddresses);
                command.Parameters.AddWithValue("@order_date", orderDateDateTime);
                command.Parameters.AddWithValue("@delivery_date", deliveryDateDateTime);
                command.Parameters.AddWithValue("@message", message);
                command.Parameters.AddWithValue("@status", status);
                command.Parameters.AddWithValue("@id_clients", idClients);
                command.Parameters.AddWithValue("@id_shops", idShops);
                command.Parameters.AddWithValue("@id_standard", idStandard);
                command.ExecuteNonQuery();
            }
        }
        Console.WriteLine("Order created. Press any key to continue.");
        Console.ReadKey();
        menu();
    }
    void enterDateTime(ref DateTime deliveryDateDateTime)
    {
        Console.Clear();
        Console.WriteLine("CREATE ORDER");
        Console.Write("Enter your delivery date (YYYY-MM-DD): ");
        string deliveryDate = Console.ReadLine();
        try
        {
            deliveryDateDateTime = Convert.ToDateTime(deliveryDate);
        }
        catch (FormatException)
        {
            Console.WriteLine("Invalid date. You must enter a date in the format YYYY-MM-DD. Press any key to continue.");
            Console.ReadKey();
            enterDateTime(ref deliveryDateDateTime);
        }
    }
}
Console.WriteLine("Press any key to exit.");
Console.ReadKey();