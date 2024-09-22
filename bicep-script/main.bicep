var appName    = 'balazs-test'
var nameToken  = take(uniqueString(resourceGroup().id), 5)
var cosmosName = toLower('${appName}-${nameToken}-cosmos')

var rgLocation = resourceGroup().location

// --> CosmosDB
// https://learn.microsoft.com/en-us/azure/templates/microsoft.documentdb/databaseaccounts

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

#disable-next-line outputs-should-not-contain-secrets
output CosmosDB_ConnString string = cosmosDB.listConnectionStrings().connectionStrings[0].connectionString