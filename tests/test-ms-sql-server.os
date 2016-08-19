#Использовать asserts

Перем юТест;

Процедура Инициализация()
	ПодключитьВнешнююКомпоненту(КаталогПрограммы()+"\ext\sql\sql.dll");
КонецПроцедуры

Функция ПолучитьСписокТестов(Тестирование) Экспорт

	юТест = Тестирование;

	СписокТестов = Новый Массив;
	СписокТестов.Добавить("Тест_Должен_СоздатьТаблицу");
	СписокТестов.Добавить("Тест_Должен_ДобавитьСтроки");
	СписокТестов.Добавить("Тест_Должен_ДолженИзменитьСтроки");
	СписокТестов.Добавить("Тест_Должен_ДолженПолучитьВыборку");

	Возврат СписокТестов;

КонецФункции

Процедура Тест_Должен_СоздатьТаблицу() Экспорт
	
	Соединение = Новый Соединение();
	Соединение.ТипСУБД = Соединение.ТипыСУБД.MSSQLServer;
	Соединение.Сервер = "FIN91\SQLEXPRESS";
	Соединение.ИмяБазы = "test";
	Соединение.Открыть();

	ЗапросВставка = Новый Запрос();
	ЗапросВставка.УстановитьСоединение(Соединение);
	ЗапросВставка.Текст = "Create table #users (id integer, name varchar(50))";
	ЗапросВставка.ВыполнитьКоманду();
	
	Соединение.Закрыть();

КонецПроцедуры

Процедура Тест_Должен_ДобавитьСтроки() Экспорт
	
	Соединение = Новый Соединение();
	Соединение.ТипСУБД = Соединение.ТипыСУБД.MSSQLServer;
	Соединение.Сервер = "FIN91\SQLEXPRESS";
	Соединение.ИмяБазы = "test";
	Соединение.Открыть();

	ЗапросВставка = Новый Запрос();
	ЗапросВставка.УстановитьСоединение(Соединение);
	ЗапросВставка.Текст = "Create table #users (id integer, name varchar(50))";
	ЗапросВставка.ВыполнитьКоманду();

	ЗапросВставка.Текст = "insert into #users (name) values(@name)";
	ЗапросВставка.УстановитьПараметр("name", "Сергей");
	Результат = ЗапросВставка.ВыполнитьКоманду();

	Соединение.Закрыть();

	Ожидаем.Что(Результат).Равно(1);

КонецПроцедуры

Процедура Тест_Должен_ДолженИзменитьСтроки() Экспорт
	
	Соединение = Новый Соединение();
	Соединение.ТипСУБД = Соединение.ТипыСУБД.MSSQLServer;
	Соединение.Сервер = "FIN91\SQLEXPRESS";
	Соединение.ИмяБазы = "test";
	Соединение.Открыть();

	ЗапросВставка = Новый Запрос();
	ЗапросВставка.УстановитьСоединение(Соединение);
	ЗапросВставка.Текст = "Create table #users (id integer, name varchar(50))";
	ЗапросВставка.ВыполнитьКоманду();

	ЗапросВставка.Текст = "insert into #users (id, name) values(1, @name)";
	ЗапросВставка.УстановитьПараметр("name", "Сергей");
	ЗапросВставка.ВыполнитьКоманду();

	ЗапросВставка.Текст = "update #users set name = @name";
	ЗапросВставка.УстановитьПараметр("name", "Сергей Александрович");
	Результат = ЗапросВставка.ВыполнитьКоманду();

	Соединение.Закрыть();

	Ожидаем.Что(Результат).Равно(1);

КонецПроцедуры


Процедура Тест_Должен_ДолженПолучитьВыборку() Экспорт

	Соединение = Новый Соединение();
	Соединение.ТипСУБД = Соединение.ТипыСУБД.MSSQLServer;
	Соединение.Сервер = "FIN91\SQLEXPRESS";
	Соединение.ИмяБазы = "test";
	Соединение.Открыть();

	ЗапросВставка = Новый Запрос();
	ЗапросВставка.УстановитьСоединение(Соединение);
	ЗапросВставка.Текст = "Create table #users (id integer, name varchar(50))";
	ЗапросВставка.ВыполнитьКоманду();

	ЗапросВставка.Текст = "insert into #users (name) values(@name)";
	ЗапросВставка.УстановитьПараметр("name", "Сергей");
	ЗапросВставка.ВыполнитьКоманду();

	ЗапросВставка.Параметры.Очистить();
	ЗапросВставка.Текст = "select * from #users";
	ТЗ = ЗапросВставка.Выполнить().Выгрузить();

	Ожидаем.Что(ТЗ.Количество()).Равно(1);

	Соединение.Закрыть();

КонецПроцедуры

//////////////////////////////////////////////////////////////////////////////////////
// Инициализация

Инициализация();
