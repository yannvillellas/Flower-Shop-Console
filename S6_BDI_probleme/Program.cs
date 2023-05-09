using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1;
using S6_BDI_probleme;
using System.Xml.Serialization;
using System.Text.Json;

string connectionStringAdmin = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=root;PASSWORD=root;"; // string connection for admin
string connectionStringUser = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=bozo;PASSWORD=bozo;"; // string connection for user
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
    connection.Open(); // open connection
    authentication(); // call authentication function
    Console.WriteLine("Press any key to exit.");
    Console.ReadKey();

    // function to authenticate user
    void authentication()
    {
        Console.Clear();
        Console.WriteLine("AUTHENTICATION");
        Console.Write("Enter email: ");
        email = Console.ReadLine();

        // Check if user already exists
        int count;
        string selectQuery = "SELECT COUNT(*) FROM clients WHERE email = @email"; // query
        using (MySqlCommand command = new(selectQuery, connection))
        {
            command.Parameters.AddWithValue("@email", email);
            count = Convert.ToInt32(command.ExecuteScalar());
        }

        if (count == 1) // user exists
        {
            login();
        }
        else // user doesn't exist
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
                authentication();
            }
        }
    }
    // function to login an existing client
    void login()
    {
        int attempts = 3;
        bool loggedIn = false;
        while (attempts > 0 && !loggedIn) // while there are attempts left and the user is not logged in
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

            if (passwordCount == 1) // password matches
            {
                Console.WriteLine("Login successful.");
                loggedIn = true;
            }
            else // password doesn't match
            {
                Console.WriteLine("Incorrect password. " + --attempts + " attempt(s) left.");
                Console.WriteLine("Do you want to enter another email? (Y/N) "); // ask if user wants to enter another email
                string answer = Console.ReadLine().ToLower();
                if (answer == "y") // (here is a security problem because the user can enter the same email and habe unlimited attempts)
                {
                    authentication();
                }
            }
        }
        if (!loggedIn)
        {
            Console.WriteLine("Max attempts exceeded. Exiting program.");
            Environment.Exit(0);
        }
        else
        {
            menu();
        }
    }
    // function to create client
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
        enterAddress(newClient); //create address for the new client
        string selectQuery = "SELECT id_addresses FROM addresses ORDER BY id_addresses DESC LIMIT 1"; // get last address created (line just before)
        using (MySqlCommand command = new(selectQuery, connection))
        {
            newClient.BillingAddressID = Convert.ToInt32(command.ExecuteScalar()); // set billing address id into client object
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
            command.ExecuteNonQuery(); // create client with client object

        }
        Console.WriteLine("Account created. Press any key to continue.");
        Console.ReadKey();
        authentication();
    }
    // function to enter address for a user, could be a new client or a recipient for an order (isClient = false)
    void enterAddress(Client user, bool isClient = true)
    {
        Console.Clear();
        Console.WriteLine("CREATE ADDRESS");
        if (!isClient) // is a recipient so we don't know these infos
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
            "\r\nVALUES (@firstName, @lastName, @phone, @city, @zipCode, @streetNumber, @streetName);"; // query
        using (MySqlCommand command = new(insertQuery, connection))
        {
            command.Parameters.AddWithValue("@firstName", user.FirstName);
            command.Parameters.AddWithValue("@lastName", user.LastName);
            command.Parameters.AddWithValue("@phone", user.PhoneNumber);
            command.Parameters.AddWithValue("@city", city);
            command.Parameters.AddWithValue("@zipCode", zipCode);
            command.Parameters.AddWithValue("@streetNumber", streetNumber);
            command.Parameters.AddWithValue("@streetName", streetName);
            command.ExecuteNonQuery(); // execute query
        }
        Console.WriteLine("Address created. Press any key to continue.");
        Console.ReadKey();
    }
    void menu()
    {
        Console.Clear();
        Console.WriteLine("MENU OPTIONS");
        Console.WriteLine("1. Order existing bouquet of flowers");
        Console.WriteLine("2. Personnalize your bouquet");
        Console.WriteLine("3. Display your order history");
        Console.WriteLine("4. Display your loyalty status");
        string selectQuery = "SELECT admin FROM clients WHERE email = @email"; // check if user is admin
        bool isAdmin;
        using (MySqlCommand command = new(selectQuery, connection))
        {
            command.Parameters.AddWithValue("@email", email);
            isAdmin = Convert.ToBoolean(command.ExecuteScalar());
        }
        if (isAdmin)
        {
            Console.WriteLine("5. Admin menu"); // if admin show admin menu
        }
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
            case "5":
                if (isAdmin) //check if admin to access admin menu
                {
                    menuAdmin();
                }
                else
                {
                    Console.WriteLine("Invalid choice. Press any key to continue.");
                    Console.ReadKey();
                    menu();
                }
                break;
            case "0":
                authentication();
                break;
            default:
                Console.WriteLine("Invalid choice. Press any key to continue.");
                Console.ReadKey();
                menu();
                break;
        }
    }
    void orderBouquet()
    {
        Console.Clear();
        Console.WriteLine("ORDER BOUQUET");
        string selectQuery = "SELECT * FROM standard;"; // get all standard bouquets
        using (MySqlCommand command = new(selectQuery, connection))
        {
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["id_standard"]}. {reader["name_bouquet"]} - {reader["category"]}: {reader["description_standard"]} - {reader["price_standard"]}$"); // display all standard bouquets
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
                createOrder(false, 0, idStandard); // create order with standard bouquet id and no personalized bouquet so false and 0
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
    void chooseFlowers(Bouquet personalizedBouquet)
    {
        Console.Clear();
        Console.WriteLine("CHOOSE FLOWER(S)");
        string selectQuery = "SELECT * FROM flowers WHERE stock_flowers > 0;"; // here we only want flowers that are in stock - we are not checking for month availability yet
        using (MySqlCommand command = new(selectQuery, connection))
        {
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["id_flowers"]}. {reader["name_flowers"]} - ${reader["price_flowers"]}"); // display all flowers in stock
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
                personalizedBouquet.Flowers.Add(idFlowers); // add flower to bouquet object
                Console.WriteLine("Flower added.");
                Console.WriteLine("Do you want to add another flower to your bouquet? (Y/N)");
                string choice = Console.ReadLine().ToLower();
                if (choice == "y")
                {
                    chooseFlowers(personalizedBouquet); // if yes, call method again
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
        string selectQuery = "SELECT * FROM accessories WHERE stock_accessories > 0;";
        using (MySqlCommand command = new(selectQuery, connection))
        {
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["id_accessories"]}. {reader["name_accessories"]} - ${reader["price_accessories"]}"); // display all accessories in stock
                }
            }
            Console.WriteLine("0. Return to personalize options");
            Console.Write("Enter the id of the accessorie you want to add to your bouquet: ");
            int idAccessories = Convert.ToInt32(Console.ReadLine());
            if (idAccessories == 0)
            {
                personalizeBouquet(personalizedBouquet); // if 0, return to personalize options
            }
            else
            {
                personalizedBouquet.Accessories.Add(idAccessories); // add accessorie to bouquet object
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
    // function to enter budget
    void enterBudget(Bouquet personalizedBouquet)
    {
        Console.Clear();
        Console.WriteLine("PRICE");
        Console.Write("Enter your budget: ");
        string budget = Console.ReadLine();
        double budgetDouble = 0.0;
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
        personalizedBouquet.Price = budgetDouble; // add budget to bouquet object
        Console.WriteLine("Budget entered. Press any key to continue.");
        Console.ReadKey();
        personalizeBouquet(personalizedBouquet);
    }
    void enterSpecialRequest(Bouquet personalizedBouquet)
    {
        Console.Clear();
        Console.WriteLine("SPECIAL REQUEST");
        Console.Write("Enter your special request: ");
        personalizedBouquet.Description = Console.ReadLine(); // add special request to bouquet object
        Console.WriteLine("Special request entered. Press any key to continue.");
        Console.ReadKey();
        personalizeBouquet(personalizedBouquet);
    }
    void validateBouquet(Bouquet personalizedBouquet)
    {
        if (personalizedBouquet.Flowers.Count == 0 && personalizedBouquet.Accessories.Count == 0 || personalizedBouquet.Price == 0)
        {
            // if no flowers or accessories have been added, or if no budget has been entered, return to personalize options
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
                command.ExecuteNonQuery(); // insert personalized bouquet into database
                Console.WriteLine("Bouquet validated. Press any key to continue.");
            }
            Console.ReadKey();
            int idPersonalized;
            string selectQuery = "SELECT id_personalized FROM personalized ORDER BY id_personalized DESC LIMIT 1;"; // get id of last personalized bouquet added
            using (MySqlCommand command = new(selectQuery, connection))
            {
                idPersonalized = Convert.ToInt32(command.ExecuteScalar());
            }
            createOrder(true, idPersonalized); // call method to create order with last personalized bouquet added
        }
    }
    void displayOrderHistory()
    {
        // use email as primary key to look for this client
        Console.Clear();
        Console.WriteLine("ORDER HISTORY");
        // // COMPLEX QUERY - join query
        string selectQuery = "SELECT * FROM orders JOIN clients ON orders.id_clients = clients.id_clients WHERE email = @email;";
        using (MySqlCommand command = new(selectQuery, connection))
        {
            command.Parameters.AddWithValue("@email", email);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["id_orders"]}. {reader["message"]} - {reader["order_date"]} - {reader["delivery_date"]} - {reader["status"]}"); // display all orders of this client
                }
            }
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
            menu();
        }
    }
    void displayLoyaltyStatus()
    {
        Console.Clear();
        Console.WriteLine("LOYALTY STATUS");
        string selectQuery = "SELECT \r\n                                                                  id_clients,\r\n                    first_name,\r\n                    last_name,\r\n                    CASE\r\n                        WHEN (\r\n                            SELECT COUNT(*)\r\n                            FROM Orders\r\n                            WHERE id_clients = Clients.id_clients\r\n                                AND MONTH(order_date) = MONTH(CURDATE())\r\n                        ) > 5 THEN 'OR'\r\n                        WHEN (\r\n                            SELECT COUNT(*)\r\n                            FROM Orders\r\n                            WHERE id_clients = Clients.id_clients\r\n                                AND MONTH(order_date) = MONTH(CURDATE())\r\n                        ) >= 1 THEN 'Bronze'\r\n                        ELSE 'Non défini'\r\n                    END AS loyalty\r\n                FROM Clients;\r\n";
        using (MySqlCommand command = new(selectQuery, connection))
        {
            command.Parameters.AddWithValue("@email", email);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["loyalty"]}"); // display loyalty status of this client
                }
            }
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
            menu();
        }
    }
    // function to create an order, personalized or standard
    void createOrder(bool isPersonalizedCommand, int idPersonalized = 0, int idStandard = 0)
    {
        Console.Clear();
        Client recipient = new();

        Console.WriteLine("CREATE ORDER");

        Console.Write("Enter your shipping address. Press any key to continue.");
        Console.ReadKey();
        enterAddress(recipient, false); // false because it's not the billing address
        int idAddresses = 0;
        string selectQuery = "SELECT id_addresses FROM addresses ORDER BY id_addresses DESC LIMIT 1"; // get the id of the last address added
        using (MySqlCommand command = new(selectQuery, connection))
        {
            idAddresses = Convert.ToInt32(command.ExecuteScalar());
        }

        DateTime orderDateDateTime = DateTime.Now; // get the current date and time for the order

        DateTime deliveryDateDateTime = new();
        enterDateTime(ref deliveryDateDateTime); // get the delivery date for the order

        Console.WriteLine("Enter a personnalized message for the recipient: ");
        string message = Console.ReadLine();

        string status = "";
        bool inventoryVerified = false;
        TimeSpan difference = deliveryDateDateTime - orderDateDateTime; // get the difference between the order date and the delivery date
        bool delayIsEnough = difference.TotalDays > 3; // check if the delay is enough to know if the inventory must be verified
        if (isPersonalizedCommand)
        {
            // personnalized command to verify
            status = "CPAV";
        }
        else // standard command
        {
            if (!delayIsEnough) //delay is less than 3 days
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
        int idClients = 0;
        selectQuery = "SELECT id_clients FROM clients WHERE email = @email";
        using (MySqlCommand command = new(selectQuery, connection))
        {
            command.Parameters.AddWithValue("@email", email);
            idClients = Convert.ToInt32(command.ExecuteScalar()); // get the id of the client with email provided
        }
        Console.Clear();
        Console.WriteLine("CHOOSE SHOP");
        selectQuery = "SELECT * FROM shops"; // get all the shops
        using (MySqlCommand command = new(selectQuery, connection))
        {
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"{reader["id_shops"]} - {reader["city_shops"]}"); // display the shops
            }
            reader.Close();
        }
        Console.Write("Enter the id of the shop you want to order from: ");
        int idShops = Convert.ToInt32(Console.ReadLine()); // get the id of the shop chosen

        if (isPersonalizedCommand) // if the command is personalized with idPersonalizd parameter
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
        else // standard with id_standard
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
    // function to enter a date and time, ref is used to modify the variable in the main function
    void enterDateTime(ref DateTime deliveryDateDateTime)
    {
        Console.Clear();
        Console.WriteLine("CREATE ORDER");
        Console.Write("Enter your delivery date (YYYY-MM-DD): ");
        string deliveryDate = Console.ReadLine();
        try
        {
            deliveryDateDateTime = Convert.ToDateTime(deliveryDate); // convert the string to a DateTime
        }
        catch (FormatException)
        {
            Console.WriteLine("Invalid date. You must enter a date in the format YYYY-MM-DD. Press any key to continue.");
            Console.ReadKey();
            enterDateTime(ref deliveryDateDateTime);
        }
    }
    void menuAdmin()
    {
        Console.Clear();
        Console.WriteLine("ADMIN MENU");
        Console.WriteLine("1. Add a flower");
        Console.WriteLine("2. Add an accessorie");
        Console.WriteLine("3. Add a bouquet");
        Console.WriteLine("4. Statistics menu");
        Console.WriteLine("5. Add a new employee (admin)");
        Console.WriteLine("6. Display all employees (admin)");
        Console.WriteLine("7. Change the status of an order");
        Console.WriteLine("8. Export in XML all clients");
        Console.WriteLine("9. Export in JSON all personalized bouquets");
        Console.WriteLine("0. Return to menu");
        Console.Write("Enter your choice: ");
        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                addFlower();
                break;
            case "2":
                addAccessorie();
                break;
            case "3":
                addBouquet();
                break;
            case "4":
                menuStats();
                break;
            case "5":
                addAdmin();
                break;
            case "6":
                seeAdmins();
                break;
            case "7":
                changeStatus();
                break;
            case "8":
                exportXML();
                break;
            case "9":
                exportJSON();
                break;
            case "0":
                menu();
                break;
            default:
                Console.WriteLine("Invalid choice. Press any key to continue.");
                Console.ReadKey();
                Console.Clear();
                menuAdmin();
                break;
        }
    }
    void addFlower()
    {
        Console.Clear();
        Console.WriteLine("ADD A FLOWER");
        Console.Write("Enter the name of the flower: ");
        string nameFlowers = Console.ReadLine();
        Console.Write("Enter the price of the flower: ");
        double priceFlowers = Convert.ToDouble(Console.ReadLine());
        Console.Write("Enter the stock of the flower: ");
        int stockFlowers = Convert.ToInt32(Console.ReadLine());
        Console.Write("Enter the month of the beginning of availability of the flower: ");
        int startMonth = Convert.ToInt32(Console.ReadLine());
        Console.Write("Enter the month of the end of availability of the flower: ");
        int endMonth = Convert.ToInt32(Console.ReadLine());
        string insertQuery = $"INSERT INTO flowers (name_flowers, price_flowers, stock_flowers, start_month, end_month) VALUES ('{nameFlowers}', {priceFlowers}, {stockFlowers}, {startMonth}, {endMonth});";
        using (MySqlCommand command = new(insertQuery, connection))
        {
            command.ExecuteNonQuery();
        }
        Console.WriteLine("Flower added. Press any key to continue.");
        Console.ReadKey();
        menuAdmin();
    }
    void addAccessorie()
    {
        Console.Clear();
        Console.WriteLine("ADD AN ACCESSORIE");
        Console.Write("Enter the name of the accessorie: ");
        string nameAccessories = Console.ReadLine();
        Console.Write("Enter the price of the accessorie: ");
        double priceAccessories = Convert.ToDouble(Console.ReadLine());
        Console.Write("Enter the stock of the accessorie: ");
        int stockAccessories = Convert.ToInt32(Console.ReadLine());
        string insertQuery = $"INSERT INTO accessories (name_accessories, price_accessories, stock_accessories) VALUES ('{nameAccessories}', {priceAccessories}, {stockAccessories});";
        using (MySqlCommand command = new(insertQuery, connection))
        {
            command.ExecuteNonQuery();
        }
        Console.WriteLine("Accessorie added. Press any key to continue.");
        Console.ReadKey();
        menuAdmin();
    }
    void addBouquet()
    {
        //parameters are name_bouquet, description_standard, price_standard, category
        Console.Clear();
        Console.WriteLine("ADD A BOUQUET");
        Console.Write("Enter the name of the bouquet");
        string nameBouquet = Console.ReadLine();
        Console.Write("Enter the description of the bouquet");
        string descriptionStandard = Console.ReadLine();
        Console.Write("Enter the price of the bouquet");
        double priceStandard = Convert.ToDouble(Console.ReadLine());
        Console.Write("Enter the category of the bouquet");
        string category = Console.ReadLine();
        string insertQuery = $"INSERT INTO bouquets (name_bouquet, description_standard, price_standard, category) VALUES ('{nameBouquet}', '{descriptionStandard}', {priceStandard}, '{category}');";
        using (MySqlCommand command = new(insertQuery, connection))
        {
            command.ExecuteNonQuery();
        }
        Console.WriteLine("Bouquet added. Press any key to continue.");
        Console.ReadKey();
        menuAdmin();
    }
    void menuStats()
    {
        Console.Clear();
        Console.WriteLine("MENU STATS");
        Console.WriteLine("1. Mean price of bouquets");
        Console.WriteLine("2. Best client of the month");
        Console.WriteLine("3. Best seller for the standard bouquet");
        Console.WriteLine("4. Best Shop");
        Console.WriteLine("5. Worst exotic flower");
        Console.WriteLine("0. Return to menu");
        Console.Write("Enter your choice: ");
        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                Console.Clear();
                Console.WriteLine("MENU STATS - AVERAGE BOUQUET PRICE");
                // select the average price of the bouquets
                // COMPLEX QUERY - union query
                string selectQuery = "SELECT AVG(average_price) AS overall_average_price\r\nFROM (\r\n  SELECT AVG(price_standard) AS average_price\r\n  FROM Standard\r\n  UNION ALL\r\n  SELECT AVG(price_personalized) AS average_price\r\n  FROM Personalized\r\n) AS subquery;";
                using (MySqlCommand command = new(selectQuery, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            decimal averagePrice = reader.GetDecimal(0);
                            Console.WriteLine($"The average price of the bouquets is: ${averagePrice}.");
                        }
                    }
                }
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
                menuStats();
                break;
            case "2":
                Console.Clear();
                Console.WriteLine("MENU STATS - BEST CLIENT");
                string selectQuery2 = "SELECT c.id_clients, c.first_name, c.last_name, COUNT(o.id_orders) AS total_orders\r\nFROM Clients c\r\nINNER JOIN Orders o ON c.id_clients = o.id_clients\r\nWHERE MONTH(o.order_date) = MONTH(CURRENT_DATE())\r\nGROUP BY c.id_clients, c.first_name, c.last_name\r\nORDER BY total_orders DESC\r\nLIMIT 1;";
                using (MySqlCommand command = new(selectQuery2, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int clientId = reader.GetInt32(0);
                            string firstName = reader.GetString(1);
                            string lastName = reader.GetString(2);
                            Console.WriteLine($"The best client of the month is: {firstName} {lastName} (ID: {clientId}).");
                        }

                    }
                }
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
                menuStats();
                break;
            case "3":
                Console.Clear();
                Console.WriteLine("MENU STATS - BEST SELLER");
                // select the best seller for the standard bouquet
                string selectQuery3 = "SELECT id_standard, name_bouquet\r\nFROM Standard\r\nORDER BY (SELECT COUNT(*) FROM Orders WHERE id_standard = Standard.id_standard) DESC\r\nLIMIT 1;";
                using (MySqlCommand command = new(selectQuery3, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idStandard = reader.GetInt32(0);
                            string nameBouquet = reader.GetString(1);
                            Console.WriteLine($"The best seller for the standard bouquet is: {nameBouquet} (ID: {idStandard}).");
                        }

                    }
                }
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
                menuStats();
                break;
            case "4":
                Console.Clear();
                Console.WriteLine("MENU STATS - BEST SHOP");
                // select the best shop (the one with the highest profit)
                string selectQuery4 = "SELECT id_shops, city_shops\r\nFROM Shops\r\nORDER BY (\r\n    SELECT SUM(price_standard) \r\n    FROM Orders \r\n    JOIN Standard ON Orders.id_standard = Standard.id_standard \r\n    WHERE Orders.id_shops = Shops.id_shops\r\n) + (\r\n    SELECT SUM(price_personalized) \r\n    FROM Orders \r\n    JOIN Personalized ON Orders.id_personalized = Personalized.id_personalized \r\n    WHERE Orders.id_shops = Shops.id_shops\r\n) DESC\r\nLIMIT 1;\r\n";
                using (MySqlCommand command = new(selectQuery4, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idShops = reader.GetInt32(0);
                            string cityShops = reader.GetString(1);
                            Console.WriteLine($"The city of the shop with the highest turnover is {cityShops} (ID of the shop: {idShops}).");
                        }
                    }
                }
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
                menuStats();
                break;
            case "5":
                Console.Clear();
                Console.WriteLine("MENU STATS - WORST EXOTIC FLOWER");
                // select the worst exotic flower (the one that has been ordered the least)
                // COMPLEX QUERY - synchronized query
                string selectQuery5 = "SELECT id_flowers, name_flowers\r\nFROM Flowers\r\nWHERE name_flowers = 'Ginger' OR name_flowers = 'Oiseaux du paradis'\r\nORDER BY (SELECT COUNT(*) FROM Orders JOIN Personalized ON Orders.id_personalized = Personalized.id_personalized WHERE Personalized.flowers_personalized LIKE CONCAT('%', Flowers.name_flowers, '%')) ASC\r\nLIMIT 1;";
                using (MySqlCommand command = new(selectQuery5, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idFlowers = reader.GetInt32(0);
                            string nameFlowers = reader.GetString(1);
                            Console.WriteLine($"The least used flower in personalized bouquets containing the flowers 'Ginger' or 'Birds of paradise' is {nameFlowers} (ID: {idFlowers}).");
                        }
                    }
                }
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
                menuStats();
                break;
            case "0":
                menuAdmin();
                break;
            default:
                Console.WriteLine("Invalid choice. Press any key to continue.");
                Console.ReadKey();
                Console.Clear();
                menuStats();
                break;
        }
    }
    void addAdmin()
    {
        Console.Clear();
        Console.WriteLine("CHANGE ADMIN STATUS");
        // show all the clients with their id
        string selectQuery = "SELECT * FROM Clients";
        using (MySqlCommand command = new(selectQuery, connection))
        {
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string firstName = reader.GetString(1);
                    string lastName = reader.GetString(2);
                    string phone = reader.GetString(3);
                    string email = reader.GetString(4);
                    string password = reader.GetString(5);
                    string loyalty = reader.GetString(6);
                    bool admin = reader.GetBoolean(7);
                    int idAddresses = reader.GetInt32(8);
                    Console.WriteLine($"ID: {id} | First Name: {firstName} | Last Name: {lastName} | Phone: {phone} | Email: {email} | Password: {password} | Loyalty: {loyalty} | Admin: {admin} | ID Addresses: {idAddresses}");
                }
            }
        }
        Console.Write("Enter the ID of the client you want to change the admin status of : ");
        int idClient = Convert.ToInt32(Console.ReadLine());
        Console.Write("Enter the new admin status (true or false) : ");
        bool newAdminStatus = Convert.ToBoolean(Console.ReadLine());
        string updateQuery = $"UPDATE Clients SET admin = {newAdminStatus} WHERE id_clients = {idClient}";
        using (MySqlCommand command = new(updateQuery, connection))
        {
            command.ExecuteNonQuery();
        }
        Console.WriteLine("Press any key to continue.");
        Console.ReadKey();
        menu();
    }
    void seeAdmins()
    {
        Console.Clear();
        Console.WriteLine("SEE ADMINS");
        string selectQuery = "SELECT * FROM Clients WHERE admin = true";
        using (MySqlCommand command = new(selectQuery, connection))
        {
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string firstName = reader.GetString(1);
                    string lastName = reader.GetString(2);
                    string phone = reader.GetString(3);
                    string email = reader.GetString(4);
                    string password = reader.GetString(5);
                    string loyalty = reader.GetString(6);
                    bool admin = reader.GetBoolean(7);
                    int idAddresses = reader.GetInt32(8);
                    Console.WriteLine($"ID: {id} | First Name: {firstName} | Last Name: {lastName} | Phone: {phone} | Email: {email} | Password: {password} | Loyalty: {loyalty} | Admin: {admin} | ID Addresses: {idAddresses}");
                }
            }
        }
        Console.WriteLine("Press any key to continue.");
        Console.ReadKey();
        menuAdmin();
    }
    void changeStatus()
    {
        // show all the commands with status to be changed (CPAV or VINV)
        Console.Clear();
        Console.WriteLine("CHANGE STATUS");
        string selectQuery = "SELECT * FROM Orders WHERE status = 'CPAV' OR status = 'VINV'";
        using (MySqlCommand command = new(selectQuery, connection))
        {
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string order_date = reader.GetString(2);
                    string delivery_date = reader.GetString(3);
                    string status = reader.GetString(4);
                    Console.WriteLine($"ID: {id} | Order date: {order_date} | Delivery date: {delivery_date} | Status: {status}");
                }
            }
        }
        Console.Write("Enter the ID of the order you want to change the status of : ");
        int idOrder = Convert.ToInt32(Console.ReadLine());
        Console.Write("Enter the new status (CC?): ");
        string newStatus = Console.ReadLine();
        string updateQuery = $"UPDATE Orders SET status = '{newStatus}' WHERE id_orders = {idOrder}";
        using (MySqlCommand command = new(updateQuery, connection))
        {
            command.ExecuteNonQuery();
        }
        Console.WriteLine("Do you want to change another status ? (Y/N)");
        string answer = Console.ReadLine().ToLower();
        if (answer == "y")
        {
            changeStatus();
        }
        else
        {
            menuAdmin();
        }
    }
    void exportXML()
    {
        Console.Clear();
        Console.WriteLine("EXPORT XML");
        // export all clients in XML
        List<Client> clients = new();
        string selectQuery = "SELECT * FROM Clients";
        using (MySqlCommand command = new(selectQuery, connection))
        {
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string firstName = reader.GetString(1);
                    string lastName = reader.GetString(2);
                    string phone = reader.GetString(3);
                    string email = reader.GetString(4);
                    string password = reader.GetString(5);
                    string loyalty = reader.GetString(6);
                    bool admin = reader.GetBoolean(7);
                    int idAddresses = reader.GetInt32(8);
                    clients.Add(new Client(id, firstName, lastName, phone, email, password, loyalty, admin, idAddresses));
                }
            }
        }
        XmlSerializer serializer = new(typeof(List<Client>));
        using (FileStream stream = File.Create("clients.xml"))
        {
            serializer.Serialize(stream, clients);
        }
        Console.WriteLine("Data exported in XML.");
        Console.WriteLine("Press any key to continue.");
        Console.ReadKey();
        menuAdmin();
    }
    void exportJSON()
    {
        Console.Clear();
        Console.WriteLine("EXPORT JSON");
        // export all personalized bouquets in JSON
        List<Bouquet> personalizedBouquets = new();
        string selectQuery = "SELECT * FROM Personalized;";
        using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
        {
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string description = reader.GetString(2);
                    string Flowers = reader.GetString(3);
                    List<int> flowers = new();
                    foreach (string flower in Flowers.Split(','))
                    {
                        //try convert to int but it can be empty
                        if (int.TryParse(flower, out int result))
                        {
                            flowers.Add(result);
                        }
                    }
                    string Accessories = reader.GetString(4);
                    List<int> accessories = new();
                    foreach (string accessory in Accessories.Split(','))
                    {
                        //try convert to int but it can be empty
                        if (int.TryParse(accessory, out int result))
                        {
                            accessories.Add(result);
                        }
                    }
                    double price = reader.GetInt32(1);
                    personalizedBouquets.Add(new Bouquet(id, description, price, flowers, accessories));
                }
            }
        }
        Console.WriteLine($"Found {personalizedBouquets.Count} personalized bouquets to export");
        using (StreamWriter writer = new StreamWriter("personalizedBouquets.json"))
        {
            writer.Write(JsonSerializer.Serialize(personalizedBouquets));
        }
        Console.WriteLine("Data exported in JSON.");
        Console.WriteLine("Press any key to continue.");
        Console.ReadKey();
        menuAdmin();
    }
}