# Analýza

## Popis úkolu

Jakožto mojí práci jsem si vybral vytvoření Twitch Chat bota v češtině.
 - Twitch = internetová platforma určená k živému vysílání (streamování), zejména videoher a povídání
 - Chat = uživatelé sledující živá vysílání (streamy) mají možnost psát zprávy do chatu a komunikovat tak se streamujícím uživatelem (streamerem) a ostatními diváky 
 - bot = program, který bude na chat připojen, a bude reagovat na různé příkazy
Součástí bota bude databáze uživatelů a historie chatů, ve kterých bude bot připojený.
Tuto práci jsem si vybral proto, že existuje celkem velké množství podobných botů v angličtině, ale žádné v češtině. Proto si myslím, že i přes to, že již existující boté budou mít více funkcí, bude mít můj bot alespoň nějaké využití.
Bot je určen převážně pro lidi, kteří jsou aktivní v tzv. offline chatu (?) a většina příkazů půjde používat pouze pokud daný streamer zrovna nebude streamovat.
Bot bude fungovat zejména pomocí příkazů, které musí nějaký uživatel napsat do chatu. Zároveň bude mít pár událostí, ke kterým se uživatel jednou přihlásí, a poté bude vždy upozorněn při dané události.
Příkazy budou různorodé a budou pracovat zejména s různými API nebo např. s historií chatu.
Událostí může být např. zapnutí streamu nebo změna právě hrané hry.

## Popis stávajícího stavu

Jak jsem již uvedl, podobní boté existují v zahraničí, a budou zdrojem inspirace, zejména co se týče příkazů. Zároveň bych ale rád využil to, že bot bude v češtině, a používal i různé české API, vytvářejíc originální příkazy. 
Většina botů je napsána v jazyku JavaScript, zatímco já budu používat C#, což je u takovéto práce vcelku nezvyklé. V JavaScriptu je také většina dokumentace, takže je možné, že ne vždy najdu řešení problému, se kterým si nebudu vědět rady.

## Popis výběru prostředků

### Samotná práce

 - Programovací jazyk:
    - C#          - velká zkušenost      - verzatilnější
    - JavaScript  - minimální zkušenost  - 
Budu programovat v jazyku C#, protože jsem v něm zkušenější, a zároveň v něm chci nabrat zkušeností víc.

 - IDE:
    - Visual Studio      - velká zkušenost    - hodně funkcí       - free
    - Visual Studio Code - průměrná zkušenost - méně funkcí        - free
    - Rider              - žádná zkušenost    - hodně funkcí       - 360kč/m
    - MonoDevelop        - žádná zkušenost    - téměř žádné funkce - free
Používat budu převážně Visual Studio, občas možná Visual Studio Code např. na úpravu json souborů.

 - Databáze:
    - MySQL      - pomalejší - větší komunita
    - PostgreSQL - rychlejší - menší komunita
Vybral jsem si PostgreSQL hlavně kvůli rychlosti. Od databáze nepotřebuji nic složitého/náročného, takže mi nevadí, že toho o ní tolik nenajdu na internetu.

 - Správa databáze:
    - PGAdmin 4 - velice pomalý - více funkcí - neintuitivní UI
    - OmniDB    - rychlý        - méně funkcí - lepší UI
Na správu databáze jsem vybral OmniDB vzhledem k rychlosti a jednoduchosti, kterou nabízí. 

### Plakát

 - Grafický editor:
    - Photoshop - 24,2€/měsíc - nejvíce funkcí  - žádná zkušenost, ale podobné photopee
    - Paint.net - zdarma      - dostatek funkcí - žádná zkušenost
    - Photopea  - zdarma      - dostatek funkcí - velká zkušenost
Na tvorbu plakátu (a další případnou grafiku) budu používat photopeu. Mám s ní dobré zkušenosti, a i normálně ji využívám na veškerou práci s 2D grafikou.

### Závěrečná zpráva

 - Textový editor:
   - Word        - průměrné zkušenosti - zdarma license od školy
   - LibreOffice - žádná zkušenost     - zdarma
Na tvorbu závěrečné práce a analýzy budu používat Word, protože ho na rozdíl od LibreOfficu mám nainstalovaný.

### misc.

 - Git hosting
    - GitHub
    - Gitlab
    - BitBucket
Na ukládání mé práce budu používat službu github.com, protože na ní mám uloženou veškerou dosavadní práci, a chci mít vše pohromadě.

## Výběr variant řešení a postupu

## Dílčí úkoly

Program bude rozdělen na dvě verze: main a dev. Main je verze co poběží na serveru, dev je verze na které budu pracovat. Jakmile bude nová funkce hotová na dev, tak bude dev spojen s mainem, a program na serveru se vypne, aktualizuje a zase spustí.

1. příprava a základy
   Nejdříve musím připravit půdu pro tvorbu samotné práce. Pod to spadá:
   - vytvoření vlastního serveru ze starého PC, ne kterém program poběží;
   - vytvoření databáze se systémem tabulek, do kterých se budou ukládat data která bude bot sbírat
   Součástí přípravy je také založení samotného projektu a naprogramování/nastavení věcí potřebných k napojení na Twitch (zejména obnovování autentifikačního klíče) a databázi.
   Po prvním výstupu by měla program být připravená pro konstantní běh na serveru.
2. nasazení na server a první uživatelské funkce
   V průbehu druhého výstupu vytvořím první funkce pro uživatele, a také začnu ukládat historii zpráv pro pozdější využití. Také vytvořím základ systému pro příkazy, který více využiju později.
   Na konci druhého výstupu budou pro uživatele dostupné minimálně tyto funkce:
   - Výpis emotikonů, které byly naposledy přidány
   - Výpis emotikonů, které byly naposledy odebrány
   - Základní jednoduché příkazy
   Na konci druhého výstupu budou hotové nebo rozdělané (ale funkční) tyto systémy:
   - Komunikace s databází
   - Framework pro příkazy
3. příkazy
   V rámci třetího výstupu vytvořím co nejvíce různých příkazů a funkcí. Nelze přesně určit počet nových příkazů, vzhledem k tomu, že přesně nevím složitost implementace každého z nich, ale mým cílem je alespoň 15.
   Mezi příkazy které bych rád přidal patří např. vypsání náhodné zprávy z nějakého zpravodajského serveru, vypsání počasí v dané lokaci, vypsání náhodné zprávy kterou daný uživatel napsal
4. polish
   Poslední výstup bude zejména o opravách a vylepšování stávajících funkcí a systému. Zároveň ale pravděpodobně přidám ještě nějaké příkazy, které jsem buď přidat nestihl, které mě napadly později, popřípadě které napadly nějakého uživatele.

Součástí každého výstupu jsou opravy, zejména akutních problémů, které nepříjemně ovlivňují funkci programu.