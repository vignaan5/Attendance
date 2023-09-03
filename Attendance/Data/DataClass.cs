
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.Data
{

	public class DataClass
	{
		public string connstring { get; set; }
		public bool is_conn_open { get; set; }

		public string emp_id2 { get ; set; }	

		public bool is_admin { get; set; }	

		 public MySqlConnection connection;
		public DataClass()
		{
			this.connstring = "Server=MYSQL8002.site4now.net;Database=db_a9daf3_vignaan;Uid=a9daf3_vignaan;Pwd=gyanu@18;SSL MODE = None;";
		    this.is_conn_open = false;
		}

		public DataClass(string emp_id)
		{
			this.connstring = "Server=MYSQL8002.site4now.net;Database=db_a9daf3_vignaan;Uid=a9daf3_vignaan;Pwd=gyanu@18;SSL MODE = None;";
			this.is_conn_open = false;
			get_emp_id();

			if(this.emp_id2 != null) 
			{
				start_connection();
				check_admin(emp_id2);
				close_connection();
			}
              		
		}

	    
		public List<string> get_all_employee_id_in_a_specific_state(string state)
		{
			if (!is_conn_open)
				return null;

			List<string> emp_id = new List<string>();

			string sql_q = string.Format("select emp_id from employee where state='{0}';", state);

			MySqlCommand cmd = new MySqlCommand(sql_q, connection);

			try
			{
				cmd.ExecuteNonQuery();
			}
			catch (Exception ex)
			{

			}

			MySqlDataReader reader = null;
			try
			{
				reader = cmd.ExecuteReader();

			}
			catch (Exception ex)
			{

			}

			while(!reader.IsClosed && reader.Read())
			{
				if (reader[0] != null && reader[0].ToString()!=null)
				{
					emp_id.Add(reader[0].ToString());
				}
			}

			return emp_id;
		}


		public int get_employee_sales_on_a_specific_day(string sql_date,string state,string temp_emp_id)
		{
			if (!is_conn_open)
				return 0;

			int sum = 0;
			string mysql_string = String.Format("select sum(es.amount) from employee e left join employee_sales2 es on e.emp_id=es.emp_id and The_date='{0}' where The_date is not null and es.emp_id='{1}' and e.state='{2}';",sql_date,temp_emp_id,state);

			MySqlCommand cmd = null;

			try
			{
			 cmd=	new MySqlCommand(mysql_string, connection);
			}
			catch (Exception ex)
			{

			}
			try
			{
				cmd.ExecuteNonQuery();
			}
			catch (Exception ex) 
			{ 
			    
			}

			MySqlDataReader reader = null;
			try
			{
				reader = cmd.ExecuteReader();
			
			}
			catch (Exception ex)
			{

			}

			while(!reader.IsClosed && reader.Read())
			{
				if (reader[0]!=null && reader[0].ToString()!=null)
				{
					int temp = 0;

					try
					{
						temp = Convert.ToInt32(reader[0].ToString());
					}
					catch 
					{
						temp = 0;
					}
					sum = temp;
				}

			
			}


			reader.Close();
			return sum;

		}




		public List<List<string>> get_rows(string sql_date,ref int sum)
		{
			if (!is_conn_open)
				return null;

			string sql_cmd = string.Format("select * from products2 left join employee_sales2   on products2.Sno = employee_sales2.sno and The_date = '{0}' ;  ",sql_date);

			   MySqlCommand cmd = new MySqlCommand(sql_cmd,connection);

			try
			{
				cmd.ExecuteNonQuery();
			}
			catch (Exception ex) 
			{ 
			
			}

			MySqlDataReader mySqlDataReader = null;

			try
			{
				mySqlDataReader= cmd.ExecuteReader();	
			}
			catch (Exception ex) 
			{ 
			     
			}

			List<List<string>> rows = new List<List<string>>();

			while (!mySqlDataReader.IsClosed && mySqlDataReader.Read()) 
			{ 
			   List<string> row = new List<string>();

				row.Add(mySqlDataReader[0].ToString());
				row.Add(mySqlDataReader[1].ToString());
				row.Add(mySqlDataReader[2].ToString());
				row.Add(mySqlDataReader[3].ToString());

				if (mySqlDataReader[6]==null)
				{
					row.Add("0");
					
				}
				else
				{
					  if(mySqlDataReader[6].ToString()=="")
						row.Add("0");

					else row.Add(mySqlDataReader[6].ToString());
				}

				if (mySqlDataReader[7] == null)
				{
					row.Add("0");
				}
				else
				{
					if (mySqlDataReader[6].ToString() == "")
						row.Add("0");

					else
					{
						row.Add(mySqlDataReader[7].ToString());
						try
						{
							string amt = mySqlDataReader[7].ToString();
							sum += Convert.ToInt32(amt);
						}
						catch { }
					}
				}

				rows.Add(row);
			}

			return rows;
		}





		public async Task get_emp_id()
		{

			string emp_id=await SecureStorage.GetAsync("employee_id");
		   this.emp_id2=emp_id;
		}

		public void start_connection()
		{
			if (is_conn_open == true)
				return;

			 connection = new MySqlConnection(this.connstring);
			try
			{
				connection.Open();
				is_conn_open = true;
			}
			catch 
			{ 
			  
			}

			return;
		}


		public void close_connection()
		{
		 if(	is_conn_open == false )
				 return;

		 connection.Close();
			is_conn_open= false;
			return;

		}


		public void update_sale_info(string query )
		{
			if (!is_conn_open)
				return;




		}





		public Dictionary<string,Dictionary<string,string>> get_products()
		{
			if (!is_conn_open)
				return null;

			Dictionary<string, Dictionary<string, string>> temp = new Dictionary<string, Dictionary<string, string>>();
			MySqlCommand cmd = null;
			string get_prod_str = "SELECT * FROM Products;";
			try
			{
				 cmd = new MySqlCommand(get_prod_str, connection);
			}
			catch(Exception e) 
			{ 
			}
			try
			{
				cmd.ExecuteNonQuery();
			}
			catch
			(Exception e)
			{ }

			MySqlDataReader reader = null;

			try
			{
			  reader =	cmd.ExecuteReader();
			}
			catch(Exception ex)
			{

			}
			while(!reader.IsClosed && reader.Read())
			{
				Dictionary<string,string> product = new Dictionary<string,string>();

				product["sno"] = reader[0].ToString();
				product["particulars"] = reader[1].ToString();
				product["hsn/sac"] = reader[2].ToString();
				product["mrp"] = reader[3].ToString();
				temp[product["particulars"]] = product;
			}


			return temp;
		}



		public int get_cumiliatvie_sales(string start_date,string end_date)
		{
			if (!is_conn_open)
				return -1;



			string cumiliative_sales_command = string.Format("select sum(amount) from employee_sales2 left join products2   on products2.Sno=employee_sales2.sno where The_date between '{0}' and '{1}';", start_date, end_date);

			MySqlCommand cumiliative_cmd = new MySqlCommand(cumiliative_sales_command, connection);

			try
			{
				cumiliative_cmd.ExecuteNonQuery();
			}
			catch (Exception e) { }

			MySqlDataReader reader = null;

			try
			{
				reader = cumiliative_cmd.ExecuteReader();
			}
			catch
			{

			}

			while(!reader.IsClosed && reader.Read())
			{
			    return  Convert.ToInt32( reader[0].ToString() );
			}

			return -1;
		}


		public int get_cumiliatvie_sales_of_an_employee_in_the_current_month()
		{
			if (!is_conn_open)
				return -1;

			DatePicker current_month = new DatePicker();

			string sql_end_date = current_month.Date.Year.ToString() +"-" + current_month.Date.Month.ToString()+"-"+current_month.Date.Day.ToString() ;

			string sql_str_date = current_month.Date.Year.ToString() + "-" + current_month.Date.Month.ToString() + "-1";


			string cumiliative_sales_command = string.Format("select sum(amount) from employee_sales2 where emp_id='{0}' and The_date between '{1}' and '{2}' ;",emp_id2,sql_str_date,sql_end_date);

			MySqlCommand cumiliative_cmd = new MySqlCommand(cumiliative_sales_command, connection);

			try
			{
				cumiliative_cmd.ExecuteNonQuery();
			}
			catch (Exception e) { }

			MySqlDataReader reader = null;

			try
			{
				reader = cumiliative_cmd.ExecuteReader();
			}
			catch
			{

			}

			while (!reader.IsClosed && reader.Read())
			{
				string temp = reader[0].ToString();


				int xyz = 0;
				try
				{
					xyz= Convert.ToInt32(temp);
				}
				catch
				{
					xyz = 0;
				}

				reader.Close();
				return xyz;
			}
			reader.Close();
			return -1;
		}


		public  int get_remaining_sales_needed_to_reach_target(ref int target)
		{
			if (!is_conn_open)
			{
				target = -1;
				return -1;
			}
			

			string sql_get_target_string = String.Format("select target from employee where employee.emp_id='{0}';",emp_id2);

			MySqlCommand cmd = null;

			try
			{
			 cmd=	new MySqlCommand(sql_get_target_string, connection);

			}
			catch (Exception ex)
			{
			
			}

			MySqlDataReader reader1 = null;

			try
			{
				reader1 = cmd.ExecuteReader();
			}
			catch (Exception ex)
			{

			}

			while(!reader1.IsClosed && reader1.Read())
			{
				string temp = reader1[0].ToString();
				try
				{
					target = Convert.ToInt32( temp );
				}
				catch(Exception ex)
				{
					target = 1;
				}
			}

			reader1.Close();


			 int current_sales = get_cumiliatvie_sales_of_an_employee_in_the_current_month();

			 if(target-current_sales <=0)
			{
				return -2;
			}

			return target - current_sales;
		
		}






		public Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>> get_all_store_details()
		{
			if (!is_conn_open)
				return null;

			string get_stores_cmd = String.Format("select * from store_list;");

			MySqlCommand cmd = null;

			try
			{ 
			cmd  =	new MySqlCommand(get_stores_cmd, connection);
		    }
			catch(Exception e) 
			{ 

			}

		  try
			{
				cmd.ExecuteNonQuery();
			}
			catch(Exception ex)
			{

			}

			MySqlDataReader reader = null;

			try
			{
				reader = cmd.ExecuteReader();
			}
			catch 
			{ 
			
			}





			Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>> state_dic = new Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>>();

			while(!reader.IsClosed && reader.Read())
			{
				Dictionary<string, string> store_details_dic = new Dictionary<string, string>();
				store_details_dic["sno"] = reader[0].ToString();
				store_details_dic["store_name"] = reader[1].ToString();
				store_details_dic["store_manager"] = reader[2].ToString();
				store_details_dic["contact_no"] = reader[3].ToString();
				store_details_dic["area"] = reader[4].ToString();
				store_details_dic["state"] = reader[5].ToString();
				store_details_dic["ba_name"] = reader[6].ToString();
				store_details_dic["target"] = reader[7].ToString();

			

				if (state_dic.ContainsKey(reader[5].ToString()))
				{

					if( state_dic[reader[5].ToString()].ContainsKey(reader[4].ToString())  )
					{
						state_dic[reader[5].ToString()][reader[4].ToString()].Add(store_details_dic);
					}
					else
					{
						List<Dictionary<string, string>> area_list = new List<Dictionary<string, string>>();
						area_list.Add(store_details_dic);
						state_dic[reader[5].ToString()][reader[4].ToString()]=area_list;	
					}
							
				}
				else
				{
					state_dic[reader[5].ToString()] = new Dictionary<string, List<Dictionary<string, string>>>();
				}

			}
		

			return state_dic;
		}

		public List<Dictionary<string,string>> get_employee_recent_sales_on_the_day(string today)
		{
			if (!is_conn_open)
				return null;

			string get_recent_sales_query = String.Format("select * from products2,employee_sales2 where products2.sno=employee_sales2.sno and employee_sales2.emp_id = '{0}' and employee_sales2.The_date= '{1}' order by The_time desc;", emp_id2,today);

			MySqlCommand get_recent_sales_cmd = null;
			try
			{
				get_recent_sales_cmd = new MySqlCommand(get_recent_sales_query, connection);
			}
			catch(Exception ex)
			{

			}

			try
			{
				get_recent_sales_cmd.ExecuteNonQuery();
			}
			catch (Exception ex)
			{

			}

			MySqlDataReader reader = null;
			
			try
			{
				reader = get_recent_sales_cmd.ExecuteReader();
			}
			catch (Exception ex)
			{
				return null;
			}

			List<Dictionary<string, string>> items = new List<Dictionary<string, string>>();

			while(!reader.IsClosed && reader.Read())
			{
			  
				Dictionary<string,string> item = new Dictionary<string,string>();

				item["sno"] = reader[0].ToString();
				item["product"] = reader[1].ToString();
				item["product_price"] = reader[3].ToString();
				item["emp_id"] = reader[4].ToString();
				item["pcs"] = reader[6].ToString();
				item["your_sale"] = reader[7].ToString();
				item["sold_time"] = reader[8].ToString();
				item["sold_date"] = reader[9].ToString();

				Dictionary<string, Dictionary<string, string>> item_with_name = new Dictionary<string, Dictionary<string, string>>();

				

				items.Add(item);

			}
		


			return items;
		}




		public bool check_admin(string employee_id)
		{
			if (is_conn_open != true)
				return false;
			


			string sql_cmd_string = "select * from admin where employee_id='" + employee_id + "';";

			MySqlCommand mySqlCommand = new MySqlCommand(sql_cmd_string, connection);



			try
			{
				mySqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
			}

			MySqlDataReader reader = mySqlCommand.ExecuteReader();

			if (reader.HasRows)
			{
				
				return true;
			}

	
			return false;

		}


	}
}
