#Использовать asserts

Перем юТест;

Функция ПолучитьТекстИзФайла(ИмяФайла)
    ФайлОбмена = Новый Файл(ИмяФайла);
    Данные = "";
    Если ФайлОбмена.Существует() Тогда
        Текст = Новый ЧтениеТекста(ИмяФайла, КодировкаТекста.UTF8);
        Данные = Текст.Прочитать();
        Текст.Закрыть();
    Иначе
        Возврат Ложь;
    КонецЕсли;
    возврат Данные;
КонецФункции

Процедура Инициализация()
	ПодключитьВнешнююКомпоненту(КаталогПрограммы()+"\ext\sql\sql.dll");
КонецПроцедуры

Функция ПолучитьСписокТестов(Тестирование) Экспорт

	СуфикМетодов = "";
	СтрокаСоединения = ПолучитьТекстИзФайла("fixtures\ms-sql-server-con-str.txt");
	Если СтрокаСоединения = Ложь Тогда
		СуфикМетодов = "НетПараметровСоединения";
	КонецЕсли;


	юТест = Тестирование;

	СписокТестов = Новый Массив;
	СписокТестов.Добавить("Тест_Должен_СоздатьТаблицу" + СуфикМетодов);
	СписокТестов.Добавить("Тест_Должен_ДобавитьСтроки" + СуфикМетодов);
	СписокТестов.Добавить("Тест_Должен_ДолженИзменитьСтроки" + СуфикМетодов);
	СписокТестов.Добавить("Тест_Должен_ДолженПолучитьВыборку" + СуфикМетодов);

	Возврат СписокТестов;

КонецФункции

Процедура Тест_Должен_СоздатьТаблицу() Экспорт
	
	СтрокаСоединения = ПолучитьТекстИзФайла("fixtures\ms-sql-server-con-str.txt");

	Соединение = Новый Соединение();
	Соединение.ТипСУБД = Соединение.ТипыСУБД.MSSQLServer;
	//Соединение.СтрокаСоединения = СтрокаСоединения;
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
	
	СтрокаСоединения = ПолучитьТекстИзФайла("fixtures\ms-sql-server-con-str.txt");

	Соединение = Новый Соединение();
	Соединение.ТипСУБД = Соединение.ТипыСУБД.MSSQLServer;
	Соединение.СтрокаСоединения = СтрокаСоединения;
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
	
	СтрокаСоединения = ПолучитьТекстИзФайла("fixtures\ms-sql-server-con-str.txt");

	Соединение = Новый Соединение();
	Соединение.ТипСУБД = Соединение.ТипыСУБД.MSSQLServer;
	Соединение.СтрокаСоединения = СтрокаСоединения;
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

	СтрокаСоединения = ПолучитьТекстИзФайла("fixtures\ms-sql-server-con-str.txt");

	Соединение = Новый Соединение();
	Соединение.ТипСУБД = Соединение.ТипыСУБД.MSSQLServer;
	Соединение.СтрокаСоединения = СтрокаСоединения;
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
