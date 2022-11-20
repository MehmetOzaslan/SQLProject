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
    public string id;
    public string pw;

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

        var command = new NpgsqlCommand("SELECT count(*) FROM crimes;", connection);
        command.Prepare();
        var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
        
        Assert.True(reader.HasRows);
        while (reader.Read())
        {
            Debug.Log(reader.GetInt64(0));
        }


    }


    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator ConnectionTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
