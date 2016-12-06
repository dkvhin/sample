using System.Collections.Generic;

namespace Sample.Data.Dao
{
    public class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
