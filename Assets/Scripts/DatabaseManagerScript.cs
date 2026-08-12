using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using UnityEngine;

public class DatabaseManagerScript : MonoBehaviour
{
    [SerializeField] private bool isEndLevel;
    
    public static Action<string> SliceTheQuery;
    public static Action<string> TestEndLeve;
    
    private static readonly string _host = "localhost";
    private static readonly string _user = "postgres";
    private static readonly string _password = "admin";
    private static readonly string _database = "postgres";
    private static readonly int _port = 5432;

    private static string ConnectionString => 
        $"Host={_host};Port={_port};Username={_user};Password={_password};Database={_database}";

    private async void Start()
    {
        try
        {
            if (InputScript.IsSelectedLevel || SpawningSystemScript.RestartingLevel || isEndLevel) return;
            
            var query = await FetchQueryFromDatabaseAsync();
            // TestEndLeve?.Invoke(query);
            SliceTheQuery?.Invoke(query);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to fetch data on Start: {ex.Message}");
        }
    }

    private async Task<string> FetchQueryFromDatabaseAsync()
    {
        var query = "";

        try
        {
            // 1. Create connection using NpgsqlConnection
            await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            // 2. Define your SQL Query
            string sqlQuery = "SELECT correct_query FROM game_queries ORDER BY RANDOM() LIMIT 1";

            // 3. Prepare Command and Parameters (Prevents SQL Injection attacks)
            await using NpgsqlCommand cmd = new NpgsqlCommand(sqlQuery, conn);

            // 4. Execute Reader
            await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            // 5. Read row by row
            while (await reader.ReadAsync())
            {
                query = reader.GetString(0);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Database connection error: {ex.Message}");
        }

        return query;
    }
    
    public static async Task<bool> CheckIfQueryIsCorrectAsync(string playerMadeQuery)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(playerMadeQuery))
            {
                Debug.LogWarning("Query is empty! Please type a valid SQL query.");
                return false;
            }
            
            await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            await using NpgsqlCommand cmd = new NpgsqlCommand(playerMadeQuery, conn);
            
            await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Database connection error: {ex.Message}");
            return false;
        }

        return true;
    }
}