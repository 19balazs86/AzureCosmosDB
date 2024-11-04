var appName       = 'balazs-test'
var databaseName  = 'PlayingWithCosmosDB'
var containerName = 'Student'
var nameToken     = take(uniqueString(resourceGroup().id), 5)
var cosmosName    = toLower('${appName}-${nameToken}-cosmos')

var rgLocation = resourceGroup().location

// Example: https://github.com/Azure/azure-quickstart-templates/blob/master/quickstarts/microsoft.documentdb/cosmosdb-free/main.bicep

// --> CosmosDB
// https://learn.microsoft.com/en-us/azure/templates/microsoft.documentdb/databaseaccounts
// az resource show --ids subscriptions/<GUID>/resourceGroups/<ResGroupName>/providers/Microsoft.DocumentDB/databaseAccounts/<CosmosName>

resource cosmosDB 'Microsoft.DocumentDB/databaseAccounts@2024-05-15' = {
  name: cosmosName
  location: rgLocation
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: rgLocation
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]
  }
}

// --> Database
// https://learn.microsoft.com/en-us/azure/templates/microsoft.documentdb/databaseaccounts/sqldatabases
// az resource show --ids subscriptions/<GUID>/resourceGroups/<ResGroupName>/providers/Microsoft.DocumentDB/databaseAccounts/<CosmosName>/sqlDatabases/<DatabaseName>

resource database 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-05-15' = {
  name: databaseName
  parent: cosmosDB
  properties: {
    resource: {
      id: databaseName
    }
  }
}

// --> Container
// https://learn.microsoft.com/en-us/azure/templates/microsoft.documentdb/databaseaccounts/sqldatabases/containers
// az resource show --ids subscriptions/<GUID>/resourceGroups/<ResGroupName>/providers/Microsoft.DocumentDB/databaseAccounts/<CosmosName>/sqlDatabases/<DatabaseName>/containers/<ContainerName>

resource container 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-05-15' = {
  name: containerName
  parent: database
  properties: {
    resource: {
      id: containerName
      partitionKey: {
        paths: ['/GradeLevel']
        kind: 'Hash'
      }
    }
  }
}

#disable-next-line outputs-should-not-contain-secrets
output CosmosDB_ConnString string = cosmosDB.listConnectionStrings().connectionStrings[0].connectionString
