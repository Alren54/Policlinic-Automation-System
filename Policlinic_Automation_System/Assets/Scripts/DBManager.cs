using Npgsql;
using System.Data;
using UnityEngine;

public class DBManager : MonoBehaviour
{
    public DatabaseManager dbManager { get; set; }
    public NpgsqlConnection dbConnection { get; set; }

    void Start()
    {
        dbManager = new DatabaseManager();
        dbConnection = dbManager.Connect();
    }

    void OnDestroy()
    {
        if (dbConnection != null && dbConnection.State == ConnectionState.Open)
        {
            dbConnection.Close();
            Debug.Log("Disconnected from PostgreSQL");
        }
    }
}
