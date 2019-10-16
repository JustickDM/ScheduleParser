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
		private static Dictionary<string, int> _days = new Dictionary<string, int>(6)
		{
			{"пн", 1},
			{"вт", 2},
			{"ср", 3},
			{"чт", 4},
			{"пт", 5},
			{"сб", 6},
		};

		static void Main(string[] args)
        {
            var encoding = Encoding.GetEncoding("windows-1251");
            var web = new HtmlWeb() { OverrideEncoding = encoding };
            var html = web.Load("http://schedule.npi-tu.ru/schedule/fitu/1/42m");

			//var table = html.GetElementbyId("table_week_active");

            var nodes = html.DocumentNode.SelectNodes("//table/tr");
			var dataTable = GetTable(nodes);

			dataTable = NormaliseTable(dataTable);
			
            if (nodes != null)
            {
                Console.WriteLine("Таблица с расписанием скачана");

				var column = _days["вт"];

				var caption = dataTable.Columns[column].Caption;
				Console.WriteLine(caption);

				for(var j = 0; j < dataTable.Rows.Count; j++)
				{
					var lesson = dataTable.Rows[j][column].ToString();
					var time = dataTable.Rows[j][0].ToString();
					Console.WriteLine($"{time} {lesson}");
				}
            }
            else
            {
                Console.WriteLine("Нет таблицы с расписанием");
            }
			Console.ReadKey();
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
