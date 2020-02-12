using System;

namespace EmployeesUI
{
    public class Person
    {
        private string first_name { get; set; }
        private string second_name { get; set; }
        private string last_name { get; set; }
        private string status_name { get; set; }
        private string deps_name { get; set; }
        private string posts_name { get; set; }

        public string First_name
        {
            get => first_name;
            set
            {
                if (value != null)
                {
                    first_name = value;
                }
                else { throw new ArgumentNullException(nameof(value), "Cannot be null!"); }
            }
        }

        public string Second_name
        {
            get => second_name;
            set
            {
                if (value != null)
                {
                    second_name = value;
                }
                else { throw new ArgumentNullException(nameof(value), "Cannot be null!"); }
            }
        }

        public string Last_name
        {
            get => last_name;
            set
            {
                if (value != null)
                {
                    last_name = value;
                }
                else { throw new ArgumentNullException(nameof(value), "Cannot be null!"); }
            }
        }

        public string Status_name
        {
            get => status_name;
            set
            {
                if (value != null)
                {
                    status_name = value;
                }
                else { throw new ArgumentNullException(nameof(value), "Cannot be null!"); }
            }
        }

        public string Deps_name
        {
            get => deps_name;
            set
            {
                if (value != null)
                {
                    deps_name = value;
                }
                else { throw new ArgumentNullException(nameof(value), "Cannot be null!"); }
            }
        }

        public string Posts_name
        {
            get => posts_name;
            set
            {
                if (value != null)
                {
                    posts_name = value;
                }
                else { throw new ArgumentNullException(nameof(value), "Cannot be null!"); }
            }
        }
    }
}
