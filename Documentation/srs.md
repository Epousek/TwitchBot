# Twitch chat bot

## Specifikace požadavků 

verze 0.1, 20.10.2021  
Jan Bezouška, janbezouska@outlook.com

### Konvence

chat = 

### Odkazy na ostatní dokumenty

### Popis programu

Účelem programu je zejména přenést funkce fungující v zahraničí na českou scénu, v češtině.  
S programem bude uživatel interagovat skrze psaní do chatu na kanále (na platformě twitch.tv), na který bude bot připojený; konkrétně pomocí příkazů a jejich argumentů. Program bude mít předem určený prefix pro přikazy ($), který bude muset být napsán před každým příkazem.

### Funkce

Program bude mít dvě skupiny funkcí pro uživatele: příkazy a události, přičemž důraz bude kladen na příkazy, kterých bude o dost více.  
**Příkazy** budou fungovat následovně: uživatel napíše do chatu příkaz -> program vyhodnotí, zda tento příkaz existuje, a zda ho uživatel může použít -> příkaz se provede -> program napíše uživateli do chatu informace (záleží na konkrétním příkazu).  
Pokud se příkaz nepovede provést, bude uživateli napsána přesná chyba podle bodu, ve kterém se příkaz zasekl.  
**Události** budou fungovat následovně: uživatel se "zaregistruje" příkazem k události -> uživatel je zapsán do databáze -> nastene událost -> program vezme všechny uživatele přihlášené k dané události z db -> program napíše do chatu, že nastala daná událost a označí dané uživatele.


### Uživatelské skupiny

Hlavní uživatelské skupiny budou dvě:
a) standartní uživatel
 Věšinou člověk, který je aktivní v dané komunitě a chat používá i když momentálně neprobíhá živé vysílání. Tento typ uživatele bude program využívat zejména pro získávání informací a zábavu.
b) admin
 Člověk, který bude mít přístup k administrátorským funkcím, např. banování uživatelů. Nejdříve jím bude pouze autor, později podle potřeby další lidé.

### Provozní prostředí

Program poběži na serveru, na kterém je i databáze, se kterou bude pracovat.
 - Operační Systém: Ubuntu Server
 - Databáze: MySQL
 - Platforma: .NET

### Závislosti

### Uživatelská rozhraní

Program nemá žádné vlastní rozhraní, se kterým by interagovali uživatelé. Ti budou s programem komunikovat skrze rozhraní chatu.

### Hardwarová rozhraní

Vzhledem k tomu, že program poběží pouze na serveru, tak nejsou žádné přidané hardwarové požadavky na uživatele. Jakmile má uživatel přístup k chatu, může s programem využívat.

### Softwarová rozhraní

Uživatel musí mít přístup k chatu. Nezáleží na tom skrze jaký operační systém a webový prohlížeč či program k tomu používá.

### Vlastnosti

1. 

### Výkonnost

Maximální přijatelná doba od přijetí příkazu do jeho vykonání (pokud se má vykonat hned) jsou 4 vteřiny.  
Délka odezvy bude v některých případech záviset na různých API, nebude tak možnost ji ovlivnit a může se stát, že se tím odezva prodlouží. Zároveň se také doba provedení příkazu bude dále lišit mezi jednotlivými příkazy tím, že některé budou např. pracovat s větší tabulkou v db, zatímco jiné s db pracovat nebudou vůbec. 4 vteřiny počítají i s těmito případy, a tak většina příkazů bude provedena o dost rychleji

### Bezpečnost

Program nebude ukládat žádná citlivá data uživatelů, a ani s nimi nebude pracovat.

### Spolehlivost

Spolehlivost, stejně jako výkonnost, bude v některých případech ovlivněna API, se kterými bude program pracovat.  
Mimo tyto případy bude cílem (téměř) non-stop běh programu, s nutností restartu maximálně 2. týdně.  
Program by měl zachytávat všechny chyby, a informovat o nich uživatele, pokud jsou pro něj relevantní.

### Uživatelská dokumentace

Seznam všech příkazů a událostí bude k dispozici na githubu. Součástí seznamu budou i veškeré argumenty, které jsou s danými příkazy použitelné.
