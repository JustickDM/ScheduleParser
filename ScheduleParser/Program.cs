﻿using HtmlAgilityPack;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ScheduleParser
{
	class Program
	{
		private static Dictionary<string, CommandType> _commands = new Dictionary<string, CommandType>
		{
			{"пн", CommandType.Monday},
			{"вт", CommandType.Tuesday},
			{"ср", CommandType.Wednesday},
			{"чт", CommandType.Thursday},
			{"пт", CommandType.Friday},
			{"сб", CommandType.Saturday},
			{"вс", CommandType.Sunday},
			{"старт", CommandType.Start},
			{"регистрация", CommandType.Registration},
			{"сейчас", CommandType.Now },
			{"сегодня", CommandType.Today},
			{"завтра", CommandType.Tomorrow},
			{"текущая", CommandType.CurrentWeek},
			{"следующая", CommandType.NextWeek},
		};

		private static string FACULTY = "fitu";
		private static string COURSE = "1";
		private static string GROUP = "42m";

		static void Main(string[] args)
		{
			while (true)
			{
				var command = Console.ReadLine();

				var result = Bot(command);

				Console.WriteLine(result);
			}
		}

		private static string Bot(string command)
		{
			command = command.ToLowerInvariant();

			var sb = new StringBuilder();

			var isContainsKey = _commands.ContainsKey(command);

			if (isContainsKey)
			{
				var commandType = _commands[command];

				var nodeCollection = GetNodes(FACULTY, COURSE, GROUP);

				var currentDataTable = ParseTable(nodeCollection);
				currentDataTable = NormaliseTable(currentDataTable);

				var result = string.Empty;

				switch (commandType)
				{
					case CommandType.Start:
						result = Start();
						break;
					case CommandType.Registration:
						result = $"Введите необходимые данные:)";
						break;
					case CommandType.Now:
						result = $"Команда в разработке...";
						break;
					case CommandType.Today:
						result = $"Команда в разработке...";
						break;
					case CommandType.Tomorrow:
						result = $"Команда в разработке...";
						break;
					case CommandType.CurrentWeek:
						result = $"Команда в разработке...";
						break;
					case CommandType.NextWeek:
						result = $"Команда в разработке...";
						break;

					default:
						result = GetDayOfWeekSchedule(currentDataTable, commandType);
						break;
				}

				sb.AppendLine(result);
			}
			else
			{
				var commandDescription = CommandType.Registration.GetDescription().ToLowerInvariant();

				command = command.ToLowerInvariant();

				if (command.Contains(commandDescription))
				{
					var result = Registration(command);

					sb.AppendLine(result);
				}
				else
				{
					sb.AppendLine($"Давай ближе к делу, я не люблю общаться:)");
				}
			}

			return sb.ToString();
		}

		private static string Start()
		{
			var sb = new StringBuilder();

			sb.AppendLine($"Здравствуй, я бот для расписания нашего университета:)");
			sb.AppendLine($"Для дальнейшего взаимодействия требуется зарегистрироваться - напиши слово \"Регистрация\" (Без ковычек), а затем укажи название факультета, номер курса и название группы");
			sb.AppendLine($"Например: Регистрация fitu, 1, 42m");

			return sb.ToString();
		}

		private static string Registration(string command)
		{
			var sb = new StringBuilder();

			var commandDescription = CommandType.Registration.GetDescription().ToLowerInvariant();

			command = command.ToLowerInvariant();

			if (command.Contains(commandDescription))
			{
				var userInfo = command.Replace(commandDescription, string.Empty).Trim().Replace(" ", string.Empty).Split(',');

				if (userInfo.Length == 3)
				{
					FACULTY = userInfo[0];
					COURSE = userInfo[1];
					GROUP = userInfo[2];

					var nodeCollection = GetNodes(FACULTY, COURSE, GROUP);

					if (nodeCollection != null)
					{
						sb.AppendLine($"Регистрация прошла успешно, держи список активных команд:)");
						sb.AppendLine($"Получить расписание по дням недели: Пн, Вт, Ср, Чт, Пт, Сб, Вс");
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
					sb.AppendLine($"Введи все данные:)");
				}
			}
			else
			{
				sb.AppendLine($"Если хочешь пользоваться - напиши команду правильно:)");
			}

			return sb.ToString();
		}

		private static string GetDayOfWeekSchedule(DataTable dataTable, CommandType commandType)
		{
			var sb = new StringBuilder();
			var columnOfDay = (int)commandType;

			if (columnOfDay < (int)CommandType.Sunday)
			{
				var title = commandType.GetDescription();

				sb.AppendLine($"{title}");

				for (var j = 0; j < dataTable.Rows.Count; j++)
				{
					var time = dataTable.Rows[j][0].ToString().Insert(5, "-");
					var lesson = dataTable.Rows[j][columnOfDay].ToString();

					sb.AppendLine($"{time} {lesson}");
				}
			}
			else
			{
				sb.AppendLine($"{CommandType.Sunday.GetDescription()} - единственный день для отдыха:)");
			}

			return sb.ToString();
		}

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

			for (var i = 0; i < nodes.Count / 2; i++)
			{
				var node = nodes[i];
				var childNodes = node.ChildNodes;
				var dataRow = dataTable.NewRow();

				for (var j = 0; j < childNodes.Count; j++)
				{
					var htmlChildNode = childNodes[j];
					var text = htmlChildNode.InnerText.Trim().Replace("\n", string.Empty).Replace("\t", string.Empty);
					if (i == 0)
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

			return dataTable;
		}

		private static DataTable NormaliseTable(DataTable dataTable)
		{
			var removeRow = dataTable.Rows[0];
			dataTable.Rows.Remove(removeRow);

			var removeColumn = dataTable.Columns[0];
			dataTable.Columns.Remove(removeColumn);

			return dataTable;
		}
	}
}
