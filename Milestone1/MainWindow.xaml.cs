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
            using (var connection = new NpgsqlConnection(buildConnectionString()))
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;

                    // Create the text query
                    cmd.CommandText = "SELECT distinct state FROM business ORDER BY state";

                    try
                    {   
                        // execute command
                        var reader = cmd.ExecuteReader();
                        // iterate through results and add the read item into the list of state combobox
                        while (reader.Read())
                            stateList.Items.Add(reader.GetString(0));
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

            dataGrid.Items.Add(new Business() { name = "bus1", state = "WA", city = "Seattle" });

        }

        private void stateList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cityList.Items.Clear();

            if (stateList.SelectedIndex > -1)
            {

                using (var connection = new NpgsqlConnection(buildConnectionString()))
                {
                    connection.Open();

                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = connection;

                        // Create the text query
                        cmd.CommandText = "SELECT distinct city FROM business WHERE state = \'" + stateList.SelectedItem.ToString() + "\' ORDER BY city";

                        try
                        {
                            // execute command
                            var reader = cmd.ExecuteReader();
                            // iterate through results and add the read item into the list of state combobox
                            while (reader.Read())
                                cityList.Items.Add(reader.GetString(0));
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

                    }// using
                } // using
            } // if
        }// method end

        private void cityList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dataGrid.Items.Clear();

            if (cityList.SelectedIndex > -1)
            {

                using (var connection = new NpgsqlConnection(buildConnectionString()))
                {
                    connection.Open();

                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = connection;

                        // Create the text query
                        cmd.CommandText = "SELECT name, state, city, business_id FROM business WHERE state = \'" + stateList.SelectedItem.ToString() + "\' AND city = \'" +cityList.SelectedItem.ToString()+ "\' ORDER BY name";

                        try
                        {
                            // execute command
                            var reader = cmd.ExecuteReader();
                            // iterate through results and add the read item into the list of state combobox
                            while (reader.Read())
                            {
                                dataGrid.Items.Add(new Business()
                                { 
                                    name = reader.GetString(0), 
                                    state = reader.GetString(1), 
                                    city = reader.GetString(2),
                                    bid = reader.GetString(3)
                                });
                            }
                                
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

                    }// using
                } // using
            } // if
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedIndex >= 0)
            {
                Business B = dataGrid.Items[dataGrid.SelectedIndex] as Business;
                if ((B.bid != null) && (B.bid.ToString().CompareTo("") != 0))
                {
                    BusinessDetails businessWindow = new BusinessDetails(B.bid.ToString());
                    businessWindow.Show();
                }
            }
        }
    }
}
