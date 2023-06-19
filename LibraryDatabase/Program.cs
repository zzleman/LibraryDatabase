using System;
using System.Data;
using Microsoft.Data.SqlClient;
//using LibraryDatabase.Infrastructure.Utilities.Helpers;

public class Enums
{
    public enum MenuOption
    {
        GetBooksList = 1,
        AddBook,
        SearchBooksByISBN,
        DeleteBook,
        UpdateBook,
        SearchBooksByName,
        GetUserList,
        AddUser,
        UpdateUser,
        DeleteUser,
        SearchUserByPhone,
        BorrowBook,
        ReturnBook,
        Exit
    }
}

public class Program
{
    static string connectionStr = "Server=localhost;Database=Library;User Id=SA;Password=reallyStrongPwd123;TrustServerCertificate=True ";

    static void Main()
    {
        RunProgram();
    }

    static void DisplayMenu()
    {
        Console.WriteLine("Menu Options:");
        Console.WriteLine("1. Get Books List");
        Console.WriteLine("2. Add Book");
        Console.WriteLine("3. Search Books by ISBN");
        Console.WriteLine("4. Delete Book");
        Console.WriteLine("5. Update Book");
        Console.WriteLine("6. Search Books by Name");
        Console.WriteLine("7. Get Users List");
        Console.WriteLine("8. Add User");
        Console.WriteLine("9. Update User");
        Console.WriteLine("10. Delete User");
        Console.WriteLine("11. Search User by Phone");
        Console.WriteLine("12. Borrow Book");
        Console.WriteLine("13. Return Book");
        Console.WriteLine("14. Exit");
        Console.WriteLine("Enter your choice (1-14):");
    }

    static void MenuChoices(Enums.MenuOption choice)
    {
        switch (choice)
        {
            case Enums.MenuOption.GetBooksList:
                GetBooksList();
                break;
            case Enums.MenuOption.AddBook:
                AddBook();
                break;
            case Enums.MenuOption.SearchBooksByISBN:
                Console.WriteLine("Enter the partial ISBN:");
                string partialIsbn = Console.ReadLine();
                SearchBooksByISBN(partialIsbn);
                break;
            case Enums.MenuOption.DeleteBook:
                Console.WriteLine("Enter the ISBN of the book to delete:");
                string isbnToDelete = Console.ReadLine();
                DeleteBook(isbnToDelete);
                break;
            case Enums.MenuOption.UpdateBook:
                Console.WriteLine("Enter the ISBN of the book to update:");
                string isbnToUpdate = Console.ReadLine();
                Console.WriteLine("Enter the new name:");
                string newName = Console.ReadLine();
                Console.WriteLine("Enter the new page count:");
                int newPageCount = Convert.ToInt32(Console.ReadLine());
                UpdateBook(isbnToUpdate, newName, newPageCount);
                break;
            case Enums.MenuOption.SearchBooksByName:
                SearchBooksByName();
                break;
            case Enums.MenuOption.GetUserList:
                GetUserList();
                break;
            case Enums.MenuOption.AddUser:
                AddUser();
                break;
            case Enums.MenuOption.UpdateUser:
                GetUserList();
                Console.WriteLine("Enter the Id of the user for updating:");
                int id = Convert.ToInt32(Console.ReadLine());
                UpdateUser(id);
                break;
            case Enums.MenuOption.DeleteUser:
                DeleteUser();
                break;
            case Enums.MenuOption.SearchUserByPhone:
                SearchUserByPhone();
                break;
            case Enums.MenuOption.BorrowBook:
                BorrowBook();
                break;
            case Enums.MenuOption.ReturnBook:
                ReturnBook();
                break;
            case Enums.MenuOption.Exit:
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("Exiting...");
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Invalid menu choice.");
                break;
        }
    }

 
    static void RunProgram()
    {
        while (true)
        {
            DisplayMenu();
            string choiceStr = Console.ReadLine();
            if (Enum.TryParse(choiceStr, out Enums.MenuOption choice))
            {
                MenuChoices(choice);
            }
            else
            {
                Console.WriteLine("Invalid menu choice.");
            }
        }
    }

    static void GetBooksList()
    {
        string query = "SELECT * FROM Books";
        using (SqlConnection conn = new SqlConnection(connectionStr))
        {
            try
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"Id: {reader[0]}; Name: {reader[1]}; ISBN: {reader[2]}; Pages: {reader[3]}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            conn.Close();
        }
    }

    static void AddBook()
    {
        Console.WriteLine("Enter the ISBN, name, and page count (separated by new lines):");

        Console.WriteLine("ISBN:");
        string isbn = Console.ReadLine();

        Console.WriteLine("Name:");
        string name = Console.ReadLine();

        Console.WriteLine("Page Count:");
        int pageCount = Convert.ToInt32(Console.ReadLine());

        using (SqlConnection conn = new SqlConnection(connectionStr))
        {
            string duplicateCheckQuery = "SELECT COUNT(*) FROM Books WHERE ISBN = @ISBN OR Name = @Name";
            using (SqlCommand duplicateCheckCommand = new SqlCommand(duplicateCheckQuery, conn))
            {
                duplicateCheckCommand.Parameters.AddWithValue("@ISBN", isbn);
                duplicateCheckCommand.Parameters.AddWithValue("@Name", name);

                conn.Open();
                int duplicateCount = (int)duplicateCheckCommand.ExecuteScalar();

                if (duplicateCount > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("This book already exists.");
                    return;
                }
            }

            string insertQuery = "INSERT INTO Books (ISBN, Name, PageCount) VALUES (@ISBN, @Name, @PageCount)";
            using (SqlCommand insertCommand = new SqlCommand(insertQuery, conn))
            {
                insertCommand.Parameters.AddWithValue("@ISBN", isbn);
                insertCommand.Parameters.AddWithValue("@Name", name);
                insertCommand.Parameters.AddWithValue("@PageCount", pageCount);

                insertCommand.ExecuteNonQuery();
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("A new book added successfully.");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    static void SearchBooksByISBN(string partialIsbn)
    {
        string query = "SELECT * FROM Books WHERE ISBN LIKE @PartialISBN";
        using (SqlConnection conn = new SqlConnection(connectionStr))
        {
            try
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@PartialISBN", partialIsbn + "%");

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"Id: {reader.GetInt32(0)}; Name: {reader.GetString(1)}; ISBN: {reader.GetString(2)}; Pages: {reader.GetInt32(3)}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No books found with the given partial ISBN.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    static void DeleteBook(string isbn)
    {
        string query = "DELETE FROM Books WHERE ISBN = @ISBN";
        using (SqlConnection conn = new SqlConnection(connectionStr))
        {
            try
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@ISBN", isbn);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Book deleted successfully.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No book found with the given ISBN.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    static void UpdateBook(string isbn, string newName, int newPageCount)
    {
        string query = "UPDATE Books SET Name = @NewName, PageCount = @NewPageCount WHERE ISBN = @ISBN";
        using (SqlConnection conn = new SqlConnection(connectionStr))
        {
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NewName", newName);
                    cmd.Parameters.AddWithValue("@NewPageCount", newPageCount);
                    cmd.Parameters.AddWithValue("@ISBN", isbn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    static void SearchBooksByName()
    {
        string name;
        do
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Please enter at least two letters to search for books by name:");
            Console.ForegroundColor = ConsoleColor.White;
            name = Console.ReadLine();
        } while (string.IsNullOrWhiteSpace(name) || name.Length < 2);

        string query = "SELECT * FROM Books WHERE Name LIKE @Name";
        using (SqlConnection conn = new SqlConnection(connectionStr))
        {
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", "%" + name + "%");

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine($"Id: {reader.GetInt32(0)}; Name: {reader.GetString(1)}; ISBN: {reader.GetString(2)}; Pages: {reader.GetInt32(3)}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No books found with the given name.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
    static void GetUserList()
    {
        string query = "SELECT * FROM Users";
        using (SqlConnection conn = new SqlConnection(connectionStr))
        {
            try
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            string surname = reader.GetString(2);
                            string phone = reader.GetString(3);
                            string address = reader.GetString(4);
                            string email = reader.GetString(5);

                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine($"Id: {id}; Name: {name}; Surname: {surname}; Phone: {phone}; Address: {address}; Email: {email}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No users found.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    static void AddUser()
    {
        Console.WriteLine("Enter the user details:");
        Console.WriteLine("Name:");
        string name = Console.ReadLine();

        Console.WriteLine("Surname:");
        string surname = Console.ReadLine();

        Console.WriteLine("Phone:");
        string phone = Console.ReadLine();

        Console.WriteLine("Address:");
        string address = Console.ReadLine();

        Console.WriteLine("Mail:");
        string mail = Console.ReadLine();

        using (SqlConnection conn = new SqlConnection(connectionStr))
        {
            try
            {
                conn.Open();

                string duplicateCheckQuery = "SELECT COUNT(*) FROM Users WHERE Phone = @Phone OR Mail = @Mail";
                using (SqlCommand duplicateCheckCommand = new SqlCommand(duplicateCheckQuery, conn))
                {
                    duplicateCheckCommand.Parameters.AddWithValue("@Phone", phone);
                    duplicateCheckCommand.Parameters.AddWithValue("@Mail", mail);

                    int duplicateCount = (int)duplicateCheckCommand.ExecuteScalar();

                    if (duplicateCount > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("A user with the same phone number or mail already exists.");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }

                string insertQuery = "INSERT INTO Users (Name, Surname, Phone, Address, Mail) VALUES (@Name, @Surname, @Phone, @Address, @Mail)";
                using (SqlCommand insertCommand = new SqlCommand(insertQuery, conn))
                {
                    insertCommand.Parameters.AddWithValue("@Name", name);
                    insertCommand.Parameters.AddWithValue("@Surname", surname);
                    insertCommand.Parameters.AddWithValue("@Phone", phone);
                    insertCommand.Parameters.AddWithValue("@Address", address);
                    insertCommand.Parameters.AddWithValue("@Mail", mail);

                    insertCommand.ExecuteNonQuery();
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("A new user added successfully.");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }



    static void UpdateUser(int userId)
    {
        using (SqlConnection conn = new SqlConnection(connectionStr))
        {
            try
            {
                conn.Open();

                Console.WriteLine("Which value would you like to update?");
                Console.WriteLine("1. Name");
                Console.WriteLine("2. Surname");
                Console.WriteLine("3. Phone");
                Console.WriteLine("4. Address");
                Console.WriteLine("5. Mail");

                Console.Write("Enter your choice (1-5): ");
                int choice = Convert.ToInt32(Console.ReadLine());

                string updateQuery = "";
                string columnName = "";

                switch (choice)
                {
                    case 1:
                        updateQuery = "UPDATE Users SET Name = @NewValue WHERE Id = @UserId";
                        columnName = "Name";
                        break;
                    case 2:
                        updateQuery = "UPDATE Users SET Surname = @NewValue WHERE Id = @UserId";
                        columnName = "Surname";
                        break;
                    case 3:
                        updateQuery = "UPDATE Users SET Phone = @NewValue WHERE Id = @UserId";
                        columnName = "Phone";
                        break;
                    case 4:
                        updateQuery = "UPDATE Users SET Address = @NewValue WHERE Id = @UserId";
                        columnName = "Address";
                        break;
                    case 5:
                        updateQuery = "UPDATE Users SET Mail = @NewValue WHERE Id = @UserId";
                        columnName = "Mail";
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        return;
                }

                Console.Write("Enter the new value: ");
                string newValue = Console.ReadLine();

                using (SqlCommand command = new SqlCommand(updateQuery, conn))
                {
                    command.Parameters.AddWithValue("@NewValue", newValue);
                    command.Parameters.AddWithValue("@UserId", userId);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{columnName} updated successfully.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No user found with the given ID.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    static void DeleteUser()
    {
        GetUserList();

        Console.WriteLine("Enter the user ID of the user to delete:");
        int userId = Convert.ToInt32(Console.ReadLine());

        using (SqlConnection conn = new SqlConnection(connectionStr))
        {
            try
            {
                conn.Open();
                string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Id = @UserId";
                using (SqlCommand checkUserCommand = new SqlCommand(checkUserQuery, conn))
                {
                    checkUserCommand.Parameters.AddWithValue("@UserId", userId);

                    int userCount = (int)checkUserCommand.ExecuteScalar();

                    if (userCount > 0)
                    {
                        string deleteUserQuery = "DELETE FROM Users WHERE Id = @UserId";
                        using (SqlCommand deleteUserCommand = new SqlCommand(deleteUserQuery, conn))
                        {
                            deleteUserCommand.Parameters.AddWithValue("@UserId", userId);
                            deleteUserCommand.ExecuteNonQuery();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("User deleted successfully.");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("User not found.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
    static void SearchUserByPhone()
    {
        Console.WriteLine("Enter the phone number to search for:");
        string phoneNumber = Console.ReadLine();

        string query = "SELECT * FROM Users WHERE Phone LIKE @Phone";
        using (SqlConnection conn = new SqlConnection(connectionStr))
        {
            try
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Phone", "%" + phoneNumber + "%");

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"Id: {reader.GetInt32(0)}; Name: {reader.GetString(1)}; Surname: {reader.GetString(2)}; Phone: {reader.GetString(3)};" +
                                $" Address: {reader.GetString(4)}; Email:{reader.GetString(5)}");
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No users found with the given phone number.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    static void BorrowBook()
    {
        Console.WriteLine("Enter the user ID:");
        int userId = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Enter the book ID:");
        int bookId = Convert.ToInt32(Console.ReadLine());

        DateTime borrowingDate = DateTime.Now;

        using (SqlConnection conn = new SqlConnection(connectionStr))
        {
            try
            {
                conn.Open();

                string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Id = @UserId";
                using (SqlCommand checkUserCommand = new SqlCommand(checkUserQuery, conn))
                {
                    checkUserCommand.Parameters.AddWithValue("@UserId", userId);

                    int userCount = (int)checkUserCommand.ExecuteScalar();

                    if (userCount == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("User not found.");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }

                string checkBookQuery = "SELECT COUNT(*) FROM Books WHERE Id = @BookId";
                using (SqlCommand checkBookCommand = new SqlCommand(checkBookQuery, conn))
                {
                    checkBookCommand.Parameters.AddWithValue("@BookId", bookId);

                    int bookCount = (int)checkBookCommand.ExecuteScalar();

                    if (bookCount == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Book not found.");
                        Console.ForegroundColor = ConsoleColor.White;

                        return;
                    }
                }

                string checkBorrowedQuery = "SELECT COUNT(*) FROM Borrowings WHERE BookId = @BookId AND ReturnDate IS NULL";
                using (SqlCommand checkBorrowedCommand = new SqlCommand(checkBorrowedQuery, conn))
                {
                    checkBorrowedCommand.Parameters.AddWithValue("@BookId", bookId);

                    int borrowedCount = (int)checkBorrowedCommand.ExecuteScalar();

                    if (borrowedCount > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("This book is already borrowed by someone else.");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }

                string insertBorrowQuery = "INSERT INTO Borrowings (BookId, UserId, BorrowingDate) VALUES (@BookId, @UserId, @BorrowingDate)";
                using (SqlCommand insertBorrowCommand = new SqlCommand(insertBorrowQuery, conn))
                {
                    insertBorrowCommand.Parameters.AddWithValue("@BookId", bookId);
                    insertBorrowCommand.Parameters.AddWithValue("@UserId", userId);
                    insertBorrowCommand.Parameters.AddWithValue("@BorrowingDate", borrowingDate);

                    insertBorrowCommand.ExecuteNonQuery();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Book borrowed successfully.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    static void ReturnBook()
    {
        Console.WriteLine("Enter the user ID:");
        int userId = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Enter the book ID:");
        int bookId = Convert.ToInt32(Console.ReadLine());

        DateTime returnDate = DateTime.Now;

        using (SqlConnection conn = new SqlConnection(connectionStr))
        {
            try
            {
                conn.Open();

                string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Id = @UserId";
                using (SqlCommand checkUserCommand = new SqlCommand(checkUserQuery, conn))
                {
                    checkUserCommand.Parameters.AddWithValue("@UserId", userId);

                    int userCount = (int)checkUserCommand.ExecuteScalar();

                    if (userCount == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("User not found.");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }

                string checkBookQuery = "SELECT COUNT(*) FROM Books WHERE Id = @BookId";
                using (SqlCommand checkBookCommand = new SqlCommand(checkBookQuery, conn))
                {
                    checkBookCommand.Parameters.AddWithValue("@BookId", bookId);

                    int bookCount = (int)checkBookCommand.ExecuteScalar();

                    if (bookCount == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Book not found.");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }

                string checkBorrowedQuery = "SELECT COUNT(*) FROM Borrowings WHERE BookId = @BookId AND UserId = @UserId AND ReturnDate IS NULL";
                using (SqlCommand checkBorrowedCommand = new SqlCommand(checkBorrowedQuery, conn))
                {
                    checkBorrowedCommand.Parameters.AddWithValue("@BookId", bookId);
                    checkBorrowedCommand.Parameters.AddWithValue("@UserId", userId);

                    int borrowedCount = (int)checkBorrowedCommand.ExecuteScalar();

                    if (borrowedCount == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("This book is not borrowed by the user.");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }

                string updateBorrowQuery = "UPDATE Borrowings SET ReturnDate = @ReturnDate WHERE BookId = @BookId AND UserId = @UserId AND ReturnDate IS NULL";
                using (SqlCommand updateBorrowCommand = new SqlCommand(updateBorrowQuery, conn))
                {
                    updateBorrowCommand.Parameters.AddWithValue("@ReturnDate", returnDate);
                    updateBorrowCommand.Parameters.AddWithValue("@BookId", bookId);
                    updateBorrowCommand.Parameters.AddWithValue("@UserId", userId);

                    int rowsAffected = updateBorrowCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Book returned successfully.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Failed to return the book.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }



}