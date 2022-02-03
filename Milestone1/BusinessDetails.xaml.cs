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
using System.Windows.Shapes;
using Npgsql;

namespace Milestone1
{
    /// <summary>
    /// Interaction logic for BusinessDetails.xaml
    /// </summary>
    public partial class BusinessDetails : Window
    {
        private string bid = "";

        public BusinessDetails(string bid)
        {
            InitializeComponent();
            this.bid = String.Copy(bid);

            loadBusinessDetailsQuery();
            loadBusinessNumbers();

        }

        // general execute query command
        public void executeQuery(string query, Action<NpgsqlDataReader> lambda)
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
                        reader.Read();
                        lambda(reader);
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

        // updates text boxes with the bus info from query results
        private void setBusinessDetails(NpgsqlDataReader reader)
        {
            bnameBox.Text = reader.GetString(0);
            stateBox.Text = reader.GetString(1);
            cityBox.Text = reader.GetString(2);
        }

        // updates label
        private void setNumInState(NpgsqlDataReader reader)
        {
            stateNumLabel.Content = reader.GetInt16(0).ToString();
        }

        // updates city label
        private void setNumInCity(NpgsqlDataReader reader)
        {
            cityNumLabel.Content = reader.GetInt16(0).ToString();
        }

        // connection and authentication to DB
        private string buildConnectionString()
        {
            return "Host = localhost; Username = postgres; Database = milestone1db; password=TwilightOne1!";
        }

        // Query for the business details
        private void loadBusinessDetailsQuery()
        {
            string query = "SELECT name, state, city FROM business WHERE business_id = \'" + this.bid + "\';";
            executeQuery(query, setBusinessDetails);
        }

        private void loadBusinessNumbers()
        {
            string query1 = "SELECT count(*) from business WHERE state = (SELECT state FROM business WHERE business_id = \'" + this.bid + "\');";
            executeQuery(query1, setNumInState);
            string query2 = "SELECT count(*) from business WHERE city = (SELECT city FROM business WHERE business_id = \'" + this.bid + "\');";
            executeQuery(query2, setNumInCity);

            //SELECT count(*) from business WHERE city = (SELECT state FROM business WHERE business_id = 'bMEev3B405vXG2SJthbcBw';
        }

    }
}
