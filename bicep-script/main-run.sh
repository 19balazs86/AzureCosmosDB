#!/bin/bash

# https://learn.microsoft.com/en-us/cli/azure/deployment/group?view=azure-cli-latest#az-deployment-group-create

az deployment group create \
    --name "CosmosDB-Deployment" \
    --resource-group "rg-sand-box" \
    --template-file "main.bicep" \
    --query "properties.outputs"