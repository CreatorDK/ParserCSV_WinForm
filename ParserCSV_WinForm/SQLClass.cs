using System;
using System.Data.SqlClient;
using System.Data;

namespace ParserCSV_WinForm
{
    public static class SQLClass
    {
        public static SqlConnection sqlcn = null;
        public static SqlCommand sqlcmd1 = null;
        public static SqlCommand sqlcmd2 = null;
        public static SqlCommand sqlcmd3 = null;
        public static SqlCommand sqlcmd4 = null;
        public static SqlCommand sqlcmd5 = null;
        public static SqlCommand sqlcmd6 = null;

        public static void Inntitialize(string conection_string)
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

        public static bool TestSql()
        {
            try
            {
                sqlcn.Open();
                return true;
            }
            catch(SqlException)
            {
                return false;
            }
        }

        public static void Connect()
        {
            if(sqlcn.State != System.Data.ConnectionState.Open)
               sqlcn.Open();
        }

        public static void Disconnect()
        {
            if (sqlcn.State == System.Data.ConnectionState.Open)
                sqlcn.Close();
        }

        public static int InsertRegion(string region)
        {
            int region_id;
            sqlcmd1.Parameters["@region"].Value = region;
            sqlcmd1.ExecuteNonQuery();
            region_id = Convert.ToInt32(sqlcmd1.Parameters["@region_id"].Value);
            return region_id;
        }

        public static int InsertArea(string area, int region_id)
        {
            int area_id;
           
            sqlcmd2.Parameters["@area"].Value = area;
            sqlcmd2.Parameters["@region_id"].Value = region_id;
            sqlcmd2.ExecuteNonQuery();
            area_id = Convert.ToInt32(sqlcmd2.Parameters["@area_id"].Value);
            return area_id;
        }

        public static int InsertLocality(string locality, int region_id, int area_id)
        {
            int locality_id;
            sqlcmd3.Parameters["@locality"].Value = locality;
            sqlcmd3.Parameters["@region_id"].Value = region_id;
            sqlcmd3.Parameters["@area_id"].Value = area_id;
            sqlcmd3.ExecuteNonQuery();
            locality_id = Convert.ToInt32(sqlcmd3.Parameters["@locality_id"].Value);
            return locality_id;
        }

        public static int InsertIndex(string index)
        {
            int index_id;
            sqlcmd4.Parameters["@index"].Value = index;
            sqlcmd4.ExecuteNonQuery();
            index_id = Convert.ToInt32(sqlcmd4.Parameters["@index_id"].Value);
            return index_id;
        }

        public static int InsertStreet(string street, int locality_id, int index_id)
        {
            int street_id;
            sqlcmd5.Parameters["@street"].Value = street;
            sqlcmd5.Parameters["@locality_id"].Value = locality_id;
            sqlcmd5.Parameters["@index_id"].Value = index_id;
            sqlcmd5.ExecuteNonQuery();
            street_id = Convert.ToInt32(sqlcmd5.Parameters["@street_id"].Value);
            return street_id;
        }

        public static int InsertBulding(string building, int street_id)
        {
            int building_id;
            sqlcmd6.Parameters["@building"].Value = building;
            sqlcmd6.Parameters["@street_id"].Value = street_id;
            sqlcmd6.ExecuteNonQuery();
            building_id = Convert.ToInt32(sqlcmd6.Parameters["@building_id"].Value);
            return building_id;
        }
    }
}
