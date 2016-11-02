Проект выполнен в Visual Studio 2015.
Проект имеет трехуровневую архитектур:
 - Core: описание сущностей
 - BusinessLogic: логика взаимодействия с БД (описаны: репозитории, контекст, датаменеджер)
 - Web: реализация Web-приложения.

Возможности данного проекта:
- ввод монет в автомат
- выбор доступных напитков по указанной стоимости
- получение сдачи в виде монет конкретного номинала
- сброс ввода монет
Администрирование:
- создание/редактирование/удаление/блокирование монет
- создание/редактирование/удаление напитков
- доступ в админзону по секретному ключу
- импорт напитков в файле .xml

Все необязательные требования выполнены:
•	При возврате сдачи показывать количество и номинал монет. 
•	Возможность импорта напитков.

Внедрение зависимостей осуществляется при помощи Ninject.

Контроллеры:
MVC: HomeController, AdminController
WebApi: BaseApiController, CoinController, DrinkController, UploadController

MyHelpers - htmlHelper для отображения сдачи

Доступ в административный интерфейс осуществляется по секретному ключу, зашитому в web.config.
<add key="Admin" value="adminpassword"/>
Передается в качестве параметра в адресной строке:
http:\localhost:****\admin?adminpassword

Scripts:
indexHome.js для \Home\Index
indexAdmin.js для \Admin\Index
manageCoin.js для PartialView _ManageCoin  (модальное окно)
manageDrink.js для PartialView _ManageDrink  (модальное окно)
change.js для PartialView _Change  (модальное окно)

БД имеет формат .mdf. Начальные данные присутствуют. Подключение по AttachDbFilename.
При создании БД использовал Migrations.

Стек технологий:
C#, ASP.NET MVC WebApi, EF, Ninject, JavaScript, jQuery, AJAX, Bootstrap