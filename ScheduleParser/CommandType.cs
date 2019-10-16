using System.ComponentModel;

namespace ScheduleParser
{
	enum CommandType
	{
		/// <summary>
		/// Понедельник
		/// </summary>
		[Description("Понедельник")]
		Monday = 1,

		/// <summary>
		/// Вторник
		/// </summary>
		[Description("Вторник")]
		Tuesday = 2,

		/// <summary>
		/// Среда
		/// </summary>
		[Description("Среда")]
		Wednesday = 3,

		/// <summary>
		/// Четверг
		/// </summary>
		[Description("Четверг")]
		Thursday = 4,

		/// <summary>
		/// Пятница
		/// </summary>
		[Description("Пятница")]
		Friday = 5,

		/// <summary>
		/// Суббота
		/// </summary>
		[Description("Суббота")]
		Saturday = 6,

		/// <summary>
		/// Воскресенье
		/// </summary>
		[Description("Воскресенье")]
		Sunday = 7,

		/// <summary>
		/// Старт
		/// </summary>
		[Description("Старт")]
		Start = 8,

		/// <summary>
		/// Регистрация
		/// </summary>
		[Description("Регистрация")]
		Registration = 9,

		/// <summary>
		/// Сегодня
		/// </summary>
		[Description("Сегодня")]
		Today = 10,

		/// <summary>
		/// Завтра
		/// </summary>
		[Description("Завтра")]
		Tomorrow = 11,

		/// <summary>
		/// Текущая неделя
		/// </summary>
		[Description("Текущая неделя")]
		CurrentWeek = 12,

		/// <summary>
		/// Следующая неделя
		/// </summary>
		[Description("Следующая неделя")]
		NextWeek = 13,
		
	}
}
