using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityNpgsql;

public class ConnectionTest
{
    [SerializeField]
    public string id = "";
    [SerializeField]
    public string pw = "";

    // A Test behaves as an ordinary method
    [Test]
    public void ConnectionTestSimplePasses()
    {
        var builder = new NpgsqlConnectionStringBuilder();
        builder.Port = 5433;
        builder.Database = "csci403";
        builder.UserName = id;
        builder.Password = pw;
        builder.Host = "codd.mines.edu";

        var connection = new NpgsqlConnection(builder);
        connection.Open();

        Assert.AreEqual(connection.FullState, System.Data.ConnectionState.Open);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void TestSelectStatement()
    {

        var builder = new NpgsqlConnectionStringBuilder();
        builder.Port = 5433;
        builder.Database = "csci403";
        builder.UserName = id;
        builder.Password = pw;
        builder.Host = "codd.mines.edu";


        var connection = new NpgsqlConnection(builder);
        connection.Open();

        var command = new NpgsqlCommand("SELECT * FROM music;", connection);
        command.Prepare();
        var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
        
        Assert.True(reader.HasRows);
        while (reader.Read())
        {
            Debug.Log(reader.GetString(0));
        }
    }

    // A Test behaves as an ordinary method
    [Test]
    public void TestCrimeGeoLocation()
    {

        var builder = new NpgsqlConnectionStringBuilder();
        builder.Port = 5433;
        builder.Database = "csci403";
        builder.UserName = id;
        builder.Password = pw;
        builder.Host = "codd.mines.edu";


        var connection = new NpgsqlConnection(builder);
        connection.Open();

        var command = new NpgsqlCommand("set search_path TO f22_group20;", connection);
        command.Prepare();
        command.ExecuteNonQuery();

        command = new NpgsqlCommand(
         "SELECT geo_x, geo_y FROM crimes, crime_locations WHERE location_id = id AND EXTRACT(YEAR FROM first_occurrence_date) = 2021; ", connection);

        command.Prepare();

        var reader = command.ExecuteReader(CommandBehavior.CloseConnection);

        Assert.True(reader.HasRows);
        while (reader.Read())
        {
            Debug.Log(reader.GetDouble(0) +", " + reader.GetDouble(1));
        }
    }


}
