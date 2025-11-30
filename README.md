# BeFit - Aplikacja do Śledzenia Treningów

Aplikacja ASP.NET Core MVC do zarządzania sesjami treningowymi i śledzenia postępów.

## Funkcjonalności

### Typy ćwiczeń
- Przeglądanie dostępnych typów ćwiczeń (publicznie)
- Zarządzanie typami ćwiczeń (tylko administrator)

### Sesje treningowe
- Tworzenie, edycja i usuwanie sesji treningowych
- Automatyczne przypisanie do zalogowanego użytkownika
- Pełna izolacja danych między użytkownikami

### Ćwiczenia w sesji
- Dodawanie ćwiczeń do sesji treningowych
- Śledzenie obciążenia, serii i powtórzeń
- Listy wyboru z czytelnymi nazwami

### Statystyki
- Podsumowanie z ostatnich 4 tygodni
- Liczba wykonań każdego ćwiczenia
- Łączna liczba powtórzeń
- Średnie i maksymalne obciążenie

## Wymagania

- .NET 8.0
- SQL Server

## Uruchomienie

1. Sklonuj repozytorium
2. Zaktualizuj connection string w `appsettings.json` jeśli potrzebne
3. Uruchom migracje: `dotnet ef database update`
4. Uruchom aplikację: `dotnet run`

## Dane logowania administratora

- **Email:** kielpinski@admin.pl
- **Hasło:** Haslo123!

## Technologie

- ASP.NET Core 8.0 MVC
- Entity Framework Core
- ASP.NET Core Identity
- Bootstrap 5
- SQL Server

