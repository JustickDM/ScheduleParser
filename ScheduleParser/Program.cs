using HtmlAgilityPack;

using Schedule.Models.Entities;

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ScheduleParser
{
	class Program
	{
		private static Dictionary<string, BotCommandType> _commands = new Dictionary<string, BotCommandType>
		{
			{"пн", BotCommandType.Monday},
			{"вт", BotCommandType.Tuesday},
			{"ср", BotCommandType.Wednesday},
			{"чт", BotCommandType.Thursday},
			{"пт", BotCommandType.Friday},
			{"сб", BotCommandType.Saturday},
			{"вс", BotCommandType.Sunday},
			{"сейчас", BotCommandType.Now },
			{"сегодня", BotCommandType.Today},
			{"завтра", BotCommandType.Tomorrow},
			{"текущая", BotCommandType.CurrentWeek},
		};

		private static Dictionary<string, string> _dayOfWeek = new Dictionary<string, string>
		{
			{"monday", "пн"},
			{"tuesday", "вт"},
			{"wednesday", "ср"},
			{"thursday", "чт"},
			{"friday", "пт"},
			{"saturday", "сб"},
			{"sunday", "вс"}
		};

		private static string FACULTY = "fitu";
		private static int COURSE = 1;
		private static string GROUP = "4v";

		private static DateTime _dateTime;
		private static int _userId = 173605099;

		static void Main(string[] args)
		{
			_dateTime = DateTime.Now.AddDays(3);

			while(true)
			{
				var command = Console.ReadLine();

				var result = Work(command);

				Console.WriteLine(result);
			}
		}

		public static string Work(string command)
		{
			command = command.ToLowerInvariant().Trim();

			var sb = new StringBuilder();
			var result = string.Empty;

			var user = new User()
			{
				Faculty = FACULTY,
				Course = COURSE,
				Group = GROUP
			};

			//using(var db = new DatabaseContext())
			//{
			//	user = db.Users.FirstOrDefault(u => u.UserId == _userId);
			//}

			if(user != null)
			{
				var isContainsKey = _commands.ContainsKey(command);

				if(isContainsKey)
				{
					var commandType = _commands[command];

					var nodeCollection = GetNodes(user.Faculty, user.Course.ToString(), user.Group);
					var currentDataTable = ParseTable(nodeCollection);

					switch(commandType)
					{
						case BotCommandType.Monday:
							result = GetDayOfWeekSchedule(currentDataTable, commandType, commandType);
							break;
						case BotCommandType.Tuesday:
							result = GetDayOfWeekSchedule(currentDataTable, commandType, commandType);
							break;
						case BotCommandType.Wednesday:
							result = GetDayOfWeekSchedule(currentDataTable, commandType, commandType);
							break;
						case BotCommandType.Thursday:
							result = GetDayOfWeekSchedule(currentDataTable, commandType, commandType);
							break;
						case BotCommandType.Friday:
							result = GetDayOfWeekSchedule(currentDataTable, commandType, commandType);
							break;
						case BotCommandType.Saturday:
							result = GetDayOfWeekSchedule(currentDataTable, commandType, commandType);
							break;
						case BotCommandType.Sunday:
							result = GetDayOfWeekSchedule(currentDataTable, commandType, commandType);
							break;
						case BotCommandType.CurrentWeek:
							result = GetCurrentWeekSchedule(currentDataTable, commandType);
							break;
						case BotCommandType.Today:
							result = GetTodaySchedule(currentDataTable, commandType);
							break;
						case BotCommandType.Tomorrow:
							result = GetTomorrowSchedule(currentDataTable, commandType);
							break;
						case BotCommandType.Now:
							result = GetNowSchedule(currentDataTable, commandType);
							break;
					}
				}
				else
				{
					result = command.Contains("регистрация")
						? Registration(command)
						: $"Я бы хотел с тобой поговорить на свободные темы, но умею только показывать расписание:(";
				}
			}
			else
			{
				result = command.Contains("регистрация")
					? Registration(command)
					: $"Давай ближе к делу, я не люблю общаться:)";
			}

			sb.AppendLine(result);

			return sb.ToString();
		}

		private static string Registration(string command)
		{
			var sb = new StringBuilder();
			var commandDescription = "регистрация";
			var userInfo = command.Replace(commandDescription, string.Empty).Trim().Replace(" ", string.Empty).Split(',');

			if(userInfo.Length == 3)
			{
				var faculty = userInfo[0];
				var course = userInfo[1];
				var group = userInfo[2];

				var nodeCollection = GetNodes(faculty, course, group);

				if(nodeCollection != null && nodeCollection.Count > 0)
				{
					//using(var db = new DatabaseContext())
					//{
					//var user = db.Users.FirstOrDefault(u => u.UserId == _userId);
					var user = new User()
					{
						Faculty = FACULTY,
						Course = COURSE,
						Group = GROUP
					};

					//if(user != null)
					//{
					//	db.Users.Remove(user);
					//}

					//user = new User()
					//{
					//	UserId = _userId,
					//	Faculty = faculty,
					//	Course = int.Parse(course),
					//	Group = group,
					//};

					//db.Users.Add(user);
					//db.SaveChanges();
					//}

					sb.AppendLine($"Регистрация прошла успешно, держи список активных команд:)");
					sb.AppendLine($"Команды: Пн, Вт, Ср, Чт, Пт, Сб, Вс, Текущая, Сегодня, Завтра, Сейчас");
				}
				else
				{
					sb.AppendLine($"Не могу найти нужное расписание, проверь правильность данных:)");
					sb.AppendLine($"Хотя...Может опять сайт с расписанием прилег отдохнуть:(");
					sb.AppendLine($"Но ты все равно проверь данные, на всякий случай:)");
				}
			}
			else
			{
				sb.AppendLine($"Введи правильно все данные:)");
				sb.AppendLine($"Например (писать вместе со словом регистрация): Регистрация fitu, 1, 42m");
			}

			return sb.ToString();
		}

		private static string GetDayOfWeekSchedule(DataTable dataTable, BotCommandType selectedCommandType, BotCommandType writedCommandType)
		{
			var sb = new StringBuilder();
			var columnOfDay = (int)selectedCommandType;

			var rowsCount = dataTable.Rows.Count;
			var isNotDayOfWeekCommand = false;

			if(selectedCommandType != writedCommandType)
			{
				switch(writedCommandType)
				{
					case BotCommandType.Today:
						rowsCount /= 2;
						isNotDayOfWeekCommand = true;
						break;
					case BotCommandType.Tomorrow:
						rowsCount /= 2;
						isNotDayOfWeekCommand = true;
						break;
					case BotCommandType.Now:
						rowsCount /= 2;
						isNotDayOfWeekCommand = true;
						break;
				}
			}		

			if(columnOfDay < (int)BotCommandType.Sunday)
			{
				var shortCaption = dataTable.Columns[columnOfDay].Caption;
				var count = 0;

				if(!isNotDayOfWeekCommand) sb.AppendLine($"--------{shortCaption}, Текущая--------");
				else sb.AppendLine($"--------{shortCaption}--------");

				for(var j = 0; j < rowsCount; j++)
				{
					var time = dataTable.Rows[j][0].ToString();
					if(string.IsNullOrWhiteSpace(time))
					{
						if(count == 0) sb.AppendLine($"Урааа, выходной!:)");
						count = 0;
						sb.AppendLine(string.Empty);
						if(!isNotDayOfWeekCommand) sb.AppendLine($"--------{shortCaption}, Следующая--------");
						else sb.AppendLine($"--------{shortCaption}--------");
						continue;
					}
					else
					{
						time = time.Insert(5, " - ");
					}

					var lesson = dataTable.Rows[j][columnOfDay].ToString();

					if(string.IsNullOrWhiteSpace(lesson) || string.IsNullOrWhiteSpace(time))
					{
						continue;
					}

					sb.AppendLine($"{time} {lesson}");
					count++;
				}

				if(count == 0) sb.AppendLine($"Урааа, выходной!:)");
			}
			else
			{
				sb.AppendLine($"{BotCommandType.Sunday.GetDescription()} - единственный день для отдыха:)");
			}

			return sb.ToString();
		}

		private static string GetCurrentWeekSchedule(DataTable dataTable, BotCommandType commandType)
		{
			var sb = new StringBuilder();
			var title = commandType.GetDescription();

			sb.AppendLine($"{title}");
			sb.AppendLine(string.Empty);

			for(var i = 1; i < dataTable.Columns.Count; i++)
			{
				var shortCaption = dataTable.Columns[i].Caption;
				var count = 0;

				sb.AppendLine($"--------{shortCaption}--------");

				for(var j = 0; j < dataTable.Rows.Count; j++)
				{
					var time = dataTable.Rows[j][0].ToString();
					if(string.IsNullOrWhiteSpace(time))
					{
						continue;
					}
					else
					{
						time = time.Insert(5, " - ");
					}

					var lesson = dataTable.Rows[j][i].ToString();

					if(string.IsNullOrWhiteSpace(lesson) || string.IsNullOrWhiteSpace(time))
					{
						continue;
					}

					sb.AppendLine($"{time} {lesson}");
					count++;
				}

				if(count == 0) sb.AppendLine($"Урааа, выходной!:)");
				if(i < dataTable.Columns.Count - 1) sb.AppendLine(string.Empty);
			}

			return sb.ToString();
		}

		private static string GetTodaySchedule(DataTable dataTable, BotCommandType commandType)
		{
			var today = _dateTime.DayOfWeek.GetDescription().ToLowerInvariant();
			var todayDayOfweek = _dayOfWeek[today];
			var todayCommand = _commands[todayDayOfweek];

			var result = GetDayOfWeekSchedule(dataTable, todayCommand, commandType);

			return result;
		}

		private static string GetTomorrowSchedule(DataTable dataTable, BotCommandType commandType)
		{
			var tomorrow = _dateTime.AddDays(1);
			var tomorrowDescription = tomorrow.DayOfWeek.GetDescription().ToLowerInvariant();
			var tomorrowDayOfWeek = _dayOfWeek[tomorrowDescription];
			var tomorrowCommand = _commands[tomorrowDayOfWeek];

			var result = GetDayOfWeekSchedule(dataTable, tomorrowCommand, commandType);

			return result;
		}

		private static string GetNowSchedule(DataTable dataTable, BotCommandType commandType)
		{
			var sb = new StringBuilder();
			var dayOfWeek = _dateTime.DayOfWeek.GetDescription().ToLowerInvariant();
			var day = _dayOfWeek[dayOfWeek];
			var command = _commands[day];
			var columnOfDay = (int)command;

			var shortCaption = dataTable.Columns[columnOfDay].Caption;
			var IsHaveLesson = false;

			sb.AppendLine($"--------{shortCaption}--------");

			for(var j = 0; j < dataTable.Rows.Count / 2; j++)
			{
				var time = dataTable.Rows[j][0].ToString();
				if(string.IsNullOrWhiteSpace(time))
				{
					continue;
				}
				else
				{
					time = time.Insert(5, "|");
				}

				var lesson = dataTable.Rows[j][columnOfDay].ToString();
				var times = time.Split('|');
				var timeStart = DateTime.Parse(times[0]);
				var timeFinish = DateTime.Parse(times[1]);

				if((timeStart <= _dateTime) && (_dateTime <= timeFinish))
				{
					if(!string.IsNullOrWhiteSpace(lesson))
					{
						time = time.Replace('|', ' ').Insert(5, " - ");
						sb.AppendLine($"{time} {lesson}");
						IsHaveLesson = true;
						break;
					}
					else
					{
						IsHaveLesson = false;
					}
				}
				else
				{
					IsHaveLesson = false;
				}
			}

			if(!IsHaveLesson)
			{
				sb.AppendLine($"В данный момент пары нет:)");
			}

			return sb.ToString();
		}

		#region Получение таблицы с расписание, её парсин и нормализация

		private static HtmlNodeCollection GetNodes(string faculty, string course, string group)
		{
			var encoding = Encoding.GetEncoding("windows-1251");
			var web = new HtmlWeb() { OverrideEncoding = encoding };
			var htmlDocument = web.Load($"http://schedule.npi-tu.ru/schedule/{faculty}/{course}/{group}");
			var nodeCollection = htmlDocument.DocumentNode.SelectNodes("//table/tr");

			//var table = html.GetElementbyId("table_week_active");

			return nodeCollection;
		}

		private static DataTable ParseTable(HtmlNodeCollection nodes)
		{
			var dataTable = new DataTable("dataTable");

			for(var i = 0; i < nodes.Count; i++)
			{
				var node = nodes[i];
				var childNodes = node.ChildNodes;
				var dataRow = dataTable.NewRow();

				for(var j = 0; j < childNodes.Count; j++)
				{
					var htmlChildNode = childNodes[j];
					var text = htmlChildNode.InnerText.Trim().Replace("\n", string.Empty).Replace("\t", string.Empty);

					if(i == 0)
					{
						dataTable.Columns.Add(text);
					}
					else
					{
						dataRow[j] = text;
					}
				}

				dataTable.Rows.Add(dataRow);
			}

			return NormaliseTable(dataTable);
		}

		private static DataTable NormaliseTable(DataTable dataTable)
		{
			var removeRow = dataTable.Rows[0];
			dataTable.Rows.Remove(removeRow);

			var removeColumn = dataTable.Columns[0];
			dataTable.Columns.Remove(removeColumn);

			return dataTable;
		}

		#endregion
	}
}
