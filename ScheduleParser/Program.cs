using HtmlAgilityPack;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ScheduleParser
{
	class Program
	{
		private static Dictionary<string, int> _days = new Dictionary<string, int>
		{
			{"понедельник", 1},
			{"вторник", 2},
			{"среда", 3},
			{"четверг", 4},
			{"пятница", 5},
			{"суббота", 6},
		};

		private static Dictionary<string, CommandType> _commands = new Dictionary<string, CommandType>
		{
			{"сегодня", CommandType.Today},
			{"завтра", CommandType.Tomorrow},
			{"текущая неделя", CommandType.CurrentWeek},
		};

		static void Main(string[] args)
        {
            var encoding = Encoding.GetEncoding("windows-1251");
            var web = new HtmlWeb() { OverrideEncoding = encoding };
            var html = web.Load("http://schedule.npi-tu.ru/schedule/fitu/1/42m");

			//var table = html.GetElementbyId("table_week_active");

			var day = DateTime.Now.DayOfWeek; //EN to RU

			var nodes = html.DocumentNode.SelectNodes("//table/tr");
			var dataTable = GetTable(nodes);

			dataTable = NormaliseTable(dataTable);
			
            if (nodes != null)
            {
                Console.WriteLine("Таблица с расписанием скачана");			
            }
            else
            {
                Console.WriteLine("Нет таблицы с расписанием");
            }

			while(true)
			{
				var selectedDay = Console.ReadLine();
				var schedule = GetSelectedDaySchedule(dataTable, selectedDay);

				Console.WriteLine(schedule);
			}
		}

		private static string GetSelectedDaySchedule(DataTable dataTable, string day)
		{
			var sb = new StringBuilder();

			var column = _days[day.ToLowerInvariant()];

			for(var j = 0; j < dataTable.Rows.Count; j++)
			{
				var lesson = dataTable.Rows[j][column].ToString();
				var time = dataTable.Rows[j][0].ToString().Insert(5, "-");
				sb.AppendLine($"{time} {lesson}");
			}

			return sb.ToString();
		}

		private static DataTable GetTable(HtmlNodeCollection htmlNodes)
		{
			var dataTable = new DataTable("dataTable");

			for(var i = 0; i < htmlNodes.Count / 2; i++)
			{
				var htmlNode = htmlNodes[i];
				var htmlChildNodes = htmlNode.ChildNodes;
				var dataRow = dataTable.NewRow();

				for(var j = 0; j < htmlChildNodes.Count; j++)
				{
					var htmlChildNode = htmlChildNodes[j];
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
