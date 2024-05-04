using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Npgsql;
using System;
using TMPro;

public class DatabaseManager
{
    private const string connectionString = "Host=127.0.0.1:5432;Username=postgres;Password=123456;Database=DermanDB;";
    public NpgsqlConnection Connect()
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        try
        {
            connection.Open();
            Debug.Log("Connected to PostgreSQL");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error: {ex.Message}");
        }

        return connection;
    }

    public object ExecuteQuery(int todo, NpgsqlConnection connection, string query, params object[] parameters)
    {
        try
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@param{i + 1}", parameters[i]);
                    }
                }
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        switch (todo)
                        {
                            case 0: //ID
                                return reader.GetInt32(reader.GetOrdinal("user_id"));
                            default:
                                break;
                        }
                    }
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error executing query: {ex.Message}");
            return null;
        }
    }
}

