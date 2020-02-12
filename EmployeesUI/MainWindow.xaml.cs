using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.ComponentModel;
using System.Threading.Tasks;

namespace EmployeesUI
{
    public partial class MainWindow : Window
    {
        #region Supporting data

        SqlConnection sqlConnection;
        public string connectionString;
        List<Person> persons;
        List<Deps> deps = new List<Deps>();
        List<Status> status;
        List<Posts> posts;
        int id_status;
        int id_dep;
        int id_post;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            #region Load

            main.Loaded += (s, e) => FormLoad(s, e);
            combobox_deps.Loaded += (s, e) => DepsBoxLoad(s, e);
            combobox_status.Loaded += (s, e) => StatusBoxLoad(s, e);

            #endregion

            #region Sorting

            search.TextChanged += (s, e) => TextChange(s, e);
            combobox_deps.SelectionChanged += (s, e) => DepsBoxSelectionChanged(s, e);
            combobox_post.SelectionChanged += (s, e) => PostBoxSelectionChanged(s, e);
            combobox_status.SelectionChanged += (s, e) => StatusBoxSelectionChanged(s, e);
            clear.Click += (s, e) => Clear(s, e);

            #endregion
        }

        #region DataLoad

        /// <summary>
        /// Uploading data to the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormLoad(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (sqlConnection = new SqlConnection(connectionString))
            {
                #region Uploading data to the form

                sqlConnection.Open();

                string SqlExpression = "getPersonData";
                SqlCommand command = new SqlCommand(SqlExpression, sqlConnection);

                command.CommandType = System.Data.CommandType.StoredProcedure;
                var reader = command.ExecuteReader();

                persons = new List<Person>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        persons.Add(new Person()
                        {
                            First_name = reader[0].ToString(), Second_name = reader[1].ToString(),
                            Last_name = reader[2].ToString(), Status_name = reader[3].ToString(),
                            Deps_name = reader[4].ToString(), Posts_name = reader[5].ToString()
                        });
                    }
                }

                foreach (Person person in persons)
                    personData.Items.Add(person);

                reader.Close();

                #endregion
            }
        }

        /// <summary>
        /// Load all deps
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DepsBoxLoad(object sender, EventArgs e)
        {
            using (sqlConnection = new SqlConnection(connectionString))
            {
                #region Load all deps

                await sqlConnection.OpenAsync();

                string SqlExpression = "getDeps";
                SqlCommand command = new SqlCommand(SqlExpression, sqlConnection);

                command.CommandType = System.Data.CommandType.StoredProcedure;
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        deps.Add(new Deps()
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
                reader.Close();

                combobox_deps.ItemsSource = deps.Select(q => q.Name);

                #endregion
            }
        }

        /// <summary>
        /// Load all statuses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StatusBoxLoad(object sender, EventArgs e)
        {
            using (sqlConnection = new SqlConnection(connectionString))
            {
                #region Load all statuses

                await sqlConnection.OpenAsync();

                string SqlExpression = "getStatus";
                SqlCommand command = new SqlCommand(SqlExpression, sqlConnection);

                command.CommandType = System.Data.CommandType.StoredProcedure;
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    status = new List<Status>();
                    while (reader.Read())
                    {
                        status.Add(new Status()
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
                reader.Close();

                combobox_status.ItemsSource = status.Select(q => q.Name);

                #endregion
            }
        }

        #endregion

        #region Sorting

        /// <summary>
        /// Sort by deps
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DepsBoxSelectionChanged(object sender, EventArgs e)
        {
            if (combobox_deps.SelectedItem != null)
            {
                id_dep = deps.Where(i => i.Name.Equals(combobox_deps.SelectedItem)).Select(q => q.Id).Single();

                using (sqlConnection = new SqlConnection(connectionString))
                {
                    #region Loading posts

                    await sqlConnection.OpenAsync();

                    string LoadingPostExpression = "getPost";
                    SqlCommand command = new SqlCommand(LoadingPostExpression, sqlConnection);

                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    SqlParameter depParam = new SqlParameter
                    {
                        ParameterName = "@id_dep",
                        Value = id_dep
                    };
                    command.Parameters.Add(depParam);

                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        posts = new List<Posts>();
                        while (reader.Read())
                        {
                            posts.Add(new Posts()
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                    combobox_post.ItemsSource = posts.Select(q => q.Name);
                    reader.Close();

                    #endregion

                    #region Sort by selected dep

                    if (id_status != default)
                    {
                        await SortByStatus_Dep();
                    }
                    else
                    {
                        string SqlExpression = "sortByDep";
                        SqlCommand sort = new SqlCommand(SqlExpression, sqlConnection);
                        sort.CommandType = System.Data.CommandType.StoredProcedure;

                        depParam = new SqlParameter
                        {
                            ParameterName = "@id_dep",
                            Value = id_dep
                        };
                        sort.Parameters.Add(depParam);

                        reader = sort.ExecuteReader();

                        personData.Items.Clear();
                        persons = new List<Person>();

                        while (reader.Read())
                        {
                            persons.Add(new Person()
                            {
                                First_name = reader[0].ToString(), Second_name = reader[1].ToString(),
                                Last_name = reader[2].ToString(), Status_name = reader[3].ToString(),
                                Deps_name = reader[4].ToString(), Posts_name = reader[5].ToString()
                            });
                        }

                        foreach (Person person in persons)
                            personData.Items.Add(person);
                        reader.Close();
                    }

                    #endregion
                }
            }
        }

        /// <summary>
        /// Sort by dep and post
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PostBoxSelectionChanged(object sender, EventArgs e)
        {
            if (combobox_post.SelectedItem != null)
            {
                id_dep = deps.Where(i => i.Name.Equals(combobox_deps.SelectedItem)).Select(q => q.Id).Single();
                id_post = posts.Where(i => i.Name.Equals(combobox_post.SelectedItem)).Select(q => q.Id).Single();      

                using (sqlConnection = new SqlConnection(connectionString))
                {
                    #region Sort by selected post

                    if (id_status != default)
                    {
                        await MultipleSorting();
                    }
                    else
                    {
                        await sqlConnection.OpenAsync();

                        string SqlExpression = "sortByDep_Post";
                        SqlCommand command = new SqlCommand(SqlExpression, sqlConnection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        SqlParameter depParam = new SqlParameter
                        {
                            ParameterName = "@id_dep",
                            Value = id_dep
                        };
                        command.Parameters.Add(depParam);

                        SqlParameter postParam = new SqlParameter
                        {
                            ParameterName = "@id_post",
                            Value = id_post
                        };
                        command.Parameters.Add(postParam);

                        var reader = command.ExecuteReader();
                        personData.Items.Clear();
                        persons = new List<Person>();

                        while (reader.Read())
                        {
                            persons.Add(new Person()
                            {
                                First_name = reader[0].ToString(), Second_name = reader[1].ToString(),
                                Last_name = reader[2].ToString(), Status_name = reader[3].ToString(),
                                Deps_name = reader[4].ToString(), Posts_name = reader[5].ToString()
                            });
                        }

                        foreach (Person person in persons)
                            personData.Items.Add(person);

                        reader.Close();
                    }

                    #endregion
                }
            }
        }

        /// <summary>
        /// Sort by status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StatusBoxSelectionChanged(object sender, EventArgs e)
        {
            if (combobox_status.SelectedItem != null)
            {
                id_status = status.Where(i => i.Name.Equals(combobox_status.SelectedItem)).Select(q => q.Id).Single();

                using (sqlConnection = new SqlConnection(connectionString))
                {
                    #region Sort by selected status

                    await sqlConnection.OpenAsync();

                    if (id_dep != default && id_post != default)
                    {
                        await MultipleSorting();
                    }
                    else if (id_dep != default)
                    {
                        await SortByStatus_Dep();
                    }
                    else
                    {
                        string SqlExpression = "sortByStatus";
                        SqlCommand sort = new SqlCommand(SqlExpression, sqlConnection);
                        sort.CommandType = System.Data.CommandType.StoredProcedure;

                        var statusParam = new SqlParameter
                        {
                            ParameterName = "@id_status",
                            Value = id_status
                        };
                        sort.Parameters.Add(statusParam);

                        var reader = sort.ExecuteReader();

                        personData.Items.Clear();
                        persons = new List<Person>();

                        while (reader.Read())
                        {
                            persons.Add(new Person()
                            {
                                First_name = reader[0].ToString(), Second_name = reader[1].ToString(),
                                Last_name = reader[2].ToString(), Status_name = reader[3].ToString(),
                                Deps_name = reader[4].ToString(), Posts_name = reader[5].ToString()
                            });
                        }

                        foreach (Person person in persons)
                            personData.Items.Add(person);
                        reader.Close();
                    }

                    #endregion
                }
            }
        }

        /// <summary>
        /// Sort by status and dep
        /// </summary>
        private async Task SortByStatus_Dep()
        {
            using (sqlConnection = new SqlConnection(connectionString))
            {
                #region Sort by status and dep

                await sqlConnection.OpenAsync();

                string SqlExpression = "sortByStatus_Dep";
                SqlCommand command = new SqlCommand(SqlExpression, sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter depParam = new SqlParameter
                {
                    ParameterName = "@id_dep",
                    Value = id_dep
                };
                command.Parameters.Add(depParam);

                SqlParameter statusParam = new SqlParameter
                {
                    ParameterName = "@id_status",
                    Value = id_status
                };
                command.Parameters.Add(statusParam);

                var reader = command.ExecuteReader();
                personData.Items.Clear();
                persons = new List<Person>();

                while (reader.Read())
                {
                    persons.Add(new Person()
                    {
                        First_name = reader[0].ToString(), Second_name = reader[1].ToString(),
                        Last_name = reader[2].ToString(), Status_name = reader[3].ToString(),
                        Deps_name = reader[4].ToString(), Posts_name = reader[5].ToString()
                    });
                }

                foreach (Person person in persons)
                    personData.Items.Add(person);

                reader.Close();

                #endregion
            }
        }

        /// <summary>
        /// Sort by dep, post and status
        /// </summary>
        private async Task MultipleSorting()
        {
            using (sqlConnection = new SqlConnection(connectionString))
            {
                #region Sort by dep, post and status

                await sqlConnection.OpenAsync();

                string SqlExpression = "multipleSorting";
                SqlCommand command = new SqlCommand(SqlExpression, sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter depParam = new SqlParameter
                {
                    ParameterName = "@id_dep",
                    Value = id_dep
                };
                command.Parameters.Add(depParam);

                SqlParameter postParam = new SqlParameter
                {
                    ParameterName = "@id_post",
                    Value = id_post
                };
                command.Parameters.Add(postParam);

                SqlParameter statusParam = new SqlParameter
                {
                    ParameterName = "@id_status",
                    Value = id_status
                };
                command.Parameters.Add(statusParam);

                var reader = command.ExecuteReader();

                personData.Items.Clear();
                persons = new List<Person>();

                while (reader.Read())
                {
                    persons.Add(new Person()
                    {
                        First_name = reader[0].ToString(), Second_name = reader[1].ToString(),
                        Last_name = reader[2].ToString(), Status_name = reader[3].ToString(),
                        Deps_name = reader[4].ToString(), Posts_name = reader[5].ToString()
                    });
                }

                foreach (Person person in persons)
                    personData.Items.Add(person);

                reader.Close();

                #endregion
            }
        }

        /// <summary>
        /// Setting default data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clear(object sender,EventArgs e)
        {
            #region Setting default

            combobox_deps.SelectedItem = null;
            combobox_post.SelectedItem = null;
            combobox_status.SelectedItem = null;
            id_status = default;
            id_dep = default;
            id_post = default;
            personData.Items.Clear();

            #endregion

            FormLoad(sender, e);
        }

        #endregion

        #region Search

        /// <summary>
        /// Search by first_name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextChange(object sender, EventArgs e)
        {
            var term = search.Text.ToLower();
            var result = persons.Where(i => i.First_name.ToLower().Contains(term)).Select(q => q).Distinct().ToList();

            personData.Items.Clear();

            foreach (Person person in result)
                personData.Items.Add(person);
        }

        #endregion

        #region Sort column GridView

        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;

        /// <summary>
        /// Column header click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                    Sort(sortBy, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        /// <summary>
        /// Sort(asc, desc)
        /// </summary>
        /// <param name="sortBy"></param>
        /// <param name="direction"></param>
        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(personData.Items);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

        #endregion
    }
}