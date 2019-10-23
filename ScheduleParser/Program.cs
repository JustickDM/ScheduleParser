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
			{"следующая", BotCommandType.NextWeek},
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
		private static string GROUP = "7";

		private static DateTime _dateTime;
		private static int _userId = 173605099;
		private static bool _isFirstWeek = true;
		private static int _nextWeekStartIndex = 0;
		private static DataTable _firstWeekSchedule;
		private static DataTable _secondWeekSchedule;

		static void Main(string[] args)
		{
			_dateTime = new DateTime(2019, 10, 23, 10, 44, 0);

			while(true)
			{
				var command = Console.ReadLine();

				var result = Work(command);

				Console.WriteLine(result);
			}
		}

		private static void PrintWeek(DataTable dataTable)
		{
			for(var i = 0; i < dataTable.Columns.Count; i++)
			{
				for(var j = 0; j < dataTable.Rows.Count; j++)
				{
					var dataRow = dataTable.Rows[j][i].ToString();

					Console.WriteLine(dataRow);
				}
			}
		}

		public static string Work(string command)
		{
			var sb = new StringBuilder();
			var result = string.Empty;

			command = command.ToLowerInvariant().Trim();			

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
					DataTable currentWeekSchedule, nextWeekSchedule;

					var commandType = _commands[command];
					var nodeCollection = GetNodes(user.Faculty, user.Course.ToString(), user.Group);			

					_firstWeekSchedule = GetFirstWeek(nodeCollection);
					_secondWeekSchedule = GetSecondWeek(nodeCollection);

					currentWeekSchedule = _isFirstWeek ? _firstWeekSchedule : _secondWeekSchedule;
					nextWeekSchedule = !_isFirstWeek ? _firstWeekSchedule : _secondWeekSchedule;

					switch(commandType)
					{
						case BotCommandType.Monday:
							result = GetSelectedDayOfWeekSchedule(commandType, commandType); //+
							break;
						case BotCommandType.Tuesday:
							result = GetSelectedDayOfWeekSchedule(commandType, commandType); //+
							break;
						case BotCommandType.Wednesday:
							result = GetSelectedDayOfWeekSchedule(commandType, commandType); //+
							break;
						case BotCommandType.Thursday:
							result = GetSelectedDayOfWeekSchedule(commandType, commandType); //+
							break;
						case BotCommandType.Friday:
							result = GetSelectedDayOfWeekSchedule(commandType, commandType); //+
							break;
						case BotCommandType.Saturday:
							result = GetSelectedDayOfWeekSchedule(commandType, commandType); //+
							break;
						case BotCommandType.Sunday:
							result = GetSelectedDayOfWeekSchedule(commandType, commandType); //+
							break;
						case BotCommandType.Now:
							result = GetNowSchedule(currentWeekSchedule, commandType);
							break;
						case BotCommandType.Today:
							result = GetTodaySchedule(currentWeekSchedule, commandType);
							break;
						case BotCommandType.Tomorrow:
							result = GetTomorrowSchedule(currentWeekSchedule, commandType);
							break;
						case BotCommandType.CurrentWeek:
							result = GetWeekSchedule(currentWeekSchedule, commandType); //+
							break;
						case BotCommandType.NextWeek:
							result = GetWeekSchedule(nextWeekSchedule, commandType); //+
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
					sb.AppendLine($"Команды: Пн, Вт, Ср, Чт, Пт, Сб, Вс, Сейчас, Сегодня, Завтра, Текущая, Следующая");
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

		private static string GetSelectedDayOfWeekSchedule(BotCommandType selectedCommandType, BotCommandType writedCommandType)
		{
			var sb = new StringBuilder();
			var columnOfDay = (int)selectedCommandType;

			if(selectedCommandType == writedCommandType)
			{
				if(columnOfDay < (int)BotCommandType.Sunday)
				{
					var shortCaption = selectedCommandType.GetDescription();
					var dayOfFirstWeek = GetDayOfOfWeekSchedule(_firstWeekSchedule, selectedCommandType);
					var dayOfSecondWeek = GetDayOfOfWeekSchedule(_secondWeekSchedule, selectedCommandType);

					sb.AppendLine($"--------{shortCaption}, Первая--------");
					sb.AppendLine(dayOfFirstWeek);
					sb.AppendLine($"--------{shortCaption}, Вторая--------");
					sb.AppendLine(dayOfSecondWeek);
				}
				else
				{
					sb.AppendLine($"{BotCommandType.Sunday.GetDescription()} - единственный день для отдыха:)");
				}
			}
			else
			{
				DataTable currentWeekSchedule;

				switch(writedCommandType)
				{
					case BotCommandType.Today:
						if(columnOfDay != (int)BotCommandType.Sunday)
						{
							currentWeekSchedule = _isFirstWeek ? _firstWeekSchedule : _secondWeekSchedule;

							var shortCaption = selectedCommandType.GetDescription();
							var dayOfCurrentWeek = GetDayOfOfWeekSchedule(currentWeekSchedule, selectedCommandType);

							sb.AppendLine($"--------{shortCaption}--------");
							sb.AppendLine(dayOfCurrentWeek);
						}
						else
						{
							sb.AppendLine($"{BotCommandType.Sunday.GetDescription()} - единственный день для отдыха:)");
						}
						break;
					case BotCommandType.Tomorrow:
						{
							if(columnOfDay == (int)BotCommandType.Monday)
							{
								currentWeekSchedule = !_isFirstWeek ? _firstWeekSchedule : _secondWeekSchedule;
							}
							else
							{
								currentWeekSchedule = _isFirstWeek ? _firstWeekSchedule : _secondWeekSchedule;
							}

							var shortCaption = selectedCommandType.GetDescription();
							var tomorrowDay = GetDayOfOfWeekSchedule(currentWeekSchedule, selectedCommandType);

							sb.AppendLine($"--------{shortCaption}--------");
							sb.AppendLine(tomorrowDay);
						}					
						break;
				}
			}

			return sb.ToString();
		}

		private static string GetDayOfOfWeekSchedule(DataTable dataTable, BotCommandType selectedCommandType)
		{
			var sb = new StringBuilder();
			var columnOfDay = (int)selectedCommandType;
			var count = 0;

			for(var i = 0; i < dataTable.Rows.Count; i++)
			{
				var time = dataTable.Rows[i][0].ToString().Insert(5, " - ");
				var lesson = dataTable.Rows[i][columnOfDay].ToString();

				if(!string.IsNullOrWhiteSpace(lesson))
				{
					sb.AppendLine($"{time} {lesson}");
					count++;
				}
			}

			if(count == 0) sb.AppendLine($"Урааа, выходной!:)");

			return sb.ToString();
		}

		private static string GetNowSchedule(DataTable dataTable, BotCommandType commandType)
		{
			var sb = new StringBuilder();
			var dayOfWeek = _dateTime.DayOfWeek.GetDescription().ToLowerInvariant();
			var day = _dayOfWeek[dayOfWeek];
			var command = _commands[day];
			var columnOfDay = (int)command;

			if(columnOfDay != (int)BotCommandType.Sunday)
			{
				var shortCaption = dataTable.Columns[columnOfDay].Caption;
				var IsHaveLesson = false;

				sb.AppendLine($"--------{shortCaption}--------");

				for(var j = 0; j < dataTable.Rows.Count; j++)
				{
					var time = dataTable.Rows[j][0].ToString().Insert(5, "|");
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

				if(!IsHaveLesson) sb.AppendLine($"В данный момент пары нет:)");
			}
			else
			{
				sb.AppendLine($"Сегодня воскресенье, а это значит, что сейчас пар нет и можно весь день тусииить!:)");
				sb.AppendLine($"Или делать домашку на следующую неделю:D");
			}

			return sb.ToString();
		}

		private static string GetTodaySchedule(DataTable dataTable, BotCommandType commandType)
		{
			var today = _dateTime.DayOfWeek.GetDescription().ToLowerInvariant();
			var todayDayOfweek = _dayOfWeek[today];
			var todayCommand = _commands[todayDayOfweek];

			var result = GetSelectedDayOfWeekSchedule(todayCommand, commandType);

			return result;
		}

		private static string GetTomorrowSchedule(DataTable dataTable, BotCommandType commandType)
		{
			var tomorrow = _dateTime.AddDays(1);
			var tomorrowDescription = tomorrow.DayOfWeek.GetDescription().ToLowerInvariant();
			var tomorrowDayOfWeek = _dayOfWeek[tomorrowDescription];
			var tomorrowCommand = _commands[tomorrowDayOfWeek];

			var result = GetSelectedDayOfWeekSchedule(tomorrowCommand, commandType);

			return result;
		}

		private static string GetWeekSchedule(DataTable dataTable, BotCommandType commandType)
		{
			var sb = new StringBuilder();
			var title = commandType.GetDescription();
			var rowsCount = dataTable.Rows.Count;
			var rowStart = 0;
			var rowFinish = rowsCount;

			sb.AppendLine($"{title}");
			sb.AppendLine(string.Empty);

			for(var i = 1; i < dataTable.Columns.Count; i++)
			{
				var shortCaption = dataTable.Columns[i].Caption;
				var count = 0;

				sb.AppendLine($"--------{shortCaption}--------");

				for(var j = rowStart; j < rowFinish; j++)
				{
					var time = dataTable.Rows[j][0].ToString().Insert(5, " - ");
					var lesson = dataTable.Rows[j][i].ToString();

					if(string.IsNullOrWhiteSpace(lesson)) continue;

					sb.AppendLine($"{time} {lesson}");
					count++;
				}

				if(count == 0)sb.AppendLine($"Урааа, выходной!:)");
				if(i < dataTable.Columns.Count - 1) sb.AppendLine(string.Empty);
			}

			return sb.ToString();
		}

		#region Получение таблиц с расписанием, их парсинг и нормализация

		private static HtmlNodeCollection GetNodes(string faculty, string course, string group)
		{
			var encoding = Encoding.GetEncoding("windows-1251");
			var web = new HtmlWeb() { OverrideEncoding = encoding };
			var htmlDocument = web.Load($"http://schedule.npi-tu.ru/schedule/{faculty}/{course}/{group}");
			var nodeCollection = htmlDocument.DocumentNode.SelectNodes("//table/tr");

			return nodeCollection;
		}

		private static DataTable GetFirstWeek(HtmlNodeCollection nodes)
		{
			var firstWeekTable = new DataTable("firstWeekTable");

			for(var i = 0; i < nodes.Count; i++)
			{
				var node = nodes[i];
				var childNodes = node.ChildNodes;
				var firstWeekRow = firstWeekTable.NewRow();

				for(var j = 0; j < childNodes.Count; j++)
				{
					var childNode = childNodes[j];
					var text = childNode.InnerText.Trim().Replace("\n", string.Empty).Replace("\t", string.Empty);

					if(i == 0)
					{
						firstWeekTable.Columns.Add(text);
					}
					else
					{
						firstWeekRow[j] = text;
					}
				}

				if(!IsNullRow(firstWeekRow))
				{
					if(IsNextWeek(firstWeekRow))
					{
						_nextWeekStartIndex = i;
						break;
					}

					firstWeekTable.Rows.Add(firstWeekRow);
				}

			}

			firstWeekTable = NormaliseTable(firstWeekTable);

			return firstWeekTable;
		}

		private static DataTable GetSecondWeek(HtmlNodeCollection nodes)
		{
			var secondWeekTable = new DataTable("secondWeekTable");

			for(var i = _nextWeekStartIndex; i < nodes.Count; i++)
			{
				var node = nodes[i];
				var childNodes = node.ChildNodes;
				var secondWeekRow = secondWeekTable.NewRow();

				for(var j = 0; j < childNodes.Count; j++)
				{
					var childNode = childNodes[j];
					var text = childNode.InnerText.Trim().Replace("\n", string.Empty).Replace("\t", string.Empty);

					if(i == _nextWeekStartIndex)
					{
						secondWeekTable.Columns.Add(text);
					}
					else
					{
						secondWeekRow[j] = text;
					}
				}

				if(!IsNullRow(secondWeekRow))
				{
					secondWeekTable.Rows.Add(secondWeekRow);
				}

			}

			secondWeekTable = NormaliseTable(secondWeekTable);

			return secondWeekTable;
		}

		private static bool IsNullRow(DataRow dataRow)
		{
			var arrayLength = dataRow.ItemArray.Length;
			var countNullObjects = 0;

			foreach(var obj in dataRow.ItemArray)
				if(string.IsNullOrWhiteSpace(obj.ToString())) countNullObjects++;

			return countNullObjects == arrayLength;
		}

		private static bool IsNextWeek(DataRow dataRow)
		{
			return string.IsNullOrWhiteSpace(dataRow[1].ToString());
		}

		private static DataTable NormaliseTable(DataTable dataTable)
		{
			var columnCount = dataTable.Columns.Count;
			var rowCount = dataTable.Rows.Count;

			var countNullObjectsInFirstColumn = 0;

			for(var i = 0; i < dataTable.Rows.Count; i++)
				if(string.IsNullOrWhiteSpace(dataTable.Rows[i][0].ToString())) countNullObjectsInFirstColumn++;

			if(countNullObjectsInFirstColumn == rowCount)
			{
				var removeColumn = dataTable.Columns[0];
				dataTable.Columns.Remove(removeColumn);

				columnCount = dataTable.Columns.Count;
			}

			return dataTable;
		}

		#endregion
	}
}
