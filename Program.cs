using System;
using MySqlConnector;

namespace PrestamoBiblioteca
{
    internal class Program
    {
        // ⚠️ Sustituye 'tu_contraseña' por la clave de tu servidor MySQL local
        private const string CadenaConexion = "Server=localhost;Database=mi_proyecto;User=root;Password=MiPassword123;";

        static void Main(string[] args)
        {
            try
            {
                CrearTablasSiNoExisten();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al conectar a MySQL: {ex.Message}");
                Console.WriteLine("Asegúrate de cambiar 'tu_contraseña' en CadenaConexion por la clave de tu MySQL.");
                return;
            }

            bool salir = false;

            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("================================================");
                Console.WriteLine("  SISTEMA DE PRÉSTAMOS DE BIBLIOTECA (MySQL)    ");
                Console.WriteLine("================================================");
                Console.WriteLine("1. Registrar Autor");
                Console.WriteLine("2. Registrar Lector");
                Console.WriteLine("3. Registrar Libro");
                Console.WriteLine("4. Registrar Préstamo de Libro");
                Console.WriteLine("5. Ver Préstamos Registrados");
                Console.WriteLine("6. Ver Autores Registrados");
                Console.WriteLine("7. Ver Lectores Registrados");
                Console.WriteLine("8. Salir");
                Console.WriteLine("================================================");
                Console.Write("Selecciona una opción (1-8): ");

                string? opcion = Console.ReadLine();

                using (var conn = new MySqlConnection(CadenaConexion))
                {
                    try
                    {
                        conn.Open();
                        Console.WriteLine();

                        switch (opcion)
                        {
                            case "1":
                                RegistrarAutor(conn);
                                break;
                            case "2":
                                RegistrarLector(conn);
                                break;
                            case "3":
                                RegistrarLibro(conn);
                                break;
                            case "4":
                                RegistrarPrestamo(conn);
                                break;
                            case "5":
                                VerPrestamos(conn);
                                break;
                            case "6":
                                VerAutores(conn);
                                break;
                            case "7":
                                VerLectores(conn);
                                break;
                            case "8":
                                salir = true;
                                Console.WriteLine("¡Saliendo del sistema!");
                                continue;
                            default:
                                Console.WriteLine("Opción no válida. Intenta de nuevo.");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n[Error de Base de Datos]: {ex.Message}");
                    }
                }

                if (!salir)
                {
                    Console.WriteLine("\nPresiona cualquier tecla para volver al menú...");
                    Console.ReadKey();
                }
            }
        }

        static void CrearTablasSiNoExisten()
        {
            using var conn = new MySqlConnection(CadenaConexion);
            conn.Open();

            string sql = @"
                CREATE TABLE IF NOT EXISTS AUTOR (
                    ID INT AUTO_INCREMENT PRIMARY KEY,
                    NOMBRE VARCHAR(255) NOT NULL
                );

                CREATE TABLE IF NOT EXISTS LECTORES (
                    ID INT AUTO_INCREMENT PRIMARY KEY,
                    CODIGO VARCHAR(50) NOT NULL,
                    NOMBRE VARCHAR(255) NOT NULL,
                    CORREO VARCHAR(255) NOT NULL,
                    TELEFONO VARCHAR(50) NOT NULL
                );

                CREATE TABLE IF NOT EXISTS LIBROS (
                    ID INT AUTO_INCREMENT PRIMARY KEY,
                    CODIGO VARCHAR(50) NOT NULL,
                    TITULO VARCHAR(255) NOT NULL,
                    AUTOR INT,
                    EJEMPLARES INT NOT NULL,
                    CHECK (EJEMPLARES >= 0),
                    FOREIGN KEY (AUTOR) REFERENCES AUTOR(ID) ON DELETE SET NULL
                );

                CREATE TABLE IF NOT EXISTS PRESTAMOS (
                    ID INT AUTO_INCREMENT PRIMARY KEY,
                    LECTORID INT NOT NULL,
                    LIBROID INT NOT NULL,
                    FECHAPRESTAMO DATE NOT NULL,
                    FECHADEV DATE NOT NULL,
                    FECHALIMITE DATE NOT NULL
                );
            ";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        static void RegistrarAutor(MySqlConnection conn)
        {
            Console.WriteLine("--- REGISTRAR NUEVO AUTOR ---");
            Console.Write("Nombre del Autor: ");
            string nombre = Console.ReadLine() ?? "";

            string query = "INSERT INTO AUTOR (NOMBRE) VALUES (@nombre);";
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.ExecuteNonQuery();
                Console.WriteLine("✔ Autor registrado exitosamente en MySQL.");
            }
        }

        static void RegistrarLector(MySqlConnection conn)
        {
            Console.WriteLine("--- REGISTRAR NUEVO LECTOR ---");
            Console.Write("Código del Lector: ");
            string codigo = Console.ReadLine() ?? "";
            Console.Write("Nombre completo: ");
            string nombre = Console.ReadLine() ?? "";
            Console.Write("Correo electrónico: ");
            string correo = Console.ReadLine() ?? "";
            Console.Write("Teléfono: ");
            string telefono = Console.ReadLine() ?? "";

            string query = "INSERT INTO LECTORES (CODIGO, NOMBRE, CORREO, TELEFONO) VALUES (@codigo, @nombre, @correo, @telefono);";
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@codigo", codigo);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@correo", correo);
                cmd.Parameters.AddWithValue("@telefono", telefono);
                cmd.ExecuteNonQuery();
                Console.WriteLine("✔ Lector registrado exitosamente en MySQL.");
            }
        }

        static void RegistrarLibro(MySqlConnection conn)
        {
            Console.WriteLine("--- REGISTRAR NUEVO LIBRO ---");
            Console.Write("Código del Libro: ");
            string codigo = Console.ReadLine() ?? "";
            Console.Write("Título del Libro: ");
            string titulo = Console.ReadLine() ?? "";
            Console.Write("ID del Autor: ");
            int autorId = int.Parse(Console.ReadLine() ?? "0");
            Console.Write("Número de Ejemplares: ");
            int ejemplares = int.Parse(Console.ReadLine() ?? "0");

            string query = "INSERT INTO LIBROS (CODIGO, TITULO, AUTOR, EJEMPLARES) VALUES (@codigo, @titulo, @autorId, @ejemplares);";
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@codigo", codigo);
                cmd.Parameters.AddWithValue("@titulo", titulo);
                cmd.Parameters.AddWithValue("@autorId", autorId);
                cmd.Parameters.AddWithValue("@ejemplares", ejemplares);
                cmd.ExecuteNonQuery();
                Console.WriteLine("✔ Libro registrado exitosamente en MySQL.");
            }
        }

        static void RegistrarPrestamo(MySqlConnection conn)
        {
            Console.WriteLine("--- REGISTRAR PRÉSTAMO ---");
            Console.Write("ID del Lector: ");
            int lectorId = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("ID del Libro: ");
            int libroId = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Días de préstamo (ej. 7, 14): ");
            int dias = int.Parse(Console.ReadLine() ?? "0");

            string checkQuery = "SELECT EJEMPLARES FROM LIBROS WHERE ID = @libroId;";
            using (var checkCmd = new MySqlCommand(checkQuery, conn))
            {
                checkCmd.Parameters.AddWithValue("@libroId", libroId);
                object? result = checkCmd.ExecuteScalar();

                if (result == null || Convert.ToInt32(result) <= 0)
                {
                    Console.WriteLine("❌ No hay ejemplares disponibles para prestar este libro.");
                    return;
                }
            }

            DateTime fechaPrestamo = DateTime.Now;
            DateTime fechaLimite = fechaPrestamo.AddDays(dias);
            DateTime fechaDev = fechaLimite;

            string insertQuery = @"INSERT INTO PRESTAMOS (LECTORID, LIBROID, FECHAPRESTAMO, FECHADEV, FECHALIMITE) 
                                 VALUES (@lectorId, @libroId, @fechaPrestamo, @fechaDev, @fechaLimite);";

            using (var cmd = new MySqlCommand(insertQuery, conn))
            {
                cmd.Parameters.AddWithValue("@lectorId", lectorId);
                cmd.Parameters.AddWithValue("@libroId", libroId);
                cmd.Parameters.AddWithValue("@fechaPrestamo", fechaPrestamo.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@fechaDev", fechaDev.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@fechaLimite", fechaLimite.ToString("yyyy-MM-dd"));
                cmd.ExecuteNonQuery();
            }

            string updateQuery = "UPDATE LIBROS SET EJEMPLARES = EJEMPLARES - 1 WHERE ID = @libroId;";
            using (var updateCmd = new MySqlCommand(updateQuery, conn))
            {
                updateCmd.Parameters.AddWithValue("@libroId", libroId);
                updateCmd.ExecuteNonQuery();
            }

            Console.WriteLine($"✔ Préstamo registrado correctamente. Devolver antes de: {fechaLimite.ToString("yyyy-MM-dd")}");
        }

        static void VerPrestamos(MySqlConnection conn)
        {
            Console.WriteLine("--- LISTADO DE PRÉSTAMOS ---");
            string query = @"SELECT p.ID, 
                                    COALESCE(l.NOMBRE, CONCAT('Sin Lector (ID: ', p.LECTORID, ')')) AS Lector, 
                                    COALESCE(b.TITULO, CONCAT('Sin Libro (ID: ', p.LIBROID, ')')) AS Libro, 
                                    p.FECHAPRESTAMO, p.FECHALIMITE 
                             FROM PRESTAMOS p
                             LEFT JOIN LECTORES l ON p.LECTORID = l.ID
                             LEFT JOIN LIBROS b ON p.LIBROID = b.ID;";

            using (var cmd = new MySqlCommand(query, conn))
            using (var reader = cmd.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    Console.WriteLine("No existen préstamos registrados.");
                    return;
                }

                while (reader.Read())
                {
                    string fPrestamo = Convert.ToDateTime(reader["FECHAPRESTAMO"]).ToString("yyyy-MM-dd");
                    string fLimite = Convert.ToDateTime(reader["FECHALIMITE"]).ToString("yyyy-MM-dd");

                    Console.WriteLine($"[ID Préstamo: {reader["ID"]}] | Lector: {reader["Lector"]} | Libro: {reader["Libro"]}");
                    Console.WriteLine($"   Fecha Préstamo: {fPrestamo} | Fecha Límite: {fLimite}");
                    Console.WriteLine("------------------------------------------------------------------");
                }
            }
        }

        static void VerAutores(MySqlConnection conn)
        {
            Console.WriteLine("--- LISTADO DE AUTORES ---");
            string query = "SELECT ID, NOMBRE FROM AUTOR;";

            using (var cmd = new MySqlCommand(query, conn))
            using (var reader = cmd.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    Console.WriteLine("No existen autores registrados.");
                    return;
                }

                while (reader.Read())
                {
                    Console.WriteLine($"[ID Autor: {reader["ID"]}] - Nombre: {reader["NOMBRE"]}");
                }
            }
        }

        static void VerLectores(MySqlConnection conn)
        {
            Console.WriteLine("--- LISTADO DE LECTORES ---");
            string query = "SELECT ID, CODIGO, NOMBRE, CORREO, TELEFONO FROM LECTORES;";

            using (var cmd = new MySqlCommand(query, conn))
            using (var reader = cmd.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    Console.WriteLine("No existen lectores registrados.");
                    return;
                }

                while (reader.Read())
                {
                    Console.WriteLine($"[ID: {reader["ID"]}] Código: {reader["CODIGO"]} | Nombre: {reader["NOMBRE"]} | Correo: {reader["CORREO"]}");
                }
            }
        }
    }
}