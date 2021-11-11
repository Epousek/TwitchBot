# Twitch chat bot

## Návrh

verze 0.5, 29.10.2021  
Jan Bezouška, janbezouska@outlook.com

### Konvence
 
 - twitch = internetová platforma určená k živému vysílání (streamování), zejména videoher a povídání  
 - stream = živé vysílání (v tomto kontextu konkrétně na platformě Twitch.tv)  
 - streamer = uživatel, který streamuje
 - kanál = účet na platformě twitch.tv; součástí každého kanálu je jeho vlastní chat
 - chat = uživatelé sledující živá vysílání (streamy) mají možnost psát zprávy do chatu a komunikovat tak se streamujícím uživatelem (streamerem) a ostatními diváky  
 - offline chat = chat kanálu, která momentálně nestreamuje
 - bot = program, který bude na chat připojen, a bude reagovat na různé přikazy
 - emote/emotikona = obrázek nebo gif, který lze použít v chatu, zejména k vyjádření určité emoce nebo pocitu; jsou důležitou součástí komunikace na twitchi; každý kanál má emotikony dle vlastního výběru, takže se kanál od kanálu liší  
 
u vlastností: *kurziva* = nepovinný argument

### Odkazy na ostatní dokumenty

### Popis programu

Účelem bota je zvýšit aktivitu v offline chatu. Toho bude dosahovat poskytováním různých zajímavých informací pomocí příkazů. Uživatelé mohou příkazy využívat z několika důvodů, např.: potřeba získat konkrétní informace, které bot nabízí; nudí se, a chce "zabít" čas; chce být aktivní v offline chatu.  
Bot je inspirován zahraničními boty s podobnými funkcemi. Jednou z motivací pro vznik tohoto bota je přenést tyto funkce do českého prostředí.  
S botem bude uživatel interagovat skrze psaní do chatu na kanále, na který bude bot připojený; konkrétně pomocí příkazů a jejich argumentů. Bot bude mít předem určený prefix (předponu) pro přikazy ($), který bude muset být napsán před každým příkazem.

### Rizika

Program bude využívat knihovnu TwitchLib, pomocí které bude napojen na Twitch. Největším rizikem je, že nebude updatována, a v budoucnu se stane nekompatibilní s API Twitche nebo nebude obsahovat její nové funkce. V takovém případě bych musel komunikovat s Twitchem sám, což je možné, ale více zdlouhavé a náročné.  
Rizikem můžou být i další knihovny, které bude program používat - Newtonsoft, RestSharp a MySql.Data.  
Dalším rizikem je prostředí. Program poběží na serveru, který jsem vytvořil ze starého počítače. Problém může nastat v několika případech - komponenty v PC nejsou nejnovější, a mohly by přestat fungovat; něco je špatně nastavené, a zjistí se to až za běhu.  

### Funkce

Bot bude mít dvě skupiny funkcí pro uživatele: příkazy a události, přičemž důraz bude kladen na příkazy, kterých bude o dost více.  
**Příkazy** budou fungovat následovně: uživatel napíše do chatu příkaz -> bot vyhodnotí, zda tento příkaz existuje, a zda ho uživatel může použít -> příkaz se provede -> bot napíše uživateli do chatu informace (záleží na konkrétním příkazu).  
Pokud se příkaz nepovede provést, bude uživateli napsána přesná chyba podle bodu, ve kterém se příkaz zasekl.  
**Události** budou fungovat následovně: uživatel se "zaregistruje" příkazem k události -> uživatel je zapsán do databáze -> nastene událost -> bot získá všechny uživatele přihlášené k dané události z db -> bot napíše do chatu, že nastala daná událost a označí dané uživatele.

### Vymezení rozsahu

Bot bude obsahovat zejména informativní funkce.  
Někteří podobní boté obsahují i funkce pro moderaci chatu, což nebude součástí tohoto bota.

### Uživatelské role

Budou existovat tři uživatelské role pro bota (nemají nic společného s rolemi na twitch):
  - Vlastník - pouze autor bota; nejvyšší oprávnění, přístup ke všem funkcím  
  - Admin - malý počet lidí, kterým vlastník důvěřuje; možnost dávat a odebírat bany běžným uživatelům  
  - Běžný uživatel - drtivá většina uživatelů; přístup k uživatelským funkcím; většinou člověk, co je aktivní v chatu i pokud zrovna není stream

### Způsoby použití

Každý příkaz má trochu jiné využití, ale obecně vzato se dají rozdělit do následujících kategorií:
  - Informace o streamu - např. jaké emotikony byly naposledy přidány
  - Informace o konkrétním uživateli - např. jaký byl první kanál, který sledoval
  - Obecné informace - např. jaké je počasí v dané lokalitě
  - Mezi-uživatelské použití - např. připomínka pro jiného uživatele

### Provozní prostředí

Bot poběži na serveru, na kterém je i databáze, se kterou bude pracovat.
 - Operační Systém: Ubuntu Server 20.04
 - Databáze: MySQL
 - Platforma: .NET

### Uživatelská rozhraní

Bot nemá žádné vlastní rozhraní, se kterým by interagovali uživatelé. Ti budou s botem komunikovat skrze rozhraní chatu.  
Na rozhraní chatu nemá bot žádný vliv, a rozhraní nemá žádný vliv na bota. Tímto rozhraním bude ve většině případů buď to, [které poskytuje twitch na každém kanále](twitch_chat.png), nebo nějaký [program vytvořený čistě pro používání chatu.](third_party_program.png).

### Hardwarová rozhraní

Vzhledem k tomu, že bot poběží pouze na serveru, tak nejsou žádné přidané hardwarové požadavky na uživatele. Jakmile má uživatel přístup k chatu, může bota využívat.

### Softwarová rozhraní

Uživatel musí mít přístup k chatu. Nezáleží na tom skrze jaký operační systém a webový prohlížeč či program k tomu používá.

### Výkonnost

Maximální přijatelná doba od přijetí příkazu do jeho vykonání (pokud se má vykonat hned) jsou 4 vteřiny.  
Délka odezvy bude v některých případech záviset na různých API, nebude tak možnost ji ovlivnit a může se stát, že se tím odezva prodlouží. Zároveň se také doba provedení příkazu bude dále lišit mezi jednotlivými příkazy tím, že některé budou např. pracovat s větší tabulkou v db, zatímco jiné s db pracovat nebudou vůbec. 4 vteřiny počítají i s těmito případy, a tak většina příkazů bude provedena o dost rychleji

### Bezpečnost

Bot nebude ukládat žádná citlivá data uživatelů, a ani s nimi nebude pracovat.

### Spolehlivost

Spolehlivost, stejně jako výkonnost, bude v některých případech ovlivněna API, se kterými bude bot pracovat.  
Mimo tyto případy bude cílem (téměř) non-stop běh bota, s nutností restartu způsobeného chybou maximálně 2. týdně.  
Bot by měl zachytávat všechny chyby, a informovat o nich uživatele, pokud jsou pro něj relevantní.

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
 - *Motivace*: uživatel příhlášený k této události chce vědět, kdy streamer začne hrát jinou hru (např. proto, že ho právě hraná hra nebaví, ale ta, co bude po ní ho baví)
 - *Spouštěč:* změna kategorie hry
 - *Podmínky:* uživatel(é) se musí nejprve přihlásit k této události
 - *Úspěšný scénář:*
   1. Změní se hra
   2. Bot získá z databáze seznam přihlášených uživatelů
   3. Přihlášení uživatelé budou označeni v chatu s tím, že se změnila hra, a co za hru se nyní hraje
 - *Alternativní scénář:* pokud se hra již změnila během posledních 10 minut, uživatelé upozorněni nebudou

#### Zapnutí streamu
 - *Motivace*: uživatel přihlášený k této události chce být upozorněn jakmile začne stream (aby nezmeškal začátek)
 - *Spouštěč:* zapnutí streamu
 - *Podmínky:* uživatel(é) se musí nejprve přihlásit k této události
 - *Úspěšný scénář:*
   1. Streamer zapne stream
   2. Bot získá z databáze přihlášené uživatele
   3. Přihlášení uživatelé budou označeni v chatu s tím, že začal stream
  
### Vlastnosti - příkazy
  
#### $added

 - *Alias*: emotes, emotikony
 - *Motivace*: uživatel chce vědět, zda se v podlední době přidaly nějaké emotikony
 - *Spouštěč:* uživatel napíše do chatu $added
 - *Podmínky:* žádné
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Bot získá z databáze list emotikonů pro daný kanál
   3. Bot list seřadí podle data přidání, a vybere prvních 8
   4. Bot vypíše uživateli vybrané emoty

#### $removed
 - *Alias:* 
 - *Motivace*: uživatel chce vědět, zda se v podlední době odstranily nějaké emotikony
 - *Spouštěč:* uživatel napíše do chatu $removed
 - *Podmínky:* žádné
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Bot získá z databáze list emotikonů pro daný kanál
   3. Bot list seřadí podle data odebrání, a vybere prvních 6
   4. Bot vypíše uživateli vybrané emotikony

#### $remind
 - *Alias:* 
 - *Motivace*: uživatel chce na něco upozornit jiného uživatele (nebo sebe) za nějakou dobu, nebo až příště napíší do chatu
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
   2. Bot si upozornění uloží
   3. Cílový uživatel napíše do chatu
   4. Bot vypíše cílovému uživateli upozornění (od koho je, zprávu, a před jak ldouhou dobou bylo vytvořeno)
 - *Alternativní scénář:*
   1. Uživatel napíše příkaz s časovou specifikací
   2. Za danou dobu bude cílový uživatel upozorněn v chatu, nehledě na to, jestli je aktivní

#### $gn
 - *Alias:*
 - *Motivace*: uživatel chce informovat ostatní o tom, že jde spát (a až příště napíše do chatu o tom, že už nespí)
 - *Argumenty:*
   - *zpráva*
 - *Spouštěč:* uživatel napíše $gn
 - *Podmínky:*
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Bot napíše do chatu, že jde uživatel spát (se zprávou, pokud jí uživatel zadal)
   3. Bot si uloží uživatele do listu
   4. Uživatel napíše do chatu
   5. Bot napíše, že už uživatel nespí (se zprávou a časem)

#### $afk
 - *Alias:*
 - *Motivace*: uživatel chce informovat ostatní o tom, že přestává být aktivní (a až příště napíše do chatu o tom, že už opět aktivní je)
 - *Argumenty:*
   - *zpráva*
 - *Spouštěč:* uživatel napíše $afk
 - *Podmínky:*
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Bot napíše do chatu, že uživatel nebude přítomen (se zprávou, pokud jí uživatel zadal)
   3. Bot si uloží uživatele do listu
   4. Uživatel napíše do chatu
   5. Bot napíše, že je uživatel opět aktivní (se zprávou a časem)

#### $translate
 - *Alias:* přelož
 - *Motivace*: uživatel chce přeložit nějaký krátký text
 - *Argumenty:*
   - text
     - musí být v uvozovkách
   - *jazyk*
     - pokud není uveden, přeloží se buď z češtiny do angličtiny, nebo naopak (podle jazyku textu)
 - *Spouštěč:* uživatel napíše např. "Hello, world" cz
 - *Podmínky:*
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Bot zformátuje argumenty a pošle žádost DeepL API
   3. Bot získá z odpovědi přeložený text
   4. Bot uživateli vypíše přeložený text

#### $firstfollowed
 - *Alias:*
 - *Motivace*: uživatel chce zjistit, jaký kanál daný uživatel sledoval jako první
 - *Argumenty:*
   - *uživatel*
     - pokud není specifikován, použit bude uživatel, která příkaz napsal
 - *Spouštěč:* uživatel napíše např. $firstfollowed user
 - *Podmínky:* 
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Bot pošle žádost na DecAPI s uživatelským jménem
   3. Bot vypíše první kanál, který uživatel sledoval

#### $news
 - *Alias:* novinky
 - *Motivace*: uživatele si chce přečíst náhodnou novinku
 - *Argumenty:*
   - *kategorie*
 - *Spouštěč:* uživatel napíše např. $news sport
 - *Podmínky:*
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Bot pošle žádost na NewsAPI (podle argumentů)
   3. Bot zformátuje odpověď a vybere konkrétní článěk (náhodně)
   4. Bot vypíše uživateli perex vybraného článku

#### $multitwitch
 - *Alias:*
 - *Motivace*: uživatel chce sledovat více streamerů najednou (pravděpodobně proto, že streamují spolu, a on chce vidět různé pohledy)
 - *Argumenty:*
   - kanály (alespoň dva)
 - *Spouštěč:* uživatel napíše např. $multitwitch kanál1, kanál2
 - *Podmínky:*
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Bot vytvoří link na stránku, na které lze sledovat více streamů najednou
   3. Bot uživateli pošle odkaz na danou stránku s danými kanály

#### $randomline
 - *Alias:* rl
 - *Motivace*: uživatel se nudí
 - *Argumenty:*
   - *uživatel*
     - pokud není specifikován, použit bude uživatel, která příkaz napsal
 - *Spouštěč:* uživatel napíše např. $randomline user
 - *Podmínky:* cílový uživatel nevypnul možnost na něj použít tento příkaz
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Bot získá z databáze náhodnou zprávu od cílového uživatele
   3. Bot danou zprávu vypíše

#### $weather
 - *Alias:* počasí
 - *Motivace*: uživatel chce zjistit/informovat jaké je počasí v dané lokaci
 - *Argumenty:*
   - lokace (město)
 - *Spouštěč:* uživatel napíše např. $weather Praha
 - *Podmínky:*
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Bot pošle žádost na OpenWeatherMap API
   3. Bot zformátuje získaná data
   4. Bot vypíše uživateli počasí v dané lokaci

#### $lasttimeout
 - *Alias:*
 - *Motivace*: uživatel chce zjistit kdy dal daný moderátor naposledy timeout
 - *Argumenty:*
   - uživatel (musí být moderátor)
 - *Spouštěč:* uživatel napíše $lasttimeout moderátor
 - *Podmínky:*
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Bot najde v databázi, kdy dal daný uživatel naposledy timeout
   3. Bot vypíše uživateli před jakou dobou dal naposledy daný uživatel timeout

#### $suggest
 - *Alias:* návrh
 - *Motivace*: uživatel chce navrhnout novou funkci (např. příkaz), nebo upozornit na nějakou chybu
 - *Argumenty:*
   - text
 - *Spouštěč:* uživatel napíše $suggest návrh
 - *Podmínky:*
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Bot uloží daný návrh do databáze
   3. Bot informuje uživatele o tom, že byl návrh úspěšně uložen

#### $about
 - *Alias:* info
 - *Motivace*: uživatel chce zjistit informace o botovi
 - *Argumenty:*
   - *příkaz*
 - *Spouštěč:* uživatel napíše např. $about suggest
 - *Podmínky:*
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz bez argumentů
   2. Bot uživateli vypíše základní informace o botovi
 - *Alternativní scénář:*
   1. Uživatel napíše příkaz s argumentem
   2. Bot vypíše informace o daném příkazu

#### $help
 - *Alias:*
 - *Motivace*: uživatel chce zjistit, jak použít daný příkaz
 - *Argumenty:*
   - příkaz
 - *Spouštěč:* uživatel napíše např. $help suggest
 - *Podmínky:*
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Bot uživateli vypíše, jak použít daný příkaz

#### $optout
 - *Alias:*
 - *Motivace*: uživatel nechce být cílem daného příkazu
 - *Argumenty:*
   - příkaz (např. remind) nebo all
 - *Spouštěč:* uživatel napíše např. $optout all
 - *Podmínky:* uživatel není daného příkazu odhlášený
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Bot uloží informaci o tom, že na tohoto uživatele nelze použít daný příkaz
   3. Bot uživatele informuje o úspěchu

#### $optin
 - *Alias:*
 - *Motivace*: uživatel chce opět být potencionálním cílem daného příkazu
 - *Argumenty:*
   - příkaz (např. remind) nebo all
 - *Spouštěč:* uživatel napíše např. $optin all
 - *Podmínky:* uživatel je od daného příkazu odhlášený
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Bot uloží informaci o tom, že na tohoto uživatele opět lze použít daný příkaz
   3. Bot uživatele informuje o úspěchu

#### $notify
 - *Alias:*
 - *Motivace*: uživatel chce být upozorněn, když se stane daná událost
 - *Argumenty:*
   - událost (game nebo live) nebo all
 - *Spouštěč:* uživatel napíše např. $notify all
 - *Podmínky:* uživatel není přihlášen k dané události
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Bot si uloží informaci o tom, že se tento uživatel přihlásil k dané události
   3. Bot informuje uživatele o úspěchu

#### $dontnotify
 - *Alias:*
 - *Motivace*: uživatel již nechce být upozorněn, když se stane daná událost
 - *Argumenty:*
   - událost (game nebo live) nebo all
 - *Spouštěč:* uživatel napíše např. $dontnotify all
 - *Podmínky:* uživatel je přihlášen k dané události
 - *Úspěšný scénář:*
   1. Uživatel napíše příkaz
   2. Bot si uloží informaci o tom, že se tento uživatel odhlásil od dané události
   3. Bot informuje uživatele o úspěchu

#### $ban
 - *Alias:*
 - *Motivace*: admin chce zakázat danému uživateli používat bota
 - *Argumenty:*
   - uživatel
 - *Spouštěč:* admin napíše $ban user
 - *Podmínky:* použitelné pouze adminem nebo vlastníkem
 - *Úspěšný scénář:*
   1. Admin napíše příkaz
   2. Bot si uloží informaci o tom, že daný uživatel je zabanován
   3. Bot informuje admina o úspěchu

#### $unban
 - *Alias:*
 - *Motivace*: admin chce povolit danému uživateli používat bota
 - *Argumenty:*
   - uživatel
 - *Spouštěč:* admin napíše $unban user
 - *Podmínky:* použitelné pouze adminem nebo vlastníkem
 - *Úspěšný scénář:*
   1. Admin napíše příkaz
   2. Bot si uloží informaci o tom, že daný uživatel již není zabanován
   3. Bot informuje admina o úspěchu

#### $admin
 - *Alias:*
 - *Motivace*: vlastník chce dát danému uživateli oprávnění admina
 - *Argumenty:*
   - uživatel
 - *Spouštěč:* vlastník napíše $admin user
 - *Podmínky:* použitelné pouze vlastníkem
 - *Úspěšný scénář:*
   1. Vlastník napíše příkaz
   2. Bot si uloží informaci o tom, že daný uživatel je nyní adminem
   3. Bot informuje vlastníka o úspěchu

#### $unadmin
 - *Alias:*
 - *Motivace*: vlastník chce odebrat danému uživateli oprávnění admina
 - *Argumenty:*
   - uživatel
 - *Spouštěč:* vlastník napíše $unadmin user
 - *Podmínky:* použitelné pouze vlastníkem
 - *Úspěšný scénář:*
   1. Vlastník napíše příkaz
   2. Bot si uloží informaci o tom, že daný uživatel již není adminem
   3. Bot informuje vlastníka o úspěchu

### Vlastnosti - na pozadí

#### Aktualizace emotikonů
Bot bude automaticky každých 5 minut získávat aktivní emotikony na všech kanálech, na které bude připojený, a případně aktualizovat informace o nich v databázi.

#### Určení vhodného/použitelného emotikonu
Pokud bude bot psát do chatu zprávu obsahující emotikon, tato funkce vybere nejvhodnější z listu, podle toho, který je na daném kanále aktivní.

#### Převod uživatelského jména na uživatelské ID a naopak

### Databáze

 - ConnectedChannels
   - jméno kanálu
   - nastavení pro kanál
   - kdy se bot na kanál připojil
 - Chatters
   - uživatelské jméno
   - uživatelské ID
   - oprávnění
   - příkazy, které uživatel zakázal
   - poslední zpráva
   - kdy byla poslední zpráva poslána
   - kde byla poslední zpráva poslána
 - Chatters*Channel* - informace o uživatelých specifické pro každý kanál
   - je uživatel přihlášen k události změny hry?
   - je uživatel přihlášen k události zapnutí streamu?
   - je uživatel zabanován na tomto kanále?
 - Logs - historie zpráv pro každý kanál
   - uživatelské jméno
   - zpráva
   - čas, kdy byla zpráva poslána
 - Emotes - seznam momentálně aktivních, a dřívě aktivních, emotikonů pro každý kanál
   - jméno emotikonu
   - čas, kdy byl přidán
   - čas, kdy byl odebrán
   - je momentálně aktivní?
 - Commands
   - kolikrát byl příkaz použit
   - kdy byl příkaz použit naposledy
 - Suggestions - tabulka pro ukládání návrhů ($suggest)
   - uživatelské jméno
   - návrh
   - čas, kdy byl navržen

### Datové struktury

 - ICommand
   - cooldown
   - popis příkazu ($about)
   - popis, jak příkaz použít ($help)
   - potřebná oprávnění
   - lze příkaz použít pouze pokud není zapnutý stream?
   - může tento příkaz používat zabanovaný uživatel? (např. pro odhlášení od události)
   - lze z tohoto příkazu optoutnout
 - ChatMessage
   - kanál, na kterém byla zpráva odeslána
   - uživatel, který zprávu odeslal
   - text zprávy
   - čas, kdy byla odeslána
 - IUser
   - uživatelské jméno
   - uživatelské ID
 - UserDoingSomething : IUser - při použití $afk, $gn atd.
   - text zprávy
   - kdy začal danou věc dělat
 - UserToRemind : IUser - uživatel, kterému má být něco připomenuto ($remind)
   - kdo vytvořil upozornění
   - text upozornění
   - kdy bylo upozornění vytvořeno
 - Emote
   - jméno
   - čas, kdy byl přidán
   - čas, kdy byl odebrán
   - je momentálně aktivní?
 - Pro každou API, se kterou bude bot komunikovat, bude datová struktura pro uložení potřebných informací.
