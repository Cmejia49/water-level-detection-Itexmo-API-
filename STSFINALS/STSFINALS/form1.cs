using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

namespace STSFINALS
{
    public partial class form1 : Form
    {
        private const string conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:/christopher/Documents/Database1.mdb";
        readonly OleDbConnection con = new OleDbConnection(conString);
        OleDbCommand cmd;
        OleDbDataAdapter adapter;
        readonly DataTable dt = new DataTable();
        string serialValue;
        int counter = 0;
      
        public form1()
        {
       
            InitializeComponent();
            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "ID";
            dataGridView1.Columns[1].Name = "Phone Number";
            dataGridView1.Columns[2].Name = "Address";
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //SELECTION MODE
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            add(phonetxt.Text, addresstxt.Text);
        }

        /***********************************************************************************
       *              INSERT FUNCTION MS ACCESS MS SQL                                     *
       *                                                                                   *
       ************************************************************************************/
        private void add(string phoneNumber, string address)
        {
            //SQL STMT
            const string sql = "INSERT INTO Contact(phoneNumber,address) VALUES(@PHONENUMBER,@ADDRESS)";
            cmd = new OleDbCommand(sql, con);

            //ADD PARAMS
            cmd.Parameters.AddWithValue("@PHONENUMBER", phoneNumber);
            cmd.Parameters.AddWithValue("@ADDRESS", address);


            //OPEN CON AND EXEC INSERT
            try
            {
                if (con.State != ConnectionState.Open)
                { 
                con.Open();
              }
             
                if (cmd.ExecuteNonQuery() > 0)
                {
                    clearTxts();
                    MessageBox.Show(@"Successfully Inserted");
                }
                con.Close();
                retrieve();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
            finally
            {
                con.Close();
            }
        }

        private void clearTxts()
        {
            phonetxt.Clear();
            addresstxt.Clear();
        }
        /***********************************************************************************
        *              FILL THE DATAGRIDVIEW                                               *
        *                                                                                  *
        ************************************************************************************/
        private void populate(string id, string phoneNumber, string address)
        {
            dataGridView1.Rows.Add(id, phoneNumber, address);
        }

       /***********************************************************************************
       *              GETTING THE DATA IN THE MS ACCESS DATABASE                          *
       *                                                                                  *
       ************************************************************************************/

        private void retrieve()
        {
            dataGridView1.Rows.Clear();
            //SQL STATEMENT
            String sql = "SELECT * FROM Contact ";
            cmd = new OleDbCommand(sql, con);
            try
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                adapter = new OleDbDataAdapter(cmd);
                adapter.Fill(dt);
                //LOOP THROUGH DATATABLE
                foreach (DataRow row in dt.Rows)
                {
                    populate(row[0].ToString(), row[1].ToString(), row[2].ToString());
                }

                con.Close();
                //CLEAR DATATABLE
                dt.Rows.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
            finally
            {
                con.Close();
            }
        }

     
        /********************************************************************************
         *   DELETE FUNCTION                                                            *
         *        MS SQL                                                                *
         ********************************************************************************/
        private void delete(int id)
        {
            //SQL STATEMENTT
            String sql = "DELETE FROM Contact WHERE ID=" + id + "";
            cmd = new OleDbCommand(sql, con);

            //'OPEN CONNECTION,EXECUTE DELETE,CLOSE CONNECTION
            try
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                adapter = new OleDbDataAdapter(cmd);
                adapter.DeleteCommand = con.CreateCommand();
                adapter.DeleteCommand.CommandText = sql;

                //PROMPT FOR CONFIRMATION BEFORE DELETING
                if (MessageBox.Show(@"Are you sure to permanently delete this?", @"DELETE", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show(@"Successfully deleted");
                    }
                }
                con.Close();
                retrieve();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
            finally
            {
                con.Close();
            }
        }
       
        private void deleteBtn_Click(object sender, EventArgs e)
        {
            int selectedIndex = dataGridView1.SelectedRows[0].Index;
            if (selectedIndex != -1)
            {
                String selected = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                int id = Convert.ToInt32(selected);
                delete(id);
            }

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = dataGridView1.SelectedRows[0].Index;
                if (selectedIndex != -1)
                {
                    if (dataGridView1.SelectedRows[0].Cells[0].Value != null)
                    {
                        string phonenumber = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                        string address = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                       

                        phonetxt.Text = phonenumber;
                        addresstxt.Text = address;
                 
                    }

                }
            }
            catch (ArgumentOutOfRangeException)
            {

            }

        }

        private void deleteBtn_Click_1(object sender, EventArgs e)
        {
            int selectedIndex = dataGridView1.SelectedRows[0].Index;
            if (selectedIndex != -1)
            {
                String selected = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                int id = Convert.ToInt32(selected);
                delete(id);
            }

        }

        private void form1_Load(object sender, EventArgs e)
        {
            try
            {
           //     serialPort1.Open();
                //serialPort1.Close();
                retrieve();
            }
            catch(IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

      /*################################################################################################
       #                                                                                              #
       #                                                                                              #
       #                                  !--ITEXTMO API--!                                           #
       #                                                                                              #
       #                           Arduino serial data transmitter                                    #
       #                                                                                              #
       #                                                                                              #
       ################################################################################################*/



        public object getPhoneNumber()
        {
         //  dataGridView1.Rows.Clear();
            //SQL STATEMENT
            String sql = "SELECT phoneNumber FROM Contact ";
            object functionReturnValue = null;
            string holder;
            cmd = new OleDbCommand(sql, con);
            try
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                adapter = new OleDbDataAdapter(cmd);
                adapter.Fill(dt);
                //LOOP THROUGH DATATABLE
                foreach (DataRow row in dt.Rows)
                {
                
                    holder = row[1].ToString();
                 //Console.WriteLine(holder);
                    using (System.Net.WebClient client = new System.Net.WebClient())
                    {
                        System.Collections.Specialized.NameValueCollection parameter = new System.Collections.Specialized.NameValueCollection();
                        string url = "https://www.itexmo.com/php_api/api.php";
                        parameter.Add("1", holder);
                        parameter.Add("2", "FLOOD WARNING IN THIS AREA");
                        parameter.Add("3",  "TR-CHRIS972561_9DZMG");
                        dynamic rpb = client.UploadValues(url, "POST", parameter);
                        functionReturnValue = (new System.Text.UTF8Encoding()).GetString(rpb);
                    }
                    //return functionReturnValue;
                }

                con.Close();
                //CLEAR DATATABLE
                dt.Rows.Clear();
             
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
            finally
            {
                con.Close();
            }
            return functionReturnValue;
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            int val = 0;
            serialPort1.ReadTimeout = 500;
            serialPort1.WriteTimeout = 500;
            serialValue = serialPort1.ReadLine();
           Int32.TryParse(serialValue,out val);
          //  Console.WriteLine(val);
            if (val >= 80 && counter == 0)
            {
                try
                {
                    counter += 1;
                    //getPhoneNumber();
                    MessageBox.Show("WARNING DANGER");
                    getPhoneNumber();
                    serialPort1.Dispose();
       
                }
                catch(Exception ex)
                {
                
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    System.Threading.Thread.Sleep(1000);
                    serialPort1.Dispose();
                }
            }
       
        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(1000);
            serialPort1.Close();
            Application.Exit();
            Application.ExitThread();
            
        }
    }
}
    

