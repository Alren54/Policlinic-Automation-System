using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Npgsql;
using System;
using TMPro;

public class DatabaseManager
{
    private const string connectionString = "Host=127.0.0.1:8086;Username=postgres;Password=123456;Database=DermanDB;";
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
                        //reader.GetInt32(reader.GetOrdinal("user_id"));
                        switch (todo)
                        {
                            case 0: //Insert, Delete Queries
                                return null;
                            case 1: //Login Verification
                                Tuple<bool, string, string> a = new Tuple<bool, string, string>(true, reader.GetString(reader.GetOrdinal("ad")), reader.GetString(reader.GetOrdinal("soyad")));
                                return a;
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

