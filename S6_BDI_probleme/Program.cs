using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;
using MySql.Data.MySqlClient;
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
    void enterAddress(Client user, bool isClient = true)
    {
        if (!isClient)
        {
            Console.Clear();
            Console.WriteLine("Enter the first name of the recipient: ");
            user.FirstName = Console.ReadLine();
            Console.WriteLine("Enter the last name of the recipient: ");
            user.LastName = Console.ReadLine();
            Console.WriteLine("Enter the phone number of the recipient: ");
            user.PhoneNumber = Console.ReadLine();
        }
        Console.Clear();
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
        Console.WriteLine("MENU OPTIONS");
        Console.WriteLine("1. Display existing bouquets of flowers");
        Console.WriteLine("2. Personnalize your bouquet");
        Console.WriteLine("3. Display your order history");
        Console.WriteLine("4. Display your loyalty status");
        Console.WriteLine("0. Exit");
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
            case "0":
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
        Console.WriteLine("0. Exit");
        Console.Write("Enter your choice: ");
        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                choosesFlowers();
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
                createOrder(true); //to change because it neeeds to verif if correct options
                validateBouquet(personalizedBouquet);
                break;
            case "0":
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
    void choosesFlowers()
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
        Console.WriteLine("Special request entered. Press any key to continue.");
        Console.ReadKey();
        personalizeBouquet();
    }
    void validateBouquet(Bouquet personalizedBouquet)
    {
        if (personalizedBouquet.Flowers.Count == 0 || personalizedBouquet.Accessories.Count == 0 || personalizedBouquet.Price == 0.0)
        {
            Console.WriteLine("You must choose at least one flower and one accessorie and enter a budget. Press any key to continue.");
            Console.ReadKey();
            personalizeBouquet();
        }
        else
        {
            Console.WriteLine("Bouquet validated. Press any key to continue.");
            Console.ReadKey();
            createOrder(true);
        }
    }
    void createOrder(bool isPersonalizedCommand)
    {
        Console.Clear();
        
        Client recipient = new();
        
        int idAddresses = 0;
        Console.Write("Enter your shipping address. Press any key to continue.");
        Console.ReadKey();
        enterAddress(recipient, false);
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
                status = "CC";
            }
        }
        if (inventoryVerified==true)
        {
            // completed command
            status = "CC";

        }
        int idClients = 0;
        selectQuery = "SELECT id_clients FROM clients WHERE email = @email";
        using (MySqlCommand command = new(selectQuery, connection))
        {
            command.Parameters.AddWithValue("@email",email);
            idClients = Convert.ToInt32(command.ExecuteScalar());
        }

        Console.WriteLine("Enter the id of the shop you want to order from: ");
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
        int idShops = Convert.ToInt32(Console.ReadLine());

        string insertQuery = "INSERT INTO orders (message, order_date, delivery_date, status, id_clients, id_addresses, id_shops)" +
            "\r\nVALUES (@message, @orderDate, @deliveryDate, @status, @idClients, @idAddresses, @idShops)";
        using (MySqlCommand command = new(insertQuery, connection))
        {
            command.Parameters.AddWithValue("@message", message);
            command.Parameters.AddWithValue("@orderDate", orderDateDateTime);
            command.Parameters.AddWithValue("@deliveryDate", deliveryDateDateTime);
            command.Parameters.AddWithValue("@status", status);
            command.Parameters.AddWithValue("@idClients", idClients);
            command.Parameters.AddWithValue("@idAddresses", idAddresses);
            command.Parameters.AddWithValue("@idShops", idShops);
            command.ExecuteNonQuery();
        }
        Console.WriteLine("Order created. Press any key to continue.");
        Console.ReadKey();
        menu();
    }
    void enterDateTime(ref DateTime deliveryDateDateTime)
    {
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