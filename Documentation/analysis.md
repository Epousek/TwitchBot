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

Jak jsem již uvedl, podobní boté existují v zahraničí, a budou pro mě zdrojem inspirace, zejména co se táče příkazů. Budu se ale snažitvyužít i co nejvíce českých API, a vytvářet tak originální příkazy.
Velká většina podobných botů je napsána v JavaScriptu, zatímco já budu používat C#, ke kterému není zdaleka tolik dokumentace a zdrojů na internetu pro tento konkrétní úkol. To znamená, že budu možná budu mít problém najít řešení na každý problém který nastane, a budu se s tím muset vypořádat sám.

## Popis výběru prostředků

### Samotná práce

 - Programovací jazyk:
    - C#          - velká zkušenost      - verzatilnější
    - JavaScript  - minimální zkušenost  - \
Budu programovat v jazyku C#, protože jsem v něm zkušenější, a zároveň v něm chci nabrat zkušeností víc.

 - IDE:
    - Visual Studio      - velká zkušenost    - hodně funkcí       - free
    - Visual Studio Code - průměrná zkušenost - méně funkcí        - free
    - Rider              - žádná zkušenost    - hodně funkcí       - 360kč/m
    - MonoDevelop        - žádná zkušenost    - téměř žádné funkce - free\
Na programování budu používat čistě Visual Studio. Na úpravu např. json souborů budu používat Visual Studio Code.

 - Databáze:
    - MySQL      - rychlejší - Méně funkcí - větší komunita
    - PostgreSQL - pomalejší - více funkcí - menší komunita\
Vybral jsem si PostgreSQL hlavně kvůli rychlosti. Od databáze nepotřebuji nic složitého/náročného, takže mi nevadí, že toho o ní tolik nenajdu na internetu.

 - Správa databáze:
Na správu databáze budu používat MySQL Workbench, protože je dělaný přímo pro MySQL, a navíc nepotřebuji od toho programu nic složitého, a nemá tak cenu používat jiný program.

 - Git hosting
    - GitHub
    - Gitlab
    - BitBucket\
Na ukládání mé práce budu používat službu github.com, protože na ní mám uloženou veškerou dosavadní práci, a chci mít vše pohromadě.

### Plakát

 - Grafický editor:
    - Photoshop - 24,2€/měsíc - nejvíce funkcí  - žádná zkušenost, ale podobné photopee
    - Paint.net - zdarma      - dostatek funkcí - žádná zkušenost
    - Photopea  - zdarma      - dostatek funkcí - velká zkušenost\
Plakát, a případnou další grafiku, budu vytvářet pomocí stánky photopea.com. Mám s ní dobré zkušenosti a běžně ji využívám na veškerou práci s 2D grafikou.

### Závěrečná zpráva

 - Textový editor:
   - Word        - průměrné zkušenosti - zdarma license od školy
   - LibreOffice - žádná zkušenost     - zdarma\
Ke tvorbě závěrečné práce budu používat Word, vzledem k tomu že ho už mám a používám, a nemá cenu stahovat i LibreOffice.

## Výběr variant řešení a postupu

## Dílčí úkoly

Program bude rozdělen na dvě verze: main a dev. Main je verze co poběží na serveru, dev je verze na které budu pracovat. Jakmile bude nová funkce hotová na dev, tak bude dev spojen s mainem, a program na serveru se vypne, aktualizuje a zase spustí.

1. příprava a základy
   Nejdříve musím připravit půdu pro tvorbu samotné práce. Pod to spadá:
   - vytvoření vlastního serveru ze starého PC, ne kterém program poběží;
   - vytvoření databáze se systémem tabulek, do kterých se budou ukládat data která bude bot sbírat.\
   Součástí přípravy je také založení samotného projektu a naprogramování/nastavení věcí potřebných k napojení na Twitch (zejména obnovování autentifikačního klíče) a databázi.
   Po prvním výstupu by měla program být připravená pro konstantní běh na serveru.
2. nasazení na server a první uživatelské funkce
   V průbehu druhého výstupu vytvořím první funkce pro uživatele, a také začnu ukládat historii zpráv pro pozdější využití. Také vytvořím základ systému pro příkazy, který více využiju později.
   Na konci druhého výstupu budou pro uživatele dostupné minimálně tyto funkce:
   - Výpis emotikonů, které byly naposledy přidány
   - Výpis emotikonů, které byly naposledy odebrány
   - Základní jednoduché příkazy\
   Na konci druhého výstupu budou hotové nebo rozdělané (ale funkční) tyto systémy:
   - Komunikace s databází
   - Framework pro příkazy
3. příkazy
   V rámci třetího výstupu vytvořím co nejvíce různých příkazů a funkcí. Nelze přesně určit počet nových příkazů, vzhledem k tomu, že přesně nevím složitost implementace každého z nich, ale mým cílem je alespoň 15.
   Mezi příkazy které bych rád přidal patří např. vypsání náhodné zprávy z nějakého zpravodajského serveru, vypsání počasí v dané lokaci, vypsání náhodné zprávy kterou daný uživatel napsal
4. polish
   Poslední výstup mám rezervovaný zejména pro opravy, úpravy a vylepšování stávajících funkcí a systémů. Zároveň ještě budu přidávat nové příkazy, které jsem buď přidat nestihl, které mě napadli později, poopřípadě které napadly nějakého uživatele. U tohoto výstupu lze ještě hůře určit počet nových příkazů, vzledem k tomu že netuším kolik času mi zaberou opravy a vylepšování.

Součástí každého výstupu jsou opravy akutních problémů, díky kterým bot nefunguje nebo je méně stabilní.
