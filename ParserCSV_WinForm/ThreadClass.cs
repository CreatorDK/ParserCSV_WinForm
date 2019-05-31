using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace ParserCSV_WinForm
{
    class ThreadClass
    {
        public int current_thread;
        public int number_of_threads;
        public int total_lines;
        public string current_line;
        public SqlConnection sqlcn = null;
        public SqlCommand sqlcmd1 = null;
        public SqlCommand sqlcmd2 = null;
        public SqlCommand sqlcmd3 = null;
        public SqlCommand sqlcmd4 = null;
        public SqlCommand sqlcmd5 = null;
        public SqlCommand sqlcmd6 = null;

        int region_id;
        int area_id;
        int locality_id;
        int index_id;
        int street_id;
        int building_id;

        public ThreadClass(int current_thread_, int number_of_threads_, int total_lines_) { current_thread = current_thread_; number_of_threads = number_of_threads_; total_lines = total_lines_; }

        public void Inntitialize(string conection_string)
        {
            sqlcn = new SqlConnection(conection_string);

            sqlcmd1 = new SqlCommand();
            sqlcmd1.Connection = sqlcn;
            sqlcmd1.Parameters.Add("@region", SqlDbType.VarChar, 64);
            sqlcmd1.Parameters.Add("@region_id", SqlDbType.Int).Direction = ParameterDirection.Output;
            sqlcmd1.CommandText = "IF NOT EXISTS (Select id from address_regions where name = @region) BEGIN Insert into address_regions (name) values (@region); END; Select @region_id = id from address_regions where name = @region;";

            sqlcmd2 = new SqlCommand();
            sqlcmd2.Connection = sqlcn;
            sqlcmd2.Parameters.Add("@area", SqlDbType.VarChar, 64);
            sqlcmd2.Parameters.Add("@region_id", SqlDbType.Int);
            sqlcmd2.Parameters.Add("@area_id", SqlDbType.Int).Direction = ParameterDirection.Output;
            sqlcmd2.CommandText = "IF NOT EXISTS (Select id from address_areas where name = @area and region_id = @region_id) BEGIN Insert into address_areas (name, region_id) values (@area, @region_id); END; Select @area_id = id from address_areas where name = @area and region_id = @region_id;";

            sqlcmd3 = new SqlCommand();
            sqlcmd3.Connection = sqlcn;
            sqlcmd3.Parameters.Add("@locality", SqlDbType.VarChar, 64);
            sqlcmd3.Parameters.Add("@region_id", SqlDbType.Int);
            sqlcmd3.Parameters.Add("@area_id", SqlDbType.Int);
            sqlcmd3.Parameters.Add("@locality_id", SqlDbType.Int).Direction = ParameterDirection.Output;
            sqlcmd3.CommandText = "IF NOT EXISTS (Select id from address_localities where name = @locality and region_id = @region_id and area_id = @area_id) BEGIN Insert into address_localities (name, region_id, area_id) values (@locality, @region_id, @area_id); END; Select @locality_id = id from address_localities where name = @locality and region_id = @region_id and @area_id = area_id;";

            sqlcmd4 = new SqlCommand();
            sqlcmd4.Connection = sqlcn;
            sqlcmd4.Parameters.Add("@index", SqlDbType.VarChar, 64);
            sqlcmd4.Parameters.Add("@index_id", SqlDbType.Int).Direction = ParameterDirection.Output;
            sqlcmd4.CommandText = "IF NOT EXISTS (Select id from address_indexes where number = @index) BEGIN Insert into address_indexes (number) values (@index); END; Select @index_id = id from address_indexes where number = @index;";

            sqlcmd5 = new SqlCommand();
            sqlcmd5.Connection = sqlcn;
            sqlcmd5.Parameters.Add("@street", SqlDbType.VarChar, 64);
            sqlcmd5.Parameters.Add("@locality_id", SqlDbType.Int);
            sqlcmd5.Parameters.Add("@index_id", SqlDbType.Int);
            sqlcmd5.Parameters.Add("@street_id", SqlDbType.Int).Direction = ParameterDirection.Output;
            sqlcmd5.CommandText = "IF NOT EXISTS (Select id from address_streets where name = @street and locality_id = @locality_id and index_id = @index_id) BEGIN Insert into address_streets (name, locality_id, index_id) values (@street, @locality_id, @index_id); END; Select @street_id = id from address_streets where name = @street and locality_id = @locality_id and index_id = @index_id;";

            sqlcmd6 = new SqlCommand();
            sqlcmd6.Connection = sqlcn;
            sqlcmd6.Parameters.Add("@building", SqlDbType.VarChar, 64);
            sqlcmd6.Parameters.Add("@street_id", SqlDbType.Int);
            sqlcmd6.Parameters.Add("@building_id", SqlDbType.Int).Direction = ParameterDirection.Output;
            sqlcmd6.CommandText = "IF NOT EXISTS (Select id from address_buildings where number = @building and street_id = @street_id) BEGIN Insert into address_buildings (number, street_id) values (@building, @street_id); END; Select @building_id = id from address_buildings where number = @building and street_id = @street_id;";
        }

        public void Connect()
        {
            if (sqlcn.State != System.Data.ConnectionState.Open)
                sqlcn.Open();
        }

        public void Disconnect()
        {
            if (sqlcn.State == System.Data.ConnectionState.Open)
                sqlcn.Close();
        }

        public int InsertRegion(string region)
        {
            int region_id;
            sqlcmd1.Parameters["@region"].Value = region;
            sqlcmd1.ExecuteNonQuery();
            region_id = Convert.ToInt32(sqlcmd1.Parameters["@region_id"].Value);
            return region_id;
        }

        public int InsertArea(string area, int region_id)
        {
            int area_id;

            sqlcmd2.Parameters["@area"].Value = area;
            sqlcmd2.Parameters["@region_id"].Value = region_id;
            sqlcmd2.ExecuteNonQuery();
            area_id = Convert.ToInt32(sqlcmd2.Parameters["@area_id"].Value);
            return area_id;
        }

        public int InsertLocality(string locality, int region_id, int area_id)
        {
            int locality_id;
            sqlcmd3.Parameters["@locality"].Value = locality;
            sqlcmd3.Parameters["@region_id"].Value = region_id;
            sqlcmd3.Parameters["@area_id"].Value = area_id;
            sqlcmd3.ExecuteNonQuery();
            locality_id = Convert.ToInt32(sqlcmd3.Parameters["@locality_id"].Value);
            return locality_id;
        }

        public int InsertIndex(string index)
        {
            int index_id;
            sqlcmd4.Parameters["@index"].Value = index;
            sqlcmd4.ExecuteNonQuery();
            index_id = Convert.ToInt32(sqlcmd4.Parameters["@index_id"].Value);
            return index_id;
        }

        public int InsertStreet(string street, int locality_id, int index_id)
        {
            int street_id;
            sqlcmd5.Parameters["@street"].Value = street;
            sqlcmd5.Parameters["@locality_id"].Value = locality_id;
            sqlcmd5.Parameters["@index_id"].Value = index_id;
            sqlcmd5.ExecuteNonQuery();
            street_id = Convert.ToInt32(sqlcmd5.Parameters["@street_id"].Value);
            return street_id;
        }

        public int InsertBulding(string building, int street_id)
        {
            int building_id;
            sqlcmd6.Parameters["@building"].Value = building;
            sqlcmd6.Parameters["@street_id"].Value = street_id;
            sqlcmd6.ExecuteNonQuery();
            building_id = Convert.ToInt32(sqlcmd6.Parameters["@building_id"].Value);
            return building_id;
        }


        public void Parse()
        {
            object obj = new object();

            Form1 form = Form1.form;

            //Берем строку из массива
            for(int j = current_thread; j <= total_lines; j = j + number_of_threads)
            {
                lock(obj)
                {
                    current_line = StaticClass.work_lines[j];
                }

                string[] fields = current_line.Split(';');

                for (int i = 0; i <= fields.Length - 1; i++)
                {
                    switch (i)
                    {
                        case 0:
                            try
                            {
                                region_id = InsertRegion(fields[i]);
                            }
                            catch (SqlException ex)
                            {
                                form.ShowErrorMessage("Невозможно добавить запись в таблицу Областей. Подробнее:\n" + ex.Message, "Ошибка");
                            }
                            break;
                        case 1:
                            try
                            {
                                area_id = InsertArea(fields[i], region_id);
                            }
                            catch (SqlException ex)
                            {
                                form.ShowErrorMessage("Невозможно добавить запись в таблицу Районов. Подробнее:\n" + ex.Message, "Ошибка");
                            }
                            break;
                        case 2:
                            try
                            {
                                locality_id = InsertLocality(fields[i], region_id, area_id);
                            }
                            catch (SqlException ex)
                            {
                                form.ShowErrorMessage("Невозможно добавить запись в таблицу Населенных Пунктов. Подробнее:\n" + ex.Message, "Ошибка");
                            }
                            break;
                        case 3:
                            try
                            {
                                index_id = InsertIndex(fields[i]);
                            }
                            catch (SqlException ex)
                            {
                                form.ShowErrorMessage("Невозможно добавить запись в таблицу Почтовых Индексов. Подробнее:\n" + ex.Message, "Ошибка");
                            }
                            break;
                        case 4:
                            try
                            {
                                street_id = InsertStreet(fields[i], locality_id, index_id);
                            }
                            catch (SqlException ex)
                            {
                                form.ShowErrorMessage("Невозможно добавить запись в таблицу Улиц. Подробнее:\n" + ex.Message, "Ошибка");
                            }
                            break;
                        case 5:
                            string[] streets_list = fields[i].Split(',');
                            foreach (string street in streets_list)
                            {
                                try
                                {
                                    building_id = InsertBulding(street, street_id);
                                }
                                catch (SqlException ex)
                                {
                                    form.ShowErrorMessage("Невозможно добавить запись в таблицу Номера дома. Подробнее:\n" + ex.Message, "Ошибка");
                                }

                            }
                            break;
                    }
                }

                lock (obj)
                {
                    StaticClass.current_line++;
                    double percent;
                    percent = ((double)StaticClass.current_line / (double)total_lines) * 100.0;
                    if (percent > 100.0)
                    {
                        percent = 100.0;
                    }

                    //Заполняем прогрес-бар
                    if (form.progressBar1.InvokeRequired)
                    {
                        form.progressBar1.Invoke(new Action(() => { form.progressBar1.Value = Convert.ToInt32(percent); }));
                    }
                    else
                    {
                        form.progressBar1.Value = Convert.ToInt32(percent);
                    }

                    if (form.label_status.InvokeRequired)
                    {
                        form.label_status.Invoke(new Action(() => { form.label_status.Text = "Строка: " + StaticClass.current_line + "/" + StaticClass.total_lines; }));
                    }
                    else
                    {
                        form.label_status.Text = "Строка: " + StaticClass.current_line + "/" + StaticClass.total_lines;
                    }
                }
            }
        }
        
    }
}
