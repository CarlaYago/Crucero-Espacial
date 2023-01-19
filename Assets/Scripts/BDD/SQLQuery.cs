using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Globalization;

public class SQLQuery
{
    IDbConnection dbconn; // Connection to database
    IDbCommand dbcmd; // SQL query
    IDataReader reader; // Read values from database

    public SQLQuery(string bdd)
    {
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/" + bdd + ".db"; ;
        dbconn = new SqliteConnection(conn);
        dbconn.Open();
    }

    public void Query(string command)
    {
        dbcmd = dbconn.CreateCommand();
        dbcmd.CommandText = command;
        RunQuery();
        CloseReader();
    }

    public List<bool> BoolReader(int pos)
    {
        RunQuery();
        List<bool> values = new List<bool>();

        while (reader.Read())
        {
            bool value = true;

            if (!reader.IsDBNull(pos - 1))
            {
                int number = reader.GetInt32(pos - 1);
                if (number == 0) value = false;
            }

            values.Add(value);
        }

        CloseReader();
        return values;
    }

    public Dictionary<int, bool> BoolReaderID(int posID, int posBool)
    {
        RunQuery();
        Dictionary<int, bool> values = new Dictionary<int, bool>();

        while (reader.Read())
        {
            bool value = true;

            if (!reader.IsDBNull(posBool - 1))
            {
                int key = reader.GetInt32(posID - 1);

                int number = reader.GetInt32(posBool - 1);
                if (number == 0) value = false;

                values.Add(key, value);
            }
        }

        CloseReader();
        return values;
    }

    public List<int> IntReader(int pos)
    {
        RunQuery();
        List<int> values = new List<int>();

        while (reader.Read())
        {
            int value = -1;

            if (!reader.IsDBNull(pos - 1))
            {
                value = reader.GetInt32(pos - 1);
            }

            values.Add(value);
        }

        CloseReader();
        return values;
    }

    public Dictionary<int, int> IntReaderID(int posID, int posInt)
    {
        RunQuery();
        Dictionary<int, int> values = new Dictionary<int, int>();

        while (reader.Read())
        {
            if (!reader.IsDBNull(posInt - 1))
            {
                int key = reader.GetInt32(posID - 1);
                int value = reader.GetInt32(posInt - 1);

                values.Add(key, value);
            }
        }

        CloseReader();
        return values;
    }

    public List<int[]> IntArrayReader(int posArray, char separator = '/')
    {
        RunQuery();
        List<int[]> values = new List<int[]>();

        while (reader.Read())
        {
            string[] stringArray = null;

            if (!reader.IsDBNull(posArray - 1))
            {
                string value = reader.GetString(posArray - 1);
                stringArray = value.Split(separator);
            }

            int[] array = new int[stringArray.Length];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = int.Parse(stringArray[i]);
            }

            values.Add(array);
        }

        CloseReader();
        return values;
    }

    public Dictionary<int, int[]> IntArrayReaderID(int posID, int posArray, char separator = '/')
    {
        RunQuery();
        Dictionary<int, int[]> values = new Dictionary<int, int[]>();

        while (reader.Read())
        {
            if (!reader.IsDBNull(posArray - 1))
            {
                int key = reader.GetInt32(posID - 1);

                string value = reader.GetString(posArray - 1);
                string[] stringArray = value.Split(separator);

                int[] array = new int[stringArray.Length];

                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = int.Parse(stringArray[i]);
                }

                values.Add(key, array);
            }
        }

        CloseReader();
        return values;
    }

    public List<float[]> FloatArrayReader(int posArray, char separator = '/')
    {
        RunQuery();
        List<float[]> values = new List<float[]>();

        while (reader.Read())
        {
            string[] stringArray = null;

            if (!reader.IsDBNull(posArray - 1))
            {
                string value = reader.GetString(posArray - 1);
                stringArray = value.Split(separator);
            }

            float[] array = new float[stringArray.Length];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = float.Parse(stringArray[i], CultureInfo.InvariantCulture);
            }

            values.Add(array);
        }

        CloseReader();
        return values;
    }

    public int Count()
    {
        RunQuery();
        int count = 0;

        while (reader.Read())
        {
            count++;
        }

        CloseReader();
        return count;
    }

    public List<string> StringReader(int pos)
    {
        RunQuery();
        List<string> values = new List<string>();

        while (reader.Read())
        {
            string value = null;

            if (!reader.IsDBNull(pos - 1))
            {
                value = reader.GetString(pos - 1);
            }

            values.Add(value);
        }

        CloseReader();
        return values;
    }

    public Dictionary<int, string> StringReaderID(int posID, int posString)
    {
        RunQuery();
        Dictionary<int, string> values = new Dictionary<int, string>();

        while (reader.Read())
        {
            int key = 0;
            string value = null;

            if (!reader.IsDBNull(posID - 1))
            {
                key = reader.GetInt32(posID - 1);
                value = reader.GetString(posString - 1);
            }

            if (key != 0)
                values.Add(key, value);
        }

        CloseReader();
        return values;
    }

    public List<string[]> StringArrayReader(int posArray, char separator = '/')
    {
        RunQuery();
        List<string[]> values = new List<string[]>();

        while (reader.Read())
        {
            string[] array = null;

            if (!reader.IsDBNull(posArray - 1))
            {
                string value = reader.GetString(posArray - 1);
                array = value.Split(separator);
            }

            values.Add(array);
        }

        CloseReader();
        return values;
    }

    public Dictionary<int, string[]> StringArrayReaderID(int posID, int posArray, char separator = '/')
    {
        RunQuery();
        Dictionary<int, string[]> values = new Dictionary<int, string[]>();

        while (reader.Read())
        {
            int key = 0;
            string[] array = null;

            if (!reader.IsDBNull(posArray - 1))
            {
                key = reader.GetInt32(posID - 1);
                string value = reader.GetString(posArray - 1);
                array = value.Split(separator);
            }

            if (key != 0)
                values.Add(key, array);
        }

        CloseReader();
        return values;
    }

    public Dictionary<int, DateTime> DateTimeReaderID(int posID, int posDateTime)
    {
        RunQuery();
        Dictionary<int, DateTime> values = new Dictionary<int, DateTime>();

        while (reader.Read())
        {
            if (!reader.IsDBNull(posDateTime - 1))
            {
                int key = reader.GetInt32(posID - 1);

                string date = reader.GetString(posDateTime - 1);
                date = date.Replace(" ", "/");
                date = date.Replace(":", "/");

                string[] dateValues = date.Split('/');

                int[] numericDateValues = new int[6];
                for (int i = 0; i < 6; i++)
                {
                    numericDateValues[i] = int.Parse(dateValues[i]);
                }

                DateTime value = new DateTime(numericDateValues[2], numericDateValues[1], numericDateValues[0], numericDateValues[3], numericDateValues[4], numericDateValues[5]);

                values.Add(key, value);
            }
        }

        CloseReader();
        return values;
    }

    public List<Texture2D> ImageReader(int pos, int dimensions)
    {
        RunQuery();
        List<Texture2D> values = new List<Texture2D>();

        while (reader.Read())
        {
            Texture2D value = null;

            if (!reader.IsDBNull(pos - 1))
            {
                byte[] imgData = (byte[])reader[pos - 1];
                value = new Texture2D(dimensions, dimensions);
                value.LoadImage(imgData);
            }

            values.Add(value);
        }

        CloseReader();
        return values;
    }

    public void InsertImage(string table, string column, byte[] data, int id)
    {
        //string query = "INSERT INTO " + table + " (" + column + ") VALUES (@data)";

        string query = "UPDATE " + table + " SET " + column + " = @data WHERE ID_PALABRA = " + id;

        dbcmd = dbconn.CreateCommand();
        dbcmd.CommandText = query;
        SqliteParameter setParam = new SqliteParameter("@data", data);
        dbcmd.Parameters.Add(setParam);

        dbcmd.ExecuteNonQuery();
    }

    void RunQuery()
    {
        reader = dbcmd.ExecuteReader();
    }

    void CloseReader()
    {
        reader.Close();
        reader = null;
    }
}
