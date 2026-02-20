using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ADO_StoredPracticle
{
    internal class Program
    {
        static SqlConnection con;
        static SqlCommand cmd;
        static string sqlCon= "Integrated Security = SSPI;Persist Security Info=False;Initial Catalog = sample ;Data Source=LAPTOP-QK4AG1TO";


        public void ShowData()
        {   
            try
            {
                con= new SqlConnection(sqlCon);
                cmd = new SqlCommand();
                cmd.Connection= con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "uspProduct";
                cmd.Parameters.AddWithValue("@intMode", 2);
                con.Open();

                SqlDataReader reader= cmd.ExecuteReader();

                decimal grandTotal = 0;
                decimal Discount = 0;

                Console.WriteLine("---------------------------------------------------------------------");
                Console.WriteLine("{0,-10} {1,-15} {2,-10} {3,-10} {4,-10}", "Prod_ID", "Prod_Name", "Prod_qty", "Prod_Price", "Total");
                Console.WriteLine("---------------------------------------------------------------------");

                while(reader.Read())
                {
                    int qty = Convert.ToInt32(reader["Prod_qty"]);
                    decimal price = Convert.ToDecimal(reader["Prod_Price"]);
                    decimal total = qty * price;

                    grandTotal += total;

                    Console.WriteLine("{0,-10} {1,-15} {2,-10} {3,-10} {4,-10}",
                    reader["Prod_ID"],
                    reader["Prod_Name"],
                    qty,
                    price,
                    total);
                }
                Console.WriteLine("---------------------------------------------------------------------");
                Console.WriteLine("Grand Total: " + grandTotal);

                Console.WriteLine("-----------------");
                Console.Write("Congratulations You Have Saved Rs:");
                if(grandTotal>3000)
                {
                    Discount = grandTotal * (5m / 100m);
                    Console.WriteLine(Discount);
                }
                else
                {
                    Console.WriteLine("Your Amount is Less than 3000");
                }
                decimal Payable = grandTotal - Discount;
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("Amount You have To pay IS:"+ Payable);
                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    
        public void InsertData()
        {
            Console.WriteLine("Enter Product Name:");
            string name = Console.ReadLine();
            Console.WriteLine("Enter Product Quantity:");
            int Qty = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter Product Price:");
            decimal Price = Convert.ToDecimal(Console.ReadLine());

            con = new SqlConnection(sqlCon);
            cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "uspProduct";
            cmd.Parameters.AddWithValue("@intMode", 1);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@qty", Qty);
            cmd.Parameters.AddWithValue("@price", Price);
            con.Open();
            int flag = cmd.ExecuteNonQuery();
            con.Close();
            if (flag ==1 )
            {
                Console.WriteLine("Record not Stored!");
            }
            else
            {
                MessageBox.Show("Record insert Successully!", "Insert Method", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            }
            
        }

        public void DeleteData()
        {
            ShowData();
            Console.WriteLine("Enter Id of Product to Delete:");
            int id=Convert.ToInt32(Console.ReadLine());
            con=new SqlConnection(sqlCon);
            cmd=new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType= CommandType.StoredProcedure;
            cmd.CommandText = "uspProduct";
            cmd.Parameters.AddWithValue("@intMode", 3);
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            int flag = cmd.ExecuteNonQuery();
            con.Close();
            if (flag == 1)
            {
                Console.WriteLine("Data not Found");
            }
            else
            {
                MessageBox.Show("Record Deleted Successfully", "Delete Method", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            }    
        }
            
        public void UpdateData()
        {
            ShowData();
            Console.WriteLine("Enter the ID of the Product to Update:");
            int id= Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter Product Name:");
            string name = Console.ReadLine();
            Console.WriteLine("Enter Product Quantity:");
            int Qty = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter Product Price:");
            decimal Price = Convert.ToDecimal(Console.ReadLine());

            con = new SqlConnection(sqlCon);
            cmd=new SqlCommand();
            cmd.Connection= con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "uspProduct";
            cmd.Parameters.AddWithValue("@intMode", 4);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@qty", Qty);
            cmd.Parameters.AddWithValue("@price", Price);
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            int flag= cmd.ExecuteNonQuery();
            con.Close();
            if(flag==1)
            {
                Console.WriteLine("Data not Update");
            }
            else
            {
                MessageBox.Show("Data Updated SuccessFully", "Update Method", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void SearchData()
        {
            Console.WriteLine("Enter Product ID to search (or press Enter to search by Name):");
            string input = Console.ReadLine();

            int id;
            string name=null;

            con = new SqlConnection(sqlCon);
            cmd=new SqlCommand();
            cmd.Connection= con;
            cmd.CommandType= CommandType.StoredProcedure;
            cmd.CommandText = "uspProduct";
            cmd.Parameters.AddWithValue("@intMode",5);

            // Decide whether to search by ID or Name
            if (int.TryParse(input, out id))
            {
                cmd.Parameters.AddWithValue("@id", id);
            }
            else
            {
                name=input;
                cmd.Parameters.AddWithValue("@name", name);
            }
              con.Open();
           SqlDataReader reader= cmd.ExecuteReader(); 
            if(reader.HasRows)
            {
                Console.WriteLine("Prod_ID\t\tProd_Name\t\tProd_qty\t\tProd_Price");
                while (reader.Read())
                {
                    Console.WriteLine("{0}\t\t{1}\t\t\t{2}\t\t{3}",
                        reader["Prod_ID"],
                        reader["Prod_Name"],
                        reader["Prod_qty"],
                        reader["Prod_Price"]);
                }
            }
            else
            {
                    Console.WriteLine("No matching product found.");
            }
            reader.Close();
        }

        static void Main(string[] args)
        {
            Program objP=new Program();
            int ch;
            do
            {
                Console.WriteLine("1.Insert Data:");
                Console.WriteLine("2.update Data:");
                Console.WriteLine("3.Delate Data:");
                Console.WriteLine("4.Search Data:");
                Console.WriteLine("5.Show Data:");
                Console.WriteLine("6.Exit:");
                Console.Write("Enter Your Choice:");
                ch = Convert.ToInt32(Console.ReadLine());
                switch (ch)
                {
                    case 1:
                        char choice;
                        do
                        {
                            objP.InsertData();
                            Console.WriteLine("Do you want to insert another record? (y/n)");
                            choice = Convert.ToChar(Console.ReadLine());
                        } while (choice == 'y' || choice == 'Y');
                        break;

                    case 2:
                        objP.UpdateData();
                        break;

                    case 3:
                        objP.DeleteData();
                        break;

                    case 4:
                        objP.SearchData();
                        break;

                    case 5:
                        objP.ShowData();
                        break;
                        
                    case 6:
                        Console.WriteLine("Exit");
                        break;
                        
                    default:
                        Console.WriteLine("Your choice is wrong!");
                        break;
                }
            }while (ch!=6);
        }
    }
}
