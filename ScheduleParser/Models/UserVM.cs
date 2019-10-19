using Schedule.Models.Entities;

namespace Schedule.Models
{
	public class UserVM
    {
		public int Id { get; set; }
		public int UserId { get; set; }
		public string Faculty { get; set; }
		public int Course { get; set; }
		public string Group { get; set; }

		public static implicit operator UserVM(User dbUser)
        {
            return new UserVM()
            {
                Id = dbUser.Id,
				UserId = dbUser.UserId,
				Faculty = dbUser.Faculty,
				Course = dbUser.Course,
				Group = dbUser.Group
            };
        }
    }
}