#load "..\CiqsHelpers\All.csx"

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.IO;
using System.Data.SqlClient;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    try
    {
        var parametersReader = await CiqsInputParametersReader.FromHttpRequestMessage(req);
    
        string sqlServerName = parametersReader.GetParameter<string>("sqlServer"); 
        string sqlDbName = parametersReader.GetParameter<string>("sqlDatabase"); 
        string sqlServerUsername = parametersReader.GetParameter<string>("sqlServerUsername"); 
        string sqlServerPassword = parametersReader.GetParameter<string>("sqlServerPassword"); 
    
        string sqlConnectionString = $"Server=tcp:{sqlServerName}.database.windows.net,1433;Database={sqlDbName};" + 
            $"User ID={sqlServerUsername};Password={sqlServerPassword};Trusted_Connection=False;Encrypt=True;" + 
            "Connection Timeout=30";
    
        SqlConnection conn = new SqlConnection(sqlConnectionString);
    
        Server server = new Server(new ServerConnection(conn));
    
        string script = File.ReadAllText(@"d:\home\site\wwwroot\prepsql\createdb.sql");
    
        server.ConnectionContext.ExecuteNonQuery(script);
        
        script = File.ReadAllText(@"d:\home\site\wwwroot\prepsql\insertdata.sql");
    
        server.ConnectionContext.ExecuteNonQuery(script);
    }
    catch (Exception ex)
    {
        log.Info(ex.GetType().Name);
        log.Info(ex.Message);
        log.Info(ex.StackTrace);
    }
    
    return new object();
}