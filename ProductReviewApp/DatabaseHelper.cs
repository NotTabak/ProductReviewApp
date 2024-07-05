using ProductReviewApp;
using System.Collections.Generic;
using System.Data.SQLite;

public static class DatabaseHelper
{
    private const string ConnectionString = "Data Source=reviews.db;Version=3;";

    static DatabaseHelper()
    {
        InitializeDatabase();
        InitializeData(); // Dodajemy wywołanie metody inicjującej dane
    }

    private static void InitializeDatabase()
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Products (
                    ProductId INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL
                );
                CREATE TABLE IF NOT EXISTS Reviews (
                    ReviewId INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProductId INTEGER NOT NULL,
                    Content TEXT NOT NULL,
                    Rating INTEGER NOT NULL,
                    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
                );
            ";
            command.ExecuteNonQuery();
        }
    }

    private static void InitializeData()
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Products";
            long count = (long)command.ExecuteScalar();

            if (count == 0)
            {
                command.CommandText = @"
                    INSERT INTO Products (Name) VALUES
                    ('Product 1'),
                    ('Product 2'),
                    ('Product 3'),
                    ('Product 4'),
                    ('Product 5');
                ";
                command.ExecuteNonQuery();

                command.CommandText = @"
                    INSERT INTO Reviews (ProductId, Content, Rating) VALUES
                    (1, 'Review 1 for Product 1', 5),
                    (1, 'Review 2 for Product 1', 4),
                    (2, 'Review 1 for Product 2', 3),
                    (2, 'Review 2 for Product 2', 2),
                    (3, 'Review 1 for Product 3', 1);
                ";
                command.ExecuteNonQuery();
            }
        }
    }

    public static List<Product> GetProducts()
    {
        var products = new List<Product>();
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Products";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        ProductId = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
            }
        }
        return products;
    }

    public static List<Review> GetReviews(int productId)
    {
        var reviews = new List<Review>();
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Reviews WHERE ProductId = @ProductId";
            command.Parameters.AddWithValue("@ProductId", productId);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    reviews.Add(new Review
                    {
                        ReviewId = reader.GetInt32(0),
                        ProductId = reader.GetInt32(1),
                        Content = reader.GetString(2),
                        Rating = reader.GetInt32(3)
                    });
                }
            }
        }
        return reviews;
    }

    public static void AddReview(Review review)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Reviews (ProductId, Content, Rating) VALUES (@ProductId, @Content, @Rating)";
            command.Parameters.AddWithValue("@ProductId", review.ProductId);
            command.Parameters.AddWithValue("@Content", review.Content);
            command.Parameters.AddWithValue("@Rating", review.Rating);
            command.ExecuteNonQuery();
        }
    }

    public static void DeleteReview(int reviewId)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Reviews WHERE ReviewId = @ReviewId";
            command.Parameters.AddWithValue("@ReviewId", reviewId);
            command.ExecuteNonQuery();
        }
    }
}
