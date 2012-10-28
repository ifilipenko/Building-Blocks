﻿#language: ru-RU
Функционал: хранилище учетных записей пользователей

Контекст:
Пусть существуют пользователи
	| имя     | email               |
	| Сидоров | Сидоров@outlook.com |
	| Петров  | Петров@mail.ru      |
	| Иванов  | Иванов@gmail.com    |

Структура сценария: проверка существования пользователя по имени
	Когда проверяют что пользователь с именем "<имя>" существует
	Тогда результат проверки признает что пользователь "<существует>"
Примеры: 
	| имя      | существует    |
	| Иванов   | существует    |
	| Петров   | существует    |
	| Сидоров  | существует    |
	| Медведев | не существует |

Структура сценария: проверка существования пользователя по email
	Когда проверяют что пользователь с email "<email>" существует
	Тогда результат проверки признает что пользователь "<существует>"
Примеры: 
	| email               | существует    |
	| Иванов@gmail.com    | существует    |
	| Петров@mail.ru      | существует    |
	| Сидоров@outlook.com | существует    |
	| Иванов@mail.ru      | не существует |

Сценарий: поиск пользователей по списку имен
	Когда получают список пользователей содержащих имена
	| имя    |
	| Путин  |
	| Иванов |
	| Петров |
	Тогда возвращается следующий список пользователей
	| имя    |
	| Иванов |
	| Петров |

Сценарий: поиск пользователя по email
	Когда ищут пользователя с email "Сидоров@outlook.com"
	Тогда возвращается следующий список пользователей
	| имя     |
	| Сидоров |

Сценарий: поиск пользователя по не существующему email
	Когда ищут пользователя с email "Сидоров@outlook.net"
	Тогда не возвращается ни одного пользователя

Сценарий: поиск пользователя по Id
	Пусть пользователь "Сидоров" имеет Id "94BB18FA-5198-4445-88A0-4475A7A715FC"
	Когда ищут пользователя с Id "94BB18FA-5198-4445-88A0-4475A7A715FC"
	Тогда возвращается следующий список пользователей
	| имя     |
	| Сидоров |

Сценарий: поиск пользователя по не существующему Id
	Пусть пользователь "Сидоров" имеет Id "94BB18FA-5198-4445-88A0-4475A7A715FC"
	Когда ищут пользователя с Id "6D4DA51A-FCF3-4343-B63D-E20468F90B0B"
	Тогда не возвращается ни одного пользователя

Структура сценария: постраничная загрузка пользователей соответствующих части email
	Пусть существуют пользователи
	| имя       | email            |
	| Медведев  | Сидоров@kreml.uc |
	| Медведева | Петров@kreml.uc  |
	| Путин     | Путин@kreml.uc   |
	| Кабаева   | Кабаева@kreml.uc |
	| Сабчак    | Сабчак@mail.ru   |
	Когда загружают 1 страницу пользоватлей по 3 пользователя с фильтром по email "<фильтр>"
	Тогда возвращается страница пользователей
	| всего   | на странице   |
	| <всего> | <на странице> |
Примеры: 
	| фильтр   | всего | на странице |
	| kreml.uc | 4     | 3           |
	|          | 8     | 3           |

Структура сценария: постраничная загрузка пользователей соответствующих части email и имени приложения
	Пусть существуют пользователи
	| имя       | email              | приложение |
	| Медведев  | Сидоров@kreml.uc   | Ru         |
	| Медведева | Петров@kreml.uc    | Ru         |
	| Путин     | Путин@kreml.uc     | Ru         |
	| Кабаева   | Кабаева@kreml.uc   | Ru         |
	| Кабаев    | Кабаева@yanndex.uc | Ru         |
	| Сабчак    | Сабчак@kreml.ru    | Yandex     |
	Когда загружают 1 страницу пользоватлей по 3 пользователя, по приложению "<приложение>" и части email "<фильтр>"
	Тогда возвращается страница пользователей
	| всего   | на странице   |
	| <всего> | <на странице> |
Примеры: 
	| фильтр   | приложение | всего | на странице |
	| kreml.uc | Ru         | 4     | 3           |
	|          | Ru         | 5     | 3           |
	|          | Yandex     | 1     | 1           |
	| kreml    | Yandex     | 1     | 1           |
	| kreml.uc | Yandex     | 0     | 0           |

Сценарий: постраничная загрузка отсортированных по имени пользователей пользователей соответствующих части email
	Пусть существуют пользователи
	| имя       | email            |
	| Медведев  | Сидоров@kreml.uc |
	| Медведева | Петров@kreml.uc  |
	| Путин     | Путин@kreml.uc   |
	| Кабаева   | Кабаева@kreml.uc |
	| Сабчак    | Сабчак@mail.ru   |
	Когда загружают 1 страницу пользоватлей по 3 пользователя с фильтром по email "kreml.uc"
	Тогда возвращается страница с пользователями
	| имя       |
	| Кабаева   |
	| Медведев  |
	| Медведева |

Структура сценария: постраничная загрузка пользователей соответствующих части имени
	Пусть существуют пользователи
	| имя       |
	| Медведев  |
	| Медведева |
	| Медвед    |
	| Медведица |
	| Сабчак    |
	Когда загружают 1 страницу пользоватлей по 3 пользователя с фильтром по имени "<фильтр>"
	Тогда возвращается страница пользователей
	| всего   | на странице   |
	| <всего> | <на странице> |
Примеры: 
	| фильтр | всего | на странице |
	| едве   | 4     | 3           |
	|        | 8     | 3           |

Структура сценария: постраничная загрузка пользователей соответствующих приложению и части имени
	Пусть существуют пользователи
	| имя       | приложение |
	| Медведев  | Ru         |
	| Медведева | Ru         |
	| Медвед    | Ru         |
	| Медведица | Ru         |
	| Сабчак    | Yandex     |
	Когда загружают 1 страницу пользоватлей по 3 пользователя, по приложению "<приложение>" и части имени "<фильтр>"
	Тогда возвращается страница пользователей
	| всего   | на странице   |
	| <всего> | <на странице> |
Примеры: 
	| фильтр | приложение | всего | на странице |
	| едве   | Ru         | 4     | 3           |
	| едве   | Yandex     | 0     | 0           |
	|        | Ru         | 4     | 3           |
	|        | Yandex     | 1     | 1           |

Сценарий: постраничная загрузка отсортированных по имени пользователей соответствующих части имени
	Пусть существуют пользователи
	| имя       |
	| Медведев  |
	| Медведева |
	| Медвед    |
	| Медведица |
	| Сабчак    |
	Когда загружают 1 страницу пользоватлей по 3 пользователя с фильтром по имени "едве"
	Тогда возвращается страница с пользователями
	| имя       |
	| Медвед    |
	| Медведев  |
	| Медведева |

Сценарий: постраничная загрузка пользователей
	Пусть существуют пользователи
	| имя       |
	| Медведев  |
	| Медведева |
	| Медвед    |
	| Медведица |
	| Сабчак    |
	Когда загружают 1 страницу пользоватлей по 3 пользователя
	Тогда возвращается страница пользователей
	| всего | на странице |
	| 8     | 3           |

Сценарий: постраничная загрузка отсортированных по имени пользователей
	Пусть существуют пользователи
	| имя       |
	| Медведев  |
	| Медведева |
	| Путин     |
	| Кабаева   |
	| Сабчак    |
	Когда загружают 1 страницу пользоватлей по 3 пользователя
	Тогда возвращается страница с пользователями
	| имя      |
	| Иванов   |
	| Кабаева  |
	| Медведев |

Сценарий: количество пользователей с последней активностью не меньше указанной даты
	Пусть задана активность пользователей
	| имя     | активность |
	| Сидоров | 12.02.2012 |
	| Иванов  | 30.10.2012 |
	| Петров  | 29.10.2012 |
	Когда получают количество пользователей с последней активностью от 28.10.2012
	Тогда количество пользователей равно 2

Сценарий: создание пользователя
	Когда создают нового пользователя "Медведев"
	Тогда существует 4 пользователя

Сценарий: создание пользователя со списом ролей
	Пусть существуют роли
	| имя       |
	| Врач      |
	| Медсестра |
	| Админ     |
	Когда создают нового пользователя "Медведев" с назначенными ролями
	| роль      |
	| Медсестра |
	| Админ     |
	Тогда существует роль "Админ" со списком пользователей
	| имя      |
	| Медведев |
	И существует роль "Медсестра" со списком пользователей
	| имя      |
	| Медведев |
	И существует пользователь "Медведев" со списком ролей
	| роль      |
	| Медсестра |
	| Админ     |

Сценарий: обновление пользователя
	Пусть существуют пользователи
	| имя       |
	| Медведев  |
	Пусть пользователь "Медведев" имеет Id "C6AFF080-2B78-453A-B841-F37E91B36483"
	Когда для пользователя с Id "C6AFF080-2B78-453A-B841-F37E91B36483" обновляют поля
	| поле                                    | значение                             |
	| Email                                   | Медведев@mail.ru                     |
	| ApplicationName                         | Unknown                              |
	| Имя                                     | Медвед                               |
	| Пароль                                  | 7F664E0F-177E-4582-B057-23546388052D |
	| Комментарий                             | Бла бла                              |
	| ConfirmationToken                       | 123a                                 |
	| CreateDate                              | 12.02.2012                           |
	| IsApproved                              | true                                 |
	| IsLockedOut                             | false                                |
	| LastActivityDate                        | 25.10.2012                           |
	| LastLockoutDate                         | 12.06.2012                           |
	| LastLoginDate                           | 25.10.2012                           |
	| LastPasswordChangedDate                 | 13.06.2012                           |
	| LastPasswordFailureDate                 | 11.06.2012                           |
	| PasswordFailuresSinceLastSuccess        | 2                                    |
	| PasswordVerificationToken               | 456a                                 |
	| PasswordVerificationTokenExpirationDate | 26.10.2012                           |
	Тогда пользователь с Id "C6AFF080-2B78-453A-B841-F37E91B36483" имеет следующие поля
	| поле                                    | значение                             |
	| Email                                   | Медведев@mail.ru                     |
	| Имя                                     | Медвед                               |
	| ApplicationName                         | Unknown                              |
	| Пароль                                  | 7F664E0F-177E-4582-B057-23546388052D |
	| Комментарий                             | Бла бла                              |
	| ConfirmationToken                       | 123a                                 |
	| CreateDate                              | 12.02.2012                           |
	| IsApproved                              | true                                 |
	| IsLockedOut                             | false                                |
	| LastActivityDate                        | 25.10.2012                           |
	| LastLockoutDate                         | 12.06.2012                           |
	| LastLoginDate                           | 25.10.2012                           |
	| LastPasswordChangedDate                 | 13.06.2012                           |
	| LastPasswordFailureDate                 | 11.06.2012                           |
	| PasswordFailuresSinceLastSuccess        | 2                                    |
	| PasswordVerificationToken               | 456a                                 |
	| PasswordVerificationTokenExpirationDate | 26.10.2012                           |

Сценарий: обновление ролей пользователя
	Пусть существуют роли
	| имя       |
	| Врач      |
	| Медсестра |
	| Админ     |
	Когда создают нового пользователя "Медведев" с назначенными ролями
	| роль      |
	| Медсестра |
	| Админ     |
	Когда пользователю "Медведев" меняют назначение ролей
	| роль  |
	| Админ |
	| Врач  |
	Тогда существует роль "Админ" со списком пользователей
	| имя      |
	| Медведев |
	И существует пользователь "Медведев" со списком ролей
	| роль  |
	| Админ |
	| Врач  |
	Тогда существует роль "Врач" со списком пользователей
	| имя      |
	| Медведев |	
	И существует роль "Медсестра" со списком пользователей
	| имя      |

Сценарий: удаление пользователя
	Пусть существуют роли
	| имя       |
	| Врач      |
	| Медсестра |
	| Админ     |
	Когда создают нового пользователя "Медведев" с назначенными ролями
	| роль      |
	| Медсестра |
	| Админ     |
	И удаляют пользователя "Медведев"
	Тогда существует 3 пользователя		
	Тогда существует роль "Врач" со списком пользователей
	| имя      |
	И существует роль "Медсестра" со списком пользователей
	| имя      |