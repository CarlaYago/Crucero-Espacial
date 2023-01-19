using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Text.RegularExpressions;

public class BDDLexicoESP : MonoBehaviour
{
    public TextAsset lexico;
    public Lexico_SO lex_SO;
    public bool vaciarSO, rellenarSO, vaciarBDD, rellenarBDD;

    void Start()
    {
        if (vaciarSO) lex_SO.diccionario.Clear();
        if (rellenarSO) LeerLexico();

        if (vaciarBDD) EliminarRegistros();
        if (rellenarBDD) IntroducirLexico(lex_SO.diccionario);
    }

    void LeerLexico() // Lee el léxico del .txt y lo añade a un ScriptableObject
    {
        string lex = lexico.text;

        lex = Regex.Replace(lex, @"\s", ""); // Quitar todos los espacios
        lex = lex.Replace("*", ""); // Quitar referencia de inicio de fila (*)

        lex = lex.Replace("n~", "ñ"); // Leer letras ñ (n~)
        lex = lex.Replace(@"\", ""); // Quitar "acentos" (\)

        lex = lex.ToUpper(); // Convertir a mayúsculas

        lex_SO.diccionario.AddRange(lex.Split('#')); // Separar por referencia de final de fila (#)
        lex_SO.diccionario.RemoveAt(lex_SO.diccionario.Count); // Borrar último registro (vacío)

        EliminarRepetidas();
    }

    void EliminarRepetidas() // Borra las palabras repetidas (por falta de acentos)
    {
        for (int i = 0; i < lex_SO.diccionario.Count; i++)
        {
            for (int n = 0; n < lex_SO.diccionario.Count; n++)
            {
                if ((lex_SO.diccionario[i] == lex_SO.diccionario[n]) && (i != n))
                {
                    lex_SO.diccionario.RemoveAt(n);
                }
            }
        }
    }

    void IntroducirLexico(List<string> palabras) // Añade el léxico del ScriptableObject a la base de datos
    {
        string conn = "URI=file:" + Application.dataPath + "/BDD/LexicoEsp.db"; // Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); // Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        for (int i = 0; i < palabras.Count; i++)
        {
            string sqlQuery = "INSERT INTO LexicoEsp (Palabra) " + "VALUES ( '" + palabras[i] + "' )";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            reader.Close();
            reader = null;
            Debug.Log(i);
        }

        #region End Query

        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        #endregion End Query
    }

    void EliminarRegistros() // Vacía la tabla de la base de datos y reseta el autoincremento 
    {
        string conn = "URI=file:" + Application.dataPath + "/BDD/LexicoEsp.db"; // Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); // Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "DELETE FROM LexicoEsp; " + "DELETE FROM sqlite_sequence WHERE name = 'LexicoEsp';";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

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
