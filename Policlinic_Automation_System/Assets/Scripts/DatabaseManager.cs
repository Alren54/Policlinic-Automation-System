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
                    int counter = 0;
                    List<Tuple<string, string, DateTime, int, float?>> rendezvousList1 = new();
                    while (reader.Read())
                    {
                        counter++;
                        //reader.GetInt32(reader.GetOrdinal("user_id"));
                        switch (todo)
                        {
                            case 0: //Insert, Delete (Non-Return Queries)
                                return null;
                            case 1: //Login Verification
                                Tuple<bool, string, string, int> a = new Tuple<bool, string, string, int>
                                    (true, reader.GetString(reader.GetOrdinal("Ad")), reader.GetString(reader.GetOrdinal("Soyad")), reader.GetInt32(reader.GetOrdinal("Hasta_id")));
                                return a;
                            case 2: //Patient ID
                                return reader.GetInt32(0);
                            case 3: //Rendezvous v1 -> iptal, ode, randevu al
                                rendezvousList1.Add(new Tuple<string, string, DateTime, int, float?>(reader.GetString(0), reader.GetString(1), reader.GetDateTime(2), reader.GetInt32(3), null));
                                break;
                            case 4: //Rendezvous v2 -> with money
                                rendezvousList1.Add(new Tuple<string, string, DateTime, int, float?>(reader.GetString(0), reader.GetString(1), reader.GetDateTime(2), reader.GetInt32(3), reader.GetFloat(4)));
                                break;
                            case 5: //For Counters

                                break;
                            case 6:

                                break;
                            default:
                                break;
                        }
                    }
                    if (todo == 3 || todo == 4)
                    {
                        return rendezvousList1;
                    }
                    else if (todo == 5)
                    {
                        return counter;
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

