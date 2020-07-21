# Backendserver (Tobias Gilgenreiner)

## Vom Code zur Applikation

Um die Apllikation auf das eigene System anzupassen müssen folgende Schritte ausgeführt werden:

1. applicationUrl in launchSettings.json im Object DAPISmartHomeMQTT_BackendServer auf gewünschte IP-Adresse für den Backendserver anpassen.
2. DefaultConnection in appsettings.json im Object ConnectionStrings auf Zugang für eigene mariadb anpassen.
3. In Constances.cs MqttServerAddr, MqttBrokerPort und Mariadbaccess anpassen. Mariadbaccess wie bereits in appsettings.json anpassen.
4. DAPISmartHomeMQTT_BackendServer.sln kompilieren falls aVerwendung auf Linux system gewünscht noch veröffentlichen.

Im Ordner .\Backend\DAPISmartHomeMQTT_BackendServer\DAPISmartHomeMQTT_BackendServer\bin\Debug\netcoreapp3.1 befindet sich der Ordner publish. Dieser Ordner beinhaltet alle notwendigen Dateien die zum AUsführen notwendig sind.
Für Windows die DAPISmartHomeMQTT_BackendServer.exe und auf Linux kann mithilfe der dotnetcore runtime die DAPISmartHomeMQTT_BackendServer.dll ausgeführt werden.

Linux Befehl: dotnet DAPISmartHomeMQTT_BackendServer.dll --urls "http:\//\*:8080"\
wobei der --urls parameter auf den richtigen port auf dem der Server verfügbar sein soll angepasst werden muss.

Zu beachten ist, dass auf Linuxgeräten die dotnetcore runtime installiert werden muss.

## Struktur

Der Backendserver besitzt Context, Controller und Model für das Bereitstellen der Datenbank.\

Für die Kommunikation über MQTT mit Sensoren, Aktoren und der Siemenslogo besitzt der Backenserver eine ClientConnection Klasse, die für jeden Datenbankeintrag eine Verbindung bereitstellt.\

Die MQTTCLients kommunizieren über die Topic "shlogo/\[ClientID\]/" für das empfangen und "shlogo/\[ClientID\]/set" zum setzen der Aktoren.\

Um das Automatische hinzufügen von Sensoren bzw Aktoren zu ermöglichen besitzt der Server noch einen BackendDataClient.


# SLOGOClient (Tobias Gilgenreiner)

## Vom Code zur Applikation

Für dieses Programm ist eine auf das Netzwerk eingerichtete Siemenslogo mit korrekt configurierten Internet Inputs und Internet Outputs notwendig.\
(input byte 0, output byte 1 jeweils die ersten 4 bit)

Um die Apllikation auf das eigene System anzupassen müssen die Verbindungsdaten in der Klasse Constants angepasst werden.
Nach dem Kompilieren und Veröffentlichen kann wie beim BackendServer platformunabhängig mit dotnetcore das Programm ausgeführt werden.

## Struktur

Die App führt Zwei Tasks aus:

1. Das Empfangen von Daten mithilfe von MQTT und die Übermittlung der Daten an die Siemens Logo.
2. Das Übermitteln der Siemenslogo Ausgange an die Datenbank um Zeitgesteuerte Elemente der SIemenslogo nutzen zu können.

Leider ist die Verbindung mit der Siemens Logo zum aktuellen Zeitpunkt durch das notwendige Threading sehr instabiel. Deshalb ist ein regelmäßger Neustart der Software notwendig.


# Datenbank (Tobias Gilgenreiner)

Als Datenbank wird mariadb verwendet. Die Datenbank beinhaltet zwei Tabels:

1. MQTTClients mit den Spalten ClientID, Name, Topic, Room, Typeid, GroupID, CurrentValue
2. MQTTClientTypes mit den Spalten TypeID, Name, Direction, Mode, RangeMin, RangeMax

Um importieren der Datenbank und Standardwerte, kann BackendDB.zip genutzt werden.
