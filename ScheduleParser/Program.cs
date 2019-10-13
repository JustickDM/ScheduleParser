using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Data;
using System.Diagnostics;

namespace ScheduleParser
{
	class Program
	{
        static void Main(string[] args)
        {
            var encoding = Encoding.GetEncoding("windows-1251");
            var web = new HtmlWeb() { OverrideEncoding = encoding };
            var html = web.Load("http://schedule.npi-tu.ru/schedule/fitu/1/4ывыфв");
            var table = html.GetElementbyId("table_week_active");
            var dataTable = new DataTable("dataTable");
            var nodes = html.DocumentNode.SelectNodes("//table/tr");

            //nodes[0]
            //.Elements("th")
            //.Select(th => th.InnerText.Trim())
            //.ToList()
            //.ForEach(header => table.Columns.Add(header));

            //nodes
            //.Skip(1)
            //.Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToArray())
            //.ToList()
            //.ForEach(row => table.Rows.Add(row));

            if (table != null)
            {
                File.WriteAllText("schedule.txt", table.InnerText);
                Console.WriteLine("Таблица с расписанием скачана");
            }
            else
            {
                Console.WriteLine("Нет таблицы с расписанием");
            }
			Console.ReadKey();
		}
	}
}
