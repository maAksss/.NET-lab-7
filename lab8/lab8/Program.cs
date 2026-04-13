using System;
using System.Collections.Generic;
using Npgsql;

namespace StudentDatabaseApp
{
    class Program
    {
        static string connString = "Host=localhost;Username=postgres;Password=123;Database=student_db";

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Управління Студентами (PostgreSQL) ===");
                Console.WriteLine("1. Вивести список студентів");
                Console.WriteLine("2. Додати нового студента");
                Console.WriteLine("3. Оновити дані студента");
                Console.WriteLine("4. Видалити студента");
                Console.WriteLine("0. Вихід");
                Console.Write("\nОберіть дію: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": ShowAllStudents(); break;
                    case "2": AddStudent(); break;
                    case "3": UpdateStudent(); break;
                    case "4": DeleteStudent(); break;
                    case "0": return;
                }
                Console.WriteLine("\nНатисніть будь-яку клавішу...");
                Console.ReadKey();
            }
        }

        static void ShowAllStudents()
        {
            using var conn = new NpgsqlConnection(connString);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT StudentCode, FullName, GroupName, Department, Course, Specialty FROM Students ORDER BY StudentCode", conn);
            using var reader = cmd.ExecuteReader();

            Console.WriteLine("\n{0,-5} | {1,-30} | {2,-10} | {3,-5} | {4,-30} | {5,-30}",
                "Код", "ПІБ", "Група", "Курс", "Кафедра", "Спеціальність");
            Console.WriteLine(new string('-', 110));

            while (reader.Read())
                Console.WriteLine("{0,-5} | {1,-30} | {2,-10} | {3,-5} | {4,-30} | {5,-30}",
                    reader["StudentCode"], reader["FullName"], reader["GroupName"],
                    reader["Course"], reader["Department"], reader["Specialty"]);
        }

        static void AddStudent()
        {
            Console.Write("ПІБ: "); string name = Console.ReadLine();
            Console.Write("Група: "); string group = Console.ReadLine();
            Console.Write("Кафедра: "); string dept = Console.ReadLine();
            Console.Write("Курс: "); int course = int.Parse(Console.ReadLine());
            Console.Write("Спеціальність: "); string spec = Console.ReadLine();

            using var conn = new NpgsqlConnection(connString);
            conn.Open();
            string sql = "INSERT INTO Students (FullName, GroupName, Department, Course, Specialty) VALUES (@n, @g, @d, @c, @s)";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("n", name);
            cmd.Parameters.AddWithValue("g", group);
            cmd.Parameters.AddWithValue("d", dept);
            cmd.Parameters.AddWithValue("c", course);
            cmd.Parameters.AddWithValue("s", spec);
            cmd.ExecuteNonQuery();
            Console.WriteLine("Запис додано!");
        }

        static void UpdateStudent()
        {
            Console.Write("Введіть Код студента для редагування: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Нова група: ");
            string newGroup = Console.ReadLine();

            using var conn = new NpgsqlConnection(connString);
            conn.Open();
            string sql = "UPDATE Students SET GroupName = @g WHERE StudentCode = @id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("g", newGroup);
            cmd.Parameters.AddWithValue("id", id);
            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine(rows > 0 ? "Оновлено успішно!" : "Студента не знайдено.");
        }

        static void DeleteStudent()
        {
            Console.Write("Введіть Код студента для видалення: ");
            int id = int.Parse(Console.ReadLine());

            using var conn = new NpgsqlConnection(connString);
            conn.Open();
            string sql = "DELETE FROM Students WHERE StudentCode = @id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", id);
            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine(rows > 0 ? "Видалено!" : "Студента не знайдено.");
        }
    }
}