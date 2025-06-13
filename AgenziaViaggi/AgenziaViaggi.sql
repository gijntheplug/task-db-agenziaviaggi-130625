CREATE DATABASE AgenziaViaggi;
USE AgenziaViaggi;

CREATE TABLE Utente(
	utenteID INT PRIMARY KEY AUTO_INCREMENT,
    username VARCHAR(255) NOT NULL,
    password  VARCHAR(255) NOT NULL,
    isAdmin BOOLEAN DEFAULT FALSE
);

CREATE TABLE Luogo(
	luogoID INT PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(255) NOT NULL,
    descrizione TEXT,
    paese VARCHAR(255) NOT NULL,
    citta VARCHAR(255) NOT NULL
);

CREATE TABLE Attrazione(
	attrazioneID INT PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(255) NOT NULL,
    descrizione TEXT,
    luogoREF INT,
    FOREIGN KEY (luogoREF) REFERENCES Luogo (luogoID) ON DELETE CASCADE
);

CREATE TABLE Prenotazione(
	prenotazioneID INT PRIMARY KEY AUTO_INCREMENT,
    dataPrenotazione DATE,
    utenteREF INT,
    FOREIGN KEY (utenteREF) REFERENCES Utente (utenteID),
    attrazioneREF INT,
    FOREIGN KEY (attrazioneREF) REFERENCES Attrazione (attrazioneID)
);

USE AgenziaViaggi;

-- Utenti
INSERT INTO Utente (username, password, isAdmin)
VALUES 
('admin', '1234', TRUE),
('user', '1234', FALSE);

-- Luoghi
INSERT INTO Luogo (nome, descrizione, paese, citta)
VALUES 
('Parigi', 'La città delle luci e dell amore.', 'Francia', 'Parigi'),
('Tokyo', 'Una metropoli vibrante tra tradizione e tecnologia.', 'Giappone', 'Tokyo'),
('Roma', 'Culla della civiltà occidentale.', 'Italia', 'Roma');

-- Attrazioni
INSERT INTO Attrazione (nome, descrizione, luogoREF)
VALUES
('Torre Eiffel', 'Iconico monumento in ferro battuto situato a Parigi.', 1),
('Louvre', 'Uno dei più grandi musei di arte al mondo.', 1),
('Tempio Senso-ji', 'Il tempio buddhista più antico di Tokyo.', 2),
('Shibuya Crossing', 'L incrocio pedonale più affollato del mondo.', 2),
('Colosseo', 'Antico anfiteatro romano simbolo di Roma.', 3);

SELECT * FROM utente;