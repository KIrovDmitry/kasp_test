для запуска сервиса (проверки кода) желательно открыть его через visual studio community.
для этого на странице выбора проекта можно выбрать "открыть репозиторий", где выбрать сам проект, выбрать его и будет виден код и простой запуск.

в другом случае можно использовать свои способы (весь проект написан на vsc и его легко портировать в другие среды)

В СЛУЧАЕ ОТСУТСВИЯ ВОЗМОЖНОСТИ ОТКРЫТИЯ ЧЕРЕЗ visual studio community
если на компьютере установлен .NET 8 SDK откройте терминал и перейдите в папку с проектом (где лежит файл .csproj).
Введите:
dotnet restore
dotnet run
далее войдите в swagger в любом браузере, обычно они находятся по такому адресу:
https://localhost:7001/swagger или http://localhost:5000/swagger

далее приложу примеры запросов:
--------------POST /api/Vulnerabilities-------------
{
"name": "SQL Injection",
"severity": "Critical"
}
-------------GET /api/Vulnerabilities/{id}----------
ввод id из предыдущего запроса
-------------POST /api/ScanTasks--------------------
{
"repositoryUrl": "https://gitlab.com/example/my-project"
}
-------------GET /api/ScanTasks/{id}----------------
ввод id из POST /api/ScanTasks
-------------DELETE /api/ScanTasks/{id}-------------
ввод id из POST /api/ScanTasks
----------------------------------------------------

файлы: 
Kirov_DA_test/
│
├── Kirov_DA_test.csproj               
├── Program.cs                         ← главный файл
├── appsettings.json
├── README.md
│
├── Models/                            ← Доменные модели
│   ├── Vulnerability.cs               ← Модель уязвимости
│   └── ScanTask.cs                    ← Модель задачи сканирования
│
├── Services/                          ← логика и хранилища
│   ├── IVulnerabilityStore.cs         ← Интерфейс хранилища уязвимостей
│   ├── VulnerabilityStore.cs          ← Реализация
│   ├── IScanTaskStore.cs              ← Интерфейс хранилища задач
│   ├── ScanTaskStore.cs               ← Реализация
│   └── ScanBackgroundService.cs       ← Фоновый сервис сканирования
│
├── Controllers/                       ← эндпоинты
│   ├── VulnerabilitiesController.cs   ← Эндпоинты для уязвимостей
│   └── ScanTasksController.cs         ← Эндпоинты для задач сканирования
│
└── Middleware/                        ← Промежуточное ПО
    ├── RequestLoggingMiddleware.cs    ← Логирование запросов
    └── ExceptionHandlingMiddleware.cs ← Глобальная обработка ошибок

Kirov_DA_test.Tests/                   ← Проект Unit-тестов
│
├── Kirov_DA_test.Tests.csproj                
├── VulnerabilitiesControllerTests.cs  ← Unit-тесты Модели уязвимости
└── ScanTasksControllerTests.cs        ← Unit-тесты Модели задачи сканирования



Проект содержит Unit-тесты проверяющие:
- Успешное создание уязвимости (201 Created)
- Получение существующей уязвимости (200 OK)
- Обработку отсутствия данных (404 Not Found)
- Получение статуса задачи сканирования (200 OK, 404 Not Found)