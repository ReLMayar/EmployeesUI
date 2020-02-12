using System;

namespace EmployeesUI
{
    public class Status
    {
        private int id { get; set; }
        private string name { get; set; }

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
    }
}
