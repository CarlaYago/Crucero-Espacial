using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class AccessBDD : MonoBehaviour
{
    void Start()
    {
        #region Connect to Database

        string conn = "URI=file:" + Application.dataPath + "/BDD/Prueba.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        #endregion Connect to Database

        //---------------------------------------------

        #region Query

        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT ID, Nombre, Puntos " + "FROM Usuarios";
        dbcmd.CommandText = sqlQuery;

        #endregion Query

        //---------------------------------------------

        #region Get Values

        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            int value = reader.GetInt32(0);
            string name = reader.GetString(1);
            int rand = reader.GetInt32(2);

            Debug.Log("value= " + value + "  name =" + name + "  random =" + rand);
        }

        #endregion Get Values

        //---------------------------------------------

        #region End Query

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        #endregion End Query
    }
}
