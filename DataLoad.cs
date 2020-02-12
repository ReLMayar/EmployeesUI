using System;

public class DataLoad
{
    /// <summary>
    /// Загрузка данных на форму
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void FormLoad(object sender, EventArgs e)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        sqlConnection = new SqlConnection(connectionString);
        await sqlConnection.OpenAsync();

        SqlCommand command = new SqlCommand(
            "SELECT persons.first_name, persons.second_name, persons.last_name, status.name, deps.name, posts.name " +
            "FROM persons JOIN status ON status.id = persons.id_status JOIN deps ON deps.id = persons.id_dep JOIN posts ON posts.id = persons.id_post"
            , sqlConnection);

        SqlDataReader reader = command.ExecuteReader();
        persons = new List<Person>();

        while (reader.Read())
        {
            persons.Add(new Person()
            {
                First_name = reader[0].ToString(),
                Second_name = reader[1].ToString(),
                Last_name = reader[2].ToString(),
                Status_name = reader[3].ToString(),
                Deps_name = reader[4].ToString(),
                Posts_name = reader[5].ToString()
            });
        }

        foreach (Person person in persons)
            personData.Items.Add(person);

        reader.Close();
        sqlConnection.Close();
    }
}
