# Správce hesel

## Úvod

Tento projekt představuje **Správce hesel**, aplikaci navrženou pro bezpečné ukládání a správu hesel pro různé služby. Aplikace poskytuje uživatelům intuitivní grafické rozhraní pro správu přihlašovacích údajů s důrazem na šifrování a ochranu dat. Hlavní cílem je zajistit bezpečnost a snadnou použitelnost.

## **Registrace a přihlášení**

Proces začíná registrací, kde uživatel zadá jedinečné uživatelské jméno a hlavní heslo. Hlavní heslo je zahashováno pomocí BCrypt a uloženo odděleně, zatímco záznamy hesel jsou šifrovány pomocí AES s klíčem odvozeným z hlavního hesla. Při přihlášení je heslo ověřeno proti uloženému hashi, a pokud je úspěšné, načtou se šifrovaná data uživatele.

## Správa záznamů

Po přihlášení může uživatel:

- Přidávat nové záznamy obsahující název služby, uživatelské jméno a šifrované heslo.
- Upravovat nebo mazat existující záznamy.
- Prohlížet seznam uložených záznamů v přehledném rozhraní.

Všechny změny jsou uloženy do šifrovaného JSON souboru, což zajišťuje bezpečnost a konzistenci dat.

## Bezpečnostní opatření

Aplikace klade velký důraz na bezpečnost:

- **Šifrování**: Využívá AES v režimu CBC s PKCS7 paddingem a PBKDF2 pro odvození klíče z hlavního hesla.
- **Kontrola složitosti hesel**: Hlavní heslo i hesla pro služby musí obsahovat alespoň 8 znaků, velké a malé písmeno, číslo a speciální znak.
- **Oddělené ukládání**: Hlavní heslo je uloženo jako hash, zatímco záznamy jsou šifrovány, což minimalizuje riziko úniku dat.

## Podrobný popis

### Celkový účel a struktura

**Správce hesel** je desktopová aplikace postavená na frameworku Avalonia, určená pro bezpečné ukládání a správu hesel. Uživatelé mohou spravovat svá hesla prostřednictvím grafického rozhraní, které zahrnuje okna pro registraci/přihlášení, zobrazení seznamu záznamů a úpravu jednotlivých záznamů. Aplikace využívá šifrování AES a asynchronní operace pro zajištění bezpečnosti a plynulého chodu.

### Klíčové komponenty a datové struktury

| **Třída/Struktura** | **Popis** |
| --- | --- |
| **UserModel** | Uchovává uživatelské jméno, hlavní heslo (pouze v paměti) a cestu k souboru se záznamy. Hlavní heslo je nastaveno pouze při inicializaci. |
| **PasswordEntryModel** | Reprezentuje záznam hesla s unikátním ID (Guid), názvem služby, uživatelským jménem a šifrovaným heslem. |
| **ObservableCollection** | Dynamická kolekce pro zobrazení záznamů v UI, umožňující reaktivní aktualizace. |
| **Šifrovací proměnné** | Salt, IV a klíč používané třídou `EncryptionService` pro AES šifrování a dešifrování. |

### Koncepční popis programu

1. **Registrace uživatele**:

   - Uživatel zadá uživatelské jméno a hlavní heslo, které je ověřeno na složitost.
   - Hlavní heslo je zahashováno (BCrypt) a uloženo do souboru `{username}_master.hash`.
   - Vytvoří se šifrovaný JSON soubor `{username}.json` s prázdným polem (`[]`).

2. **Přihlášení uživatele**:

   - Uživatel zadá přihlašovací údaje, které jsou ověřeny proti uloženému hashi.
   - Po úspěšném ověření se načtou a dešifrují záznamy z JSON souboru.

3. **Správa záznamů**:

   - Přidávání nových záznamů s povinnou kontrolou složitosti hesla.
   - Úprava nebo mazání záznamů s asynchronním ukládáním změn do šifrovaného souboru.
   - Zobrazení záznamů v seznamu s možností výběru pro úpravu.

4. **Bezpečnostní mechanismy**:

   - AES šifrování s náhodným saltem a IV pro každou operaci.
   - PBKDF2 s 10 000 iteracemi pro odvození klíče.
   - Asynchronní operace pro načítání a ukládání dat, aby nedošlo k blokování UI.

5. **Uživatelské rozhraní**:

   - **Auth.axaml**: Okno pro přihlášení/registraci s poli pro uživatelské jméno a heslo.
   - **MainWindow.axaml**: Hlavní okno se seznamem záznamů a tlačítky pro přidávání/mazání.
   - **EditEntryWindow.axaml**: Okno pro úpravu záznamů s validací hesla.

### Vstupní omezení a možné problémy

- Uživatelské jméno musí být jedinečné.
- Hlavní heslo a hesla služeb musí splňovat požadavky na složitost (8+ znaků, velké/malé písmeno, číslo, speciální znak).
- Pokud soubor s hashem uživatele neexistuje, uživatel je považován za neexistujícího.
- Pokud soubor se záznamy chybí, automaticky se vytvoří prázdný.
- Zapomenutí hlavního hesla znemožňuje přístup k datům.
- Poškození šifrovaného souboru může způsobit ztrátu dat.
- Aplikace nepodporuje souběžný přístup více uživatelů.
- Předpokládá dostupnost souborového systému pro ukládání dat.



