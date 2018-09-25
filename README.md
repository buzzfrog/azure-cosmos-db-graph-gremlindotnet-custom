# Execute CosmosDB Gremlin Queries with Statistics
This application use a custom fork from TINKERPOP-1913. This fork includes the possibility
to get statistics from the queries, such as consuming RU:s. (As the time of writing, 
this is not included in the offical Gremlin.NET package ([3.4.0-rc1](https://www.nuget.org/packages/Gremlin.Net/3.4.0-rc1)).

## Steps to install
1. Restore all nuget packages.
2. Remove Gremlin.Net
3. Install custom fork of Gremlin.Net using the local file ./gremlin.net_fork.3.3.2.nupkg
4. copy dotenv-example to a new file with the name ".env" and edit the connection variables or 
   add the environment variables to your shell.
5. Enjoy.

## Usage

```<language>
./GremlinQueryWithStats "<query>" [true/false]
```
[true/false] If the result of the query should be displayed. (Default: false.)

## Example
```<language>
./GremlinQueryWithStats "g.V(['1','KLO-098'])"
Running this query: g.V(['1','KLO-098'])
x-ms-status-code - 200
x-ms-request-charge - 3,35
x-ms-total-request-charge - 3,35
x-ms-cosmosdb-graph-request-charge - 3,35
StorageRU - 3,35
Time to execute (incl. transfer) 894 ms
```

