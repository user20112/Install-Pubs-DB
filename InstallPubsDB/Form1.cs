using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;


namespace InstallPubsDB
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Install_Click(object sender, EventArgs e)
        {


            try
            {
                string script = File.ReadAllText("instpubs.sql");

                // split script on GO command
                System.Collections.Generic.IEnumerable<string> commandStrings = Regex.Split(script, @"^\s*GO\s*$",
                                         RegexOptions.Multiline | RegexOptions.IgnoreCase);
                using (SqlConnection connection = new SqlConnection("Integrated Security=true;Initial Catalog=master;Data Source=(local);"))
                {
                    connection.Open();
                    foreach (string commandString in commandStrings)
                    {
                        if (commandString.Trim() != "")
                        {
                            using (var command = new SqlCommand(commandString, connection))
                            {
                                try
                                {
                                    command.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {

                                    string spError = commandString.Length > 100 ? commandString.Substring(0, 100) + " ...\n..." : commandString;
                                    MessageBox.Show("Install Failed.");
                                }
                            }
                        }
                    }
                    connection.Close();
                }
                MessageBox.Show("Install suceeded");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Install Failed.");
            }

        }
    }
}
