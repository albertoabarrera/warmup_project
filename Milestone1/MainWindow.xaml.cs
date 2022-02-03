using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;

namespace Milestone1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class Business
        {
            public string bid { get; set; }
            public string name { get; set; }
            public string state { get; set; }
            public string city { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            addStates();
            addColumnsToGrid();

        }

        private string buildConnectionString()
        {
            return "Host = localhost; Username = postgres; Database = milestone1db; password=TwilightOne1!";
        }

        public void addStates()
        {
            string query = "SELECT distinct state FROM business ORDER BY state";
            executeQuery(query, populateStatesInBox);
        }

        // Function to add header to the grid.
        private void addColumnsToGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Business Name";
            col1.Width = 255;
            col1.Binding = new Binding("name");
            dataGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "State";
            col2.Binding = new Binding("state");
            col2.Width = 60;
            dataGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "City";
            col3.Binding = new Binding("city");
            col3.Width = 150;
            dataGrid.Columns.Add(col3);
            
            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Header = "bid";
            col4.Binding = new Binding("bid");
            col4.Width = 0;
            dataGrid.Columns.Add(col4);

        }

        // General query function
        public void executeQuery(string query, Action<NpgsqlDataReader> func)
        {
            using (var connection = new NpgsqlConnection(buildConnectionString()))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = query;
                    try
                    {
                        // execute command
                        var reader = cmd.ExecuteReader();
                        // iterate through results and add the read item into the list of state combobox
                        while (reader.Read())
                            func(reader);
                    }
                    catch (NpgsqlException ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                        System.Windows.MessageBox.Show("SQL Error: " + ex.Message.ToString());
                    }
                    finally
                    {
                        connection.Close();
                    }

                }
            }
        }

        // lambda func to execute initial query to populate the states
        private void populateStatesInBox(NpgsqlDataReader reader)
        {
            stateList.Items.Add(reader.GetString(0));
        }

        // Lambda func for selecting a state and populating the second box with cities.
        private void populateCitiesInBox(NpgsqlDataReader reader)
        {
            cityList.Items.Add(reader.GetString(0));
        }

        // Lambda Func for adding business rows based on state and city selected
        private void addGridRow(NpgsqlDataReader reader)
        {
            dataGrid.Items.Add(new Business()
            {
                name = reader.GetString(0),
                state = reader.GetString(1),
                city = reader.GetString(2),
                bid = reader.GetString(3)
            });
        }

        // Call the DB and populate cities based on state.
        private void stateList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cityList.Items.Clear();

            if (stateList.SelectedIndex > -1)
            {
                string query = "SELECT distinct city FROM business WHERE state = \'" + stateList.SelectedItem.ToString() + "\' ORDER BY city";
                executeQuery(query, populateCitiesInBox);
            }
        }

        // When a city is selected, we want to reload the businesses
        private void cityList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dataGrid.Items.Clear();

            if (cityList.SelectedIndex > -1)
            {
                // Create the text query
                string query = "SELECT name, state, city, business_id FROM business WHERE state = \'" +
                    stateList.SelectedItem.ToString() +
                    "\' AND city = \'" +
                    cityList.SelectedItem.ToString() +
                    "\' ORDER BY name";
                executeQuery(query, addGridRow);
            }
        }

        // New window for when a business is selected from grid
        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedIndex >= 0)
            {
                // TODO how does this cast work??
                Business B = dataGrid.Items[dataGrid.SelectedIndex] as Business;

                // if business id is not null or empty
                if ((B.bid != null) && (B.bid.ToString().CompareTo("") != 0))
                {
                    BusinessDetails businessWindow = new BusinessDetails(B.bid.ToString());
                    businessWindow.Show();
                }
            }
        }
    }
}
