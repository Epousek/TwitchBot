# Twitch chat bot

## Specifikace požadavků 

verze 0.1, 28.10.2021  
Jan Bezouška, janbezouska@outlook.com

### Rizika

Program bude využívat knihovnu TwitchLib, pomocí které bude napojen na Twitch. Největším rizikem je, že nebude updatována, a v budoucnu se stane nekompatibilní s API Twitche nebo nebude obsahovat její nové funkce.  
Rizikem můžou být i další knihovny, které bude program používat - Newtonsoft, RestSharp a MySql.Data.  
Dalším rizikem je prostředí. Program poběží na serveru, který jsem vytvořil ze starého počítače. Problém může nastat v několika případech - komponenty v PC nejsou nejnovější, a mohly by přestat fungovat; něco je špatně nastavené, a zjistí se to až za běhu.  

### Uživatelské role

Budou existovat tři uživatelské role:
  - Vlastník - pouze autor programu; nejvyšší oprávnění, přístup ke všem funkcím  
  - Admin - malý počet lidí, kterým vlastník důvěřuje; možnost dávat a odebírat bany běžným uživatelům  
  - Běžný uživatel - drtivá většina uživatelů; přístup k uživatelským funkcím

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

### Způsoby použití

Každý příkaz má trochu jiné využití, ale obecně vzato se dají rozdělit do následujících kategorií:
  - Informace o streamu - např. jaké emotikony byly naposledy přidány
  - Informace o konkrétním uživateli - např. jaký byl první kanál, který sledoval
  - Obecné informace - např. jaké je počasí v dané lokalitě
  - Mezi-uživatelské použití - např. připomínka pro jiného uživatele

### Vymezení rozsahu

Program bude obsahovat zejména informativní funkce.  
Některé podobné programy obsahují i funkce pro moderaci chatu, což nebude součástí tohoto programu.




