using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Npgsql;
using System;
using TMPro;
using System.Text;
using Unity.VisualScripting.FullSerializer;

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
                    List<Tuple<string, string, DateTime, int, float?, bool>> debtsList = new();
                    List<Tuple<string, List<DateTime>, int>> rendezvousList2 = new();
                    List<string> strInputs = new();
                    List<DateTime> rendezvousDates = new List<DateTime> {
                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0),
                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 30, 0),
                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 0, 0),
                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 30, 0),
                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 0, 0),
                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 30, 0),
                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0),
                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 30, 0),
                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 13, 0, 0),
                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 13, 30, 0),
                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 0, 0),
                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 30, 0),
                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 15, 0, 0),
                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 15, 30, 0),
                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 0, 0)
                    };
                    Tuple<string, DateTime, int> rendezvous = null;
                    Tuple<string, DateTime, int> oldRendezvous = null;
                    int tempInt = -1;
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
                            case 6: //For Taking Rendezvous
                                rendezvous = new Tuple<string, DateTime, int>(reader.GetString(0), reader.GetDateTime(1), reader.GetInt32(2));
                                if (tempInt == -1)
                                {
                                    oldRendezvous = new Tuple<string, DateTime, int>(reader.GetString(0), reader.GetDateTime(1), reader.GetInt32(2));
                                    tempInt = oldRendezvous.Item3;
                                }

                                if (tempInt == rendezvous.Item3)
                                {
                                    rendezvousDates.Remove(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, rendezvous.Item2.Hour, rendezvous.Item2.Minute, rendezvous.Item2.Second));
                                }
                                else if (tempInt != rendezvous.Item3)
                                {
                                    rendezvousList2.Add(new(oldRendezvous.Item1, rendezvousDates, oldRendezvous.Item3));
                                    oldRendezvous = new Tuple<string, DateTime, int>(reader.GetString(0), reader.GetDateTime(1), reader.GetInt32(2));
                                    rendezvousDates = new List<DateTime> {
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0),
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 30, 0),
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 0, 0),
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 30, 0),
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 0, 0),
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 30, 0),
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0),
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 30, 0),
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 13, 0, 0),
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 13, 30, 0),
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 0, 0),
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 30, 0),
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 15, 0, 0),
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 15, 30, 0),
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 0, 0)
                                    };
                                    rendezvousDates.Remove(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, rendezvous.Item2.Hour, rendezvous.Item2.Minute, rendezvous.Item2.Second));
                                    Debug.Log(oldRendezvous.Item1);
                                    Debug.Log(oldRendezvous.Item3);
                                    tempInt = oldRendezvous.Item3;
                                }
                                tempInt = rendezvous.Item3;
                                break;
                            case 7:
                                debtsList.Add(new Tuple<string, string, DateTime, int, float?, bool>(reader.GetString(0), reader.GetString(1), reader.GetDateTime(2), reader.GetInt32(3), reader.GetFloat(4), reader.GetBoolean(5)));
                                break;
                            case 8:
                                strInputs.Add(reader.GetString(0));
                                strInputs.Add(reader.GetString(1));
                                strInputs.Add(reader.GetDateTime(2).ToString());
                                strInputs.Add(reader.GetInt32(3).ToString());
                                strInputs.Add(reader.GetString(4));
                                strInputs.Add(reader.GetString(5));
                                strInputs.Add(reader.GetString(6));
                                strInputs.Add(reader.GetString(7));
                                strInputs.Add(reader.GetFloat(8).ToString());
                                return strInputs;
                            case 9:
                                strInputs.Add(reader.GetString(0));
                                strInputs.Add(reader.GetString(1));
                                strInputs.Add(reader.GetDateTime(2).ToString());
                                strInputs.Add(reader.GetFloat(3).ToString());
                                return strInputs;
                            default:
                                break;
                            case 10:
                                Tuple<bool, string, string, int> b = new Tuple<bool, string, string, int>
                                    (true, reader.GetString(reader.GetOrdinal("Ad")), reader.GetString(reader.GetOrdinal("Soyad")), reader.GetInt32(reader.GetOrdinal("Doktor_id")));
                                return b;
                            case 11:
                                strInputs.Add(reader.GetString(0));
                                strInputs.Add("Tc No: " + reader.GetString(1));
                                strInputs.Add(reader.GetDateTime(2).Date.ToString());
                                strInputs.Add("Randevu ID = " + reader.GetInt32(3).ToString());
                                return strInputs;
                        }
                    }
                    if (todo == 3 || todo == 4) return rendezvousList1;
                    else if (todo == 5) return counter;
                    else if (todo == 6)
                    {
                        if (rendezvous != null) rendezvousList2.Add(new(rendezvous.Item1, rendezvousDates, rendezvous.Item3));
                        return rendezvousList2;
                    }
                    else if (todo == 7) { return debtsList; }
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

