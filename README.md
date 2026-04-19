# Szlakomat -- System Planowania Tras Wycieczek

System planowania tras zwiedzania i wycieczek dla różnych miast, zainspirowany koncepcjami Domain-Driven Design. Pragmatyczne podejście do modelowania domeny turystycznej, demonstrujące zastosowanie wybranych wzorców DDD (bounded contexts, aggregates, value objects) bez dogmatyzmu. Zbudowany z .NET 10, CQRS poprzez MediatR, obsługą błędów zorientowaną na tory kolejowe (railway-oriented), i wymuszanymi funkcjami sprawdzającymi architekturę.

Domena jest tematycznie powiązana z **turystyką i planowaniem wycieczek dla różnych miast** (w tym Krakowa), modelując atrakcje, bilety, pakiety wycieczek i ich relacje. Architektura bazuje na uniwersalnym modelu produktu, ale jest dostosowana do specyfiki turystyki i planowania tras.

## Spis treści

[1. Wprowadzenie](#1-wprowadzenie)

&nbsp;&nbsp;[1.1 Cel tego repozytorium](#11-cel-tego-repozytorium)

[2. Domena](#2-domena)

&nbsp;&nbsp;[2.1 Opis](#21-opis)

[3. Architektura](#3-architektura)

&nbsp;&nbsp;[3.1 Widok wysokopoziomowy](#31-widok-wysokopoziomowy)

&nbsp;&nbsp;[3.2 Widok na poziomie ograniczonego kontekstu](#32-widok-na-poziomie-ograniczonego-kontekstu)

&nbsp;&nbsp;[3.3 Mapa kontekstu](#33-mapa-kontekstu)

&nbsp;&nbsp;[3.4 CQRS poprzez MediatR](#34-cqrs-poprzez-mediatr)

&nbsp;&nbsp;[3.5 Zasady modelu domeny](#35-zasady-modelu-domeny)

&nbsp;&nbsp;[3.6 Programowanie zorientowane na tory kolejowe](#36-programowanie-zorientowane-na-tory-kolejowe)

&nbsp;&nbsp;[3.7 Wzorzec Composite](#37-wzorzec-composite)

&nbsp;&nbsp;[3.8 Fluent Builders](#38-fluent-builders)

&nbsp;&nbsp;[3.9 Specyfikacje komponowalne](#39-specyfikacje-komponowalne)

&nbsp;&nbsp;[3.10 Testy jednostkowe](#310-testy-jednostkowe)

&nbsp;&nbsp;[3.11 Testy jednostkowe architektury](#311-testy-jednostkowe-architektury)

&nbsp;&nbsp;[3.12 Testy podróży (Journey Tests)](#312-testy-podróży-journey-tests)

[4. Technologia](#4-technologia)

[5. Jak uruchomić](#5-jak-uruchomić)

[6. Struktura rozwiązania](#6-struktura-rozwiązania)

[7. Punkty końcowe API](#7-punkty-końcowe-api)

[8. Licencja](#8-licencja)

[9. Podziękowania](#9-podziękowania)

## 1. Wprowadzenie

### 1.1 Cel tego repozytorium

To repozytorium jest **systemem do planowania tras wycieczek**, który demonstruje praktyczne zastosowanie wybranych koncepcji z Domain-Driven Design. Projekt **nie jest czystą implementacją DDD**, lecz pragmatycznym podejściem, które bierze najlepsze wzorce z DDD (bounded contexts, aggregates, encapsulation) bez dogmatyzmu. Służy jako:

- **Modelowy przykład domeny turystycznej** — atrakcje, bilety, pakiety wycieczek i relacje między nimi
- **Działający przykład** ścisłego wymuszania zależności warstwy, obsługi błędów zorientowanej na tory kolejowe, wzorca composite na poziomie domeny i fluent builder API

Główny nacisk położony jest na **praktyczne modelowanie domeny turystycznej**, **poprawność logiki biznesowej**, **wymuszaną architekturę** i **kompleksową strategię testowania**.

## 2. Domena

### 2.1 Opis

> "Każdy program komputerowy odnosi się do pewnej działalności lub zainteresowania użytkownika. Obszar tematu, do którego użytkownik stosuje program, to domena oprogramowania."
>
> -- Eric Evans, Domain-Driven Design

Domena **Planowania Wycieczek i Tras Turystycznych** została wybrana, ponieważ reprezentuje rzeczywisty problem biznesowy w branży turystycznej:

- Jest to **praktyczne wyzwanie** -- organizacje turystyczne muszą definiować atrakcje, bilety, pakiety wycieczek i zarządzać ich dostępnością, ograniczeniami i relacjami
- Jest to **nie trywialne** -- właściwy model musi obsługiwać różne typy atrakcji (pojedyncze bilety, pakiety wycieczek, produkty composite o zmiennym składzie) oraz relacje między produktami
- Zawiera **rzeczywiste reguły biznesowe**, które muszą być wymuszane przez domenę -- ograniczenia wartości cech produktu, reguły wyboru komponentów pakietu, warunki stosowalności produktów w danym kontekście
- Demonstruje **wyzwania bez świadomego modelu domeny** -- turystyczne systemy legacy dysponują chaotycznym kodowaniem atrakcji, niespójnymi danymi o dostępności, i każda zmiana oferty wymaga ingerencji technicznej zamiast prostej decyzji biznesowej

Domena podzielona jest na cztery poddomeny, z których każda dotyczy odrębnego problemu biznesowego.

**Katalog Produktów**

Katalog Produktów odpowiada na fundamentalne pytanie: *co oferujemy?* Organizacja musi być w stanie definiować swoje produkty w sposób strukturyzowany, jednoznaczny -- nie jako tekst wolnodzielny w arkuszu kalkulacyjnym, ale jako formalny model, który cały system rozumie.

`ProductType` (Typ Produktu) reprezentuje definicję jednego produktu -- co *jest*, a nie żadną konkretną instancję tego. Na przykład "Bilet wstępu do zamku Wawelu dla dorosłych" to `ProductType`. Ma `ProductName` (Nazwa Produktu) widoczną dla klientów, `ProductDescription` (Opis Produktu) wyjaśniającą oczekiwania klienta i `ProductTrackingStrategy` (Strategia Śledzenia Produktu), która mówi systemowi, jak konkretne instancje tego produktu powinny być śledzone. Produkt, który jest `IndividuallyTracked` (Indywidualnie Śledzony) wymaga numeru seryjnego dla każdej instancji. Produkt, który jest `BatchTracked` (Śledzony Partiami) grupuje instancje w partie produkcyjne. Produkt, który jest `Identical` (Identyczny) nie rozróżnia między instancjami -- tylko ilość ma znaczenie.

Każdy `ProductType` może definiować wpisy `ProductFeatureType` (Typ Cechy Produktu) -- zarówno obowiązkowe jak i opcjonalne. Cecha obowiązkowa musi być podana przy tworzeniu instancji; cecha opcjonalna może być pominięta. Każda cecha nosi `IFeatureValueConstraint` (Ograniczenie Wartości Cechy), które ogranicza akceptowalne wartości: `AllowedValues` (Dozwolone Wartości) dla wyborów wyliczeniowych, `NumericRange` (Zakres Numeryczny) czy `DecimalRange` (Zakres Dziesiętny) dla liczb ograniczonych, `DateRange` (Zakres Dat) dla granic czasowych, `Regex` dla dopasowania wzorca, czy `Unconstrained` (Bez Ograniczeń) gdy dowolna wartość jest ważna.

Produkty mogą być również komponowane w pakiety. `PackageType` (Typ Pakietu) to produkt composytowy zbudowany z `PackageStructure` (Struktury Pakietu) -- kolekcja wpisów `ProductSet` (Zestaw Produktów) połączonych z specyfikacjami `SelectionRule` (Reguła Wyboru). Reguły wyboru są w pełni komponowalne: `And`, `Or`, `Not` i `IfThen` mogą być zagnieżdżane arbitralnie. Pakiet "Krakowski Karnet Królewski" może wymagać dokładnie jednego wyboru z zestawu wycieczek po zamkach *i* opcjonalnie jednego wyboru z zestawu rejsów po rzece. Reguły wyboru wymuszają te ograniczenia na poziomie domeny.

Typy produktów są **niezmienne po utworzeniu**. Nie ma polecenia aktualizacji. To jest celowa decyzja projektowa: gdy typ produktu został zdefiniowany i mogą istnieć instancje, zmiana definicji tworzyłaby niespójności między typem a jego istniejącymi instancjami.

**Oferta Komercyjna**

Subdomena Oferta Komercyjna zarządza *kiedy i jak* produkt jest przedstawiany klientom. `CatalogEntry` (Wpis Katalogowy) opakowuje `ProductType` z metadanymi marketingowymi: `DisplayName` (Nazwa Wyświetlana, która może różnić się od technicznej nazwy produktu), `Category` (Kategorię) do nawigacji i `Validity` (Ważność) okres określający, kiedy wpis jest dostępny.

Okres `Validity` (Ważności) to zakres czasowy -- data `From` (Od) i data `To` (Do). Gdy klient szuka w katalogu, zwracane są tylko wpisy, których okno ważności obejmuje datę wyszukiwania. Jednak ważność `CatalogEntry` *nie* blokuje tworzenia instancji produktu. To jest celowe rozdzielenie obaw: warstwa komercyjna kontroluje widoczność, ale warstwa operacyjna wciąż może tworzyć instancje niezależnie. Operator magazynu nie musi sprawdzać, czy zespół marketingowy opublikował wpis katalogowy przed utworzeniem magazynu.

**Instancje Produktów**

Subdomena Instancje Produktów tworzy **konkretne realizacje** typów produktów. O ile katalog definiuje *co* produkt jest, instancje reprezentują *co faktycznie istnieje* -- konkretny bilet z kodem rezerwacji, konkretna partia karnetów grupowych, zakupiony pakiet z wybranymi komponentami.

`ProductInstance` (Instancja Produktu) to konkretny egzemplarz `ProductType` (Typu Produktu). W zależności od strategii śledzenia, może on niesć `SerialNumber` (Numer Seryjny) (dla produktów indywidualnie śledzonych), `BatchId` (ID Partii) (dla produktów śledzonych partiami), obydwa lub żaden (dla produktów identycznych). System wspiera polimorficzne numery seryjne: `TextualSerialNumber` (Tekstowy Numer Seryjny) dla identyfikatorów swobodnych jak kody rezerwacji, `ImeiSerialNumber` (Numer Seryjny IMEI) dla urządzeń mobilnych z walidacją cyfry kontrolnej Luhn i `VinSerialNumber` (Numer Seryjny VIN) dla pojazdów.

Gdy `ProductInstance` jest tworzona, system wymusza ograniczenia cech typu produktu. Każdy obowiązkowy `ProductFeatureType` musi mieć wartość, a każda podana wartość musi spełniać `IFeatureValueConstraint` cechy. `Quantity` (Ilość) instancji musi używać tej samej `Unit` (Jednostki) co preferowana jednostka typu produktu -- nie możesz utworzyć instancji mierzonej w kilogramach dla typu produktu, który oczekuje sztuk.

`PackageInstance` (Instancja Pakietu) odzwierciedla strukturę composytową na poziomie instancji. Zawiera wybór instancji komponentów, które muszą spełniać reguły wyboru typu pakietu. `Batch` (Partia) grupuje instancje według produkcji lub dostawy, niosąc opcjonalne daty dla produkcji, sprzedaży, użytkowania i najlepszego terminu.

**Relacje Produktów**

Produkty nie istnieją w izolacji. `ProductRelationship` (Relacja Produktu) to połączenie skierowane między dwoma typami produktów, niosące `ProductRelationshipType` (Typ Relacji Produktu), który opisuje naturę połączenia. Wspierane są sześć typów relacji: `UpgradableTo` (Do Ulepszenia), `SubstitutedBy` (Zastępowalny), `ReplacedBy` (Zastąpiony), `ComplementedBy` (Uzupełniany), `CompatibleWith` (Kompatybilny) i `IncompatibleWith` (Niekompatybilny).

Relacje są bramkowane przez `IProductRelationshipDefiningPolicy` (Polityka Definiowania Relacji Produktu) -- punkt rozszerzywalności, który pozwala systemowi wymuszać reguły biznesowe dotyczące tego, które relacje są dozwolone. Domyślną polityką jest `AlwaysAllow` (Zawsze Zezwalaj), ale organizacje mogą wdrożyć niestandardowe polityki, które na przykład zapobiegają relacjom między produktami w różnych kategoriach lub wymagają zatwierdzenia dla pewnych typów relacji.

## 3. Architektura

> **Diagramy wizualne:** Interaktywne diagramy architektury znajdują się w katalogu [`diagrams/`](diagrams/):
> - [`layers.html`](diagrams/layers.html) — warstwy i kierunek zależności
> - [`bounded_contexts.html`](diagrams/bounded_contexts.html) — bounded contexty i ich relacje
> - [`request_flow.html`](diagrams/request_flow.html) — przepływ requestu od kontrolera przez MediatR do repozytorium

### 3.1 Widok wysokopoziomowy

System podąża za **ścisłą architekturą warstwową** z regułą zależności wymuszaną przez automatyczne testy architektury:

```
Domena  <--  Aplikacja  <--  Infrastruktura  <--  Api
(brak zależ.)  (MediatR)      (DI, repozytoria)   (ASP.NET Core)
```

**Opisy warstw:**

- **Domena** -- Zawiera model domeny: agregaty, obiekty wartości, interfejsy repozytoriów i logikę domeny. Ma zero zewnętrznych zależności (oprócz `CommunityToolkit.Diagnostics` dla klauzul chroniących). To jest serce systemu.
- **Aplikacja** -- Zawiera polecenia CQRS, zapytania i ich obsługi orkiestrowane przez MediatR. Zależy tylko od Domeny. Ta warstwa definiuje przypadki użycia systemu.
- **Infrastruktura** -- Zawiera implementacje interfejsów repozytoriów (w pamięci), rejestrację DI i dane seed. Zależy od Domeny i Aplikacji.
- **Api** -- Kontrolery ASP.NET Core Web API, które tłumaczą żądania HTTP na polecenia/zapytania. Zależy od wszystkich warstw i łączy wszystko razem.

**Kluczowe założenia:**

1. Kierunek zależności jest ściśle wymuszany: warstwy wewnętrzne nigdy nie referencjonują warstw zewnętrznych. Domena nic nie wie o Aplikacji, Infrastrukturze czy Api.
2. Każdy ograniczony kontekst ma swój własny podkatalog w `Szlakomat.Products.Domain/` i `Szlakomat.Products.Application/`, zapewniając jasne rozdzielenie obaw.
3. `InternalsVisibleTo` jest używany, aby pozwolić Aplikacji widzieć nieoficjalne elementy Domeny i Infrastrukturze widzieć nieoficjalne elementy Aplikacji -- ale nigdy w kierunku odwrotnym.
4. Wszystkie reguły architektoniczne są weryfikowane przez automatyczne funkcje sprawdzające na każdym przebiegu testów.

### 3.2 Widok na poziomie ograniczonego kontekstu

Każdy ograniczony kontekst podąża za Clean Architecture wewnętrznie. Warstwa domeny definiuje agregaty i obiekty wartości; warstwa aplikacji definiuje polecenia, zapytania i obsługi; warstwa infrastruktury zapewnia implementacje repozytoriów.

#### Katalog Produktów (`Catalog/`)

Definiuje **czym produkt jest**. Dwa agregaty: `ProductType` (liść) i `PackageType` (composite).

Kluczowe pojęcia:
- `ProductFeatureType` z `IFeatureValueConstraint` -- AllowedValues, NumericRange, DecimalRange, DateRange, Regex, Unconstrained
- `PackageStructure` = `ProductSet` + `SelectionRule` -- reguły komponowalne: And/Or/Not/IfThen, min/max ilościowe
- `ProductTrackingStrategy` -- Identical, IndividuallyTracked, BatchTracked, IndividuallyAndBatchTracked, Unique
- Typy są **niezmienne po utworzeniu** (brak poleceń aktualizacji)

#### Oferta Komercyjna (`CommercialOffer/`)

Zarządza **kiedy i jak produkt jest prezentowany**. Pojedynczy agregat: `CatalogEntry`.

- `Validity` (okres ważności), `DisplayName` (Nazwa Wyświetlana), `Category` (Kategoria) -- warstwa marketingowa na górze ProductType
- `CatalogEntry.Validity` *nie* blokuje tworzenia instancji (celowe rozdzielenie obaw)

#### Instancje Produktów (`Instances/`)

Tworzy **konkretne realizacje** typów produktów. Agregaty: `ProductInstance` (liść), `PackageInstance` (composite), `Batch`.

- `ProductTrackingStrategy` wymuszana przy tworzeniu instancji (wymagania serial/batch)
- Wartości cech weryfikowane względem ograniczeń `ProductFeatureType`
- `Quantity.Unit` musi być zgodny z `ProductType.PreferredUnit`

#### Relacje Produktów (`Relationships/`)

Zarządza **relacjami skierowanymi** między typami produktów. Pojedynczy agregat: `ProductRelationship`.

- Typy: UpgradableTo, SubstitutedBy, ReplacedBy, ComplementedBy, CompatibleWith, IncompatibleWith
- `IProductRelationshipDefiningPolicy` bramkuje tworzenie (domyślnie: AlwaysAllow)
- Zwraca `Result<string, ProductRelationship>` -- obsługa błędów zorientowana na tory kolejowe

### 3.3 Mapa kontekstu

Katalog Produktów jest **upstream** dla wszystkich trzech innych kontekstów:

```
                     +-----------------+
                     | Katalog Produktów|
                     |   (upstream)     |
                     +-------+---------+
                             |
              +--------------+--------------+
              |              |              |
     +--------v---+  +-------v------+  +---v-----------+
     | Oferta     |  |   Instancje  |  |    Relacje    |
     | Komercyjna |  |   Produktów  |  |   Produktów   |
     |(downstream)|  | (downstream) |  | (downstream)  |
     +-------------+ +--------------+  +---------------+
```

**Wspólne jądro** (typy używane w kontekstach): `IProduct`, `IInstance`, `IProductIdentifier`, `ISerialNumber`, `Result<F,S>`, `Quantity/Unit`.

### 3.4 CQRS poprzez MediatR

CQRS (Command Query Responsibility Segregation) rozdziela operacje odczytu i zapisu na odrębne modele. W tym systemie polecenia i zapytania są definiowane jako rekordy `IRequest<TResponse>`, a obsługi to `sealed` klasy implementujące `IRequestHandler<TRequest, TResponse>`, umieszczone w tej samej przestrzeni nazw co ich polecenie. MediatR zapewnia mediator w procesie, który kieruje żądania do obsług.

Więcej informacji: [CQRS -- Microsoft Docs](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs)

**Definicja polecenia** (`DefineProductType.cs`):

```csharp
public record DefineProductType(
    string ProductIdType,
    string ProductId,
    string Name,
    string Description,
    string Unit,
    string TrackingStrategy,
    IReadOnlySet<MandatoryFeature>? MandatoryFeatures,
    IReadOnlySet<OptionalFeature>? OptionalFeatures,
    IReadOnlyDictionary<string, string>? Metadata
) : IRequest<Result<string, IProductIdentifier>>;
```

**Obsługa** (`DefineProductTypeHandler.cs`):

```csharp
internal sealed class DefineProductTypeHandler
    : IRequestHandler<DefineProductType, Result<string, IProductIdentifier>>
{
    private readonly IProductTypeRepository _repository;

    public DefineProductTypeHandler(IProductTypeRepository repository)
    {
        _repository = repository;
    }

    public Task<Result<string, IProductIdentifier>> Handle(
        DefineProductType command, CancellationToken cancellationToken)
    {
        try
        {
            var identifier = ProductTypeMapper.ParseProductIdentifier(
                command.ProductIdType, command.ProductId);
            var trackingStrategy = ProductTypeMapper.ParseTrackingStrategy(
                command.TrackingStrategy);
            var unit = ProductTypeMapper.ParseUnit(command.Unit);

            var mainBuilder = new ProductBuilder(
                identifier,
                ProductName.Of(command.Name),
                ProductDescription.Of(command.Description));

            var typeBuilder = mainBuilder.AsProductType(unit, trackingStrategy);

            // ... rejestracja cech pominięta dla zwięzłości ...

            var productType = typeBuilder.Build();
            _repository.Save(productType);
            return Task.FromResult(
                Result<string, IProductIdentifier>.SuccessOf(productType.Id()));
        }
        catch (Exception e)
        {
            return Task.FromResult(
                Result<string, IProductIdentifier>.FailureOf(e.Message));
        }
    }
}
```

**Kluczowe zalety CQRS:**

- Każda obsługa polecenia/zapytania ma **jedną odpowiedzialność**, co czyni kod łatwym do zrozumienia i testowania
- Polecenia i zapytania mogą być **rozwijane niezależnie** -- modele odczytu mogą być optymalizowane bez wpływu na logikę zapisu
- Obsługi są naturalnie **izolowane** -- zmiana jednego przypadku użycia nie stwarza ryzyka rozbicia innego
- Pipeline MediatR wspiera **problemy przekrojowe** (walidacja, logowanie) poprzez zachowania

**Wadą:** CQRS dodaje narzut strukturalny. Dla prostych operacji CRUD rozdzielenie polecenia/obsługi może wydawać się niepotrzebną ceremonią. W tym projekcie narzut jest uzasadniony złożonością reguł domeny, które każda obsługa musi wymuszać.

### 3.5 Zasady modelu domeny

Model Domeny jest centralną i najkrytyczniejszą częścią systemu. Enkapsuluje wszystkie reguły biznesowe, niezmienniki i logikę domeny. Wszystkie inne warstwy istnieją, aby mu służyć. Następujące zasady kierują jego projektem:

1. **Enkapsulacja** -- Wszystkie obiekty domeny strażują swoje niezmienniki. Agregaty ujawniają zachowanie poprzez metody, a nie publiczne settery. Klasy wewnętrzne są ukryte za publicznymi interfejsami:

```csharp
// ProductType jest wewnętrzny -- konsumenci widzą tylko IProduct
internal class ProductType : IProduct
{
    private readonly IProductIdentifier _id;
    private readonly ProductName _name;
    private readonly ProductDescription _description;
    private readonly Unit _preferredUnit;
    private readonly ProductTrackingStrategy _trackingStrategy;
    private readonly ProductFeatureTypes _featureTypes;
    private readonly ProductMetadata _metadata;
    private readonly IApplicabilityConstraint _applicabilityConstraint;

    // Konstruktor waliduje wszystkie niezmienniki poprzez klauzule Guard
    public ProductType(
        IProductIdentifier? id, ProductName? name, ...)
    {
        Guard.IsNotNull(id);
        Guard.IsNotNull(name);
        // ... wszystkie pola chronione
    }
}
```

2. **Bogate obiekty wartości** -- Obiekty wartości enkapsulują pojęcia domeny z walidacją i zachowaniem. `ProductName`, `ProductDescription`, `ProductMetadata`, `Quantity`, `Unit` to wszystkie obiekty wartości. Identyfikatory (`GTIN`, `ISBN`, `IMEI`, `VIN`) walidują format przy konstruowaniu.

3. **Niezmienność** -- Typy są niezmienne po utworzeniu -- nie ma poleceń aktualizacji dla `ProductType` czy `PackageType`. Obiekty wartości nie mają publicznych setterów (wymuszane przez testy architektury).

4. **Brak prymitywnej obsesji** -- Pojęcia domeny są opakowane w dedykowane typy zamiast przekazywane jako surowe stringi czy inty. Identyfikatory produktów, numery seryjne, ilości i jednostki mają własne typy z walidacją.

5. **Publiczny interfejs, wewnętrzna implementacja** -- `ProductType`, `PackageType`, `ProductBuilder` i `InstanceBuilder` to wszystkie `internal`. Publiczna powierzchnia API składa się z interfejsów (`IProduct`, `IInstance`), obiektów wartości i kontraktów repozytoriów.

6. **Statyczne metody fabryki** -- Agregaty zapewniają nazwy metody fabryki wyrażające intencję:

```csharp
ProductType.Define(id, name, description);        // Prosty produkt
ProductType.Unique(id, name, description);         // Jeden egzemplarz
ProductType.IndividuallyTracked(id, name, desc, unit);
ProductType.BatchTracked(id, name, desc, unit);
```

### 3.6 Programowanie zorientowane na tory kolejowe

Programowanie Zorientowane na Tory Kolejowe to funkcjonalne podejście do obsługi błędów, które traktuje sukces i porażkę jako dwa równoległe "tory" -- dane płyną wzdłuż toru sukcesu aż błąd przenosi je na tor porażki, a gdy są na torze porażki, kolejne operacje są pomijane. To eliminuje zagnieżdżone sprawdzanie błędów `if/else` i czyni propagację błędów jawną w systemie typów.

Więcej informacji: [Railway Oriented Programming](https://fsharpforfunandprofit.com/rop/) autorstwa Scotta Wlaschina

Typ `Result<TF, TS>` zapewnia ten wzorzec, wdrażając go jako rozłączne ujednolicenie poprzez sealed records:

```csharp
public abstract record Result<TF, TS>
{
    public sealed record Success : Result<TF, TS> { public TS Value { get; } }
    public sealed record Failure : Result<TF, TS> { public TF Value { get; } }

    // Operacje podstawowe
    public Result<TF, TR> Map<TR>(Func<TS, TR> mapper);
    public Result<TF, TR> FlatMap<TR>(Func<TS, Result<TF, TR>> mapping);
    public TU Fold<TU>(Func<TF, TU> leftMapper, Func<TS, TU> rightMapper);

    // Połącz dwa rezultaty
    public Result<TR, TU> Combine<TR, TU>(
        Result<TF, TS> secondResult,
        Func<TF?, TF?, TR> failureCombiner,
        Func<TS, TS, TU> successCombiner);

    // Efekty uboczne
    public Result<TF, TS> Peek(Action<TS> successConsumer, Action<TF> failureConsumer);

    // Metody fabryki
    public static Result<TF, TS> SuccessOf(TS value);
    public static Result<TF, TS> FailureOf(TF value);

    // Akumulacja (zbierz wiele rezultatów na liście/zbiorze)
    public static CompositeResult<TF, TS> Composite();
    public static CompositeSetResult<TF, TS> CompositeSet();
}
```

Użycie w obsługach zwraca `Result<string, T>` gdzie typem porażki jest string komunikatu błędu. Kontrolery wzorują się na wyniku:

```csharp
var result = await mediator.Send(cmd);
if (!result.IsSuccess())
    return BadRequest(new { error = result.GetFailure() });
```

### 3.7 Wzorzec Composite

[Wzorzec Composite](https://refactoring.guru/design-patterns/composite) pozwala komponować obiekty w struktury drzewiaste, a następnie pracować z tymi strukturami tak, jakby były poszczególnymi obiektami. W tej domenie zarówno produkty jak i instancje tworzą hierarchie composite -- pakiet to produkt złożony z innych produktów, a instancja pakietu to instancja złożona z innych instancji.

Wzorzec Composite jest stosowany na dwóch poziomach modelu domeny:

**Poziom definicji produktu**:

```csharp
// Komponent
public interface IProduct
{
    IProductIdentifier Id();
    ProductName Name();
    ProductDescription Description();
    ProductMetadata Metadata();
    IApplicabilityConstraint ApplicabilityConstraint();
    bool IsApplicableFor(ApplicabilityContext context);
}

// Liść
internal class ProductType : IProduct { /* definicja pojedynczego produktu */ }

// Composite
internal class PackageType : IProduct
{
    private readonly PackageStructure _structure; // zawiera ProductSets + SelectionRules
    public PackageValidationResult ValidateSelection(List<SelectedProduct> selection);
}
```

**Poziom instancji** odzwierciedla tę samą strukturę:
- `IInstance` (komponent)
- `ProductInstance` (liść) -- konkretny bilet z numerem seryjnym i cechami
- `PackageInstance` (composite) -- zakupiony pakiet z wybranymi instancjami komponentów

### 3.8 Fluent Builders

Dwie hierarchie builderów zapewniają płynny interfejs API do konstruowania obiektów domeny:

**ProductBuilder** -- buduje zarówno `ProductType` jak i `PackageType`:

```csharp
// Zbuduj ProductType (liść)
ProductType laptop = new ProductBuilder(id, name, description)
    .WithMetadata("category", "electronics")
    .AsProductType(Unit.Pieces(), ProductTrackingStrategy.IndividuallyTracked)
        .WithMandatoryFeature(colorFeature)
        .Build();

// Zbuduj PackageType (composite)
PackageType bundle = new ProductBuilder(id, name, description)
    .WithMetadata("promotion", "summer2025")
    .AsPackageType()
        .WithSingleChoice("Memory", ram8GB.Id(), ram16GB.Id())
        .WithOptionalChoice("Accessories", mouse.Id(), bag.Id())
        .Build();
```

**InstanceBuilder** -- buduje zarówno `ProductInstance` jak i `PackageInstance`:

```csharp
// Zbuduj ProductInstance
ProductInstance ticket = new InstanceBuilder(instanceId)
    .WithSerial(serialNumber)
    .AsProductInstance(ticketType)
        .WithQuantity(Quantity.Of(1, Unit.Pieces()))
        .WithFeature(dateFeature, "2025-04-15")
        .Build();

// Zbuduj PackageInstance
PackageInstance pass = new InstanceBuilder(instanceId)
    .WithSerial(passCode)
    .AsPackageInstance(packageType)
        .WithSelection(selectedInstances)
        .Build();
```

Oba buildery używają **dwufazowego projektowania**: parent builder zbiera wspólne atrybuty (`id`, `name`, `metadata`), następnie `AsProductType()` / `AsPackageType()` (lub `AsProductInstance()` / `AsPackageInstance()`) zwraca specjalizowany inner builder dla atrybutów specyficznych dla typu.

### 3.9 Specyfikacje komponowalne

Dwa systemy specyfikacji zapewniają komponowalne reguły biznesowe:

**`IApplicabilityConstraint`** -- określa, czy produkt ma zastosowanie w danym kontekście:
- Prymitywne: `Equals`, `In`, `GreaterThan`, `LessThan`, `Between`
- Logiczna kompozycja: `And`, `Or`, `Not`
- Przykład: "Ten bilet ma zastosowanie gdy `season` (sezon) jest `In("summer", "spring")` (w letniości lub wiosny) I `customerAge` (wiek klienta) jest `GreaterThan(18)` (większe niż 18)"

**`ISelectionRule`** -- waliduje wybór produktu w pakiecie:
- `IsSubsetOf(productSet, min, max)` -- wybór musi pochodzić z danego zestawu z ograniczeniami kardynalności
- Logiczna kompozycja: `And`, `Or`, `Not`, `IfThen` (reguły warunkowe)
- Przykład: "Klient musi wybrać dokładnie 1 z opcji Pamięć I 0-1 z Akcesoriów"

Oba systemy podążają za wzorcem Specification, pozwalając na składanie reguł arbitralnie w czasie wykonania.

### 3.10 Testy jednostkowe

> "Test jednostkowy to zautomatyzowany fragment kodu, który wywołuje jednostkę pracy w systemie, a następnie sprawdza jedno założenie na temat zachowania tej jednostki pracy."
>
> -- Roy Osherove, The Art of Unit Testing

**Atrybuty dobrych testów jednostkowych:**

- **Szybkie** -- Testy jednostkowe uruchamiają się w milisekundach. Testują izolowaną logikę domeny bez I/O, bez kontenera DI, bez sieci.
- **Izolowane** -- Każdy test weryfikuje jedno zachowanie. Awaria wskazuje dokładnie jedną przyczynę.
- **Powtarzalne** -- Taki sam wynik za każdym razem, niezależnie od kolejności czy środowiska.
- **Samoweryfikujące** -- Pass lub fail, bez ręcznej inspekcji potrzebnej.
- **Terminowe** -- Napisane razem ze (lub przed) kodem, który weryfikują.

#### Implementacja

Testy jednostkowe w `Szlakomat.Products.Domain.Tests` podążają za wzorcem **Arrange/Act/Assert**. Każdy test tworzy minimalną konfigurację potrzebną, wykonuje jedną operację i asertuje jeden wynik. FluentAssertions zapewnia czytelną składnię asertacji.

**Prosty test obiektu wartości:**

```csharp
[Fact]
public void Map_OnSuccess_ShouldTransformValue()
{
    var result = Result<string, int>.SuccessOf(10);

    var mapped = result.Map(x => x * 2);

    mapped.IsSuccess().Should().BeTrue();
    mapped.GetSuccess().Should().Be(20);
}
```

**Test łańcuchu monadycznego:**

```csharp
[Fact]
public void FlatMap_OnSuccess_ShouldChainResults()
{
    var result = Result<string, int>.SuccessOf(10);

    var chained = result.FlatMap(x =>
        x > 5
            ? Result<string, string>.SuccessOf($"big: {x}")
            : Result<string, string>.FailureOf("too small"));

    chained.IsSuccess().Should().BeTrue();
    chained.GetSuccess().Should().Be("big: 10");
}
```

**Test akumulacji:**

```csharp
[Fact]
public void Composite_ShouldAccumulateSuccesses()
{
    var composite = Result<string, int>.Composite()
        .Accumulate(Result<string, int>.SuccessOf(1))
        .Accumulate(Result<string, int>.SuccessOf(2))
        .Accumulate(Result<string, int>.SuccessOf(3));

    composite.IsSuccess().Should().BeTrue();
    var listResult = composite.ToResult();
    listResult.GetSuccess().Should().BeEquivalentTo(new List<int> { 1, 2, 3 });
}
```

Dodatkowe klasy testów domeny obejmują walidację sumy kontrolnej GTIN-13, walidację formatu ISBN-10/13, walidację cyfry kontrolnej Luhn IMEI i zachowanie granic zakresu numerycznego.

### 3.11 Testy jednostkowe architektury

Konwencje architektoniczne, które nie są wymuszalne w czasie kompilacji, mogą wciąż być weryfikowane w czasie testów. Architektura warstwowa nic nie oznacza, jeśli każdy deweloper może dodać referencję z Domeny do Infrastruktury, a pipeline CI tego nie wyłapie. Testy jednostkowe architekury czynią te reguły wykonywalne.

Testy używają [NetArchTest.Rules](https://github.com/BenMorris/NetArchTest) -- biblioteki .NET inspirowanej [ArchUnit](https://www.archunit.org/), która pozwala napisać asercje dotyczące struktury i zależności zestawów.

**Wymuszanie zależności warstwy** (`DomainLayerTests.cs`):

```csharp
public class DomainLayerTests : TestBase
{
    [Fact]
    public void Domain_ShouldNotReference_ApplicationLayer()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn("Szlakomat.Products.Application")
            .GetResult();

        AssertArchTestResult(result);
    }

    [Fact]
    public void Domain_ShouldNotReference_InfrastructureLayer() { /* ... */ }

    [Fact]
    public void Domain_ShouldNotReference_ApiLayer() { /* ... */ }

    [Fact]
    public void Domain_ShouldNotReference_MediatR() { /* ... */ }
}
```

**Wymuszanie konwencji CQRS** (`CqrsConventionTests.cs`):

```csharp
public class CqrsConventionTests : TestBase
{
    [Fact]
    public void Handlers_ShouldBe_Sealed()
    {
        var handlers = ApplicationAssembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType
                && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            .ToList();

        foreach (var handler in handlers)
            handler.IsSealed.Should().BeTrue(
                $"{handler.Name} should be sealed to prevent inheritance");
    }

    [Fact]
    public void Handlers_ShouldReside_InSameNamespace_AsCommand() { /* ... */ }

    [Fact]
    public void Handlers_ShouldHave_SingleResponsibility() { /* ... */ }
}
```

Pełna lista kategorii testów architektury:

| Kategoria | Klasa Testów | Co wymusza |
|-----------|--------------|-----------|
| Zależności warstwy | `DomainLayerTests` | Domena nie może referencjonować Aplikacji/Infrastruktury/Api/MediatR |
| Zależności warstwy | `ApplicationLayerTests` | Aplikacja nie może referencjonować Infrastruktury/Api/ASP.NET Core |
| Zależności warstwy | `InfrastructureLayerTests` | Infrastruktura nie może referencjonować Api |
| Konwencje CQRS | `CqrsConventionTests` | Polecenia implementują `IRequest<>`, obsługi są sealed, pojedyncza odpowiedzialność, współlokalizacja |
| Niezmienność | `ValueObjectImmutabilityTests` | Obiekty wartości nie mają publicznych setterów |
| Widoczność | `DomainVisibilityTests` | Walidacja ograniczeń InternalsVisibleTo |
| Widoczność | `ApplicationVisibilityTests` | Reguły widoczności warstwy aplikacji |
| Widoczność | `InfrastructureVisibilityTests` | Reguły widoczności warstwy infrastruktury |
| Projekt domeny | `DomainDesignTests` | Konwencje projektowania domeny |
| Nazewnictwo | `NamingConventionTests` | Wymuszanie konwencji nazewnictwa |

### 3.12 Testy podróży (Journey Tests)

#### Definicja

Test podróży weryfikuje kompletny scenariusz biznesowy od początku do końca, ćwicząc cały stos aplikacji (polecenia, obsługi, repozytoria, logika domeny) poprzez realistyczną sekwencję operacji. W przeciwieństwie do testów jednostkowych, które weryfikują izolowane zachowania, testy podróży odpowiadają na pytanie: *czy ten przypadek użycia faktycznie działa end-to-end?*

#### Podejście

Kluczowe zasady testowania podróży w tym projekcie:

- **Rzeczywisty kontener DI** -- Każdy test tworzy pełny `IServiceProvider` poprzez `ServiceProviderFactory.Create()` ze wszystkimi rejestracjami produkcyjnymi (repozytoria w pamięci, obsługi MediatR, dane seed). Brak mock'ów, brak stubów, brak fakes.
- **Jeden test = jeden przypadek użycia** -- Każda metoda testowa reprezentuje konkretną podróż użytkownika: turysta rezerwujący wstęp czasowy, organizator organizujący wizytę grupową, pracownicy walidujący rezerwację na wejściu.
- **MediatR jako jedyny punkt wejścia** -- Testy interakcjonują z systemem wyłącznie poprzez `ISender.Send()`, ten sam interfejs, który używają kontrolery API. To zapewnia, że test ćwiczy tę samą ścieżkę kodu co produkcja.
- **Izolacja ograniczonego kontekstu** -- Każdy ograniczony kontekst ma własną klasę testów podróży, testującą własne scenariusze niezależnie.

Klasy testów podróży:

| Klasa | Kontekst | Scenariusz |
|-------|----------|-----------|
| `MuseumTicketCatalogJourneyTests` | Katalog | Definiuj różne typy atrakcji/biletów |
| `TicketOfferingJourneyTests` | Oferta Komercyjna | Dodaj produkty do katalogu, zaktualizuj metadane, wycofaj |
| `VisitorTicketPurchaseJourneyTests` | Instancje | Zarezerwuj wstęp czasowy, wizytę grupową, waliduj na wejściu |
| `TicketRelationshipJourneyTests` | Relacje | Definiuj relacje ulepszenia/zastępowania/uzupełniania |
| `CityTourismPlatformIntegrationTest` | Wszystkie | Kompletny scenariusz end-to-end across wszystkich ograniczonych kontekstów |

Infrastruktura pomocnicza:
- `ServiceProviderFactory` -- tworzy rzeczywisty kontener DI z `AddProductModule()`
- `CatalogCommandAssembler` -- fluent builder dla poleceń testowych (np. `TimedEntryAttraction`, `GroupAttraction`)

## 4. Technologia

- [.NET 10.0](https://dotnet.microsoft.com/) (platforma)
- [C#](https://learn.microsoft.com/en-us/dotnet/csharp/) (język)
- [ASP.NET Core 10.0](https://learn.microsoft.com/en-us/aspnet/core/) (framework web)
- [MediatR 12](https://github.com/jbogard/MediatR) (CQRS / mediator)
- [CommunityToolkit.Diagnostics 8.2.2](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/diagnostics/) (klauzule chroniące)
- [Microsoft.Extensions.DependencyInjection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) (dependency injection)
- [xUnit 2.x](https://xunit.net/) (framework testowania)
- [FluentAssertions 8.x](https://fluentassertions.com/) (asercje testowe)
- [NetArchTest.Rules 1.x](https://github.com/BenMorris/NetArchTest) (testy architektury)

## 5. Jak uruchomić

**Wymagania wstępne**: .NET 10 SDK

Wszystkie polecenia uruchamiaj z głównego katalogu repozytorium (gdzie znajduje się `source/Szlakomat.sln`):

```bash
# Zbuduj rozwiązanie
dotnet build source/Szlakomat.sln

# Uruchom wszystkie testy (jednostkowe domeny, podróż/integracyjne, architektura)
dotnet test source/Szlakomat.sln

# Uruchom pojedynczą klasę testów
dotnet test source/Szlakomat.sln --filter "FullyQualifiedName~ResultTests"

# Uruchom testy ze szczegółowym wyjściem
dotnet test source/Szlakomat.sln --logger "console;verbosity=detailed"

# Uruchom API (uruchamia się na porcie domyślnym)
dotnet run --project source/Szlakomat.Products.Api/Szlakomat.Products.Api.csproj
```

## 6. Struktura rozwiązania

```
source/
|-- Szlakomat.sln
|
|-- Szlakomat.Products.Domain/                      # Model domeny (brak zewnętrznych zależ.)
|   |-- Common/
|   |   |-- IProduct.cs                             # Komponent wzorca composite
|   |   |-- Result.cs                               # Railway-oriented Result<TF, TS>
|   |   |-- Identifiers/                            # Identyfikatory UUID, GTIN, ISBN
|   |   +-- Applicability/                          # Specyfikacje ograniczeń komponowalne
|   |-- Catalog/
|   |   |-- ProductType/                            # Agregat liści + ProductBuilder
|   |   |-- PackageType/                            # Agregat composite + PackageStructure
|   |   |-- Features/                               # ProductFeatureType
|   |   |-- FeatureConstraints/                     # IFeatureValueConstraint (AllowedValues, NumericRange, ...)
|   |   +-- SelectionRules/                         # ISelectionRule, reguły komponowalne
|   |-- CommercialOffer/                            # Agregat CatalogEntry
|   |-- Instances/                                  # ProductInstance, PackageInstance, InstanceBuilder
|   |-- Relationships/                              # ProductRelationship, polityki
|   +-- Quantity/                                   # Obiekty wartości Quantity, Unit
|
|-- Szlakomat.Products.Application/                 # Polecenia/zapytania CQRS via MediatR
|   |-- ProductModule.cs                            # Rejestracja MediatR
|   |-- Catalog/
|   |   |-- Common/                                 # Wspólne mappery i typy pomocnicze
|   |   |-- DefineProductType/
|   |   |-- DefinePackageType.cs
|   |   |-- FindProductType/
|   |   +-- FindByTrackingStrategy/
|   |-- CommercialOffer/
|   |-- Instances/
|   +-- Relationships/
|
|-- Szlakomat.Products.Infrastructure/              # Repozytoria w pamięci, setup DI, dane seed
|
|-- Szlakomat.Products.Api/                         # ASP.NET Core Web API
|   +-- Controllers/
|       |-- ProductsController.cs
|       |-- CatalogController.cs
|       |-- InstancesController.cs
|       +-- RelationshipsController.cs
|
|-- Szlakomat.Products.Domain.Tests/                # xUnit + FluentAssertions
|   +-- Domain/ValueObjects/
|       |-- ResultTests.cs
|       |-- GtinProductIdentifierTests.cs
|       |-- IsbnProductIdentifierTests.cs
|       |-- ImeiSerialNumberTests.cs
|       +-- NumericRangeConstraintTests.cs
|
|-- Szlakomat.Products.Application.Tests/           # Testy podróży/integracyjne
|   |-- Infrastructure/ServiceProviderFactory.cs
|   |-- Assemblers/CatalogCommandAssembler.cs
|   |-- Catalog/MuseumTicketCatalogJourneyTests.cs
|   |-- CommercialOffer/TicketOfferingJourneyTests.cs
|   |-- Instances/VisitorTicketPurchaseJourneyTests.cs
|   |-- Relationships/TicketRelationshipJourneyTests.cs
|   +-- Integration/CityTourismPlatformIntegrationTest.cs
|
+-- Szlakomat.Products.Architecture.Tests/          # Funkcje sprawdzające dopasowanie architektury
    |-- SeedWork/TestBase.cs
    |-- LayerDependency/
    |   |-- DomainLayerTests.cs
    |   |-- ApplicationLayerTests.cs
    |   +-- InfrastructureLayerTests.cs
    |-- CqrsPatterns/CqrsConventionTests.cs
    |-- Immutability/ValueObjectImmutabilityTests.cs
    |-- Visibility/
    |   |-- DomainVisibilityTests.cs
    |   |-- ApplicationVisibilityTests.cs
    |   +-- InfrastructureVisibilityTests.cs
    |-- DomainDesign/DomainDesignTests.cs
    +-- NamingConventions/NamingConventionTests.cs
```

## 7. Punkty końcowe API

| Kontroler | Metoda | Ścieżka | Opis |
|-----------|--------|--------|------|
| `ProductsController` | POST | `/api/products` | Definiuj nowy typ produktu |
| `ProductsController` | GET | `/api/products/{id}` | Pobierz typ produktu po identyfikatorze |
| `ProductsController` | GET | `/api/products?trackingStrategy=...` | Wylistuj po strategii śledzenia |
| `CatalogController` | POST | `/api/catalog` | Dodaj produkt do oferty komercyjnej |
| `CatalogController` | PUT | `/api/catalog/{id}` | Zaktualizuj metadane wpisu katalogowego |
| `CatalogController` | GET | `/api/catalog` | Przeszukaj/wylistuj wpisy katalogowe |
| `CatalogController` | GET | `/api/catalog/available` | Pobierz aktualnie dostępne wpisy |
| `CatalogController` | DELETE | `/api/catalog/{id}` | Wycofaj wpis katalogowy |
| `InstancesController` | POST | `/api/instances` | Utwórz instancję produktu |
| `InstancesController` | POST | `/api/instances/package` | Utwórz instancję pakietu |
| `InstancesController` | GET | `/api/instances/{id}` | Pobierz instancję po ID |
| `RelationshipsController` | POST | `/api/relationships` | Definiuj relację (wymaga `fromIdentifierType` + `toIdentifierType`: `UUID`/`ISBN`/`GTIN`/`INSPIRE`) |
| `RelationshipsController` | GET | `/api/relationships?from=...` | Pobierz relacje od produktu (zwraca `ProductRelationshipView`) |
| `RelationshipsController` | DELETE | `/api/relationships/{id}` | Usuń relację |

**Przykład żądania** -- definiuj typ produktu:

```bash
curl -X POST http://localhost:5000/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "productIdType": "UUID",
    "productId": "550e8400-e29b-41d4-a716-446655440000",
    "name": "Bilet wstępu do Zamku Wawelu",
    "description": "Standardowy wstęp dorosłych do Królewskiego Zamku na Wawelu",
    "unit": "szt",
    "trackingStrategy": "INDIVIDUALLY_TRACKED",
    "metadata": {
      "category": "atrakcja",
      "location": "Kraków"
    }
  }'
```

**Przykład żądania** -- definiuj relację między produktami identyfikowanymi przez INSPIRE (rejestr NID):

```bash
curl -X POST http://localhost:5000/api/relationships \
  -H "Content-Type: application/json" \
  -d '{
    "fromIdentifierType": "INSPIRE",
    "fromProductId": "PL.1.9.ZIPOZ.NID_N_12_BK.217616",
    "toIdentifierType": "INSPIRE",
    "toProductId": "PL.1.9.ZIPOZ.NID_N_12_KS.217617",
    "relationshipType": "COMPLEMENTED_BY"
  }'
```

## 8. Licencja

Ten projekt jest licencjonowany na warunkach licencji **Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International (CC BY-NC-SA 4.0)**.

Jesteś wolny do korzystania z tego materiału do celów edukacyjnych i projektów niekomercyjnych, pod warunkiem odpowiedniego przypisania autorstwa i rozpowszechniania wszelkich adaptacji na warunkach tej samej licencji.

Patrz plik [LICENSE](LICENSE) dla pełnego tekstu licencji.

## 9. Podziękowania

Szczególne podziękowania i uznanie:

- **Bartłomiej Słota**, **Jakub Pilimon** i **Sławomir Sobótka**
- **[Kamil Grzybek](https://github.com/kgrzybek)**
