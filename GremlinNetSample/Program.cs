﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gremlin.Net;
using Gremlin.Net.Driver;
using Newtonsoft.Json;
using Gremlin.Net.Structure.IO.GraphSON;
using dotenv.net;
using System.Diagnostics;

namespace GremlinQueryWithStats
{
    class Program
    {
        // Azure Cosmos DB Configuration variables
        private static string hostname;
        private static int port = 443;
        private static string authKey;
        private static string database;
        private static string collection;

        private static bool printResult = false;

        // Starts a console application that executes every Gremlin query in the gremlinQueries dictionary. 
        static void Main(string[] args)
        {
            DotEnv.Config();
            hostname = Environment.GetEnvironmentVariable("HOSTNAME");
            authKey = Environment.GetEnvironmentVariable("AUTHKEY");
            database = Environment.GetEnvironmentVariable("DATABASE");
            collection = Environment.GetEnvironmentVariable("COLLECTION");

            var query = args[0];
            printResult = args.Length > 1 ? Convert.ToBoolean(args[1]) : false;
            SubmitGremlinRequest(query).GetAwaiter().GetResult();

            if (System.Diagnostics.Debugger.IsAttached) Console.ReadLine();
           
        }

        static async Task SubmitGremlinRequest(string query)
        {
            var gremlinServer = new GremlinServer(hostname, port, enableSsl: true,
                                                    username: "/dbs/" + database + "/colls/" + collection,
                                                    password: authKey);

            using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
            {
                Console.WriteLine(String.Format($"Running this query: {query}"));

                // Create async task to execute the Gremlin query.
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var response = await gremlinClient.SubmitAsync<dynamic>(query);

                stopwatch.Stop();

                // Obtain resultset.
                var resultSet = response.AsResultSet();


                // Print all status attributes
                foreach (var attribute in resultSet.StatusAttributes)
                {
                    Console.WriteLine($"{attribute.Key} - {attribute.Value}");
                }

                Console.WriteLine($"Time to execute (incl. transfer) {stopwatch.ElapsedMilliseconds.ToString()} ms");

                if (printResult)
                {
                    foreach (var result in resultSet)
                    {
                        // The vertex results are formed as Dictionaries with a nested dictionary for their properties.
                        string output = JsonConvert.SerializeObject(result, Formatting.Indented);
                        Console.WriteLine(String.Format("\nResult:\n{0}", output));
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
