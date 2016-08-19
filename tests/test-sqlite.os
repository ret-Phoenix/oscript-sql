#Использовать asserts

Перем юТест;

Процедура Инициализация()
	ПодключитьВнешнююКомпоненту(КаталогПрограммы()+"\ext\sql\sql.dll");
КонецПроцедуры

Функция ПолучитьСписокТестов(Тестирование) Экспорт

	юТест = Тестирование;

	СписокТестов = Новый Массив;
	СписокТестов.Добавить("Тест_Должен_СоздатьБД");
	СписокТестов.Добавить("Тест_Должен_СоздатьТаблицу");
	СписокТестов.Добавить("Тест_Должен_ДобавитьСтроки");
	СписокТестов.Добавить("Тест_Должен_ДолженИзменитьСтроки");
	СписокТестов.Добавить("Тест_Должен_ДолженПолучитьВыборку");

	СписокТестов.Добавить("Тест_Должен_СоздатьИнМемориБД");

	Возврат СписокТестов;

КонецФункции

Процедура Тест_Должен_СоздатьБД() Экспорт
	
	ФайлБД = Новый Файл("fixtures\test.sqlite");
	Если (ФайлБД.Существует()) Тогда
		УдалитьФайлы(ФайлБД.ПолноеИмя);
	КонецЕсли;

	Соединение = Новый Соединение();
	Соединение.ТипСУБД = Соединение.ТипыСУБД.sqlite;
	Соединение.ИмяБазы = "fixtures\test.sqlite";
	Соединение.Открыть();
	Соединение.Закрыть();

	Ожидаем.Что(ФайлБД.Существует()).ЭтоИстина();

	ФайлБД = Неопределено;

КонецПроцедуры

Процедура Тест_Должен_СоздатьТаблицу() Экспорт
	
	ФайлБД = Новый Файл("fixtures\test-create-table.sqlite");
	Если (ФайлБД.Существует()) Тогда
		УдалитьФайлы(ФайлБД.ПолноеИмя);
	КонецЕсли;

	Соединение = Новый Соединение();
	Соединение.ТипСУБД = Соединение.ТипыСУБД.sqlite;
	Соединение.ИмяБазы = ФайлБД.ПолноеИмя;
	Соединение.Открыть();

	ЗапросВставка = Новый Запрос();
	ЗапросВставка.УстановитьСоединение(Соединение);
	ЗапросВставка.Текст = "Create table users (id integer, name text)";
	ЗапросВставка.ВыполнитьКоманду();
	
	Соединение.Закрыть();

КонецПроцедуры

Процедура Тест_Должен_ДобавитьСтроки() Экспорт
	
	ФайлБД = Новый Файл("fixtures\test-table-add.sqlite");
	Если (ФайлБД.Существует()) Тогда
		УдалитьФайлы(ФайлБД.ПолноеИмя);
	КонецЕсли;

	Соединение = Новый Соединение();
	Соединение.ТипСУБД = Соединение.ТипыСУБД.sqlite;
	Соединение.ИмяБазы = ФайлБД.ПолноеИмя;
	Соединение.Открыть();

	ЗапросВставка = Новый Запрос();
	ЗапросВставка.УстановитьСоединение(Соединение);
	ЗапросВставка.Текст = "Create table users (id integer, name text)";
	ЗапросВставка.ВыполнитьКоманду();

	ЗапросВставка.Текст = "insert into users (name) values(@name)";
	ЗапросВставка.УстановитьПараметр("name", "Сергей");
	Результат = ЗапросВставка.ВыполнитьКоманду();

	Соединение.Закрыть();

	Ожидаем.Что(Результат).Равно(1);

КонецПроцедуры

Процедура Тест_Должен_ДолженИзменитьСтроки() Экспорт
	
	ФайлБД = Новый Файл("fixtures\test-table-edit.sqlite");
	Если (ФайлБД.Существует()) Тогда
		УдалитьФайлы(ФайлБД.ПолноеИмя);
	КонецЕсли;

	Соединение = Новый Соединение();
	Соединение.ТипСУБД = Соединение.ТипыСУБД.sqlite;
	Соединение.ИмяБазы = ФайлБД.ПолноеИмя;
	Соединение.Открыть();

	ЗапросВставка = Новый Запрос();
	ЗапросВставка.УстановитьСоединение(Соединение);
	ЗапросВставка.Текст = "Create table users (id integer, name text)";
	ЗапросВставка.ВыполнитьКоманду();

	ЗапросВставка.Текст = "insert into users (name) values(@name)";
	ЗапросВставка.УстановитьПараметр("name", "Сергей");
	ЗапросВставка.ВыполнитьКоманду();

	ЗапросВставка.Текст = "update  users set name = @name";
	ЗапросВставка.УстановитьПараметр("name", "Сергей Александрович");
	Результат = ЗапросВставка.ВыполнитьКоманду();

	Соединение.Закрыть();

	Ожидаем.Что(Результат).Равно(1);

КонецПроцедуры


Процедура Тест_Должен_ДолженПолучитьВыборку() Экспорт

	ФайлБД = Новый Файл("fixtures\test-table-select.sqlite");
	Если (ФайлБД.Существует()) Тогда
		УдалитьФайлы(ФайлБД.ПолноеИмя);
	КонецЕсли;

	Соединение = Новый Соединение();
	Соединение.ТипСУБД = Соединение.ТипыСУБД.sqlite;
	Соединение.ИмяБазы = ФайлБД.ПолноеИмя;
	Соединение.Открыть();

	ЗапросВставка = Новый Запрос();
	ЗапросВставка.УстановитьСоединение(Соединение);
	ЗапросВставка.Текст = "Create table users (id integer, name text)";
	ЗапросВставка.ВыполнитьКоманду();

	ЗапросВставка.Текст = "insert into users (name) values(@name)";
	ЗапросВставка.УстановитьПараметр("name", "Сергей");
	ЗапросВставка.ВыполнитьКоманду();

	ЗапросВставка.Текст = "select * from users";
	ТЗ = ЗапросВставка.Выполнить().Выгрузить();

	Ожидаем.Что(ТЗ.Количество()).Равно(1);

	Соединение.Закрыть();

КонецПроцедуры

Процедура Тест_Должен_СоздатьИнМемориБД() Экспорт

	Соединение = Новый Соединение();
	Соединение.ТипСУБД = Соединение.ТипыСУБД.sqlite;
	Соединение.ИмяБазы = ":memory:";
	Соединение.Открыть();

	ЗапросВставка = Новый Запрос();
	ЗапросВставка.УстановитьСоединение(Соединение);
	ЗапросВставка.Текст = "Create table users (id integer, name text)";
	ЗапросВставка.ВыполнитьКоманду();

	ЗапросВставка.Текст = "insert into users (name) values(@name)";
	ЗапросВставка.УстановитьПараметр("name", "Сергей");
	ЗапросВставка.ВыполнитьКоманду();

	ЗапросВставка.Текст = "select * from users";
	ТЗ = ЗапросВставка.Выполнить().Выгрузить();

	Ожидаем.Что(ТЗ.Количество()).Равно(1);
	Ожидаем.Что(ТЗ[0][1]).Равно("Сергей");

	Соединение.Закрыть();


КонецПроцедуры

//////////////////////////////////////////////////////////////////////////////////////
// Инициализация

Инициализация();
