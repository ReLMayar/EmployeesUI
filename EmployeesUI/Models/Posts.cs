using System;

namespace EmployeesUI
{
    public class Posts
    {
        private int id { get; set; }
        private string name { get; set; }
        private int id_deps { get; set; }

        public int Id
        {
            get => id;
            set
            {
                if (value != default)
                {
                    id = value;
                }
                else { throw new ArgumentNullException(nameof(value), "Id cannot be zero!"); }
            }
        }

        public string Name
        {
            get => name;
            set
            {
                if (value != null)
                {
                    name = value;
                }
                else { throw new ArgumentNullException(nameof(value), "Cannot be null!"); }
            }
        }

        public int Id_deps
        {
            get => id_deps;
            set
            {
                if (value != default)
                {
                    id_deps = value;
                }
                else { throw new ArgumentNullException(nameof(value), "Id cannot be zero!"); }
            }
        }
    }
}
