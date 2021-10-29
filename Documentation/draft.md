# Twitch chat bot

## Návrh

verze 0.4, 29.10.2021  
Jan Bezouška, janbezouska@outlook.com

### Konvence

chat = místnost, ve které komunikují sledující pomocí zpráv  
stream = živé vysílání (konkrétně na platformě Twitch.tv)  
u vlastností: *kurziva* = nepovinný argument

### Odkazy na ostatní dokumenty

### Popis programu

Účelem programu je zejména přenést funkce fungující v zahraničí na českou scénu, v češtině.  
S programem bude uživatel interagovat skrze psaní do chatu na kanále (na platformě twitch.tv), na který bude bot připojený; konkrétně pomocí příkazů a jejich argumentů. Program bude mít předem určený prefix pro přikazy ($), který bude muset být napsán před každým příkazem.

### Rizika

Program bude využívat knihovnu TwitchLib, pomocí které bude napojen na Twitch. Největším rizikem je, že nebude updatována, a v budoucnu se stane nekompatibilní s API Twitche nebo nebude obsahovat její nové funkce. V takovém případě bych musel komunikovat s Twitchem sám, což je možné, ale více zdlouhavé a náročné.  
Rizikem můžou být i další knihovny, které bude program používat - Newtonsoft, RestSharp a MySql.Data.  
Dalším rizikem je prostředí. Program poběží na serveru, který jsem vytvořil ze starého počítače. Problém může nastat v několika případech - komponenty v PC nejsou nejnovější, a mohly by přestat fungovat; něco je špatně nastavené, a zjistí se to až za běhu.  

### Funkce

Program bude mít dvě skupiny funkcí pro uživatele: příkazy a události, přičemž důraz bude kladen na příkazy, kterých bude o dost více.  
**Příkazy** budou fungovat následovně: uživatel napíše do chatu příkaz -> program vyhodnotí, zda tento příkaz existuje, a zda ho uživatel může použít -> příkaz se provede -> program napíše uživateli do chatu informace (záleží na konkrétním příkazu).  
Pokud se příkaz nepovede provést, bude uživateli napsána přesná chyba podle bodu, ve kterém se příkaz zasekl.  
**Události** budou fungovat následovně: uživatel se "zaregistruje" příkazem k události -> uživatel je zapsán do databáze -> nastene událost -> program získá všechny uživatele přihlášené k dané události z db -> program napíše do chatu, že nastala daná událost a označí dané uživatele.

### Vymezení rozsahu

Program bude obsahovat zejména informativní funkce.  
Některé podobné programy obsahují i funkce pro moderaci chatu, což nebude součástí tohoto programu.

### Uživatelské role

Budou existovat tři uživatelské role:
  - Vlastník - pouze autor programu; nejvyšší oprávnění, přístup ke všem funkcím  
  - Admin - malý počet lidí, kterým vlastník důvěřuje; možnost dávat a odebírat bany běžným uživatelům  
  - Běžný uživatel - drtivá většina uživatelů; přístup k uživatelským funkcím; většinou člověk, co je aktivní v chatu i pokud zrovna není stream

### Způsoby použití

Každý příkaz má trochu jiné využití, ale obecně vzato se dají rozdělit do následujících kategorií:
  - Informace o streamu - např. jaké emotikony byly naposledy přidány
  - Informace o konkrétním uživateli - např. jaký byl první kanál, který sledoval
  - Obecné informace - např. jaké je počasí v dané lokalitě
  - Mezi-uživatelské použití - např. připomínka pro jiného uživatele

### Provozní prostředí

Program poběži na serveru, na kterém je i databáze, se kterou bude pracovat.
 - Operační Systém: Ubuntu Server
 - Databáze: MySQL
 - Platforma: .NET

### Uživatelská rozhraní

Program nemá žádné vlastní rozhraní, se kterým by interagovali uživatelé. Ti budou s programem komunikovat skrze rozhraní chatu.

### Hardwarová rozhraní

Vzhledem k tomu, že program poběží pouze na serveru, tak nejsou žádné přidané hardwarové požadavky na uživatele. Jakmile má uživatel přístup k chatu, může s programem využívat.

### Softwarová rozhraní

Uživatel musí mít přístup k chatu. Nezáleží na tom skrze jaký operační systém a webový prohlížeč či program k tomu používá.

### Výkonnost

Maximální přijatelná doba od přijetí příkazu do jeho vykonání (pokud se má vykonat hned) jsou 4 vteřiny.  
Délka odezvy bude v některých případech záviset na různých API, nebude tak možnost ji ovlivnit a může se stát, že se tím odezva prodlouží. Zároveň se také doba provedení příkazu bude dále lišit mezi jednotlivými příkazy tím, že některé budou např. pracovat s větší tabulkou v db, zatímco jiné s db pracovat nebudou vůbec. 4 vteřiny počítají i s těmito případy, a tak většina příkazů bude provedena o dost rychleji

### Bezpečnost

Program nebude ukládat žádná citlivá data uživatelů, a ani s nimi nebude pracovat.

### Spolehlivost

Spolehlivost, stejně jako výkonnost, bude v některých případech ovlivněna API, se kterými bude program pracovat.  
Mimo tyto případy bude cílem (téměř) non-stop běh programu, s nutností restartu způsobeného chybou maximálně 2. týdně.  
Program by měl zachytávat všechny chyby, a informovat o nich uživatele, pokud jsou pro něj relevantní.

### Chybová hlášení

Chybová hlášení budou konkrétní, a budou uživatele přesně informovat o tom, co udělal špatně.  
Budou existovat tři hlavní skupiny chyb:
  - Špatný argument 
    - pokud uživatel jakožto argument příkazu pošle špatný formát  
    - uživatel bude informován o tom, jak má argument správně vypadat
  - Chybějící argument
    - pokud uživatel nezadá povinný argument
    - uživatel bude informován, jaký argument je u daného příkazu povinný
  - Uživatel nenalezen
    - u příkazů, které vyhledávají uživatele v databázi
    - pokud uživatel není v databázi nalezen
    - uživatel bude informován, že uživatel buď neexistuje, nebo není zapsán v databázi

### Uživatelská dokumentace

Seznam všech příkazů a událostí bude k dispozici na githubu. Součástí seznamu budou i veškeré argumenty, které jsou s danými příkazy použitelné.

### Vlastnosti - události

#### Změna hry
 - *Spouštěč:* změna kategorie hry
 - *Podmínky:* uživatel(é) se musí nejprve přihlásit k této události
 - *Úspěšný scénář:*
   1. Změní se hra
   2. Program získá z databáze seznam přihlášených uživatelů
   3. Přihlášení uživatelé budou označeni v chatu s tím, že se změnila hra, a co za hru se nyní hraje
 - *Alternativní scénář:* pokud se hra již změnila během posledních 10 minut, uživatelé upozorněni nebudou

#### Zapnutí streamu
 - *Spouštěč:* zapnutí streamu
 - *Podmínky:* uživatel(é) se musí nejprve přihlásit k této události
 - *Úspěšný scénář:*
   1. Streamer zapne stream
   2. Program získá z databáze přihlášené uživatele
   3. Přihlášení uživatelé budou označeni v chatu s tím, že začal stream
  
### Vlastnosti - příkazy
  
#### $added

 - *Alias*: emotes, emotikony
 - *Spouštěč:* uživatel napíše do chatu $added
 - *Podmínky:* žádné
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Program získá z databáze list emotikonů pro daný kanál
   3. Program list seřadí podle data přidání, a vybere prvních 8
   4. Program vypíše uživateli vybrané emoty

#### $removed
 - *Alias:* 
 - *Spouštěč:* uživatel napíše do chatu $removed
 - *Podmínky:* žádné
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Program získá z databáze list emotikonů pro daný kanál
   3. Program list seřadí podle data odebrání, a vybere prvních 6
   4. Program vypíše uživateli vybrané emotikony

#### $remind
 - *Alias:* 
 - *Argumenty:*
   - uživatel
   - *za jak dlouho upozornit* 
     - pro specifikaci času potřeba napsat "in" - např. $remind uživatel in 30m
     - pokud neupřesněno, uživatel bude upozorněn až napíše do chatu
   - *zpráva*
 - *Spouštěč:* uživatel napíše např. $remind user in 2h text upozornění
 - *Podmínky:* cílový uživatel nevypnul možnost ho upozornit
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Program si upozornění uloží
   3. Cílový uživatel napíše do chatu
   4. Program vypíše cílovému uživateli upozornění (od koho je, zprávu, a před jak ldouhou dobou bylo vytvořeno)
 - *Alternativní scénář:*
   1. Uživatel napíše příkaz s časovou specifikací
   2. Za danou dobu bude cílový uživatel upozorněn v chatu, nehledě na to, jestli je aktivní

#### $gn
 - *Alias:*
 - *Argumenty:*
   - *zpráva*
 - *Spouštěč:* uživatel napíše $gn
 - *Podmínky:*
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Program napíše do chatu, že jde uživatel spát (se zprávou, pokud jí uživatel zadal)
   3. Program si uloží uživatele do listu
   4. Uživatel napíše do chatu
   5. Program napíše, že už uživatel nespí (se zprávou a časem)

#### $afk
 - *Alias:*
 - *Argumenty:*
   - *zpráva*
 - *Spouštěč:* uživatel napíše $afk
 - *Podmínky:*
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Program napíše do chatu, že uživatel nebude přítomen (se zprávou, pokud jí uživatel zadal)
   3. Program si uloží uživatele do listu
   4. Uživatel napíše do chatu
   5. Program napíše, že je uživatel opět aktivní (se zprávou a časem)

#### $translate
 - *Alias:* přelož
 - *Argumenty:*
   - text
     - musí být v uvozovkách
   - *jazyk*
     - pokud není uveden, přeloží se buď z češtiny do angličtiny, nebo naopak (podle jazyku textu)
 - *Spouštěč:* uživatel napíše např. "Hello, world" cz
 - *Podmínky:*
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Program zformátuje argumenty a pošle žádost DeepL API
   3. Program získá z odpovědi přeložený text
   4. Program uživateli vypíše přeložený text

#### $firstfollowed
 - *Alias:*
 - *Argumenty:*
   - *uživatel*
     - pokud není specifikován, použit bude uživatel, která příkaz napsal
 - *Spouštěč:* uživatel napíše např. $firstfollowed user
 - *Podmínky:* 
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Program pošle žádost na DecAPI s uživatelským jménem
   3. Program vypíše první kanál, který uživatel sledoval

#### $news
 - *Alias:* novinky
 - *Argumenty:*
   - *kategorie*
 - *Spouštěč:* uživatel napíše např. $news sport
 - *Podmínky:*
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Program pošle žádost na NewsAPI (podle argumentů)
   3. Program zformátuje odpověď a vybere konkrétní článěk (náhodně)
   4. Program vypíše uživateli perex vybraného článku

#### $multitwitch
 - *Alias:*
 - *Argumenty:*
   - kanály (alespoň dva)
 - *Spouštěč:* uživatel napíše např. $multitwitch kanál1, kanál2
 - *Podmínky:*
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Program vytvoří link na stránku, na které lze sledovat více streamů najednou
   3. Program uživateli pošle odkaz na danou stránku s danými kanály

#### $randomline
 - *Alias:* rl
 - *Argumenty:*
   - *uživatel*
     - pokud není specifikován, použit bude uživatel, která příkaz napsal
 - *Spouštěč:* uživatel napíše např. $randomline user
 - *Podmínky:* cílový uživatel nevypnul možnost na něj použít tento příkaz
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Program získá z databáze náhodnou zprávu od cílového uživatele
   3. Program danou zprávu vypíše

#### $weather
 - *Alias:* počasí
 - *Argumenty:*
   - lokace (město)
 - *Spouštěč:* uživatel napíše např. $weather Praha
 - *Podmínky:*
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Program pošle žádost na OpenWeatherMap API
   3. Program zformátuje získaná data
   4. Program vypíše uživateli počasí v dané lokaci

#### $lasttimeout
 - *Alias:*
 - *Argumenty:*
   - uživatel (musí být moderátor)
 - *Spouštěč:* uživatel napíše $lasttimeout moderátor
 - *Podmínky:*
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Program najde v databázi, kdy dal daný uživatel naposledy timeout
   3. Program vypíše uživateli před jakou dobou dal naposledy daný uživatel timeout

#### $suggest
 - *Alias:* návrh
 - *Argumenty:*
   - text
 - *Spouštěč:* uživatel napíše $suggest návrh
 - *Podmínky:*
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Program uloží daný návrh do databáze
   3. Program informuje uživatele o tom, že byl návrh úspěšně uložen

#### $about
 - *Alias:* info
 - *Argumenty:*
   - *příkaz*
 - *Spouštěč:* uživatel napíše např. $about suggest
 - *Podmínky:*
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz bez argumentů
   2. Program uživateli vypíše základní informace o programu
 - *Alternativní scénář:*
   1. Uživatel napíše příkaz s argumentem
   2. Program vypíše informace o daném příkazu

#### $help
 - *Alias:*
 - *Argumenty:*
   - příkaz
 - *Spouštěč:* uživatel napíše např. $help suggest
 - *Podmínky:*
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Program uživateli vypíše, jak použít daný příkaz

#### $optout
 - *Alias:*
 - *Argumenty:*
   - příkaz (např. remind) nebo all
 - *Spouštěč:* uživatel napíše např. $optout all
 - *Podmínky:* uživatel není daného příkazu odhlášený
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Program uloží informaci o tom, že na tohoto uživatele nelze použít daný příkaz
   3. Program uživatele informuje o úspěchu

#### $optin
 - *Alias:*
 - *Argumenty:*
   - příkaz (např. remind) nebo all
 - *Spouštěč:* uživatel napíše např. $optin all
 - *Podmínky:* uživatel je od daného příkazu odhlášený
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Program uloží informaci o tom, že na tohoto uživatele opět lze použít daný příkaz
   3. Program uživatele informuje o úspěchu

#### $notify
 - *Alias:*
 - *Argumenty:*
   - událost (game nebo live) nebo all
 - *Spouštěč:* uživatel napíše např. $notify all
 - *Podmínky:* uživatel není přihlášen k dané události
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Program si uloží informaci o tom, že se tento uživatel přihlásil k dané události
   3. Program informuje uživatele o úspěchu

#### $dontnotify
 - *Alias:*
 - *Argumenty:*
   - událost (game nebo live) nebo all
 - *Spouštěč:* uživatel napíše např. $dontnotify all
 - *Podmínky:* uživatel je přihlášen k dané události
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Program si uloží informaci o tom, že se tento uživatel odhlásil od dané události
   3. Program informuje uživatele o úspěchu

#### $ban
 - *Alias:*
 - *Argumenty:*
   - uživatel
 - *Spouštěč:* admin napíše $ban user
 - *Podmínky:* použitelné pouze adminem nebo vlastníkem
 - *Úspěšný scénář:*
   1. Admin napíše příkaz
   2. Program si uloží informaci o tom, že daný uživatel je zabanován
   3. Program informuje admina o úspěchu

#### $unban
 - *Alias:*
 - *Argumenty:*
   - uživatel
 - *Spouštěč:* admin napíše $unban user
 - *Podmínky:* použitelné pouze adminem nebo vlastníkem
 - *Úspěšný scénář:*
   1. Admin napíše příkaz
   2. Program si uloží informaci o tom, že daný uživatel již není zabanován
   3. Program informuje admina o úspěchu

#### $admin
 - *Alias:*
 - *Argumenty:*
   - uživatel
 - *Spouštěč:* vlastník napíše $admin user
 - *Podmínky:* použitelné pouze vlastníkem
 - *Úspěšný scénář:*
   1. Vlastník napíše příkaz
   2. Program si uloží informaci o tom, že daný uživatel je nyní adminem
   3. Program informuje vlastníka o úspěchu

#### $unadmin
 - *Alias:*
 - *Argumenty:*
   - uživatel
 - *Spouštěč:* vlastník napíše $unadmin user
 - *Podmínky:* použitelné pouze vlastníkem
 - *Úspěšný scénář:*
   1. Vlastník napíše příkaz
   2. Program si uloží informaci o tom, že daný uživatel již není adminem
   3. Program informuje vlastníka o úspěchu

### Vlastnosti - na pozadí

#### Aktualizace emotikonů
Program bude automaticky každou minutu získávat aktivní emotikony na všech kanálech, na které bude připojený, a případně aktualizovat informace o nich v databázi.

#### Určení vhodného/použitelného emotikonu
Pokud bude program psát do chatu zprávu obsahující emotikon, tato funkce vybere nejvhodnější z listu, podle toho, který je na daném kanále aktivní.
