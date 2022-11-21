using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityNpgsql;
using static SQL;

public class PointCache : MonoBehaviour
{
    string id;
    string pw;
    [SerializeField]
    TMP_Dropdown dropdown;

    GraphicsBuffer positionsBuffer;
    NpgsqlConnection connection;
    int pointCount;
    
    public void setID(string id)
    {
        this.id = id;
    }

    public void setPW(string pw)
    {
        this.pw = pw;
    }


    const double minX = 3050128;
    const double minY = 1648970;
    const double averageX = 3162032.5940437117 - minX; //~ 111904.594044
    const double averageY = 1694945.4596180187 - minY; //~ 45975.459618 
    const float normalizerx = (float)averageX;
    const float normalizery = (float)averageY;


    ISet<int> years = new HashSet<int>();

    CrimePoint[] vertexData;

    [SerializeField]
    int currentPointer;


    int effectBufferID = Shader.PropertyToID("GraphicsBuffer");
    int effectTexture = Shader.PropertyToID("Texture2D");


    [SerializeField]
    VisualEffect visualEffect;


    public void SetVFXYear(int year)
    {
        visualEffect.SetInt("Year", year);
    }

    void CreateConnection() {
        var builder = new NpgsqlConnectionStringBuilder();
        builder.Port = 5433;
        builder.Database = "csci403";
        builder.UserName = id;
        builder.Password = pw;
        builder.Host = "codd.mines.edu";

        connection = new NpgsqlConnection(builder);
        connection.Open();

        //Set the search path to f22group20
        var command = new NpgsqlCommand("set search_path TO f22_group20;", connection);
        command.Prepare();
        command.ExecuteNonQuery();
    }

    NpgsqlConnection GetOpenConnection()
    {
        if (connection == null)
            CreateConnection();

        if (connection.State == ConnectionState.Closed)
            CreateConnection();

        return connection;
    }

    void CloseConnection()
    {
        connection.Close();
    }


    //Dictionary<string, int> name_idMap = new Dictionary<string, int>();
    //Dictionary<int, string> id_nameMap = new Dictionary<int, string>();


    void GetCrimeTypes()
    {
        CloseConnection();
        var command = new NpgsqlCommand(
           "SELECT id, offense_type_name FROM offense_codes;", GetOpenConnection()
           );
        command.Prepare();
        var reader = command.ExecuteReader(CommandBehavior.CloseConnection);

        List<string> options = new List<string>();


        while (reader.Read())
        {
            options.Add(reader.GetString(1));
            //id_nameMap.Add(reader.GetInt32(0), reader.GetString(1));
            //name_idMap.Add(reader.GetString(1), reader.GetInt32(0));
        }

        dropdown.AddOptions(options);
    }

    public void SetCrimeType(int dropdownSelection)
    {
        //Dropdowns add an extra index to their selection.
        visualEffect.SetInt("Type", dropdownSelection+1);
    }


    //Initialize the buffer with the number of crimes in the table.
    void InitBuffer()
    {
        var command = new NpgsqlCommand(
            "SELECT count(*) FROM crimes;", GetOpenConnection()
            );
        command.Prepare();
        var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
        reader.Read();
        int pointCount = (int)reader.GetInt64(0);



        positionsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, pointCount, 20);
        vertexData = new CrimePoint[pointCount];
        currentPointer = 0;
    }
    
    public void GetPointsDropDownYear(int drowdownSelection)
    {
        int year = drowdownSelection + 2017;
        SetVFXYear(year);
        GetPoints(year);
    }

    //Paremeterized SELECT for points based on year.
    public void GetPoints(int year)
    {
        SetVFXYear(year);
        Debug.Log("Getting points for year:" + year);
        //First check if it's already been cached. If so then do nothing.
        if (years.Contains(year))
            return;

        //If not then add the year and the corresponding points.
        years.Add(year);

        CloseConnection();

        var command = new NpgsqlCommand(
         "SELECT geo_x, geo_y, offense_code_id FROM crimes, crime_locations WHERE location_id = id AND EXTRACT(YEAR FROM first_occurrence_date) = @p", GetOpenConnection());
        var p = command.Parameters.Add("p", UnityNpgsqlTypes.NpgsqlDbType.Integer);
        command.Prepare();
        p.Value = year;

        var reader = command.ExecuteReader(CommandBehavior.CloseConnection);

        while (reader.Read())
        {
            //Approximate normalization.
            float normalizedX = (float)((reader.GetDouble(0)- minX) / averageX);
            float normalizedY = (float)((reader.GetDouble(1) -minY) / averageY);

            Vector3 position = new Vector3(normalizedX, normalizedY, 0);
            vertexData[currentPointer] = new CrimePoint(position, year, reader.GetInt32(2));
            currentPointer++;
        }

        positionsBuffer.SetData(vertexData);
    }

    


    // Start is called before the first frame update
    void Start()
    {
        InitBuffer();

        //Unfortunately the buffer is garbage collected unless I do this. :(
        //GetPoints(2017);
        //GetPoints(2018);
        //GetPoints(2019);
        //GetPoints(2020);
        //GetPoints(2021);
        //GetPoints(2022);

        GetCrimeTypes();

        visualEffect.SetGraphicsBuffer(effectBufferID, positionsBuffer);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
