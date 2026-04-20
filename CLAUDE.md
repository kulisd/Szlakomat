# CLAUDE.md

Ten plik zawiera wytyczne dla Claude Code (claude.ai/code) podczas pracy z kodem w tym repozytorium.

## Materiały referencyjne

Przed zmianami przeczytaj pełny kontekst projektu — CLAUDE.md zawiera wyłącznie wytyczne specyficzne dla pracy AI, opisy domeny, architektury, wzorców i strategii testowej znajdują się w README.

- **[README.md](README.md)** — kompletna dokumentacja projektu:
  - §2 Domena — opis modelu turystyki miejskiej (produkty, pakiety, instancje, relacje)
  - §3.1–3.3 Architektura warstwowa, bounded contexty, mapa kontekstu
  - §3.4 CQRS przez MediatR
  - §3.5–3.9 Zasady modelu domeny, railway-oriented, composite, fluent builders, specyfikacje komponowalne
  - §3.10–3.12 Strategia testowa (jednostkowe, architektoniczne, journey)
  - §6 Struktura rozwiązania, §7 Endpointy API
- **[diagrams/](diagrams/)** — wizualne diagramy architektury (HTML):
  - `layers.html` — warstwy i kierunek zależności
  - `bounded_contexts.html` — bounded contexty i ich zależności
  - `request_flow.html` — przepływ requestu od kontrolera przez MediatR do repozytorium

## Komendy budowania i testowania

Wszystkie komendy `dotnet` operują na `source/Szlakomat.sln`. Katalog tego pliku CLAUDE.md zawiera podkatalog `source/`.

```bash
# Budowanie
dotnet build source/Szlakomat.sln

# Wszystkie testy (domenowe jednostkowe + journey + architektoniczne)
dotnet test source/Szlakomat.sln

# Pojedyncza klasa testowa
dotnet test source/Szlakomat.sln --filter "FullyQualifiedName~ResultTests"

# Pojedyncza metoda testowa
dotnet test source/Szlakomat.sln --filter "FullyQualifiedName~ResultTests.Map_OnSuccess_ShouldTransformValue"

# Tylko jeden z trzech projektów testowych
dotnet test source/Szlakomat.Products.Domain.Tests/Szlakomat.Products.Domain.Tests.csproj
dotnet test source/Szlakomat.Products.Application.Tests/Szlakomat.Products.Application.Tests.csproj
dotnet test source/Szlakomat.Products.Architecture.Tests/Szlakomat.Products.Architecture.Tests.csproj
```

## Twarde ograniczenia wymuszane przez fitness functions

Reguły w `Szlakomat.Products.Architecture.Tests` odrzucą każdą zmianę, która je łamie — traktuj je jako blokery, nie sugestie. Szczegóły merytoryczne są w README §3.1 (warstwy), §3.4 (CQRS), §3.5 (model domeny); poniżej praktyczna ściągawka, gdzie co jest egzekwowane:

- **Kierunek zależności warstw** (`LayerDependency/`) — Domain ⟵ Application ⟵ Infrastructure ⟵ Api. Domain nie widzi MediatR ani ASP.NET Core. Application nie widzi Infrastructure ani Api.
- **Konwencje CQRS** (`CqrsPatterns/CqrsConventionTests.cs`) — komenda/zapytanie to `IRequest<TResponse>`; handler jest `sealed`, siedzi w tej samej przestrzeni nazw co komenda, ma jedną odpowiedzialność. Każdy `IRequest<T>` musi mieć zarejestrowany `IRequestHandler<T, _>` w assembly (test `EveryRequest_ShouldHave_RegisteredHandler` — sierocej komendy nie przepuści).
- **Niezmienność value objectów** (`Immutability/`) — zero publicznych setterów.
- **Widoczność** (`Visibility/`) — implementacje agregatów (`ProductType`, `PackageType`, `ProductBuilder`, `InstanceBuilder`, …) są `internal`; publiczny interfejs to interfejsy + value objecty. Konstruktory agregatów i builderów (typy kończące się na `Type`/`Builder`) nie mogą być `public` — wymusza test `AggregatesAndBuilders_ShouldHave_NoPublicConstructors` (buduj przez statyczne fabryki). Dostęp międzyprojektowy przez `InternalsVisibleTo` (lista w `Szlakomat.Products.Domain.csproj`).
- **Nazewnictwo** (`NamingConventions/`) — wymuszone wzorce nazw w poszczególnych warstwach.

## Wskazówki dla zmian

- **Dodajesz feature**: napisz test jednostkowy domenowy dla inwariantów + journey test przez `IMediator` dla przepływu end-to-end. Jeśli wprowadzasz nową przekrojową konwencję — dopisz też test architektoniczny.
- **Nowe reguły walidacji**: komponuj z istniejących `IApplicabilityConstraint` / `ISelectionRule` (And/Or/Not/IfThen/…) zamiast tworzyć równoległe abstrakcje.
- **Błędy biznesowe**: zwracaj `Result.FailureOf(...)`, nie rzucaj wyjątkami. Kontrolery dopasowują wynik przez `IsSuccess()` / `GetFailure()`.
- **Nowy agregat**: statyczne metody fabrykujące wyrażające intencję (wzór: `ProductType.Define`, `ProductType.IndividuallyTracked`), nie publiczne konstruktory.
- **Composite**: jeśli dodajesz coś do `IProduct` albo `IInstance`, zachowaj symetrię między leaf a composite.

## Aktualizacja dokumentacji

**Każda zmiana w kodzie, która wpływa na dokumentowane elementy, musi iść w parze z aktualizacją dokumentacji w tym samym commicie.** Zmiana nie jest ukończona, dopóki dokumentacja nie odzwierciedla nowego stanu.

Zakres obowiązku:

- **[README.md](README.md)** — aktualizuj, gdy zmieniasz:
  - model domeny, bounded contexty, relacje między agregatami (§2, §3.2, §3.3)
  - warstwy, kierunek zależności, wzorce architektoniczne (§3.1, §3.4–3.9)
  - strategię testową lub strukturę projektów testowych (§3.10–3.12)
  - strukturę rozwiązania — nowe/usunięte projekty, przeniesione katalogi (§6)
  - endpointy API — nowe, usunięte lub zmienione trasy/metody HTTP (§7)
  - komendy budowania/uruchamiania (§5)
- **[diagrams/](diagrams/)** — regeneruj lub aktualizuj, gdy zmieniasz:
  - warstwy albo zależności między nimi → `layers.html`
  - bounded contexty lub ich relacje → `bounded_contexts.html`
  - ścieżkę requestu (kontroler → MediatR → handler → repozytorium) → `request_flow.html`
- **[CLAUDE.md](CLAUDE.md)** — aktualizuj, gdy zmieniasz:
  - komendy budowania/testowania
  - reguły fitness functions (nowa kategoria testów architektonicznych, zmieniona lokalizacja)
  - wzorce domenowe, które Claude ma respektować przy zmianach

Jeśli zmiana unieważnia treść dokumentacji (np. usunięty agregat opisany w README, przeniesiony endpoint), popraw dokumentację **przed** commitem. Nie zostawiaj nieaktualnych odniesień.
