# AgenziaViaggi

Un'applicazione console in C# per la gestione di un'agenzia di viaggi, progettata per offrire funzionalità complete sia agli utenti finali che agli amministratori. Gli utenti possono esplorare luoghi e attrazioni, effettuare prenotazioni, mentre gli amministratori hanno il controllo totale sulla gestione di luoghi, attrazioni e la supervisione di tutte le prenotazioni.

## Indice
- [Funzionalità](#funzionalità)
  - [Menu Principale](#menu-principale)
  - [Menu Utente](#menu-utente)
  - [Menu Amministratore](#menu-amministratore)
- [Tecnologie Utilizzate](#tecnologie-utilizzate)
- [Setup e Esecuzione](#setup-e-esecuzione)
  - [Prerequisiti](#prerequisiti)
  - [Configurazione del Database](#configurazione-del-database)
  - [Compilazione ed Esecuzione](#compilazione-ed-esecuzione)
- [Contributi e Strumenti AI](#contributi-e-strumenti-ai)

## Funzionalità

### Menu Principale
- **Login**: Permette agli utenti esistenti di accedere all'applicazione.
- **Registrazione**: Consente ai nuovi utenti di creare un account.
- **Esci**: Termina l'applicazione.

### Menu Utente
- **Visualizza Luoghi**: Mostra tutti i luoghi disponibili.
- **Visualizza Attrazioni**: Mostra tutte le attrazioni disponibili, con indicazione del luogo a cui appartengono.
- **Prenota Attrazione**: Permette all'utente di prenotare un'attrazione specifica.
- **Visualizza Prenotazioni**: Mostra tutte le prenotazioni effettuate dall'utente loggato.
- **Logout**: Disconnette l'utente e ritorna al menu principale.

### Menu Amministratore
- **Gestisci Luoghi**: Permette di aggiungere, visualizzare e rimuovere luoghi.
- **Gestisci Attrazioni**: Permette di aggiungere, visualizzare e rimuovere attrazioni.
- **Visualizza Prenotazioni**: Mostra tutte le prenotazioni effettuate da tutti gli utenti.
- **Logout**: Disconnette l'amministratore e ritorna al menu principale.

## Tecnologie Utilizzate
- **C#**: Linguaggio di programmazione principale.
- **MySQL**: Database utilizzato per memorizzare i dati di utenti, luoghi, attrazioni e prenotazioni.
- **MySql.Data**: Libreria .NET per la connettività MySQL.
- **SHA256**: Algoritmo di hashing utilizzato per la sicurezza delle password.

## Setup e Esecuzione

### Prerequisiti
- .NET SDK (versione 9.0 o superiore).
- Un server MySQL in esecuzione.
- Uno strumento per la gestione di database MySQL (es. MySQL Workbench, DBeaver, phpMyAdmin).

### Configurazione del Database
1. **Creare il database**: Utilizzare lo script SQL fornito (es. `schema.sql` o `database_setup.sql`) per creare il database e le tabelle necessarie. Questo può essere fatto importando lo script tramite il proprio strumento di gestione database o eseguendo i comandi SQL manualmente.
   ```sql
   -- Esempio di struttura del database
   CREATE DATABASE nome_database;
   USE nome_database;

   CREATE TABLE Utente (
       id INT PRIMARY KEY AUTO_INCREMENT,
       username VARCHAR(255) UNIQUE NOT NULL,
       password_hash VARCHAR(255) NOT NULL,
       is_admin BOOLEAN DEFAULT FALSE
   );

   -- Altre tabelle come Luogo, Attrazione, Prenotazione...
   ```
2. **Aggiornare la stringa di connessione**: Nel file di configurazione dell'applicazione (es. `Program.cs`), assicurarsi che la variabile di connessione al database sia configurata correttamente per connettersi al proprio server MySQL.
   ```csharp
   static string connectionString = "Server=indirizzo_server;Database=nome_database;User=nome_utente;Password=tua_password;";
   ```
   Modificare i valori `indirizzo_server`, `nome_database`, `nome_utente` e `tua_password` con le credenziali del proprio ambiente MySQL.

### Compilazione ed Esecuzione
1. **Navigare nella directory del progetto**: Aprire un terminale e navigare fino alla directory principale del progetto.
2. **Eseguire l'applicazione**: Compilare ed eseguire il progetto utilizzando il comando:
   ```bash
   dotnet run
   ```

L'applicazione si avvierà in modalità console, presentando il menu principale.

## Contributi e Strumenti AI
Questo progetto ha beneficiato dell'assistenza di strumenti di intelligenza artificiale per ottimizzare lo sviluppo e migliorare la qualità del codice:

- **GitHub Copilot**: Utilizzato per la generazione assistita di titoli, l'adattamento di query e la velocizzazione della scrittura del codice, migliorando la produttività e la coerenza stilistica.
- **ChatGPT 4o**: Impiegato per la generazione delle istruzioni SQL per l'inserimento dei dati nel database, per la ricerca e l'adattamento dell'algoritmo di hashing SHA256, e per l'integrazione di quest'ultimo nei moduli di login e registrazione, garantendo una maggiore sicurezza delle password.
