# Modify for your environment.
# ACR_NAME: The name of your Azure Container Registry
# SERVICE_PRINCIPAL_NAME: Must be unique within your AD tenant
$ACR_NAME="reuben"
$SERVICE_PRINCIPAL_NAME="reuben-acr-service-principal"

Write-Host $ACR_NAME
Write-Host $SERVICE_PRINCIPAL_NAME

# Obtain the full registry ID for subsequent command args
$ACR_REGISTRY_ID=$(az acr show --name $ACR_NAME --query id --output tsv)
Write-Host $ACR_REGISTRY_ID

# Create the service principal with rights scoped to the registry.
# Default permissions are for docker pull access. Modify the '--role'
# argument value as desired:
# acrpull:     pull only
# acrpush:     push and pull
# owner:       push, pull, and assign roles
$SP_PASSWD=$(az ad sp create-for-rbac --name http://$SERVICE_PRINCIPAL_NAME --scopes $ACR_REGISTRY_ID --role acrpull --query password --output tsv)
$SP_APP_ID=$(az ad sp show --id http://$SERVICE_PRINCIPAL_NAME --query appId --output tsv)

# Output the service principal's credentials; use these in your services and
# applications to authenticate to the container registry.
echo "Service principal ID: $SP_APP_ID"
echo "Service principal password: $SP_PASSWD"
