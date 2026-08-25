using System;
using MySql.Data.MySqlClient;

namespace Stockpulseb
{
    class Program
    {
        static string connectionString = "Server=localhost;Port=3306;Database=Stockpulse;Uid=root;Pwd=*Root@585.*$";

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\n====================================");
                Console.WriteLine("        Welcome to Stockpulse        ");
                Console.WriteLine("=======================================");
                Console.WriteLine("1. Add New item");
                Console.WriteLine("2. View All Stock");
                Console.WriteLine("3. Update Item");
                Console.WriteLine("4. Delete Item");
                Console.WriteLine("5. Search Item By Name");
                Console.WriteLine("6. Low Stock Alert");
                Console.WriteLine("7. Exit");
                Console.Write("Select Choice (1-7): ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddItem(); break;
                    case "2": ViewStock(); break;
                    case "3": UpdateItem(); break;
                    case "4": DeleteItem(); break;
                    case "5": SearchItem(); break;
                    case "6": LowStockAlert(); break;
                    case "7":
                        Console.WriteLine("Exiting System...");
                        return;
                    default:
                        Console.WriteLine("Invalid Option! Try again.");
                        break;
                }
            }
        }

        // Add Item
        static void AddItem()
        {
            Console.Write("\nEnter Item Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter Quantity: ");
            int qty = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter Price: ");
            decimal price = Convert.ToDecimal(Console.ReadLine());

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "INSERT INTO inventory (ItemName, Quantity, Price) VALUES (@Name, @Qty, @Price)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Qty", qty);
                cmd.Parameters.AddWithValue("@Price", price);

                conn.Open();
                cmd.ExecuteNonQuery();
                Console.WriteLine("SUCCESS: Item Added Successfully!");
            }
        }

        // View Item
        static void ViewStock()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM inventory";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\nID \t ITEM NAME \t\t QUANTITY \t PRICE");
                    Console.WriteLine("---------------------------------------------------------------");
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Id"]} \t {reader["ItemName"],-15} \t {reader["Quantity"]} \t\t Rs. {reader["Price"]}");
                    }
                }
            }
        }

        // Update Item (With Skip Option)
        static void UpdateItem()
        {
            Console.Write("\nEnter Item ID to Update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ERROR: Invalid ID!");
                return;
            }

            int currentQty = 0;
            decimal currentPrice = 0;
            bool exists = false;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string getQuery = "SELECT Quantity, Price FROM inventory WHERE Id = @Id";
                MySqlCommand getCmd = new MySqlCommand(getQuery, conn);
                getCmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                using (MySqlDataReader reader = getCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        currentQty = Convert.ToInt32(reader["Quantity"]);
                        currentPrice = Convert.ToDecimal(reader["Price"]);
                        exists = true;
                    }
                }
            }

            if (!exists)
            {
                Console.WriteLine("ERROR: Item ID not found!");
                return;
            }

            Console.Write($"Enter New Quantity (Current: {currentQty}, Press Enter to skip): ");
            string inputQty = Console.ReadLine();
            int newQty = string.IsNullOrWhiteSpace(inputQty) ? currentQty : Convert.ToInt32(inputQty);

            Console.Write($"Enter New Price (Current: {currentPrice}, Press Enter to skip): ");
            string inputPrice = Console.ReadLine();
            decimal newPrice = string.IsNullOrWhiteSpace(inputPrice) ? currentPrice : Convert.ToDecimal(inputPrice);

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "UPDATE inventory SET Quantity = @Qty, Price = @Price WHERE Id = @Id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Qty", newQty);
                cmd.Parameters.AddWithValue("@Price", newPrice);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                    Console.WriteLine("SUCCESS: Item Updated Successfully!");
                else
                    Console.WriteLine("ERROR: Update failed!");
            }
        }

        // Delete
        static void DeleteItem()
        {
            Console.Write("\nEnter Item ID to Delete: ");
            int id = Convert.ToInt32(Console.ReadLine());

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "DELETE FROM inventory WHERE Id = @Id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                    Console.WriteLine("SUCCESS: Item Deleted Successfully!");
                else
                    Console.WriteLine("ERROR: Item ID not found!");
            }
        }

        // Search
        static void SearchItem()
        {
            Console.Write("\nEnter Item Name to Search: ");
            string searchName = Console.ReadLine();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM inventory WHERE ItemName LIKE @Search";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Search", "%" + searchName + "%");

                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\nID \t ITEM NAME \t\t QUANTITY \t PRICE");
                    Console.WriteLine("-------------------------------------------------------------------");
                    bool found = false;
                    while (reader.Read())
                    {
                        found = true;
                        Console.WriteLine($"{reader["Id"]} \t {reader["ItemName"],-15} \t {reader["Quantity"]} \t\t Rs. {reader["Price"]}");
                    }
                    if (!found)
                        Console.WriteLine("No Matching Items Found.");
                }
            }
        }

        // Low Stock Alert
        static void LowStockAlert()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM inventory WHERE Quantity <= 5";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\n!!! Warning: Low Stock Items (Quantity <= 5) !!!");
                    bool found = false;
                    while (reader.Read())
                    {
                        found = true;
                        Console.WriteLine($"{reader["Id"]} \t {reader["ItemName"],-15} \t {reader["Quantity"]} \t\t Rs. {reader["Price"]}");
                    }
                    if (!found)
                        Console.WriteLine("All items are sufficiently stocked.");
                }
            }
        }
    }
}