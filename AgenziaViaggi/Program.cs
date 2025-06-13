using System;
using System.Text;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;

#region Variabili Globali e Metodo Main
//Classe principale del programma
class Program
{
    //void MenuAdmin(); //Test menu admin
    //return;

    //Stringa di connessione al database MySQL
    static string connString = "Server=localhost;Database=AgenziaViaggi;User=root;Password=INSERISCI LA TUA PASSWORD;";
    //ID dell'utente attualmente loggato (null se nessuno)
    static int? utenteLoggatoID = null;
    //Flag per determinare se l'utente loggato è un amministratore
    static bool isAdmin = false;

    //Metodo principale: mostra il menu iniziale e gestisce la navigazione principale
    static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== MENU PRINCIPALE ===");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Registrazione");
            Console.WriteLine("3. Esci");
            Console.Write("Scelta: ");
            string scelta = Console.ReadLine();

            switch (scelta)
            {
                case "1":
                    Login(); //Avvia la procedura di login
                    break;
                case "2":
                    Registrazione(); //Avvia la procedura di registrazione
                    break;
                case "3":
                    return; //Esce dal programma
                default:
                    Console.WriteLine("Scelta non valida.");
                    break;
            }
        }
    }
#endregion

#region Metodi di Autenticazione

    //Metodo per effettuare il login di un utente
    static void Login()
    {
        Console.Clear();
        Console.WriteLine("=== LOGIN ===");
        Console.Write("Username: ");
        string username = Console.ReadLine();
        Console.Write("Password: ");
        string password = Console.ReadLine();

        try
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT utenteID, isAdmin, password FROM Utente WHERE username = @u";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedPassword = reader.GetString("password");
                            bool authenticated = false;

                            //Prova prima la password in chiaro
                            if (storedPassword == password)
                            {
                                authenticated = true;
                            }
                            //Se non è in chiaro, prova la password hashata
                            else if (storedPassword == CalcolaSHA256(password))
                            {
                                authenticated = true;
                            }

                            if (authenticated)
                            {
                                utenteLoggatoID = reader.GetInt32("utenteID");
                                isAdmin = reader.GetBoolean("isAdmin");
                                Console.WriteLine("Login effettuato!");
                                if (isAdmin)
                                    MenuAdmin();
                                else
                                    MenuUtente();
                            }
                            else
                            {
                                Console.WriteLine("Credenziali errate.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Credenziali errate.");
                        }
                    }
                }
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Errore durante il login: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Si è verificato un errore imprevisto durante il login: " + ex.Message);
        }
    }

    //Metodo per registrare un nuovo utente
    static void Registrazione()
    {
        Console.Clear();
        Console.WriteLine("=== REGISTRAZIONE ===");
        Console.Write("Username: ");
        string username = Console.ReadLine();
        Console.Write("Password: ");
        string password = Console.ReadLine();
        string passwordHash = CalcolaSHA256(password); //Calcola l'hash della password

        using (var conn = new MySqlConnection(connString))
        {
            conn.Open();
            //Query per inserire un nuovo utente
            string query = "INSERT INTO Utente (username, password) VALUES (@u, @p)";
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", passwordHash);
                try
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Registrazione completata!");
                }
                catch (MySqlException ex)
                {
                    //Gestione errori (es. username già esistente)
                    Console.WriteLine("Errore: " + ex.Message);
                }
            }
        }
        Console.WriteLine("Premi un tasto per continuare...");
    }

    //Menu per l'utente normale (non admin)
    static void MenuUtente()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== MENU UTENTE - Benvenuto! ===");
            Console.WriteLine("1. Visualizza Luoghi");
            Console.WriteLine("2. Visualizza Attrazioni");
            Console.WriteLine("3. Prenota Attrazione");
            Console.WriteLine("4. Visualizza Prenotazioni");
            Console.WriteLine("5. Logout");
            Console.Write("Scelta: ");
            string scelta = Console.ReadLine();

            switch (scelta)
            {
                case "1":
                    VisualizzaLuoghi(); //Mostra tutti i luoghi
                    break;
                case "2":
                    VisualizzaAttrazioni(); //Mostra tutte le attrazioni
                    break;
                case "3":
                    PrenotaAttrazione(); //Permette di prenotare un'attrazione
                    break;
                case "4":
                    VisualizzaPrenotazioni(); //Mostra le prenotazioni dell'utente
                    break;
                case "5":
                    utenteLoggatoID = null;
                    isAdmin = false;
                    return; //Logout
                default:
                    Console.WriteLine("Scelta non valida.");
                    break;
            }
        }
    }

    //Menu per l'amministratore
    static void MenuAdmin()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== MENU ADMIN - Benvenuto Amministratore! ===");
            Console.WriteLine("1. Gestisci Luoghi");
            Console.WriteLine("2. Gestisci Attrazioni");
            Console.WriteLine("3. Visualizza Prenotazioni");
            Console.WriteLine("4. Logout");
            Console.Write("Scelta: ");
            string scelta = Console.ReadLine();

            switch (scelta)
            {
                case "1":
                    GestisciLuoghi(); //Gestione CRUD dei luoghi
                    break;
                case "2":
                    GestisciAttrazioni(); //Gestione CRUD delle attrazioni
                    break;
                case "3":
                    VisualizzaPrenotazioniAdmin(); //Visualizza tutte le prenotazioni
                    break;
                case "4":
                    utenteLoggatoID = null;
                    isAdmin = false;
                    return; //Logout
                default:
                    Console.WriteLine("Scelta non valida.");
                    break;
            }
        }
    }

    //================== LUOGHI ==================

    //Visualizza tutti i luoghi presenti nel database
    static void VisualizzaLuoghi()
    {
        Console.Clear();
        Console.WriteLine("=== LUOGHI ===");
        try
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT * FROM Luogo";
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader.GetInt32("luogoID")} | {reader.GetString("nome")} | {reader.GetString("paese")}, {reader.GetString("citta")}");
                        Console.WriteLine($"Descrizione: {reader["descrizione"]}");
                        Console.WriteLine("---------------------------");
                    }
                }
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Errore durante la visualizzazione dei luoghi: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Si è verificato un errore imprevisto durante la visualizzazione dei luoghi: " + ex.Message);
        }
        Console.WriteLine("Premi un tasto per continuare...");
    }

    //Menu di gestione dei luoghi (solo per admin)
    static void GestisciLuoghi()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== GESTIONE LUOGHI ===");
            Console.WriteLine("1. Visualizza Luoghi");
            Console.WriteLine("2. Aggiungi Luogo");
            Console.WriteLine("3. Rimuovi Luogo");
            Console.WriteLine("4. Torna indietro");
            Console.Write("Scelta: ");
            string scelta = Console.ReadLine();

            switch (scelta)
            {
                case "1":
                    VisualizzaLuoghi(); //Visualizza tutti i luoghi
                    break;
                case "2":
                    AggiungiLuogo(); //Permette di aggiungere un nuovo luogo
                    break;
                case "3":
                    RimuoviLuogo(); //Permette di rimuovere un luogo
                    break;
                case "4":
                    return; //Torna al menu admin
                default:
                    Console.WriteLine("Scelta non valida.");
                    break;
            }
        }
    }

    //Permette di aggiungere un nuovo luogo al database
    static void AggiungiLuogo()
    {
        Console.Clear();
        Console.WriteLine("=== AGGIUNGI LUOGO ===");
        Console.Write("Nome: ");
        string nome = Console.ReadLine();
        Console.Write("Descrizione: ");
        string descrizione = Console.ReadLine();
        Console.Write("Paese: ");
        string paese = Console.ReadLine();
        Console.Write("Città: ");
        string citta = Console.ReadLine();

        try
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "INSERT INTO Luogo (nome, descrizione, paese, citta) VALUES (@n, @d, @p, @c)";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@n", nome);
                    cmd.Parameters.AddWithValue("@d", descrizione);
                    cmd.Parameters.AddWithValue("@p", paese);
                    cmd.Parameters.AddWithValue("@c", citta);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Luogo aggiunto!");
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Errore durante l'aggiunta del luogo: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Si è verificato un errore imprevisto durante l'aggiunta del luogo: " + ex.Message);
        }
    }

    //Permette di rimuovere un luogo dal database tramite ID
    static void RimuoviLuogo()
    {
        VisualizzaLuoghi();
        Console.Write("ID del luogo da rimuovere: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            try
            {
                using (var conn = new MySqlConnection(connString))
                {
                    conn.Open();
                    string query = "DELETE FROM Luogo WHERE luogoID = @id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        int righe = cmd.ExecuteNonQuery();
                        if (righe > 0)
                            Console.WriteLine("Luogo rimosso.");
                        else
                            Console.WriteLine("Luogo non trovato.");
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Errore durante la rimozione del luogo: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Si è verificato un errore imprevisto durante la rimozione del luogo: " + ex.Message);
            }
        }
        else
        {
            Console.WriteLine("Input non valido. Inserisci un ID numerico.");
        }
    }

    //================== ATTRAZIONI ==================

    //Visualizza tutte le attrazioni presenti nel database
    static void VisualizzaAttrazioni()
    {
        Console.Clear();
        Console.WriteLine("=== ATTRAZIONI ===");
        try
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT a.attrazioneID, a.nome, a.descrizione, l.nome AS luogo
                             FROM Attrazione a LEFT JOIN Luogo l ON a.luogoREF = l.luogoID";
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader.GetInt32("attrazioneID")} | {reader.GetString("nome")} | Luogo: {reader["luogo"]}");
                        Console.WriteLine($"Descrizione: {reader["descrizione"]}");
                        Console.WriteLine("---------------------------");
                    }
                }
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Errore durante la visualizzazione delle attrazioni: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Si è verificato un errore imprevisto durante la visualizzazione delle attrazioni: " + ex.Message);
        }
        Console.WriteLine("Premi un tasto per continuare...");
    }

    //Menu di gestione delle attrazioni (solo per admin)
    static void GestisciAttrazioni()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== GESTIONE ATTRAZIONI ===");
            Console.WriteLine("1. Visualizza Attrazioni");
            Console.WriteLine("2. Aggiungi Attrazione");
            Console.WriteLine("3. Rimuovi Attrazione");
            Console.WriteLine("4. Torna indietro");
            Console.Write("Scelta: ");
            string scelta = Console.ReadLine();

            switch (scelta)
            {
                case "1":
                    VisualizzaAttrazioni(); //Visualizza tutte le attrazioni
                    break;
                case "2":
                    AggiungiAttrazione(); //Permette di aggiungere una nuova attrazione
                    break;
                case "3":
                    RimuoviAttrazione(); //Permette di rimuovere un'attrazione
                    break;
                case "4":
                    return; //Torna al menu admin
                default:
                    Console.WriteLine("Scelta non valida.");
                    break;
            }
        }
    }

    //Permette di aggiungere una nuova attrazione al database
    static void AggiungiAttrazione()
    {
        Console.Clear();
        Console.WriteLine("=== AGGIUNGI ATTRAZIONE ===");
        Console.Write("Nome: ");
        string nome = Console.ReadLine();
        Console.Write("Descrizione: ");
        string descrizione = Console.ReadLine();
        VisualizzaLuoghi(); //Mostra i luoghi per scegliere dove inserire l'attrazione
        Console.Write("ID Luogo: ");
        if (int.TryParse(Console.ReadLine(), out int luogoID))
        {
            try
            {
                using (var conn = new MySqlConnection(connString))
                {
                    conn.Open();
                    string query = "INSERT INTO Attrazione (nome, descrizione, luogoREF) VALUES (@n, @d, @l)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@n", nome);
                        cmd.Parameters.AddWithValue("@d", descrizione);
                        cmd.Parameters.AddWithValue("@l", luogoID);
                        cmd.ExecuteNonQuery();
                    }
                }
                Console.WriteLine("Attrazione aggiunta!");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Errore durante l'aggiunta dell'attrazione: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Si è verificato un errore imprevisto durante l'aggiunta dell'attrazione: " + ex.Message);
            }
        }
        else
        {
            Console.WriteLine("Input non valido. Inserisci un ID numerico per il luogo.");
        }
    }

    //Permette di rimuovere un'attrazione dal database tramite ID
    static void RimuoviAttrazione()
    {
        VisualizzaAttrazioni();
        Console.Write("ID dell'attrazione da rimuovere: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            try
            {
                using (var conn = new MySqlConnection(connString))
                {
                    conn.Open();
                    string query = "DELETE FROM Attrazione WHERE attrazioneID = @id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        int righe = cmd.ExecuteNonQuery();
                        if (righe > 0)
                            Console.WriteLine("Attrazione rimossa.");
                        else
                            Console.WriteLine("Attrazione non trovata.");
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Errore durante la rimozione dell'attrazione: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Si è verificato un errore imprevisto durante la rimozione dell'attrazione: " + ex.Message);
            }
        }
        else
        {
            Console.WriteLine("Input non valido. Inserisci un ID numerico.");
        }
    }

    //================== PRENOTAZIONI ==================

    //Permette all'utente di prenotare un'attrazione
    static void PrenotaAttrazione()
    {
        VisualizzaAttrazioni();
        Console.Write("ID dell'attrazione da prenotare: ");
        if (int.TryParse(Console.ReadLine(), out int attrazioneID))
        {
            try
            {
                using (var conn = new MySqlConnection(connString))
                {
                    conn.Open();
                    string query = "INSERT INTO Prenotazione (dataPrenotazione, utenteREF, attrazioneREF) VALUES (@d, @u, @a)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@d", DateTime.Now.Date);
                        cmd.Parameters.AddWithValue("@u", utenteLoggatoID);
                        cmd.Parameters.AddWithValue("@a", attrazioneID);
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Prenotazione effettuata!");
                    }
                }
            }
            catch (MySqlException ex)
            {
                //Gestione errori (es. prenotazione duplicata)
                Console.WriteLine("Errore durante la prenotazione: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Si è verificato un errore imprevisto durante la prenotazione: " + ex.Message);
            }
        }
        else
        {
            Console.WriteLine("Input non valido. Inserisci un ID numerico per l'attrazione.");
        }
    }

    //Visualizza le prenotazioni dell'utente loggato
    static void VisualizzaPrenotazioni()
    {
        Console.Clear();
        Console.WriteLine("=== LE TUE PRENOTAZIONI ===");
        try
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT p.prenotazioneID, p.dataPrenotazione, a.nome AS attrazione
                             FROM Prenotazione p
                             JOIN Attrazione a ON p.attrazioneREF = a.attrazioneID
                             WHERE p.utenteREF = @u";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@u", utenteLoggatoID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader.GetInt32("prenotazioneID")} | {reader.GetDateTime("dataPrenotazione"):d} | Attrazione: {reader["attrazione"]}");
                        }
                    }
                }
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Errore durante la visualizzazione delle prenotazioni: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Si è verificato un errore imprevisto durante la visualizzazione delle prenotazioni: " + ex.Message);
        }
        Console.WriteLine("Premi un tasto per continuare...");
    }

    //Visualizza tutte le prenotazioni (solo per admin)
    static void VisualizzaPrenotazioniAdmin()
    {
        Console.Clear();
        Console.WriteLine("=== TUTTE LE PRENOTAZIONI ===");
        try
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT p.prenotazioneID, p.dataPrenotazione, u.username, a.nome AS attrazione
                             FROM Prenotazione p
                             JOIN Utente u ON p.utenteREF = u.utenteID
                             JOIN Attrazione a ON p.attrazioneREF = a.attrazioneID";
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader.GetInt32("prenotazioneID")} | {reader.GetDateTime("dataPrenotazione"):d} | Utente: {reader["username"]} | Attrazione: {reader["attrazione"]}");
                    }
                }
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Errore durante la visualizzazione di tutte le prenotazioni: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Si è verificato un errore imprevisto durante la visualizzazione di tutte le prenotazioni: " + ex.Message);
        }
        Console.WriteLine("Premi un tasto per continuare...");
    }
    //Calcola l'hash SHA256 di una stringa (usato per la password)
    static string CalcolaSHA256(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha256.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
#endregion
}
